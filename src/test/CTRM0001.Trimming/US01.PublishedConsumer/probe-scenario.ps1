#Requires -Version 5.1
<#
.SYNOPSIS
    CTRM0001 trim preservation probe harness.

.DESCRIPTION
    Produces the artefacts that the CTRM0001 trimmed-host preservation probe
    asserts against:

      1. packs this repository's published projects into a private feed at one
         fresh version, with the contracts generation the current Core pins;
      2. writes a runnable consumer application that references the packed
         packages and exercises the paths the trim suppressions cover;
      3. publishes that consumer trimmed for win-x64 and records every ILLink
         diagnostic, separated into the ones that name a components assembly
         and the ones that do not;
      4. runs the trimmed application and records the observations it prints.

    The private feed keeps the shared feed untouched, so the probe never
    publishes a batch. The consumer resolves the upstream Core package and its
    closure from the shared feed, which the probe only reads.

.PARAMETER RepositoryRoot
    Repository to pack from. Defaults to the root above this script.

.PARAMETER ScenarioRoot
    Directory that receives the feed, the consumer and the publish output.
    Created if missing; the caller owns its lifetime.

.PARAMETER ContractsVersion
    The published LowlandTech.Components.Core generation the current Core pins.

.PARAMETER CoreVersion
    The upstream LowlandTech.Core package the batch pins exactly.

.PARAMETER SharedFeed
    The workspace package feed that carries Core and the framework closure.
#>
[CmdletBinding()]
param(
    [string] $RepositoryRoot,
    [Parameter(Mandatory = $true)][string] $ScenarioRoot,
    [string] $ContractsVersion = '2026.10.3-20261029112040813496109',
    [string] $CoreVersion = '2026.10.3-20261029175917',
    [string] $SharedFeed = 'D:\lowlandtech\packages'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if (!$RepositoryRoot) {
    # Walk up from the probe until the repository marker is found.
    $candidate = $PSScriptRoot
    while ($candidate -and !(Test-Path -LiteralPath (Join-Path $candidate 'eng/build-local.ps1'))) {
        $candidate = Split-Path $candidate -Parent
    }
    if (!$candidate) { throw "Could not locate the repository root above '$PSScriptRoot'." }
    $RepositoryRoot = $candidate
}
$RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$ScenarioRoot = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ScenarioRoot)
$feed = Join-Path $ScenarioRoot 'feed'
$consumer = Join-Path $ScenarioRoot 'consumer'
$publish = Join-Path $ScenarioRoot 'publish'
$logPath = Join-Path $ScenarioRoot 'probe.log'
$resultPath = Join-Path $ScenarioRoot 'probe-result.json'

New-Item -ItemType Directory -Path $feed, $consumer, $publish -Force | Out-Null
if (Test-Path -LiteralPath $logPath) { Remove-Item -LiteralPath $logPath -Force }
function Write-Step([string] $text) { Add-Content -LiteralPath $logPath -Value ("[{0:HH:mm:ss}] {1}" -f (Get-Date), $text) }

