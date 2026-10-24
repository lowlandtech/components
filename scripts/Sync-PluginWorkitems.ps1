[CmdletBinding()]
param([string]$Repository = 'lowlandtech/components', [switch]$Apply, [switch]$Offline)
$ErrorActionPreference = 'Stop'
if ($Repository -notmatch '^[\w.-]+/[\w.-]+$') { throw 'Expected owner/repository.' }
if ($Apply -and $Offline) { throw 'Offline validation cannot apply GitHub changes.' }
$root = Split-Path $PSScriptRoot -Parent
. (Join-Path $PSScriptRoot 'WorkitemRoadmap.ps1')
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
$issues = @()
if (!$Offline) {
$issues = @(foreach ($page in (Read-Gh @('api', "repos/$Repository/issues", '--method', 'GET', '-f', 'state=all', '-f', 'per_page=100', '--paginate', '--slurp'))) {
    foreach ($issue in $page) { if (!$issue.pull_request) { $issue } }
})
}
$board = Get-Content (Join-Path $root 'docs/workitems/README.md') -Raw -Encoding UTF8
$roadmap = @(Get-WorkitemRoadmap -Path (Join-Path $root 'docs/workitems/README.md'))
$results = @()
foreach ($file in (Get-ChildItem (Join-Path $root 'docs/workitems/*.md') -File | Where-Object Name -ne 'README.md' | Sort-Object Name)) {
    # Read as UTF-8: the documents contain an em dash in the title, and the
    # default code page would otherwise rewrite it and corrupt the issue body.
    $body = (Get-Content $file.FullName -Raw -Encoding UTF8).TrimEnd()
    $emDash = [char]0x2014
    if ($body -notmatch "(?m)^# ([A-Z]{4}\d{4}-US\d{2}-SC\d{2}) $emDash (.+)\r?$") { throw "Invalid title: $file" }
    $id = $Matches[1]; $title = "${id}: $($Matches[2].Trim())"
    # A finished scope rests at review until the coordinator accepts it and then
    # at done, and the board mirrors its state, so the settled states are
    # mirrored as well as backlog. A scope still in flight has no settled
    # mirror and needs a review before it is synced.
    if ($body -notmatch '(?m)^- \*\*State:\*\* (backlog|review|done)\r?$') { throw "Review non-settled state before syncing: $id" }
    $scopeState = $Matches[1]
    # The issue workflow closes a completed issue with `status: done`.
    $desiredIssueState = if ($scopeState -eq 'done') { 'closed' } else { 'open' }
    if ($body -notmatch '(?m)^- \*\*Blocked by:\*\* (.+)\r?$') { throw "Missing dependency: $id" }
    $dependencies = $Matches[1].Trim()
    $row = Assert-WorkitemRoadmapEntry -Rows $roadmap -Scope $id -State $scopeState -Dependencies $dependencies -FileName $file.Name -Body $body
    if ($Offline) {
        $results += [pscustomobject]@{work=$row.Work;scope=$id;state=$scopeState;action='validated'}
        continue
    }
    $marker = "<!-- workitem:$id -->"
    $matchesForItem = @($issues | Where-Object { ($_.body -and $_.body.Contains($marker)) -or $_.title.StartsWith("${id}:") })
    if ($matchesForItem.Count -gt 1) { throw "Duplicate issue identity: $id" }
    $labels = @('type: story', "status: $scopeState", 'area: integration', 'needs: triage', 'needs: estimate')
    if ($dependencies -ne 'none') { $labels += 'flag: blocked' }
    $desiredBody = "$marker`n`n$body`n`n---`nSource: ``docs/workitems/$($file.Name)``. Update the local document and board, then sync."
    $number = $null; $action = 'create'
    if ($matchesForItem.Count) {
        $issue = $matchesForItem[0]; $number = $issue.number
        $labels = @($labels + @($issue.labels | Where-Object { $_.name -notmatch '^status:' -and $_.name -ne 'flag: blocked' } | ForEach-Object name) | Sort-Object -Unique)
        $actualLabels = @($issue.labels | ForEach-Object name | Sort-Object -Unique)
        $action = if ($issue.title -eq $title -and $issue.body -ceq $desiredBody -and $issue.state -eq $desiredIssueState -and ($labels -join '|') -eq ($actualLabels -join '|')) { 'unchanged' } else { 'update' }
        if ($Apply -and $action -eq 'update') { $null = Write-Gh "repos/$Repository/issues/$number" PATCH @{title=$title;body=$desiredBody;labels=$labels;state=$desiredIssueState} }
    } elseif ($Apply) {
        $newIssue = Write-Gh "repos/$Repository/issues" POST @{title=$title;body=$desiredBody;labels=$labels}
        $number = $newIssue.number
    }
    $results += [pscustomobject]@{scope=$id;action=$action;number=$number;url=$(if ($number) { "https://github.com/$Repository/issues/$number" } else { $null })}
}
$results | ConvertTo-Json -Depth 4
