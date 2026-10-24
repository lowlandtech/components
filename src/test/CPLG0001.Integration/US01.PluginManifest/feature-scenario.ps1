#Requires -Version 5.1
<#
.SYNOPSIS
    CPLG0001 feature packaging scenario harness.

.DESCRIPTION
    Produces the artifacts the CPLG0001 acceptance criteria assert against:

      1. packs LowlandTech.Components.Core, LowlandTech.Components,
         LowlandTech.Components.Blocks and the opt-in
         LowlandTech.Components.Features.Session package from this repository
         into a private package feed;
      2. copies the selected upstream Core package into the same feed, so the
         feed carries the compatible Core the packages were built against;
      3. writes a JSON manifest that names every produced package.

    The acceptance assertions live in src/test; this script only performs the
    mechanical work, so the inspection always runs against packages that were
    packed from the current source.

    The script never touches the network: the feed is a local folder.

.PARAMETER RepositoryRoot
    Repository to pack from. Defaults to the root above this script.

.PARAMETER ScenarioRoot
    Directory that receives the feed and the manifest. Created if missing; the
    caller owns its lifetime.

.PARAMETER CoreVersion
    Version of the upstream LowlandTech.Core package to build against. When
    omitted, the highest version in the local feed whose nuspec depends on
    LowlandTech.Components.Core is used. That dependency is the contract that
    separates a compatible Core package from one that duplicates the moved
    Components.Core types.

.OUTPUTS
    A JSON manifest at <ScenarioRoot>\scenario.json describing every artifact.
