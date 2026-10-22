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

Use the .NET 10 SDK and Node.js/npm. The extraction currently retains a project dependency on Foundry core, so a standalone solution build needs that dependency resolved. Until it is packaged independently, use a compatible Foundry checkout with this repository's `src` tree under `src/components`. See [the extraction notes](MIGRATION.md) and [publishing instructions](HOSTING.md).

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

## Scrum workflow

For the planned optional Plugins integration, see the
[compatibility review](docs/plugin-recommendations.md) and
[local follow-up workitems](docs/workitems/README.md).

Use issues for backlog refinement, sprint planning, implementation, and review:

- **Type:** `type: epic`, `type: story`, `type: task`, `type: bug`, or `type: chore`.
- **State:** exactly one of `status: backlog`, `status: ready`, `status: in progress`, `status: review`, or `status: done`.
- **Priority:** `priority: critical`, `priority: high`, `priority: medium`, or `priority: low`.
- **Estimate:** `points: 1`, `points: 2`, `points: 3`, `points: 5`, `points: 8`, or `points: 13`; use `needs: estimate` until refined.
- **Area:** component library, Showcode, theming, integration, or documentation.
- **Flags:** `needs: triage` for imported findings awaiting verification; `flag: blocked` for impediments.

Track sprints with GitHub milestones. Add a SPEC trace to issue titles, such as `SPEC1007US02: Button behavior`, and include scope, acceptance criteria, and affected files. Move work through the status labels and close completed issues with `status: done`. Do not invent sprint assignments or estimates before refinement.

Imported workitems retain their source references and original completion state. See [the migration inventory](WORKITEMS.md) for the source-to-issue mapping.
