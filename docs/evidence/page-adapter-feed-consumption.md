# Page adapter — shared feed consumption

**Date:** 2026-10-25
**Scope:** `CPGS0001-US01-SC01` (M3-004), resolve active plugin pages
**Owner:** components lane

## What this proves

The published page adapter is consumable by a host outside this repository.
The acceptance of the scope proves the behavior of the adapter against a
private feed that the `CPKG0001` harness packs, so that a closed gate never
depends on the mutable state of a shared feed
([dependency-closure-repair.md](dependency-closure-repair.md), step 4). This
record is the separate proof of the other half: the shared feed of the
workspace carries the published identities, and an external consumer restores
them, compiles against them and drives the documented consumption path.

It matters for the `Coder` lane: `SHEL0005-US03-SC01` (M6-011) composes the
active plugin pages and waits on the Components delivery of this scope.

## The consumer

`src/test/CPGS0001.Integration/feed-consumer/` is a separate project that
references the published packages only. It never references the Components
source tree, so every type it compiles against comes from a package.

| Package | Version |
|---|---|
| `LowlandTech.Components.Features.Pages` | `2026.10.1-20261024235136415` |
| `LowlandTech.Plugins.Blazor` | `2026.10.1-20261024190755988902385` |

The nuspec of the published adapter:

```text
LowlandTech.Components.Features.Pages 2026.10.1-20261024235136415
  net10.0
    LowlandTech.Components.Core      2026.10.1-20261024235136415
    LowlandTech.Plugins.Domain       2026.10.1-20261024190755988902385
    Microsoft.AspNetCore.Components  10.0.9
```

The two LowlandTech identities exist only in the local feed of the workspace.
`Microsoft.AspNetCore.Components` comes from nuget.org, exactly as the
`Microsoft.AspNetCore.Components.Web` of `LowlandTech.Components` does, so the
adapter adds no new kind of dependency to a consumer.

## Commands and exit codes

Run from `src/test/CPGS0001.Integration/feed-consumer`:

| Command | Exit | Result |
|---|---|---|
| `dotnet restore --configfile NuGet.Config` | 0 | restored in 177 ms from the local feed and nuget.org |
| `dotnet run --no-restore` | 0 | 22 checks passed, `FEED CONSUMER OK` |

A cold run with an isolated global packages folder, pointed at an empty
directory, restored in 361 ms and resolved exactly
`lowlandtech.components.core`, `lowlandtech.components.features.pages`,
`lowlandtech.plugins.blazor`, `lowlandtech.plugins.domain` and
`microsoft.aspnetcore.app.internal.assets`. The three LowlandTech identities
therefore came from the local feed of the workspace.

## The checks

The consumer drives the documented path of a host:

1. a trusted startup map of `LowlandTech.Components.Core` binds the owner key
   to the component type of the page;
2. the bridge validates and publishes the page binding;
3. the host builds the Plugins `PluginPageCatalog` and the Plugins
   `PluginComponentCatalog` from the bridge projection, and the metadata keeps
   the label, the icon, the order, the group and the subgroup that the
   `PluginPageAttribute` of the page declares;
4. the navigation offers the page and the route resolves it;
5. the withdrawal of the owner empties every projection, the route resolves
   nothing and the owner is not active;
6. the reactivation of the owner publishes the page again, without a new
   registration.

## Limits

- The proof depends on the state of the shared feed, so it runs on demand and
  not inside the scenario suites.
- It proves the consumption of the published adapter. It does not prove the
  browser behavior of a host, which the browser scenario of the scope proves.
- The `Coder` lane still owns the host side of the contract: the route and
  page primitives of the Plugins shell resolve their target when their host
  parameter changes and do not poll the authority of the host themselves, so a
  host refreshes its route gateway when the projection of the Components
  authority changes.
