# Host fixture inputs

These files are **test inputs**, not part of any shipped package. The drift-guard
Facts in `SPEC1069.ExplorerLayout` and `SPEC1078.Session` read them out of the
test assembly's manifest resources and assert that every Foundry host page
still links the Components stylesheet and the session-tree script.

The standalone Components repository does not contain the `web`, `shell`, `app`
or `desktop` host applications, so the previous embedded-resource paths
(`..\..\web\Components\App.razor` and friends) resolved outside this repository
and the test project failed to build with `CS1566`. The files are therefore
vendored here as verbatim copies, and the embedded-resource paths in
`LowlandTech.Components.Tests.csproj` point here instead.

## Provenance

Each file is byte-identical to its source in the Foundry monorepo
(`D:/lowlandtech/develop`, branch `develop`):

| Fixture | Monorepo source | SHA-256 (first 16) |
|---|---|---|
| `web/App.razor` | `src/web/Components/App.razor` | `AB14F6273C782E41` |
| `shell/App.razor` | `src/shell/Features/Shell/App.razor` | `33510393C7ACFFFB` |
| `shell/input.css` | `src/shell/wwwroot/css/input.css` | `944289C44465A6C7` |
| `app/App.razor` | `src/app/Components/App.razor` | `2F603FF86DD196D0` |
| `desktop/index.html` | `src/desktop/wwwroot/index.html` | `58AC2709DDED7158` |

The logical manifest-resource names (`web.App.razor`, `shell.App.razor`,
`shell.input.css`, `app.App.razor`, `desktop.index.html`) are unchanged, so the
drift-guard Facts keep reading the same resources they always did.

## Why the files are not compiled

The test project uses `Microsoft.NET.Sdk.Razor`, which would otherwise treat
`App.razor` as a component of the test assembly. `LowlandTech.Components.Tests.csproj`
removes `Fixtures\**` from the `RazorComponent` item for that reason. The
removal covers only these host inputs; no scenario, Fact or acceptance
criterion is excluded by it.
