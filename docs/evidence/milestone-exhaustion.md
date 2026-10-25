# Components milestone exhaustion

**Date:** 2026-10-25
**Lane:** components
**Board:** [docs/workitems/README.md](../workitems/README.md)

## What is closed

M1, M2 and M3 are closed. Every managed row of the board reads `done` and every
managed issue is closed.

| Work | Scope | Issue | Accepted at |
|---|---|---|---|
| M1-001 | `CPKG0001-US01-SC01` | #19 | `8f3dc8f` |
| M2-001 | `CPLG0001-US01-SC01` | #20 | `afedfc9` |
| M3-001 | `CPAN0001-US01-SC01` | #17 | `b480fc8` |
| M3-002 | `CBLK0001-US01-SC01` | #16 | `e735f59` |
| M3-003 | `CTHE0001-US01-SC01` | #21 | `dc6dbaa` |
| M3-004 | `CPGS0001-US01-SC01` | #18 | `112260a` |

The external reference M3-X01 (`BLAZ0001-US01-SC01`) is owned by the Plugins
lane and stays a source snapshot here.

## The state of the lane

The last checkpoint of the branch is `c8799b0` on `origin/develop`.

- The verified upstream Core is `2026.10.3-20261025011050827978557`. The
  standalone build without it still exits 1 with one error and no warning in
  under a second, because the `RequireCompatibleCorePackage` guard of
  `src/lib/LowlandTech.Components.csproj` fails before compilation. The guard
  stays.
- The shared feed carries the batch `2026.10.1-20261024235136415` with the five
  manifest-selected packages: Components Core, controls, Blocks and the two
  optional feature packages. The batch is not published to a registry.
- The unit suite runs 2365 Facts with no failure and no skip. The browser suite
  runs 53 scenarios with no failure and no skip.
- The optional feature packages carry the activation-aware adapters of the
  Plugins lifecycle: `LowlandTech.Components.Features.Session` for the session
  blocks and `LowlandTech.Components.Features.Pages` for the plugin pages.
  Neither enters the dependency graph of the three base packages.

## The candidate that remains

[dependency-closure-repair.md](dependency-closure-repair.md) leaves one sub-item
outside the defect it repaired: the `CPKG0001` packaging harness would need a
switch that copies the published identities of the shared feed into the
scenario feed instead of packing them, so that a scenario could consume the
published batch rather than a fresh pack. That work was left as a separate
action for the coordinator to release, because it changes the gate of a closed
scope.

The shared feed consumption proof added on 2026-10-25 covers the same
prerequisite without that change: see
[page-adapter-feed-consumption.md](page-adapter-feed-consumption.md). The
harness switch is therefore a candidate for the next milestone, not an
outstanding defect.

## The coordination state

The `Coder` lane composes the active plugin pages under `SHEL0005-US03-SC01`
(M6-011). The Components half of that gate is delivered and accepted: the
published `LowlandTech.Components.Features.Pages 2026.10.1-20261024235136415`
carries the adapter and the workspace feed serves it with its closure.

One part of the contract belongs to the host. The route and page primitives of
the Plugins shell resolve their target when their host parameter changes, and
they do not poll the authority of the host themselves. A host therefore
refreshes its route gateway when the projection of the Components authority
changes, so that a withdrawal leaves the outlet and the navigation with the new
projection. The `Coder` lane was told this directly.

## The rule for the next pass

The lane has no eligible backlog row. A pass takes no scope, invents no
milestone and waits for the coordinator to publish the next one. The board and
the work items stay as they are, and every pass reads the hub inbox before it
reads the board.
