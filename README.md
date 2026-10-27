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