# A fresh version per run keeps the feed immutable and the staging directory free.
$stamp = [DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')
$batchVersion = '2026.10.3-' + $stamp

$contractsPackage = Join-Path $SharedFeed "LowlandTech.Components.Core.$ContractsVersion.nupkg"
if (!(Test-Path -LiteralPath $contractsPackage)) {
    throw "The contracts package '$contractsPackage' is not published in '$SharedFeed'."
}
Copy-Item -LiteralPath $contractsPackage -Destination (Join-Path $feed (Split-Path $contractsPackage -Leaf)) -Force

$env:LowlandTechCoreVersion = $CoreVersion
Write-Step "packing batch $batchVersion into the private feed"
& (Join-Path $RepositoryRoot 'eng/build-local.ps1') `
    -FeedPath $feed `
    -Version $batchVersion `
    -ContractsVersion $ContractsVersion `
    -Projects @(
        'src/lib/LowlandTech.Components.csproj',
        'src/blocks/LowlandTech.Components.Blocks.csproj',
        'src/features/LowlandTech.Components.Features.Session/LowlandTech.Components.Features.Session.csproj',
        'src/features/LowlandTech.Components.Features.Pages/LowlandTech.Components.Features.Pages.csproj') *>&1 |
    Tee-Object -FilePath $logPath -Append | Out-Null
if ($LASTEXITCODE -ne 0) { throw "The batch pack failed with exit code $LASTEXITCODE." }

# The consumer restores from the private feed first and the shared feed second.
Copy-Item -Path (Join-Path $PSScriptRoot 'consumer/*') -Destination $consumer -Recurse -Force
$nuget = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="probe" value="$feed" />
    <add key="shared" value="$SharedFeed" />
  </packageSources>
</configuration>
"@
Set-Content -LiteralPath (Join-Path $consumer 'NuGet.config') -Value $nuget -Encoding utf8

$project = Join-Path $consumer 'Consumer.csproj'
$arguments = @(
    'publish', $project,
    '-c', 'Release',
    '-r', 'win-x64',
    '--self-contained', 'true',
    '-p:PublishTrimmed=true',
    '-p:TrimmerSingleWarn=false',
    "-p:ComponentsBatchVersion=$batchVersion",
    "-p:LowlandTechCoreVersion=$CoreVersion",
    '-o', $publish
)
Write-Step "publishing the trimmed consumer: dotnet $($arguments -join ' ')"
$output = & dotnet @arguments 2>&1
$output | Tee-Object -FilePath $logPath -Append | Out-Null
$publishExit = $LASTEXITCODE

$warnings = @($output | Where-Object { $_ -match 'warning IL\d+' } | ForEach-Object { "$_".Trim() } | Sort-Object -Unique)
$owned = @($warnings | Where-Object { $_ -match 'LowlandTech\.Components' })
Write-Step "publish exit $publishExit, $($warnings.Count) unique ILLink warnings, $($owned.Count) naming a components assembly"

$probe = $null
$runExit = $null
$exe = Join-Path $publish 'Ctrm0001.Probe.exe'
if ($publishExit -eq 0 -and (Test-Path -LiteralPath $exe)) {
    $runOutput = & $exe 2>&1
    $runExit = $LASTEXITCODE
    $runOutput | Tee-Object -FilePath $logPath -Append | Out-Null
    $line = $runOutput | Where-Object { $_ -match '^PROBE:' } | Select-Object -First 1
    if ($line) { $probe = ("$line").Substring('PROBE:'.Length) }
    Write-Step "trimmed consumer exit $runExit"
}
else {
    Write-Step "the trimmed consumer was not produced"
}

# The declaration of the packed batch: what it pins is what a consumer resolves.
$batchPackage = Join-Path $feed "LowlandTech.Components.$batchVersion.nupkg"
$coreDependency = $null
$contractsDependency = $null
if (Test-Path -LiteralPath $batchPackage) {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [System.IO.Compression.ZipFile]::OpenRead($batchPackage)
    try {
        $entry = $archive.Entries | Where-Object { $_.FullName -like '*.nuspec' } | Select-Object -First 1
        if ($entry) {
            $reader = New-Object System.IO.StreamReader($entry.Open())
            try { $nuspec = $reader.ReadToEnd() } finally { $reader.Dispose() }
            $coreMatch = [regex]::Match($nuspec, '<dependency id="LowlandTech\.Core" version="([^"]+)"')
            $contractsMatch = [regex]::Match($nuspec, '<dependency id="LowlandTech\.Components\.Core" version="([^"]+)"')
            if ($coreMatch.Success) { $coreDependency = $coreMatch.Groups[1].Value }
            if ($contractsMatch.Success) { $contractsDependency = $contractsMatch.Groups[1].Value }
        }
    }
    finally { $archive.Dispose() }
}

$result = [pscustomobject]@{
    batchVersion        = $batchVersion
    contracts           = $ContractsVersion
    core                = $CoreVersion
    coreDependency      = $coreDependency
    contractsDependency = $contractsDependency
    publishExit         = $publishExit
    runExit             = $runExit
    warningCount        = $warnings.Count
    warnings            = $warnings
    ownedWarnings       = $owned
    probe               = $probe
}
$result | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resultPath -Encoding utf8
Write-Step "result written to $resultPath"
$result | ConvertTo-Json -Depth 6
