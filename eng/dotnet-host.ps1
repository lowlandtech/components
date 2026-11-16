<#
.SYNOPSIS
    Resolves a dotnet host that can satisfy this repository's pinned SDK.

.DESCRIPTION
    Windows composes PATH as the machine value followed by the user value, so a machine-wide
    dotnet outranks a newer .NET installation in the user profile even when the user entry
    comes first inside its own scope. This repository pins its SDK in global.json, and a host
    that cannot satisfy that pin fails every command with a misleading message about a missing
    application or assembly.

    Every script that shells out therefore asks this helper for a host instead of assuming
    the one on PATH works. The candidates are, in order: the host under DOTNET_ROOT (where a
    user-scope .NET installation lives), then the plain command from PATH. The first host that
    reports a version while the repository pin is in effect wins.

    The probe runs from the repository root, because global.json is what makes a host's answer
    meaningful: the same host can report different versions from different directories.
#>
function Resolve-DotnetHost {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)][string] $RepositoryRoot
    )

    $exe = if ($env:OS -eq 'Windows_NT') { 'dotnet.exe' } else { 'dotnet' }
    $candidates = @()
    if ($env:DOTNET_ROOT) { $candidates += (Join-Path $env:DOTNET_ROOT $exe) }
    $candidates += $exe

    $attempted = @()
    Push-Location -LiteralPath $RepositoryRoot
    try {
        foreach ($candidate in $candidates) {
            if ($candidate -ne $exe -and !(Test-Path -LiteralPath $candidate)) {
                $attempted += "$candidate (missing)"
                continue
            }

            $output = @()
            $exitCode = 1
            try {
                # Collect first: a pipeline that stops early would terminate the host and
                # leave its exit code meaningless.
                $output = & $candidate --version 2>$null
                $exitCode = $LASTEXITCODE
            }
            catch {
                $attempted += "$candidate (error)"
                continue
            }

            $version = @($output | Where-Object { $_ }) | Select-Object -First 1
            if ($exitCode -eq 0 -and $version) { return $candidate }
            $attempted += "$candidate ($version)"
        }
    }
    finally {
        Pop-Location
    }

    throw "No dotnet host satisfies the SDK pinned in '$RepositoryRoot/global.json'. Tried: $($attempted -join ', '). Install the pinned SDK or point DOTNET_ROOT at an installation that carries it."
}
