#Requires -Version 5.1
<#
.SYNOPSIS
    CPKG0001 packaging scenario harness.

.DESCRIPTION
    Produces the artifacts that the CPKG0001 acceptance criteria assert against:

      1. packs LowlandTech.Components.Core, LowlandTech.Components and
         LowlandTech.Components.Blocks from this repository into a private
         package feed;
      2. copies the selected upstream Core package, the transitive dependency
         closure and the framework packages the consumer needs into the same
         feed, so the feed is self-contained;
      3. writes a temporary consumer application that references the three
         packages and nothing else;
      4. restores that consumer against the private feed only, with an
         isolated global-packages folder, so every resolved file lands inside
         the scenario root;
      5. optionally publishes the consumer.

    The acceptance assertions live in src/test and src/e2e; this script only
    performs the mechanical work so that both projects exercise the same
    packaging path.

    The script never touches the network: the feed is a local folder and the
    consumer restores from it alone.

.PARAMETER RepositoryRoot
    Repository to pack from. Defaults to the root above this script.

.PARAMETER ScenarioRoot
    Directory that receives the feed, the consumer and the publish output.
    Created if missing; the caller owns its lifetime.

.PARAMETER CoreVersion
    Version of the upstream LowlandTech.Core package to consume. When omitted,
    the highest version in the local feed whose nuspec depends on
    LowlandTech.Components.Core is used. That dependency is the contract that
    separates a compatible Core package from one that duplicates the moved
    Components.Core types.

.PARAMETER Publish
    Also publish the consumer application.

.PARAMETER ConsumerSource
    Directory of the consumer application, relative to the repository root.
    Defaults to the CPKG0001 consumer. Another acceptance that needs the same
    packaging path hands its own consumer over this parameter; the packaging
    work itself is identical.

.PARAMETER AdditionalPackages
    Project files, relative to the repository root, that are packed into the
    private feed next to the three base packages under the same version. Their
    own dependency closure travels with them, so a consumer may reference an
    optional feature package of this repository without reaching outside the
    private feed. The CPKG0001 acceptance packs nothing extra.

.PARAMETER FeedPackages
    Package ids whose every version in the local feed travels with the private
    feed. Use it for a package the consumer needs that is not a dependency of
    the packed Components projects, for example a host framework package. The
    CPKG0001 acceptance copies nothing extra.

.PARAMETER UsePublishedBatch
    Consume the published identities of the shared feed instead of packing the
    packages of this repository. The three base packages are copied from the
    shared feed under the version that the selected Core package pins, and the
    dependency closure of the copied packages travels with them, so the
    consumer still resolves everything from the private feed alone. The
    additional packages of this repository are not part of the published batch
    and are still packed. Without the switch the harness packs the packages of
    this repository, so the gate of the closed CPKG0001 scope is unchanged.

.OUTPUTS
    A JSON manifest at <ScenarioRoot>\scenario.json describing every artifact.
