#Requires -Version 5.1
<#
.SYNOPSIS
    CPKG0006 residual-trim harness.

.DESCRIPTION
    Packs this repository's two published libraries into a private feed, publishes a
    consumer trimmed against that batch, and records:

      1. the observations the trimmed consumer prints for the three suppressed paths —
         a grid column that reads a nested property, a block page that discovers an
         attributed hook, and an outbox that serializes a payload of its own type;
      2. every ILLink diagnostic that names a components assembly;
      3. the browser toolchain state, because the browser host of the acceptance needs
         the wasm-tools workload.

    The private feed keeps the shared feed untouched, so the probe never publishes.
#>
[CmdletBinding()]
param(
    [string] $RepositoryRoot,
    [Parameter(Mandatory = $true)][string] $ScenarioRoot,
    [string] $ContractsVersion = '2026.10.3-20261029112040813496109',
    [string] $CoreVersion = '2026.10.3-20261101231129',
    [string] $SharedFeed = 'D:\lowlandtech\packages'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Add-Type -AssemblyName System.IO.Compression.FileSystem

if (!$RepositoryRoot) {
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

$version = '2026.10.3-' + [DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')
$contractsPackage = Join-Path $SharedFeed "LowlandTech.Components.Core.$ContractsVersion.nupkg"
if (!(Test-Path -LiteralPath $contractsPackage)) {
    throw "The contracts package '$contractsPackage' is not published in '$SharedFeed'."
}
Copy-Item -LiteralPath $contractsPackage -Destination (Join-Path $feed (Split-Path $contractsPackage -Leaf)) -Force

$env:LowlandTechCoreVersion = $CoreVersion
Write-Step "packing batch $version"
& (Join-Path $RepositoryRoot 'eng/build-local.ps1') `
    -FeedPath $feed `
    -Version $version `
    -ContractsVersion $ContractsVersion `
    -Projects @(
        'src/lib/LowlandTech.Components.csproj',
        'src/blocks/LowlandTech.Components.Blocks.csproj') *>&1 |
    Tee-Object -FilePath $logPath -Append | Out-Null
$packExit = $LASTEXITCODE

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

$publishExit = $null
$runExit = $null
$observations = @{}
$owned = @()
if ($packExit -eq 0) {
    Write-Step 'publishing the consumer trimmed'
    $output = & dotnet publish (Join-Path $consumer 'Consumer.csproj') -c Release -r win-x64 --self-contained true `
        -p:PublishTrimmed=true -p:TrimmerSingleWarn=false "-p:ComponentsBatchVersion=$version" `
        -o $publish 2>&1
    $output | Tee-Object -FilePath $logPath -Append | Out-Null
    $publishExit = $LASTEXITCODE

    $owned = @($output | Where-Object { "$_" -match 'warning IL\d+' -and "$_" -match 'LowlandTech\.Components' } |
        ForEach-Object { "$_".Trim() } | Sort-Object -Unique)

    $exe = Join-Path $publish 'Cpkg0006.Consumer.exe'
    if ($publishExit -eq 0 -and (Test-Path -LiteralPath $exe)) {
        $runOutput = & $exe 2>&1
        $runExit = $LASTEXITCODE
        $runOutput | Tee-Object -FilePath $logPath -Append | Out-Null
        foreach ($line in $runOutput) {
            if ("$line" -match '^([A-Z_]+)=(.*)$') { $observations[$matches[1]] = $matches[2] }
        }
        Write-Step "trimmed consumer exit $runExit with $($observations.Count) observations"
    }
}

# The browser host needs the WebAssembly toolchain; record its state instead of claiming it.
$workloads = (& dotnet workload list 2>&1) -join "`n"
$wasmWorkload = $workloads -match 'wasm-tools'

$result = [pscustomobject]@{
    version         = $version
    packExit        = $packExit
    publishExit     = $publishExit
    runExit         = $runExit
    observations    = $observations
    ownedWarnings   = $owned
    ownedCount      = $owned.Count
    wasmWorkload    = [bool]$wasmWorkload
}
$result | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resultPath -Encoding utf8
Write-Step "result written to $resultPath"
$result | ConvertTo-Json -Depth 6
