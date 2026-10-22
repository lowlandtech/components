# Making Components pluggable

Review date: 2026-10-22. Evidence is the local working tree, not the deployed demo.
This is backlog analysis; no plugin integration or package publication is delivered.

Components should both supply optional UI feature plugins and consume third-party
UI plugins through LowlandTech.Plugins. Keep the base controls usable without a
plugin manager. Put activation-aware integration in an optional adapter rather
than adding filesystem/NuGet orchestration to every Button or ThemeProvider.

## Current fit and missing integration

| Surface | Source evidence | Assessment and recommendation |
|---|---|---|
| Package distribution | `src/core`, `src/lib`, `src/blocks` csproj files are packable; Blocks owns `ISessionPlugin` | These are NuGet libraries, not yet Plugins feature packages. No `lowlandtech/plugin.json` packaging declaration was found. Keep base libraries ordinary dependencies and introduce opt-in feature manifests with stable plugin keys and declared contributions. |
| Standalone packaging | lib references `../../core/LowlandTech.Core.csproj`; lib/core icon paths escape src; MIGRATION.md documents extraction debt | The referenced Core project and expected icon are absent. Resolve the actual upstream Core package contract rather than inventing replacements. Verify all exported dependencies and assets using an isolated consumer. CPKG0001 owns this prerequisite. |
| Workspace hosting | app uses `Microsoft.NET.Sdk.BlazorWebAssembly`; `src/app/Program.cs`; Plugins `WorkspaceExtensions` only accepts Web/Razor conventional targets | Plugins currently rejects this workspace. Plugins WORK0001-US01-SC03 owns explicit WebAssembly discovery. Do not relabel the SDK or set a marker to bypass this validation. `src/lib`/`src/blocks` are package-authoring projects, not implicit workspace installation targets. |
| Browser authority | app is a static WebAssembly demo; Plugins runtime uses server filesystem/restore/deployment services | Keep feed credentials, package operations and invocation authorization in a trusted host/tool. Browser receives a minimal revisioned contribution projection and trusted startup type map. Static standalone mode uses a build-time inventory and explicit rebuild/redeploy; it does not offer live management or authoritative revocation while offline. Plugins BLAZ0001-US01-SC02 defines this boundary. |
| Session panels | `SessionPluginModels.cs`, `SessionPluginRegistry.cs`, `SessionPluginHost.razor.cs`, `SessionPluginSettingsPanel.razor` under blocks/Session | Existing DI instance capture, visibility, badge, settings and ordering are useful. Add an optional activation-aware bridge; filter both ListAsync and GetAllPlugins/render paths. Host currently queries RightSidebar only despite three placement enum values. CPAN0001 covers placement selection and active panel rendering. |
| Block discovery | `SessionBlockDiscovery.cs` scans loaded assemblies; `SessionSurfaceDiscovery.cs` aggregates prioritized registries | Loaded types are not proof of active ownership. CBLK0001 validates owned block descriptors before publication and filters all discovery paths, including reflection fallback, after withdrawal. Do not silently treat global assembly scanning as activation. |
| Themes | lib `Theme/ComponentThemeService.cs` captures startup themes; `ThemeProvider` and `IComponentThemeResolver` supply provider-local behavior | CTHE0001 bridges active theme packs through the existing resolver semantics. Preserve provider IDs, class/fragment/style families, consumer override order and host defaults. Plugins THEM completion does not prove this binary integration. |
| Routes/navigation | core `PluginPageAttribute.cs`; app `App.razor` uses a fixed AdditionalAssemblies array | CPGS0001 maps explicit active page identities through Plugins route/page primitives. Hide withdrawn navigation and direct routes; browser visibility never replaces server authorization of data/actions. |
| Static assets | lib Tailwind target and wwwroot; packable RCLs | Verify `_content` CSS/JS URLs in a clean published consumer, exclude Node inputs, preserve notices. Newly installed assemblies/assets require redeployment; withdrawal hides contributions but does not erase public deployed files. |

## Ownership and delivery

Coder.Shell is the reusable Coder application root/layout, not a fork of the
Components demo. Its new M6-009 through M6-013 scenarios consume independently
packaged Components and Plugins UI. Components provides focused startup
registrations and host context/policy callbacks; Coder.Shell owns application
slots and circuit state, and Coder.App owns Interactive Server startup.

The CPAN/CBLK/CTHE/CPGS server adapters depend on Plugins BLAZ SC01. Their earlier
SC02 dependency has been corrected: browser projection is a separate WASM mode
and must not block Interactive Server consumers. Verify each adapter through a
separate RCL consumer, with no reverse dependency on Coder or the demo host.

### Upstream Core located

The user identified `D:/lowlandtech/develop`; its
`src/core/LowlandTech.Core.csproj` is the original packable dependency and owns
the missing Abstractions and Graph.Entities namespaces. An existing candidate
package is at `artifacts/nuget/LowlandTech.Core.2026.5.29.nupkg` in that checkout.
The broken Components reference is an extraction/layout issue, not evidence that
Core needs to be recreated here.

Archive inspection confirms a net10.0 Core assembly and repository commit
41e67dc0b496f826f13d0770c419cd03da9e5a10. Its EF/Extensions 10.0.8 and
NCalcAsync 5.12.0 differ from source's 10.0.9/6.3.2. Current source also depends
on Components.Core whereas the archive does not declare that dependency.
CPKG0001 must verify full API/type ownership compatibility before selecting
the package. A matching 2026.5.29 version label is insufficient. A permanent
absolute sibling ProjectReference would defeat independent package consumption.

The new [local follow-up inventory](workitems/README.md) preserves Components'
existing issue workflow from README.md and WORKITEMS.md. It does not renumber or
reopen imported SPEC work. These proposals are now mirrored to issues #16–#21;
they have no assignee, estimate or sprint until normal refinement.

Plugins owns generic lifecycle, workspace support and optional host adapters.
Components owns reusable mappings to its real panel/block/theme/page contracts.
Coder consumes that reusable Components bridge and owns its session IDs, policies,
placement-to-slot choices and runtime tool/provider/command semantics. This avoids
implementing a separate Components panel bridge inside every consuming application.

Before execution, establish compiling acceptance inventories in Components' test
and browser suites for each UAC. The extracted solution currently has missing
Foundry source and test-resource inputs; do not add assertions or skips that
pretend those dependencies have been resolved. Existing component behavior tests
are not activation/withdrawal acceptance.

## Verification limits

The review checks source declarations and local workitem consistency. Components'
standalone build is attempted separately and its actual result is recorded below.
The analysis itself did not write issues. The authorized follow-up sync now
mirrors the six proposals; no demo is deployed and no package is published.

Recorded checks at this uncommitted working tree:

- `dotnet build components.slnx -m:1 -p:SkipTailwindBuild=true -v:minimal`:
  exit 1, 13 warnings/two errors. Missing `components/core/LowlandTech.Core.csproj`
  produces CS0234 for `LowlandTech.Core.Abstractions` and
  `LowlandTech.Core.Graph.Entities`. NU1900 warnings also report inaccessible
  vulnerability metadata. No standalone behavioral acceptance can be claimed.
- Local PowerShell proposal/board scan: exit 0, six proposals/18 unchecked UACs;
  identities, backlog states, local dependencies, links and required sections
  agree. This is a documentation consistency check, not a behavior gate.
- `git diff --check`: exit 0 for tracked Components edits. No production sources
  or imported workitem histories were changed.
