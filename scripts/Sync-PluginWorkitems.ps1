[CmdletBinding()]
param([string]$Repository = 'lowlandtech/components', [switch]$Apply)
$ErrorActionPreference = 'Stop'
if ($Repository -notmatch '^[\w.-]+/[\w.-]+$') { throw 'Expected owner/repository.' }
$root = Split-Path $PSScriptRoot -Parent
function Read-Gh([string[]]$Arguments) {
    $output = & gh @Arguments
    if ($LASTEXITCODE -ne 0) { throw 'GitHub read failed.' }
    if ($output) { return ($output -join "`n" | ConvertFrom-Json) }
}
function Write-Gh([string]$Endpoint, [string]$Method, $Payload) {
    $temporary = [IO.Path]::GetTempFileName()
    try {
        [IO.File]::WriteAllText($temporary, ($Payload | ConvertTo-Json -Depth 12), [Text.UTF8Encoding]::new($false))
        Read-Gh @('api', $Endpoint, '--method', $Method, '--input', $temporary)
    } finally { Remove-Item -LiteralPath $temporary -Force }
}
$issues = @(foreach ($page in (Read-Gh @('api', "repos/$Repository/issues", '--method', 'GET', '-f', 'state=all', '-f', 'per_page=100', '--paginate', '--slurp'))) {
    foreach ($issue in $page) { if (!$issue.pull_request) { $issue } }
})
$board = Get-Content (Join-Path $root 'docs/workitems/README.md') -Raw
$results = @()
foreach ($file in (Get-ChildItem (Join-Path $root 'docs/workitems/*.md') -File | Where-Object Name -ne 'README.md' | Sort-Object Name)) {
    $body = (Get-Content $file.FullName -Raw).TrimEnd()
    if ($body -notmatch '(?m)^# ([A-Z]{4}\d{4}-US\d{2}-SC\d{2}) — (.+)\r?$') { throw "Invalid title: $file" }
    $id = $Matches[1]; $title = "${id}: $($Matches[2].Trim())"
    if ($body -notmatch '(?m)^- \*\*State:\*\* backlog\r?$') { throw "Review non-backlog state before syncing: $id" }
    if ($body -notmatch '(?m)^- \*\*Blocked by:\*\* (.+)\r?$') { throw "Missing dependency: $id" }
    $dependencies = $Matches[1].Trim()
    $rows = @($board -split '\r?\n' | Where-Object { $_.StartsWith("| $id |") -and $_ -like '*| backlog |*' })
    if ($rows.Count -ne 1 -or $rows[0] -notlike "*| backlog | $dependencies |") { throw "Board mismatch: $id" }
    $marker = "<!-- workitem:$id -->"
    $matchesForItem = @($issues | Where-Object { ($_.body -and $_.body.Contains($marker)) -or $_.title.StartsWith("${id}:") })
    if ($matchesForItem.Count -gt 1) { throw "Duplicate issue identity: $id" }
    $labels = @('type: story', 'status: backlog', 'area: integration', 'needs: triage', 'needs: estimate')
    if ($dependencies -ne 'none') { $labels += 'flag: blocked' }
    $desiredBody = "$marker`n`n$body`n`n---`nSource: ``docs/workitems/$($file.Name)``. Update the local document and board, then sync."
    $number = $null; $action = 'create'
    if ($matchesForItem.Count) {
        $issue = $matchesForItem[0]; $number = $issue.number
        $labels = @($labels + @($issue.labels | Where-Object { $_.name -notmatch '^status:' -and $_.name -ne 'flag: blocked' } | ForEach-Object name) | Sort-Object -Unique)
        $actualLabels = @($issue.labels | ForEach-Object name | Sort-Object -Unique)
        $action = if ($issue.title -eq $title -and $issue.body -ceq $desiredBody -and $issue.state -eq 'open' -and ($labels -join '|') -eq ($actualLabels -join '|')) { 'unchanged' } else { 'update' }
        if ($Apply -and $action -eq 'update') { $null = Write-Gh "repos/$Repository/issues/$number" PATCH @{title=$title;body=$desiredBody;labels=$labels;state='open'} }
    } elseif ($Apply) {
        $newIssue = Write-Gh "repos/$Repository/issues" POST @{title=$title;body=$desiredBody;labels=$labels}
        $number = $newIssue.number
    }
    $results += [pscustomobject]@{scope=$id;action=$action;number=$number;url=$(if ($number) { "https://github.com/$Repository/issues/$number" } else { $null })}
}
$results | ConvertTo-Json -Depth 4
