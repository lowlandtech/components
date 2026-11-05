# LowlandTech Components

A Blazor component library and interactive component explorer for .NET 10. The demo includes 90 components across 12 groups, from forms and navigation to data grids, session surfaces, and work dashboards.

**[Explore the components](https://components.lowlandtech.com)** · **[Browse issues](https://github.com/lowlandtech/components/issues)** · **[Hosting and publishing](HOSTING.md)**

## Repository layout

| Project | Purpose |
| --- | --- |
| `src/core` | Shared component contracts and utilities |
| `src/lib` | Razor component library, styling, and browser interop |
| `src/blocks` | Higher-level component compositions |
| `src/stories` | Interactive examples and Showcode pages |
| `src/app` | Blazor WebAssembly demo host |
| `src/test` | Component acceptance tests |
| `src/e2e` | Browser acceptance tests |

## Development

`develop` is the default integration branch. Open pull requests against it. `main` retains the published source baseline; `gh-pages` contains the generated public demo.

Use the .NET 10 SDK and Node.js/npm. The [local NuGet build setup](docs/workitems/done/CPKG0001.US01.SC01.independent-package-consumption.md) provides shared metadata, calendar versions and the local feed workflow. Components Core builds independently; controls and Blocks require a compatible upstream Core package. The older 2026.5.29 package duplicates moved types and cannot be used. See the local build guide for the prerequisite and verification limits, plus [the extraction notes](MIGRATION.md) and [publishing instructions](HOSTING.md).

Build the demo CSS from this repository:

```powershell
npm --prefix src/lib/wwwroot ci
npm --prefix src/lib/wwwroot run build:css
npm --prefix src/app/wwwroot ci
npm --prefix src/app/wwwroot run css:build
```

From the compatible Foundry checkout:

```powershell
dotnet run --project src/components/app/LowlandTech.Components.App.csproj
```

The live demo is a static GitHub Pages deployment. Changes to source branches do not automatically rebuild it yet.

## Workitems

The sole planning board is [docs/workitems/README.md](docs/workitems/README.md). Source workitems hold specifications, dependencies, issue links and delivery evidence. Shared operating instructions and the Components trigger live under D:/lowlandtech/.skills and D:/lowlandtech/.chatbox/components/README.md. Native build-local and Publish-Pages tooling remain repository-owned.

## Trimming

The five packages declare themselves trimmable, so a consumer that publishes trimmed
trims them instead of copying them byte for byte. Measured against
`2026.10.3-20261101114716139983370`: `LowlandTech.Components` falls from 1,273,344 to
91,648 bytes and `LowlandTech.Components.Blocks` from 1,783,296 to 31,232 bytes.

The packages publish three declarations a consumer has to keep in view:

| Declaration | Member | What the consumer keeps |
| --- | --- | --- |
| `IsTrimmable` | the assembly | the linker may trim the assembly |
| `DynamicallyAccessedMembers(PublicParameterlessConstructor)` | `ComponentDescriptor.ComponentType` | the public parameterless constructor of a component type handed back through a descriptor |
| `DynamicallyAccessedMembers(PublicProperties \| PublicFields)` | `PropertyGridAttribute.OptionsProvider` | the public properties and fields of an options provider |

**Do not set `TrimMode=partial` for a consumer of these packages.** With partial trimming
the SDK drops the trimmed package assembly from `deps.json`, and the host fails with
`Could not load file or assembly 'LowlandTech.Components.Blocks, Version=2026.10.3.0, ...'`
even though the file is in the publish output. The `.NET 10` default (`TrimMode` unset,
which is `full`) loads and runs; `partial` is the default only below `.NET 8`.

The packages cannot preserve a consumer's own types. A consumer that reflects over its
own models or components keeps those members itself.

### Consuming from a local feed in a container

A container build cannot see a folder feed on the host, so it restores from a package server
instead. A plain-HTTP server (a local BaGet, for example) needs the insecure connection
declared **on the source itself** — `dotnet restore --source` refuses an HTTP URL with
`NU1302` no matter how the URL is written:

```xml
<add key="local" value="http://localhost:5000/v3/index.json" allowInsecureConnections="true" />
```