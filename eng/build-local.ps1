[CmdletBinding()]
param(
    [string]$FeedPath = 'D:\lowlandtech\packages',
    [string]$Version,
    [string]$Configuration = 'Release',
    [string]$ManifestPath,
    [string[]]$Projects
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
# $PSScriptRoot is empty inside a param() default in Windows PowerShell 5.1, so every
# default that needs the script directory is resolved in the body instead.
if (!$ManifestPath) { $ManifestPath = Join-Path $PSScriptRoot 'local-packages.json' }
$root = Split-Path $PSScriptRoot -Parent
$manifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
if (!$Version) {
    $stamp = [DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')
    $suffix = Get-Random -Minimum 100000 -Maximum 1000000
    $Version = "$($manifest.versionPrefix)-$stamp$suffix"
}
if ($Version -notmatch '^[1-9][0-9]{3}\.(?:[1-9]|1[0-2])\.(?:0|[1-9][0-9]*)(?:-(?:0|[1-9][0-9]*))?$') {
    throw 'Version must follow year.month.major[-minor], e.g. 2026.10.1 or 2026.10.1-2 (no leading zeroes).'
}
# A subset of the manifest publishes a part of the batch. The dependency
# closure of the published packages needs the identities of the parts and the
# whole in one agreed version, so a subset carries the same version as the
# whole batch it belongs to.
if (!$Projects) { $Projects = @($manifest.projects) }
$projects = @($Projects)
if (!$projects.Count) { throw 'The manifest must select at least one packable project.' }
$solution = Join-Path $root $manifest.solution
foreach ($path in @($solution) + @($projects | ForEach-Object { Join-Path $root $_ })) {
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing build input: $path" }
}
$staging = Join-Path $root "artifacts/local-packages/$Version"
if (Test-Path -LiteralPath $staging) { throw "Version already staged: $Version. Choose a new version." }
New-Item -ItemType Directory -Path $staging -Force | Out-Null
function Invoke-DotNet([string[]]$Arguments) {
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) { throw "dotnet failed ($LASTEXITCODE): $Arguments" }
}
Push-Location $root
try {
    # The batch publishes the manifest's projects. The solution also carries the
    # test, story and host projects that the batch does not publish, and
    # rebuilding them writes into the output directory a running test host
    # holds, which fails the publication with MSB3027. The batch builds only
    # what it publishes, and the solution stays the validated repository build
    # unit without being the build input of the batch.
    foreach ($project in $projects) {
        Invoke-DotNet @('build', (Join-Path $root $project), '-c', $Configuration, "-p:Version=$Version", '-m:1')
    }
    foreach ($project in $projects) {
        Invoke-DotNet @('pack', $project, '-c', $Configuration, '--no-build', "-p:Version=$Version", '-o', $staging)
    }
    $packages = @(Get-ChildItem -LiteralPath $staging -Filter '*.nupkg')
    if ($packages.Count -ne $projects.Count) { throw 'Expected one package per selected project. Check IsPackable and PackageId.' }
    # Check the entire batch before publishing; never replace an existing identity.
    foreach ($package in $packages) {
        if (Test-Path -LiteralPath (Join-Path $FeedPath $package.Name)) { throw "Package already exists: $($package.Name). Choose a new version." }
    }
    New-Item -ItemType Directory -Path $FeedPath -Force | Out-Null
    foreach ($package in $packages) {
        # Copy without overwrite, including when two builders race.
        [System.IO.File]::Copy($package.FullName, (Join-Path $FeedPath $package.Name), $false)
    }
    Write-Host "Published $($packages.Count) packages to $FeedPath"
    Write-Host "Package version: $Version"
} catch {
    # A failed publication leaves no staged version behind, so the same version
    # can be retried and a second shell can take over the batch.
    if (Test-Path -LiteralPath $staging) { Remove-Item -LiteralPath $staging -Recurse -Force }
    throw
} finally {
    Pop-Location
}
