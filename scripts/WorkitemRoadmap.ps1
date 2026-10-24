# Shared local parser for the milestone board and its managed issue mirror.
function Get-WorkitemRoadmap {
    param([Parameter(Mandatory)][string]$Path)
    $rows = @()
    $headers = @()
    foreach ($line in (Get-Content -LiteralPath $Path -Encoding UTF8)) {
        if (!$line.Trim().StartsWith('|')) { continue }
        $cells = @($line.Trim().Trim('|').Split('|') | ForEach-Object { $_.Trim() })
        if ($cells -contains 'Work' -and $cells -contains 'Scope' -and $cells -contains 'State') {
            $headers = $cells
            continue
        }
        if (!$headers.Count -or $cells.Count -ne $headers.Count -or $cells[0] -match '^[-:]+$') { continue }
        $row = @{}
        for ($i = 0; $i -lt $headers.Count; $i++) { $row[$headers[$i]] = $cells[$i] }
        if ($row.Work -notmatch '^M\d+-(?:\d+|X\d+)$') { throw "Invalid milestone item: $($row.Work)" }
        $row.Scope = $row.Scope.Trim([char]96)
        if ($row.Scope -notmatch '^[A-Z]{4}\d{4}-US\d{2}-SC\d{2}$') { throw "Invalid scope: $($row.Scope)" }
        $row.External = $row.Work -match '-X\d+$'
        if ($row.Title -notmatch '\]\(([^)]+\.md)\)') { throw "Missing workitem link: $($row.Work)" }
        $row.Source = $Matches[1]
        if (!(Test-Path -LiteralPath (Join-Path (Split-Path $Path) $row.Source) -PathType Leaf)) {
            throw "Broken workitem link: $($row.Work)"
        }
        $rows += [pscustomobject]$row
    }
    if (!$rows.Count) { throw 'No milestone workitems found.' }
    foreach ($key in @('Work', 'Scope')) {
        if (@($rows | Group-Object $key | Where-Object Count -gt 1).Count) { throw "Duplicate roadmap $key." }
    }
    foreach ($row in $rows) {
        foreach ($dependency in @($row.'Depends on' -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne 'none' })) {
            if (@($rows | Where-Object Work -eq $dependency).Count -ne 1) {
                throw "Unknown dependency '$dependency' on $($row.Work)."
            }
        }
    }
    function Visit-RoadmapItem([string]$Work, [string[]]$Trail) {
        if ($Trail -contains $Work) { throw "Roadmap dependency cycle: $($Trail -join ' -> ') -> $Work" }
        $node = @($rows | Where-Object Work -eq $Work)[0]
        if ($node.External) { return }
        foreach ($dependency in @($node.'Depends on' -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne 'none' })) {
            Visit-RoadmapItem $dependency @($Trail + $Work)
        }
    }
    foreach ($row in $rows) { Visit-RoadmapItem $row.Work @() }
    return $rows
}

function Assert-WorkitemRoadmapEntry {
    param([Parameter(Mandatory)]$Rows, [Parameter(Mandatory)][string]$Scope,
          [Parameter(Mandatory)][string]$State, [Parameter(Mandatory)][string]$Dependencies,
          [Parameter(Mandatory)][string]$FileName, [Parameter(Mandatory)][string]$Body)
    $matching = @($Rows | Where-Object { $_.Scope -eq $Scope -and !$_.External })
    if ($matching.Count -ne 1) { throw "Missing or duplicate local board entry: $Scope" }
    $row = $matching[0]
    if ($row.State -ne $State -or $row.Source -ne $FileName) { throw "Board state/link mismatch: $Scope" }
    $canonical = @()
    foreach ($dependency in @($row.'Depends on' -split ';' | ForEach-Object { $_.Trim() })) {
        if ($dependency -eq 'none') { continue }
        $target = @($Rows | Where-Object Work -eq $dependency)[0]
        if ($target.External) {
            $lane = $target.Owner
            $canonical += $lane.Substring(0, 1).ToUpperInvariant() + $lane.Substring(1) + ' ' + $target.Scope
        } else { $canonical += $target.Scope }
    }
    $actual = @($Dependencies -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne 'none' } | Sort-Object)
    if ((($canonical | Sort-Object) -join ';') -cne ($actual -join ';')) { throw "Board dependency mismatch: $Scope" }
    foreach ($field in @(@('Work', $row.Work), @('Milestone', ($row.Work -split '-')[0]), @('Owner', $row.Owner))) {
        $pattern = '(?m)^- \*\*' + [regex]::Escape($field[0]) + ':\*\* ' + [regex]::Escape($field[1]) + '\r?$'
        if ($Body -notmatch $pattern) { throw "Board $($field[0]) mismatch: $Scope" }
    }
    return $row
}
