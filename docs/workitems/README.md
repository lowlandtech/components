# Components lane roadmap

- **Lane:** `components`
- **Scope:** independent package consumption and the existing optional Plugins integration.
- **Sources:** [compatibility review](../plugin-recommendations.md), [delivery workflow](../workflow.md), [imported inventory](../../WORKITEMS.md).

This is the lane's current roadmap. Each local row links to its authoritative
workitem; its state and dependencies must agree with that file. Milestone item
IDs organize delivery; SPEC/US/SC identities and GitHub issue numbers remain stable.
Milestones describe outcomes and order, not sprints, estimates or implementation releases.

**Owner** is the accountable agent lane. A row does not acquire a runner or
transfer an active claim: the hub and repository workflow retain one active owner.
Existing review/backlog states and acceptance evidence are preserved.

## M1 - Independent package consumption

**Outcome:** an isolated consumer builds, publishes and renders Core, controls
and Blocks from a verified local NuGet closure without a Foundry checkout.

| Work | Scope | Title | Owner | Depends on | State |
|---|---|---|---|---|---|
| M1-001 | `CPKG0001-US01-SC01` | [Restore independent package consumption](CPKG0001.US01.SC01.independent-package-consumption.md) - [#19](https://github.com/lowlandtech/components/issues/19) | components | none | done |

## M2 - Opt-in UI feature manifest

**Outcome:** one existing feature has a valid optional plugin manifest and
trusted startup binding; base libraries remain ordinary dependencies.

| Work | Scope | Title | Owner | Depends on | State |
|---|---|---|---|---|---|
| M2-001 | `CPLG0001-US01-SC01` | [Publish an opt-in UI feature manifest](CPLG0001.US01.SC01.ui-feature-manifest.md) - [#20](https://github.com/lowlandtech/components/issues/20) | components | none | done |

## M3 - Activation-aware UI contributions

**Outcome:** session panels, owned blocks, provider themes and pages consume
the existing Plugins lifecycle through reusable, independently tested adapters.

| Work | Scope | Title | Owner | Depends on | State |
|---|---|---|---|---|---|
| M3-X01 | `BLAZ0001-US01-SC01` | [External: optional Blazor adapter](../../../plugins/docs/workitems/done/BLAZ0001.US01.SC01.package-optional-blazor-adapter.md) | plugins | none | done (external) |
| M3-001 | `CPAN0001-US01-SC01` | [Project active session panels](CPAN0001.US01.SC01.active-session-panels.md) - [#17](https://github.com/lowlandtech/components/issues/17) | components | M2-001; M3-X01 | done |
| M3-002 | `CBLK0001-US01-SC01` | [Resolve active owned session blocks](CBLK0001.US01.SC01.active-owned-session-blocks.md) - [#16](https://github.com/lowlandtech/components/issues/16) | components | M2-001; M3-X01 | backlog |
| M3-003 | `CTHE0001-US01-SC01` | [Resolve active provider-scoped themes](CTHE0001.US01.SC01.active-provider-themes.md) - [#21](https://github.com/lowlandtech/components/issues/21) | components | M2-001; M3-X01 | done |
| M3-004 | `CPGS0001-US01-SC01` | [Resolve active plugin pages](CPGS0001.US01.SC01.active-plugin-pages.md) - [#18](https://github.com/lowlandtech/components/issues/18) | components | M2-001; M3-X01 | backlog |

## Dependency and ownership rules

Local dependencies use milestone item IDs. Each resolves to the stable scope in
its workitem's **Blocked by** field; for example, M3-001 depends on
`CPLG0001-US01-SC01; Plugins BLAZ0001-US01-SC01`.

An `M#-X##` row is an external source reference, not a Components assignment or
a new local issue. Its source lane owns its workitem, state and acceptance.
The external status shown here is a source snapshot; recheck the linked contract
and actual delivered package before execution. Never close or reassign it here.

External references describe concrete technical prerequisites, never another
coordinator's permission, acknowledgement or whole-milestone completion. An
unmet prerequisite leaves the consuming local item blocked; record its exact
source and required action in the workitem. A refinement release authorizes
refinement only and does not waive implementation dependencies.

Keep one row per local workitem. Do not copy UACs, evidence or activity logs onto
the roadmap. Imported issues #1-#15 retain their states and remain in
[WORKITEMS.md](../../WORKITEMS.md); they are not silently released by this pilot.

## GitHub reconciliation

`scripts/Sync-PluginWorkitems.ps1 -Offline` validates milestone IDs, links,
owners, source states, dependency aliases and cycles without GitHub access.
Run the normal preview, then `-Apply` for the six managed local workitems.
External rows never create or mutate Components issues. Existing issue markers,
numbers, assignees, unmanaged labels and native GitHub milestones are preserved.
