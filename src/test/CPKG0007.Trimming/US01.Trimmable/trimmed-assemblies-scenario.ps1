#Requires -Version 5.1
<#
.SYNOPSIS
    CPKG0007 trimmed-assembly harness.

.DESCRIPTION
    Packs this repository's published projects into a private feed, publishes a consumer
    trimmed against that batch, and records what the trimmer did with the shared
    assemblies:

      1. the shipped size of each assembly inside its package;
      2. the trimmed size of each assembly in the publish output;
      3. the runtime exit and output of the trimmed consumer;
      4. the DynamicallyAccessedMembers declarations of the two public type members,
         read from the packed assembly;
      5. a package-only restore of the batch with the current Core.

    The private feed keeps the shared feed untouched, so the probe never publishes.

.PARAMETER ScenarioRoot
    Directory that receives the feed, the consumer and the publish output.

.PARAMETER ContractsVersion
    The published LowlandTech.Components.Core generation the current Core pins.

.PARAMETER CoreVersion
    The upstream LowlandTech.Core package the batch pins exactly.
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
$restore = Join-Path $ScenarioRoot 'restore'
$logPath = Join-Path $ScenarioRoot 'probe.log'
$resultPath = Join-Path $ScenarioRoot 'probe-result.json'
New-Item -ItemType Directory -Path $feed, $consumer, $publish, $restore -Force | Out-Null
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
$packOutput = & (Join-Path $RepositoryRoot 'eng/build-local.ps1') `
    -FeedPath $feed `
    -Version $version `
    -ContractsVersion $ContractsVersion `
    -Projects @(
        'src/lib/LowlandTech.Components.csproj',
        'src/blocks/LowlandTech.Components.Blocks.csproj') *>&1 |
    Tee-Object -FilePath $logPath -Append | Out-Null
$packExit = $LASTEXITCODE
$packWarnings = @($packOutput | Where-Object { "$_" -match 'warning IL\d+' }).Count

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
$runOutput = $null
if ($packExit -eq 0) {
    Write-Step 'publishing the consumer trimmed'
    $output = & dotnet publish (Join-Path $consumer 'Consumer.csproj') -c Release -r win-x64 --self-contained true `
        -p:PublishTrimmed=true "-p:ComponentsBatchVersion=$version" -p:LowlandTechCoreVersion=$CoreVersion `
        -o $publish 2>&1
    $output | Tee-Object -FilePath $logPath -Append | Out-Null
    $publishExit = $LASTEXITCODE

    $exe = Join-Path $publish 'Cpkg0007.Consumer.exe'
    if ($publishExit -eq 0 -and (Test-Path -LiteralPath $exe)) {
        $runOutput = (& $exe 2>&1) -join "`n"
        $runExit = $LASTEXITCODE
        Write-Step "trimmed consumer exit $runExit"
    }
}

# The shipped size of each assembly, from the package the batch published.
$shipped = @{}
foreach ($id in @('LowlandTech.Components', 'LowlandTech.Components.Blocks')) {
    $package = Join-Path $feed "$id.$version.nupkg"
    if (!(Test-Path -LiteralPath $package)) { continue }
    $archive = [System.IO.Compression.ZipFile]::OpenRead($package)
    try {
        $entry = $archive.Entries | Where-Object { $_.FullName -eq "lib/net10.0/$id.dll" } | Select-Object -First 1
        if ($entry) { $shipped[$id] = $entry.Length }
    }
    finally { $archive.Dispose() }
}

# The trimmed size of each assembly, from the publish output.
$trimmed = @{}
foreach ($id in @('LowlandTech.Components', 'LowlandTech.Components.Blocks')) {
    $file = Join-Path $publish "$id.dll"
    if (Test-Path -LiteralPath $file) { $trimmed[$id] = (Get-Item -LiteralPath $file).Length }
}

# The declarations the consumer needs, read from the packed assembly.
$annotations = @{}
$assemblyFile = Join-Path $ScenarioRoot 'LowlandTech.Components.dll'
$package = Join-Path $feed "LowlandTech.Components.$version.nupkg"
if (Test-Path -LiteralPath $package) {
    $archive = [System.IO.Compression.ZipFile]::OpenRead($package)
    try {
        $entry = $archive.Entries | Where-Object { $_.FullName -eq 'lib/net10.0/LowlandTech.Components.dll' } | Select-Object -First 1
        if ($entry) { [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $assemblyFile, $true) }
    }
    finally { $archive.Dispose() }
}
if (Test-Path -LiteralPath $assemblyFile) {
    $assembly = [System.Reflection.Assembly]::LoadFrom($assemblyFile)
    $dam = [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute]
    $descriptorProperty = $assembly.GetType('LowlandTech.Components.ComponentDescriptor').GetProperty('ComponentType')
    $providerProperty = $assembly.GetType('LowlandTech.Components.PropertyGridAttribute').GetProperty('OptionsProvider')
    $annotations['ComponentDescriptor.ComponentType'] = (($descriptorProperty.GetCustomAttributes($dam, $false) | Select-Object -First 1).MemberTypes).ToString()
    $annotations['PropertyGridAttribute.OptionsProvider'] = (($providerProperty.GetCustomAttributes($dam, $false) | Select-Object -First 1).MemberTypes).ToString()
    $annotations['IsTrimmable'] = [bool]($assembly.GetCustomAttributesData() | Where-Object {
        $_.AttributeType.Name -eq 'AssemblyMetadataAttribute' -and "$($_.ConstructorArguments[0].Value)" -eq 'IsTrimmable'
    })
}

# A package-only restore of the batch with the current Core.
Set-Content -LiteralPath (Join-Path $restore 'pins.csproj') -Value @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup>
    <PackageReference Include="LowlandTech.Core" Version="[$CoreVersion]" />
    <PackageReference Include="LowlandTech.Components.Core" Version="[$ContractsVersion]" />
    <PackageReference Include="LowlandTech.Components" Version="[$version]" />
    <PackageReference Include="LowlandTech.Components.Blocks" Version="[$version]" />
  </ItemGroup>
</Project>
"@ -Encoding utf8
Copy-Item -LiteralPath (Join-Path $consumer 'NuGet.config') -Destination (Join-Path $restore 'NuGet.config') -Force
$restoreOutput = (& dotnet restore (Join-Path $restore 'pins.csproj') 2>&1) -join "`n"
$restoreExit = $LASTEXITCODE
$restoreOutput | Tee-Object -FilePath $logPath -Append | Out-Null

$result = [pscustomobject]@{
    version     = $version
    packExit    = $packExit
    packWarnings = $packWarnings
    publishExit = $publishExit
    runExit     = $runExit
    runOutput   = $runOutput
    shipped     = $shipped
    trimmed     = $trimmed
    annotations = $annotations
    restoreExit = $restoreExit
    restoreText = $restoreOutput
}
$result | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $resultPath -Encoding utf8
Write-Step "result written to $resultPath"
$result | ConvertTo-Json -Depth 6
