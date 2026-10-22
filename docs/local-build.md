# Local NuGet builds

The shared package destination is `D:\lowlandtech\packages`. `NuGet.Config`
selects this folder and nuget.org. Merge required private sources/mappings when
copying that configuration to another repository.

## Current status

Shared branding and `year.month.major[-minor]` versioning are configured for
Components Core, controls and Blocks. Core is independently packable. The full
controls/Blocks build is blocked by upstream Core compatibility; it has not been
published or accepted as complete. See CPKG0001-US01-SC01 for the wider package
consumption and browser acceptance work, which remains open.

The former `../../core/LowlandTech.Core.csproj` reference is replaced by an exact
PackageReference selected through `LowlandTechCoreVersion`. The available
LowlandTech.Core 2026.5.29 archive defines Icons, BlockIntent and related types
also present in Components.Core. A real compile against that archive failed
with CS0433 duplicate-type errors. No aliases, duplicate contracts or changes to
public type ownership were used to hide this incompatibility.

A new upstream Core package must consume Components.Core instead of defining
those moved types. Publish that package under a new identity/version, then select
its verified version through the environment (inherited by the build script):

```powershell
$env:LowlandTechCoreVersion = 'VERIFIED-COMPATIBLE-VERSION'
./eng/build-local.ps1
```

The script builds the full solution in Release, including the existing npm
Tailwind build, packs the three manifest-selected projects, then copies the batch
to the shared feed without overwriting existing versions. It fails before packing
when the prerequisite is missing or is the known-incompatible 2026.5.29 version.
It does not publish a partial batch after a failed build. Existing standalone
test fixture resource gaps may require repair after Core compatibility is resolved.

## Versions and package properties

The base is `2026.10.1`. The optional numeric suffix is a NuGet prerelease, so
`2026.10.1-2` sorts before `2026.10.1`. Default local builds append a unique numeric
UTC timestamp/random suffix. To choose a version explicitly:

```powershell
./eng/build-local.ps1 -Version 2026.10.1-2
```

`-FeedPath` and `-Configuration` can override the destination and configuration.
Never rebuild different bytes under the same package version: NuGet caches them.
Staging lives in `artifacts/local-packages/<version>`. If publication is interrupted,
choose a new version; some files from a partial copy may remain in the feed.

`Directory.Build.props` centralizes author, company, product, URLs, MIT license,
icon, tags and version. `Directory.Build.targets` packs the checked-in icon,
license and README only for packable libraries and excludes Node/toolchain inputs
from static assets. Branding follows TinyTools via the Plugins setup; there are
no build dependencies on either checkout. The SDK supplies Git commit metadata.

## Build and consume Core independently

```powershell
dotnet pack src/core/LowlandTech.Components.Core.csproj -c Release -p:Version=2026.10.1-1 -o artifacts/core-feed
```

Copy the resulting archive into the shared feed without replacing an existing
file. Consumers merge NuGet.Config and use:

```xml
<PackageReference Include="LowlandTech.Components.Core" Version="[2026.10.1-1]" />
```

Once all producers build successfully, consume controls and Blocks the same way,
using the exact printed version. Internal project references stay in the producer;
other repositories replace source references with PackageReference. Controls and
Blocks are Razor libraries whose deployment must also be verified for _content
assets. No consumer repositories were automatically migrated.

## Verification on 2026-10-23

- Original Core archive SHA-512 matched the existing Plugins provenance:
  4afd43dc3bdd9d38b2e3af4b25d15f1014bfac1112b20997b252909505d491ecf18d96e6f8d3c15d04602ece70394ba39c63c27f03c08cd4d9fce8b359a6993e.
  That unchanged archive was copied to the local feed; the compile against it
  failed with duplicate types. It is not a compatible Components dependency.
- Full Release solution build: exit 1, duplicate-type errors; package batch not
  published. Existing test dependency AngleSharp 1.4.0 reported NU1902.
- Missing-Core prerequisite check: exit 1 with an explicit actionable error,
  before compilation/pack/publication.
- Core Release pack: exit 0, version 2026.10.1-1. Archive/assembly metadata and a
  separate package-only consumer passed (exit 0, zero warnings/errors). The consumer compiled Icons and FormFieldType from the package with only a local folder source. Core 2026.10.1-1 was published to D:/lowlandtech/packages without overwrite (exit 0).
- Runtime tests and browser acceptance are not claimed by this setup.
