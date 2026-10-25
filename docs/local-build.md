# Local NuGet builds

The shared package destination is `D:\lowlandtech\packages`. `NuGet.Config`
selects this folder and nuget.org. Merge required private sources/mappings when
copying that configuration to another repository.

## Current status

Shared branding and `year.month.major[-minor]` versioning are configured for
Components Core, controls and Blocks. Core is independently packable.

The upstream Core compatibility is resolved. `LowlandTech.Core
2026.10.3-20261025011050827978557` consumes Components.Core instead of
defining the moved types, and the compatible batch of the workspace is
published under `2026.10.1-20261024235136415`: Components Core, controls,
Blocks and the two optional feature packages. `CPKG0001-US01-SC01` closed with
that repair; its record is
[docs/evidence/dependency-closure-repair.md](evidence/dependency-closure-repair.md).

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
$env:LowlandTechCoreVersion = '2026.10.3-20261025011050827978557'
./eng/build-local.ps1
```

The script builds the full solution in Release, including the existing npm
Tailwind build, packs the five manifest-selected projects, then copies the batch
to the shared feed without overwriting existing versions. It fails before packing
when the prerequisite is missing or is the known-incompatible 2026.5.29 version.
It does not publish a partial batch after a failed build.

Without the variable the standalone build exits 1 with one error and no warning
in under a second, because the `RequireCompatibleCorePackage` target of
`src/lib/LowlandTech.Components.csproj` fails before compilation. That guard
stays; it is the recorded baseline of every verification run of this
repository.

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

## The optional feature packages

Two feature packages are opt-in: a consumer references them only when it needs
their surface, and they never enter the dependency graph of the three base
packages.

| Package | Adds | Depends on |
|---|---|---|
| `LowlandTech.Components.Features.Session` | the session blocks of active plugin owners | Components Core, Components Blocks |
| `LowlandTech.Components.Features.Pages` | the plugin pages of active plugin owners | Components Core, `LowlandTech.Plugins.Domain` |

Both carry the same version as the batch that publishes them. The page feature
package references the Plugins *domain* package and not
`LowlandTech.Plugins.Blazor`, so it never pulls the ASP.NET Core shared
framework into a consumer of the base packages. The route gateway that renders
the adapted pages is a host concern, so the host references the Plugins Blazor
package itself.

## Consuming the shared feed

A consumer outside this repository reads the shared feed with a `NuGet.Config`
that selects it, and pins the exact version of the batch:

```xml
<add key="lowlandtech-local" value="..\packages" />
<PackageReference Include="LowlandTech.Components.Features.Pages" Version="[2026.10.1-20261024235136415]" />
```

The closure of the published packages stays inside the workspace feed plus
nuget.org for the Microsoft packages. `LowlandTech.Plugins.Domain` carries no
dependency of its own, and the `Microsoft.AspNetCore.Components` of the page
feature package comes from nuget.org exactly as the
`Microsoft.AspNetCore.Components.Web` of the controls package does.

The shared feed consumption proof under
`src/test/CPGS0001.Integration/feed-consumer/` restores the published page
feature package from the shared feed, compiles against it without the
Components source tree and drives the documented path of a host. Its record is
[docs/evidence/page-adapter-feed-consumption.md](evidence/page-adapter-feed-consumption.md).

The acceptance scenario of a scope restores a private feed that the packaging
harness packs, so that a closed gate never depends on the mutable state of the
shared feed. The two are complementary: the harness proves the behavior of a
candidate build, the shared feed proof proves that the published identities of
that build reach a consumer.

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

## Verification on 2026-10-25

- Baseline build without `LowlandTechCoreVersion`: exit 1 with one error and no
  warning in 0.59 s. The `RequireCompatibleCorePackage` guard of
  `src/lib/LowlandTech.Components.csproj` fails before compilation and is
  unchanged.
- Release solution build with
  `LowlandTechCoreVersion=2026.10.3-20261025011050827978557`: exit 0, 0 errors.
- Unit suite: 2365 passed, 0 failed, 0 skipped.
- Browser suite: 53 passed, 0 failed, 0 skipped.
- The shared feed carries the batch `2026.10.1-20261024235136415` with the five
  manifest-selected packages, and an external consumer restores the page
  feature package from it and compiles against it: exit 0. The record is
  [docs/evidence/page-adapter-feed-consumption.md](evidence/page-adapter-feed-consumption.md).
