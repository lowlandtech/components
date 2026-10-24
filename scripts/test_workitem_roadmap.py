"""Regression checks for the milestone roadmap and managed-source mapping."""
import json
import re
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[1]


class RoadmapTests(unittest.TestCase):
    def setUp(self):
        artifacts = ROOT / 'artifacts'
        artifacts.mkdir(exist_ok=True)
        self.temp = tempfile.TemporaryDirectory(dir=artifacts)
        self.base = Path(self.temp.name).resolve()
        assert self.base.is_relative_to(artifacts.resolve())
        self.addCleanup(self.temp.cleanup)
        self.board_dir = self.base / 'workspace/components/docs/workitems'
        self.board_dir.mkdir(parents=True)
        for source in (ROOT / 'docs/workitems').glob('*.md'):
            shutil.copyfile(source, self.board_dir / source.name)
        target = self.base / 'workspace/plugins/docs/workitems/done'
        target.mkdir(parents=True)
        name = 'BLAZ0001.US01.SC01.package-optional-blazor-adapter.md'
        shutil.copyfile(ROOT.parent / 'plugins/docs/workitems/done' / name, target / name)
        self.board = self.board_dir / 'README.md'
        # Keep disposable dependency probes stable as real prerequisites finish.
        graph = {'M1-001': 'none', 'M2-001': 'M1-001'}
        lines = self.board.read_text(encoding='utf-8').splitlines()
        for i, line in enumerate(lines):
            cells = line.split('|')
            if len(cells) == 8 and cells[1].strip() in graph:
                cells[5] = ' ' + graph[cells[1].strip()] + ' '
                lines[i] = '|'.join(cells)
        self.board.write_text('\n'.join(lines) + '\n', encoding='utf-8')
        cpkg = self.board_dir / 'CPKG0001.US01.SC01.independent-package-consumption.md'
        cplg = self.board_dir / 'CPLG0001.US01.SC01.ui-feature-manifest.md'
        for path, dependency in [(cpkg, 'none'), (cplg, 'CPKG0001-US01-SC01')]:
            body = path.read_text(encoding='utf-8')
            body = re.sub(r'(?m)^- \*\*Blocked by:\*\* .*$', '- **Blocked by:** ' + dependency, body)
            path.write_text(body, encoding='utf-8')
        self.cplg_state = re.search(r'- \*\*State:\*\* (\w+)', cplg.read_text(encoding='utf-8')).group(1)
        source = self.board_dir / 'CPKG0001.US01.SC01.independent-package-consumption.md'
        self.cpkg_state = re.search(r'- \*\*State:\*\* (\w+)', source.read_text(encoding='utf-8')).group(1)

    def run_check(self):
        quote = lambda value: "'" + str(value).replace("'", "''") + "'"
        script = ". " + quote(ROOT / 'scripts/WorkitemRoadmap.ps1') + "\n"
        script += "$ErrorActionPreference='Stop'\n$rows=@(Get-WorkitemRoadmap -Path " + quote(self.board) + ")\n"
        script += r"""
foreach ($row in $rows | Where-Object { !$_.External }) {
    $body=Get-Content -LiteralPath (Join-Path """ + quote(self.board_dir) + r""" $row.Source) -Raw
    $null=$body -match '(?m)^- \*\*State:\*\* (\w+)\r?$'; $state=$Matches[1]
    $null=$body -match '(?m)^- \*\*Blocked by:\*\* (.+)\r?$'; $deps=$Matches[1]
    $null=Assert-WorkitemRoadmapEntry -Rows $rows -Scope $row.Scope -State $state -Dependencies $deps -FileName $row.Source -Body $body
}
@($rows | Select-Object Work,Scope,External) | ConvertTo-Json
"""
        return subprocess.run(['pwsh', '-NoProfile', '-Command', script], capture_output=True, text=True)

    def mutate_board(self, before, after):
        text = self.board.read_text(encoding='utf-8')
        self.assertIn(before, text)
        self.board.write_text(text.replace(before, after, 1), encoding='utf-8')

    def test_id_only_work_column_and_external_reference(self):
        result = self.run_check()
        self.assertEqual(0, result.returncode, result.stderr)
        rows = json.loads(result.stdout)
        self.assertEqual(6, sum(not row['External'] for row in rows))
        self.assertEqual(['M3-X01'], [row['Work'] for row in rows if row['External']])
        self.assertTrue(all(' ' not in row['Work'] for row in rows))

    def test_duplicate_milestone_item_is_rejected(self):
        self.mutate_board('| M3-002 |', '| M3-001 |')
        result = self.run_check()
        self.assertNotEqual(0, result.returncode)
        self.assertIn('Duplicate roadmap Work', result.stderr)

    def test_unknown_dependency_is_rejected(self):
        self.mutate_board(f'| M1-001 | {self.cplg_state} |', f'| M99-001 | {self.cplg_state} |')
        result = self.run_check()
        self.assertNotEqual(0, result.returncode)
        self.assertIn('Unknown dependency', result.stderr)

    def test_cycle_is_rejected(self):
        self.mutate_board(f'| none | {self.cpkg_state} |', f'| M2-001 | {self.cpkg_state} |')
        result = self.run_check()
        self.assertNotEqual(0, result.returncode)
        self.assertIn('dependency cycle', result.stderr)

    def test_source_state_disagreement_is_rejected(self):
        source = self.board_dir / 'CPKG0001.US01.SC01.independent-package-consumption.md'
        other_state = 'review' if self.cpkg_state == 'done' else 'done'
        source.write_text(source.read_text(encoding='utf-8').replace(f'- **State:** {self.cpkg_state}', f'- **State:** {other_state}', 1), encoding='utf-8')
        result = self.run_check()
        self.assertNotEqual(0, result.returncode)
        self.assertIn('Board state/link mismatch', result.stderr)

    def test_source_dependency_disagreement_is_rejected(self):
        source = self.board_dir / 'CPLG0001.US01.SC01.ui-feature-manifest.md'
        source.write_text(source.read_text().replace('- **Blocked by:** CPKG0001-US01-SC01', '- **Blocked by:** none', 1))
        result = self.run_check()
        self.assertNotEqual(0, result.returncode)
        self.assertIn('Board dependency mismatch', result.stderr)

    def test_offline_mirror_emits_only_six_local_scopes(self):
        result = subprocess.run(['pwsh', '-NoProfile', '-File',
            str(ROOT / 'scripts/Sync-PluginWorkitems.ps1'), '-Offline'], capture_output=True, text=True)
        self.assertEqual(0, result.returncode, result.stderr)
        rows = json.loads(result.stdout)
        self.assertEqual(6, len(rows))
        self.assertTrue(all(row['action'] == 'validated' for row in rows))
        self.assertFalse(any(row['scope'].startswith('BLAZ') for row in rows))

    def test_scoped_offline_mirror_selects_only_requested_scope(self):
        result = subprocess.run(['pwsh', '-NoProfile', '-File',
            str(ROOT / 'scripts/Sync-PluginWorkitems.ps1'), '-Offline',
            '-TraceId', 'CPKG0001-US01-SC01'], capture_output=True, text=True)
        self.assertEqual(0, result.returncode, result.stderr)
        row = json.loads(result.stdout)
        self.assertEqual('CPKG0001-US01-SC01', row['scope'])
        self.assertEqual('validated', row['action'])

    def test_unknown_mirror_scope_fails_closed(self):
        result = subprocess.run(['pwsh', '-NoProfile', '-File',
            str(ROOT / 'scripts/Sync-PluginWorkitems.ps1'), '-Offline',
            '-TraceId', 'NONE9999-US01-SC01'], capture_output=True, text=True)
        self.assertNotEqual(0, result.returncode)
        self.assertIn('No workitems match', result.stderr)


if __name__ == '__main__':
    unittest.main()

