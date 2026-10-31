[CmdletBinding()]
param(
    [string]$FeedPath = 'D:\lowlandtech\packages',
    [string]$Version,
    [string]$Configuration = 'Release',
    [string]$ManifestPath,
    [string[]]$Projects,
    [switch]$CompleteBatch,
    [string]$ContractsVersion,
    # Captures a project that the caller left as a separate positional argument,
    # so the tool can refuse with one diagnostic instead of a binding error.
    [Parameter(ValueFromRemainingArguments = $true)][string[]]$Unbound
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
# A caller that omits the selection gets the manifest. A caller that supplies an
# empty selection asked for nothing, which is refused instead of silently packing
# the whole manifest.
$supportedInvocation = 'pwsh -NoProfile -Command "& eng/build-local.ps1 -Projects @(''src/lib/LowlandTech.Components.csproj'',''src/blocks/LowlandTech.Components.Blocks.csproj'')"'
if ($Unbound -and $Unbound.Count -gt 0) {
    throw "The project selection left '$($Unbound -join ' ')', which is not a parameter of this tool. Pass the selection as a PowerShell array through -Command, not through -File, for example: $supportedInvocation"
}
# An advanced script binds named parameters only, so a stray argument of a -File
# invocation is dropped instead of reported: the script would pack fewer projects
# than the caller named. The raw command line still carries it, so the counts are
# compared for a -File invocation and the case is refused by name.
$rawArguments = [Environment]::GetCommandLineArgs()
if (($rawArguments -contains '-File') -and ($rawArguments -contains '-Projects')) {
    $rawSelection = @()
    $start = [Array]::IndexOf($rawArguments, '-Projects') + 1
    for ($i = $start; $i -lt $rawArguments.Count; $i++) {
        if ($rawArguments[$i] -match '^-') { break }
        $rawSelection += $rawArguments[$i]
    }
    if ($rawSelection.Count -gt @($Projects).Count) {
        $left = @($rawSelection | Select-Object -Skip @($Projects).Count)
        throw "The project selection left '$($left -join ' ')', which is not a parameter of this tool and was dropped by the -File binding. Pass the selection as a PowerShell array through -Command, not through -File, for example: $supportedInvocation"
    }
}
if (-not $PSBoundParameters.ContainsKey('Projects')) { $Projects = @($manifest.projects) }
$projects = @($Projects)
if ($projects.Count -eq 0) { throw 'Select at least one packable project; an empty -Projects selection packs nothing.' }
$joined = @($projects | Where-Object { $_ -match '[,;]' })
if ($joined.Count -gt 0) {
    throw "The project selection '$($joined -join ', ')' is a joined string, not a list. Pass the selection as a PowerShell array through -Command, for example: $supportedInvocation"
}
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
# The build restores the referenced contracts project under the bare package
# version, so the bracket cannot reach the build; only the pack emits it. The two
# properties therefore carry the same generation in the two forms the steps need.
$contractBuildProperties = @()
$contractPackProperties = @()
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
    $contractBuildProperties = @("-p:ComponentsContractsVersion=$ContractsVersion")
    $contractPackProperties = @("-p:ComponentsContractsVersion=[$ContractsVersion]")
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
# Two batches of this repository build the same projects, so a build and a pack
# that overlap collide on the intermediate output of the shared projects. The lock
# serializes the build and the pack across processes and lanes; a batch that finds
# the lock held waits for it instead of reporting an IO error of the build.
$lockPath = Join-Path $root 'artifacts/packing.lock'
New-Item -ItemType Directory -Path (Split-Path $lockPath -Parent) -Force | Out-Null
$lock = $null
for ($attempt = 0; $attempt -lt 900; $attempt++) {
    try {
        $lock = [System.IO.File]::Open($lockPath, [System.IO.FileMode]::OpenOrCreate, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
        break
    }
    catch [System.IO.IOException] {
        Start-Sleep -Milliseconds 500
    }
}
if ($null -eq $lock) { throw "The packing lock '$lockPath' stayed held for another batch; retry when the other batch finished." }

Push-Location $root
try {
    # The batch publishes the manifest's projects. The solution also carries the
    # test, story and host projects that the batch does not publish, and
    # rebuilding them writes into the output directory a running test host
    # holds, which fails the publication with MSB3027. The batch builds only
    # what it publishes, and the solution stays the validated repository build
    # unit without being the build input of the batch.
    foreach ($project in $projects) {
        Invoke-DotNet (@('build', (Join-Path $root $project), '-c', $Configuration, "-p:Version=$Version") + $contractBuildProperties + @('-m:1'))
    }
    foreach ($project in $projects) {
        Invoke-DotNet (@('pack', $project, '-c', $Configuration, '--no-build', "-p:Version=$Version") + $contractPackProperties + @('-o', $staging))
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
    if ($null -ne $lock) { $lock.Dispose() }
}