#>
[CmdletBinding()]
param(
    [string] $RepositoryRoot,
    [string] $ScenarioRoot,
    [string] $CoreVersion,
    [switch] $Publish,
    [string] $ConsumerSource = 'src/test/CPKG0001.Packaging/consumer',
    [string[]] $AdditionalPackages = @(),
    [string[]] $FeedPackages = @(),
    [switch] $UsePublishedBatch
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

function Get-NuspecId([string] $nupkgPath) {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($nupkgPath)
    try {
        $entry = $zip.Entries | Where-Object { $_.FullName -like '*.nuspec' } | Select-Object -First 1
        if (-not $entry) { return $null }
        $reader = New-Object System.IO.StreamReader($entry.Open())
        try { $text = $reader.ReadToEnd() } finally { $reader.Dispose() }
    }
    finally { $zip.Dispose() }

    $xml = [xml]$text
    return [string]$xml.package.metadata.id
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

function Get-PackageClosure {
    param([string[]] $assetsFiles)

    $closure = @{}
    foreach ($assetsFile in $assetsFiles) {
        if (-not (Test-Path -LiteralPath $assetsFile)) { continue }
        $assets = Get-Content -LiteralPath $assetsFile -Raw | ConvertFrom-Json
        if (-not $assets.libraries) { continue }
        foreach ($property in $assets.libraries.PSObject.Properties) {
            # The library key carries "<id>/<version>"; the value only carries the type.
            if ([string]$property.Value.type -ne 'package') { continue }
            $closure[$property.Name] = $true
        }
    }
    return @($closure.Keys)
}

function Get-AspNetCoreRuntimeVersion {
    $versions = @()
    foreach ($line in (& dotnet --list-runtimes)) {
        if ($line -match '^Microsoft\.AspNetCore\.App\s+(\S+)\s') { $versions += $Matches[1] }
    }
    if ($versions.Count -eq 0) { throw 'No Microsoft.AspNetCore.App runtime was found.' }
    $sorted = $versions | Sort-Object -Property @{ Expression = { [version](($_ -split '-')[0]) } }
    return @($sorted)[-1]
}

function Copy-CachedPackage([string] $globalPackages, [string] $id, [string] $version, [string] $feedDirectory) {
    # The global packages folder stores the id in lower case.
    $versionDirectory = Join-Path $globalPackages (Join-Path $id.ToLowerInvariant() $version)
    if (-not (Test-Path -LiteralPath $versionDirectory)) { return $false }

    $packageFile = Get-ChildItem -LiteralPath $versionDirectory -Filter '*.nupkg' | Select-Object -First 1
    if (-not $packageFile) { return $false }

    Copy-Item -LiteralPath $packageFile.FullName -Destination $feedDirectory -Force
    return $true
}

if (-not $RepositoryRoot) { $RepositoryRoot = Find-RepositoryRoot }
if (-not $ScenarioRoot) { throw 'ScenarioRoot is required.' }

$ScenarioRoot = [System.IO.Path]::GetFullPath($ScenarioRoot)
New-Item -ItemType Directory -Force -Path $ScenarioRoot | Out-Null

$feedDirectory = Join-Path $ScenarioRoot 'feed'
$packagesDirectory = Join-Path $ScenarioRoot 'packages'
$consumerDirectory = Join-Path $ScenarioRoot 'consumer'
$publishDirectory = Join-Path $ScenarioRoot 'publish'
$logPath = Join-Path $ScenarioRoot 'scenario.log'
$manifestPath = Join-Path $ScenarioRoot 'scenario.json'

New-Item -ItemType Directory -Force -Path $feedDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $packagesDirectory | Out-Null
Set-Content -LiteralPath $logPath -Value '' -Encoding UTF8

$localFeed = Get-LocalFeedPath $RepositoryRoot
$core = Resolve-CompatibleCore $localFeed $CoreVersion
$coreVersion = $core.Version

# The upstream Core package pins the Components.Core version it was built
# against, so the Components packages are packed under exactly that version.
# Any other version makes the consumer resolve a downgrade.
$packageVersion = $core.ComponentsCoreVersion

# ---------------------------------------------------------------- packages ---
$baseProjects = @(
    @{ Id = 'LowlandTech.Components.Core'; Path = 'src/core/LowlandTech.Components.Core.csproj' },
    @{ Id = 'LowlandTech.Components'; Path = 'src/lib/LowlandTech.Components.csproj' },
    @{ Id = 'LowlandTech.Components.Blocks'; Path = 'src/blocks/LowlandTech.Components.Blocks.csproj' }
)

# An optional feature package of this repository joins the batch under the same
# version, so a consumer may reference it from the private feed alone.
$additionalProjects = @()
foreach ($path in $AdditionalPackages) {
    $additionalProjects += @{
        Id   = [System.IO.Path]::GetFileNameWithoutExtension($path)
        Path = $path
    }
}

$projects = @($baseProjects) + @($additionalProjects)
$packageSource = 'packed'

if ($UsePublishedBatch) {
    # The published identities of the shared feed are the packages under test:
    # the published packages travel with the scenario feed instead of a fresh
    # pack of this repository. The additional packages of this repository are
    # not part of the published batch, so they are still packed.
    $packageSource = 'published'
    $missingPublished = @()
    foreach ($project in $baseProjects) {
        $published = Get-ChildItem -LiteralPath $localFeed -Filter "$($project.Id).$packageVersion.nupkg" |
            Select-Object -First 1
        if (-not $published) {
            $missingPublished += $project.Id
            continue
        }
        Copy-Item -LiteralPath $published.FullName -Destination $feedDirectory -Force
    }

    if ($missingPublished.Count -gt 0) {
        throw "The published batch '$packageVersion' of the shared feed is missing: " +
              "$($missingPublished -join ', '). See '$localFeed'."
    }

    $projects = @($additionalProjects)
}

$packFailures = @()
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

    if ($result.ExitCode -ne 0) { $packFailures += $project.Id }
}

if ($packFailures.Count -gt 0) {
    throw "dotnet pack failed for: $($packFailures -join ', '). See '$logPath'."
}

# The upstream Core package and the transitive closure travel with the feed so
# the consumer never needs a network source.
Copy-Item -LiteralPath $core.Package -Destination $feedDirectory -Force

$assetsFiles = @(
    (Join-Path $RepositoryRoot 'src/core/obj/project.assets.json'),
    (Join-Path $RepositoryRoot 'src/lib/obj/project.assets.json'),
    (Join-Path $RepositoryRoot 'src/blocks/obj/project.assets.json')
)

# The dependency closure of an additional package travels with it, so the
# consumer resolves it from the private feed alone.
foreach ($path in $AdditionalPackages) {
    $projectDirectory = Split-Path -Parent (Join-Path $RepositoryRoot $path)
    $assetsFiles += (Join-Path $projectDirectory 'obj/project.assets.json')
}

$closure = Get-PackageClosure $assetsFiles

if ($UsePublishedBatch) {
    # The dependency closure of the copied published packages travels with them,
    # so the consumer resolves them from the scenario feed alone.
    foreach ($project in $baseProjects) {
        $published = Get-ChildItem -LiteralPath $feedDirectory -Filter "$($project.Id).$packageVersion.nupkg" |
            Select-Object -First 1
        if (-not $published) { continue }
        foreach ($dependency in (Get-NuspecDependencies $published.FullName)) {
            $closure += "$($dependency.Id)/$($dependency.Version)"
        }
    }
}

$globalPackages = Join-Path $env:USERPROFILE '.nuget\packages'
$missing = @()
foreach ($entry in $closure) {
    $parts = $entry -split '/'
    if ($UsePublishedBatch) {
        # A package of the published batch already travels with the feed.
        $alreadyThere = Get-ChildItem -LiteralPath $feedDirectory -Filter "$($parts[0]).$($parts[1]).nupkg" |
            Select-Object -First 1
        if ($alreadyThere) { continue }
    }
    if (-not (Copy-CachedPackage $globalPackages $parts[0] $parts[1] $feedDirectory)) {
        $missing += $entry
    }
}

if ($missing.Count -gt 0) {
    throw "Transitive packages missing from the global cache: $($missing -join ', ')."
}

# A package the consumer needs that is not a dependency of the packed
# Components projects travels with the feed as well, so the consumer restores
# against the private feed alone. Every version of the requested id travels.
foreach ($id in $FeedPackages) {
    $copied = $false
    foreach ($file in Get-ChildItem -LiteralPath $localFeed -Filter '*.nupkg') {
        if ((Get-NuspecId $file.FullName) -ne $id) { continue }
        Copy-Item -LiteralPath $file.FullName -Destination $feedDirectory -Force
        $copied = $true
    }

    if (-not $copied) {
        throw "The package '$id' is not in the local feed '$localFeed'."
    }
}

# A web application resolves the ASP.NET Core framework reference through
# framework packages; they travel with the feed as well.
$aspNetVersion = Get-AspNetCoreRuntimeVersion
$frameworkPackages = @(
    "Microsoft.AspNetCore.App.Internal.Assets/$aspNetVersion",
    "Microsoft.AspNetCore.App.Runtime.win-x64/$aspNetVersion",
    "Microsoft.NETCore.App.Runtime.win-x64/$aspNetVersion"
)

$missingFramework = @()
foreach ($entry in $frameworkPackages) {
    $parts = $entry -split '/'
    if (-not (Copy-CachedPackage $globalPackages $parts[0] $parts[1] $feedDirectory)) {
        $missingFramework += $entry
    }
}

if ($missingFramework -contains "Microsoft.AspNetCore.App.Internal.Assets/$aspNetVersion") {
    throw "The framework package Microsoft.AspNetCore.App.Internal.Assets $aspNetVersion is not in the global cache."
}

# ---------------------------------------------------------------- consumer ---
$consumerSource = Join-Path $RepositoryRoot $ConsumerSource
if (-not (Test-Path -LiteralPath $consumerSource)) {
    throw "Consumer sources not found at '$consumerSource'."
}
Copy-Item -LiteralPath $consumerSource -Destination $consumerDirectory -Recurse -Force

$nugetConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="cpkg0001-offline-feed" value="$feedDirectory" />
  </packageSources>
</configuration>
"@
Set-Content -LiteralPath (Join-Path $consumerDirectory 'NuGet.Config') -Value $nugetConfig -Encoding UTF8

$consumerProject = Join-Path $consumerDirectory 'Consumer.csproj'
$previousPackages = $env:NUGET_PACKAGES
$env:NUGET_PACKAGES = $packagesDirectory
try {
    $restore = Invoke-Step 'dotnet' @(
        'restore', $consumerProject,
        "--configfile", (Join-Path $consumerDirectory 'NuGet.Config'),
        "-p:ComponentsPackageVersion=$packageVersion",
        '--nologo'
    ) $consumerDirectory $logPath

    $build = Invoke-Step 'dotnet' @(
        'build', $consumerProject,
        '-c', 'Release',
        '--no-restore',
        "-p:ComponentsPackageVersion=$packageVersion",
        '--nologo'
    ) $consumerDirectory $logPath

    $publishExitCode = 0
    if ($Publish) {
        $publishResult = Invoke-Step 'dotnet' @(
            'publish', $consumerProject,
            '-c', 'Release',
            '-o', $publishDirectory,
            '--no-restore',
            "-p:ComponentsPackageVersion=$packageVersion",
            '--nologo'
        ) $consumerDirectory $logPath
        $publishExitCode = $publishResult.ExitCode
    }
}
finally {
    $env:NUGET_PACKAGES = $previousPackages
}

$manifest = [ordered]@{
    repositoryRoot    = $RepositoryRoot
    scenarioRoot      = $ScenarioRoot
    feedDirectory     = $feedDirectory
    packagesDirectory = $packagesDirectory
    consumerDirectory = $consumerDirectory
    publishDirectory  = $publishDirectory
    assetsFile        = (Join-Path $consumerDirectory 'obj/project.assets.json')
    coreVersion       = $coreVersion
    packageVersion    = $packageVersion
    packageSource     = $packageSource
    usePublishedBatch = [bool] $UsePublishedBatch
    localFeed         = $localFeed
    restoreExitCode   = $restore.ExitCode
    buildExitCode     = $build.ExitCode
    publishExitCode   = $publishExitCode
    log               = $logPath
}

$manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
Write-Output $manifestPath

if ($restore.ExitCode -ne 0) { throw "dotnet restore failed for the consumer. See '$logPath'." }
if ($build.ExitCode -ne 0) { throw "dotnet build failed for the consumer. See '$logPath'." }
if ($publishExitCode -ne 0) { throw "dotnet publish failed for the consumer. See '$logPath'." }