#>
[CmdletBinding()]
param(
    [string] $RepositoryRoot,
    [string] $ScenarioRoot,
    [string] $CoreVersion
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

function Find-RepositoryRoot {
    $dir = $PSScriptRoot
    while ($dir) {
        if (Test-Path -LiteralPath (Join-Path $dir 'components.slnx')) { return $dir }
        $parent = Split-Path -Parent $dir
        if ($parent -eq $dir -or [string]::IsNullOrEmpty($parent)) { break }
        $dir = $parent
    }
    throw "components.slnx not found above '$PSScriptRoot'."
}

function Get-LocalFeedPath([string] $root) {
    $configPath = Join-Path $root 'NuGet.Config'
    if (-not (Test-Path -LiteralPath $configPath)) { throw "NuGet.Config not found at '$configPath'." }
    $config = [xml](Get-Content -LiteralPath $configPath -Raw)
    foreach ($source in $config.configuration.packageSources.add) {
        $key = [string]$source.key
        $value = [string]$source.value
        if ($key -match 'local') {
            if (-not [System.IO.Path]::IsPathRooted($value)) { $value = Join-Path $root $value }
            return (Resolve-Path -LiteralPath $value).Path
        }
    }
    throw "No local package source configured in '$configPath'."
}

function Get-NuspecDependencies([string] $nupkgPath) {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($nupkgPath)
    try {
        $entry = $zip.Entries | Where-Object { $_.FullName -like '*.nuspec' } | Select-Object -First 1
        if (-not $entry) { return @() }
        $reader = New-Object System.IO.StreamReader($entry.Open())
        try { $text = $reader.ReadToEnd() } finally { $reader.Dispose() }
    }
    finally { $zip.Dispose() }

    $xml = [xml]$text
    $dependencies = @()
    foreach ($node in $xml.SelectNodes("//*[local-name()='dependency']")) {
        $version = $node.GetAttribute('version') -replace '^\[', '' -replace '\]$', ''
        $dependencies += [pscustomobject]@{ Id = $node.GetAttribute('id'); Version = $version }
    }
    return $dependencies
}

function Resolve-CompatibleCore([string] $feedPath, [string] $requested) {
    if (-not (Test-Path -LiteralPath $feedPath)) { throw "Local feed '$feedPath' does not exist." }

    $compatible = @()
    foreach ($file in Get-ChildItem -LiteralPath $feedPath -Filter 'LowlandTech.Core.*.nupkg') {
        $version = ($file.Name -replace '^LowlandTech\.Core\.', '') -replace '\.nupkg$', ''
        if ($requested -and $version -ne $requested) { continue }

        $dependencies = Get-NuspecDependencies $file.FullName
        $componentsCore = $dependencies | Where-Object { $_.Id -eq 'LowlandTech.Components.Core' } | Select-Object -First 1
        if ($componentsCore) {
            $compatible += [pscustomobject]@{
                Version = $version
                ComponentsCoreVersion = $componentsCore.Version
                Package = $file.FullName
            }
        }
    }

    if ($compatible.Count -eq 0) {
        throw "No LowlandTech.Core package in '$feedPath' depends on LowlandTech.Components.Core. " +
              "A compatible Core package is required before the Components packages can be packed."
    }

    $sorted = $compatible | Sort-Object -Property @{ Expression = { [version](($_.Version -split '-')[0]) } }, @{ Expression = { $_.Version } }
    return @($sorted)[-1]
}

function Invoke-Step([string] $filePath, [string[]] $arguments, [string] $workingDirectory, [string] $logPath) {
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $filePath
    $psi.Arguments = (($arguments | ForEach-Object {
        if ($_ -match '[\s"]') { '"' + ($_ -replace '"', '\"') + '"' } else { $_ }
    }) -join ' ')
    $psi.WorkingDirectory = $workingDirectory
    $psi.UseShellExecute = $false
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true

    $process = [System.Diagnostics.Process]::Start($psi)
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    $process.WaitForExit()
    $stdout = $stdoutTask.Result
    $stderr = $stderrTask.Result

    $entry = "`n=== $filePath $($arguments -join ' ') ===`n$stdout`n$stderr`n[exit $($process.ExitCode)]`n"
    Add-Content -LiteralPath $logPath -Value $entry -Encoding UTF8
    return [pscustomobject]@{ ExitCode = $process.ExitCode; Output = $stdout + $stderr }
}

if (-not $RepositoryRoot) { $RepositoryRoot = Find-RepositoryRoot }
if (-not $ScenarioRoot) { throw 'ScenarioRoot is required.' }

$ScenarioRoot = [System.IO.Path]::GetFullPath($ScenarioRoot)
New-Item -ItemType Directory -Force -Path $ScenarioRoot | Out-Null

$feedDirectory = Join-Path $ScenarioRoot 'feed'
$logPath = Join-Path $ScenarioRoot 'scenario.log'
$manifestPath = Join-Path $ScenarioRoot 'scenario.json'

New-Item -ItemType Directory -Force -Path $feedDirectory | Out-Null
Set-Content -LiteralPath $logPath -Value '' -Encoding UTF8

$localFeed = Get-LocalFeedPath $RepositoryRoot
$core = Resolve-CompatibleCore $localFeed $CoreVersion
$coreVersion = $core.Version

# The upstream Core package pins the Components.Core version it was built
# against, so the Components packages are packed under exactly that version.
$packageVersion = $core.ComponentsCoreVersion

# ---------------------------------------------------------------- packages ---
$projects = @(
    @{ Id = 'LowlandTech.Components.Core'; Path = 'src/core/LowlandTech.Components.Core.csproj' },
    @{ Id = 'LowlandTech.Components'; Path = 'src/lib/LowlandTech.Components.csproj' },
    @{ Id = 'LowlandTech.Components.Blocks'; Path = 'src/blocks/LowlandTech.Components.Blocks.csproj' },
    @{ Id = 'LowlandTech.Components.Features.Session'; Path = 'src/features/LowlandTech.Components.Features.Session/LowlandTech.Components.Features.Session.csproj' }
)

$packFailures = @()
$packages = [ordered]@{}
foreach ($project in $projects) {
    $projectPath = Join-Path $RepositoryRoot $project.Path
    $result = Invoke-Step 'dotnet' @(
        'pack', $projectPath,
        '-c', 'Release',
        '-o', $feedDirectory,
        "-p:Version=$packageVersion",
        "-p:LowlandTechCoreVersion=$coreVersion",
        '-p:SkipTailwindBuild=true',
        '--nologo'
    ) $RepositoryRoot $logPath

    if ($result.ExitCode -ne 0) {
        $packFailures += $project.Id
        continue
    }

    $packed = Get-ChildItem -LiteralPath $feedDirectory -Filter "$($project.Id).$packageVersion.nupkg" |
        Select-Object -First 1
    if (-not $packed) {
        $packFailures += "$($project.Id) (not produced)"
        continue
    }

    $packages[$project.Id] = $packed.FullName
}

if ($packFailures.Count -gt 0) {
    throw "dotnet pack failed for: $($packFailures -join ', '). See '$logPath'."
}

# The compatible upstream Core travels with the feed, so the feed describes the
# exact closure the packages were built against.
Copy-Item -LiteralPath $core.Package -Destination $feedDirectory -Force

$manifest = [ordered]@{
    repositoryRoot = $RepositoryRoot
    scenarioRoot   = $ScenarioRoot
    feedDirectory  = $feedDirectory
    coreVersion    = $coreVersion
    packageVersion = $packageVersion
    packages       = $packages
    log            = $logPath
}

$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
Write-Output $manifestPath
