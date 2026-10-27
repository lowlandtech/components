[CmdletBinding()]
param(
    [string]$FeedPath = 'D:\lowlandtech\packages',
    [string]$Version,
    [string]$Configuration = 'Release',
    [string]$ManifestPath,
    [string[]]$Projects,
    [switch]$CompleteBatch,
    [string]$ContractsVersion
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
# A Core release pins the contracts version it was built against exactly. When
# that release keeps a contracts version that the feed holds already, the batch
# moves to a new version and the packages that depend on the contracts carry
# the published contracts version in their own dependency. The contracts
# package itself stays published, so it is not part of this batch.
$contractsId = 'LowlandTech.Components.Core'
$contractProperties = @()
if ($ContractsVersion) {
    $contractsProject = @($manifest.projects | Where-Object {
        [System.IO.Path]::GetFileNameWithoutExtension($_) -eq $contractsId
    }) | Select-Object -First 1
    if (!$contractsProject) { throw "The manifest does not select the contracts package '$contractsId'." }
    if ($projects -contains $contractsProject) {
        throw "The contracts package stays published at $ContractsVersion; remove '$contractsProject' from the projects of this batch."
    }
    $contractsPackage = "$contractsId.$ContractsVersion.nupkg"
    if (!(Test-Path -LiteralPath (Join-Path $FeedPath $contractsPackage))) {
        throw "The contracts package '$contractsPackage' is not published in '$FeedPath'."
    }
    $contractProperties = @("-p:ComponentsContractsVersion=$ContractsVersion")
}
$staging = Join-Path $root "artifacts/local-packages/$Version"
# A batch whose packages carry different dependencies on each other is published
# in parts at one version: the part that the producer pins exactly is published
# first, and the rest of the batch follows at the same version. The completing
# pass reuses the staging directory of the first pass, and the feed check below
# still refuses to replace a package that is already published.
if ((Test-Path -LiteralPath $staging) -and !$CompleteBatch) { throw "Version already staged: $Version. Choose a new version." }
$stagingExisted = Test-Path -LiteralPath $staging
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
        Invoke-DotNet (@('build', (Join-Path $root $project), '-c', $Configuration, "-p:Version=$Version") + $contractProperties + @('-m:1'))
    }
    foreach ($project in $projects) {
        Invoke-DotNet (@('pack', $project, '-c', $Configuration, '--no-build', "-p:Version=$Version") + $contractProperties + @('-o', $staging))
    }
    $staged = @(Get-ChildItem -LiteralPath $staging -Filter '*.nupkg')
    if (!$CompleteBatch) {
        # Check the entire batch before publishing; never replace an existing identity.
        foreach ($package in $staged) {
            if (Test-Path -LiteralPath (Join-Path $FeedPath $package.Name)) { throw "Package already exists: $($package.Name). Choose a new version." }
        }
    }
    # A completing pass publishes the parts of the batch that the feed does not
    # hold yet, because the packages of the first pass are published already.
    $packages = @($staged | Where-Object { !(Test-Path -LiteralPath (Join-Path $FeedPath $_.Name)) })
    if ($packages.Count -ne $projects.Count) { throw 'Expected one package per selected project. Check IsPackable and PackageId.' }
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
    if (!$stagingExisted -and (Test-Path -LiteralPath $staging)) { Remove-Item -LiteralPath $staging -Recurse -Force }
    throw
} finally {
    Pop-Location
}
