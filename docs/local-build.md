# Local build of Components

This repository targets `net10.0` and consumes one exact `LowlandTech.Core` generation. A local
build has to satisfy both pins, and neither is a machine default.

## The SDK pin

`global.json` pins SDK `10.0.100` with `rollForward: latestFeature` and prereleases disabled.

On a machine whose `PATH` names a newer host first, `dotnet --version` prints that host and then
refuses the pin:

```
11.0.100 [C:\Program Files\dotnet\sdk]
A compatible .NET SDK was not found. Requested SDK version: 10.0.100
```

Do not work around that by editing `global.json` or the target framework. Resolve a host that
satisfies the pin instead; the lane's scripts already do it through `eng/dotnet-host.ps1`:

```powershell
. .\eng\dotnet-host.ps1
$dotnet = Resolve-DotnetHost -RepositoryRoot $PWD   # e.g. %LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe
& $dotnet --version                                  # 10.0.404
```

## The Core pin

`src/lib/LowlandTech.Components.csproj` references `LowlandTech.Core` at exactly
`$(LowlandTechCoreVersion)` and refuses to build when the value is empty or the historical
sentinel `2026.5.29`, which duplicated Icons and the block intent contracts instead of consuming
`LowlandTech.Components.Core`:

```
Set LowlandTechCoreVersion to a verified Core package that references Components.Core
instead of defining its moved types.
```

The verified generation at the time of writing is **`2026.10.3-20261101231129`**, published to
the local feed `D:\lowlandtech\packages` by the Core producer. The producer workitem owns that
value; a local build selects it and never changes or publishes it.

## Build and test

Set the pin for the session and use the resolved host. MSBuild reads
`LowlandTechCoreVersion` from the environment, so no project file changes are needed:

```powershell
$env:LowlandTechCoreVersion = '2026.10.3-20261101231129'
. .\eng\dotnet-host.ps1
$dotnet = Resolve-DotnetHost -RepositoryRoot $PWD

& $dotnet build components.slnx -p:SkipTailwindBuild=true
& $dotnet test src/test/LowlandTech.Components.Tests.csproj -p:SkipTailwindBuild=true
```

`eng/build-local.ps1` packs the local feed with the same pin and refuses immutable-version
overwrite; packaging is a separate action, never part of a documentation or startup check.

The shared adapter (`eng/components/coordinator.ps1 -Command build|test|browser-test -Owner <id>`)
resolves the host and isolates output under `.tmp/<agent-id>/components`, but it does not forward
`-Version` into `-p:LowlandTechCoreVersion` yet, so pass the pin through the environment as above
until that adapter change lands.
