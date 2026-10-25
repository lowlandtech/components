# Published dependency closure repair

Components lane triage, 2026-10-24. This follows the coordinator's defect
report in [feature-feed-closure.md](feature-feed-closure.md). It reproduces the
defect from this repository, derives the exact closure the published feed
needs, names the owner of every step and states what is still blocked.

Updated 2026-10-25: steps 2, 3 and 4 are done. The upstream Core producer
published the compatible Core, this repository republished the batch against
it, and an external consumer restores, compiles and starts from the shared
feed. The closure the feed needs now resolves, and the defect below does not
reproduce for the new batch.

## The defect, reproduced here

An external consumer that references only the published feature package and
restores from the shared local feed `D:\lowlandtech\packages` fails before
compilation:

```
dotnet restore Consumer.csproj --configfile NuGet.Config
```

```
error NU1605: Detected package downgrade: LowlandTech.Components.Core from
  2026.10.2-202610242 to 2026.10.1-20261024192513816465564.
error NU1605: Consumer -> LowlandTech.Components.Features.Session
  2026.10.1-20261024192513816465564 -> LowlandTech.Components.Blocks
  2026.10.1-20261024192513816465564 -> LowlandTech.Components
  2026.10.1-20261024192513816465564 -> LowlandTech.Core 2026.10.2-202610242
  -> LowlandTech.Components.Core (= 2026.10.2-202610242)
```

Exit code 1. No package, cache entry, producer source, version constraint or
compiler warning was changed to produce this reproduction.

The reproduction names the batch `2026.10.1-20261024192513816465564`, which is
the batch that pins the upstream Core `2026.10.2-202610242`. The batch
`2026.10.1-20261024235136415` published on 2026-10-25 pins the upstream Core
`2026.10.3-20261025011050827978557`, and an external consumer restores it from
the shared feed without a downgrade.

## The root cause

The closure is not wrong in one place; it is wrong because **two producers own
the identity of `LowlandTech.Components.Core`**.

| Producer | Identity it publishes | Carries the startup contracts |
|---|---|---|
| this repository, `src/core/LowlandTech.Components.Core.csproj` | `2026.10.1-<stamp>` | yes, `ComponentsStartupMap` and the activation contracts |
| the upstream Core producer, `LowlandTech.Core` | pins `LowlandTech.Components.Core` exactly `[2026.10.2-202610242]` | no, that identity is an older assembly without them |

The published chain is exact on both sides:

- `LowlandTech.Components` pins `LowlandTech.Components.Core` exactly
  `[2026.10.1-20261024192513816465564]` and `LowlandTech.Core` exactly
  `[2026.10.2-202610242]`.
- `LowlandTech.Core 2026.10.2-202610242` pins `LowlandTech.Components.Core`
  exactly `[2026.10.2-202610242]`.

Two exact pins on one package cannot both hold, so NuGet reports the downgrade
of the exact pin of the upstream Core to the exact pin of the batch. Pinning the
upstream identity directly does not help either: that identity is an older
assembly without the startup contracts, and the feature package needs them.

The upstream Core producer builds its Core package against its own local copy
of the Components.Core sources, not against the package this repository
publishes. That copy in `src/shared/components-core` carries none of the
startup contracts, so the identity it pins can never be the identity that has
them.

## The closure the feed needs

One identity for `LowlandTech.Components.Core`, the one this repository
publishes, and one upstream Core that pins exactly that identity:

```
LowlandTech.Components.Features.Session V
  -> LowlandTech.Components.Blocks V
    -> LowlandTech.Components V
      -> LowlandTech.Components.Core V        (exact)
      -> LowlandTech.Core C                   (exact)
LowlandTech.Core C
  -> LowlandTech.Components.Core V            (exact)
```

Every pin is exact and every identity is new, so nothing is overwritten and no
pin is weakened.

## The steps and their owners

**Step 1 — this repository, done in this pass.** The identity of the
Components.Core package that the upstream Core has to pin is published on its
own, so the producer of the upstream Core can build against it:

- `LowlandTech.Components.Core 2026.10.1-20261024235136415` is published to the
  shared local feed by `eng/build-local.ps1`, which now takes the projects to
  publish as an optional argument. The package carries no dependency of its
  own, so it is consumable on its own.
- An external consumer with no `ProjectReference` restores, compiles and starts
  against that identity from the shared feed, and resolves an identity through
  the published `ComponentsStartupMap`. Exit code 0 for the restore, the build
  and the run.

**Step 2 — the producer of `LowlandTech.Core`, done on 2026-10-25.** The
producer published `LowlandTech.Core 2026.10.3-20261025011050827978557`, which
depends on `LowlandTech.Components.Core` exactly
`[2026.10.1-20261024235136415]` — the identity of step 1 — and a clean external
consumer restores, builds and runs the startup contract against it. The
coordinator reported the producer as satisfied; this lane did not publish
anything from that repository.

**Step 3 — this repository, done on 2026-10-25.** The remaining three packages
of the batch are published under the same version, with the upstream Core as
their exact dependency:

```
$env:LowlandTechCoreVersion = '2026.10.3-20261025011050827978557'
./eng/build-local.ps1 -Version 2026.10.1-20261024235136415 -Projects `
    src/lib/LowlandTech.Components.csproj `
    src/blocks/LowlandTech.Components.Blocks.csproj `
    src/features/LowlandTech.Components.Features.Session/LowlandTech.Components.Features.Session.csproj
```

Exit code 0. `Published 3 packages to D:\lowlandtech\packages`, package version
`2026.10.1-20261024235136415`. The staging folder of that version from step 1
was removed first, because the script refuses to stage into a folder that
already exists; the package it held stays published in the feed, and the feed
guard of the script still refuses to replace any published identity. No version
was overwritten: the four packages of the batch are new identities.

The closure the feed now carries, read from the published nuspecs:

```
LowlandTech.Components.Features.Session 2026.10.1-20261024235136415
  -> LowlandTech.Components.Blocks 2026.10.1-20261024235136415
    -> LowlandTech.Components 2026.10.1-20261024235136415
      -> LowlandTech.Components.Core 2026.10.1-20261024235136415
      -> LowlandTech.Core [2026.10.3-20261025011050827978557]
LowlandTech.Core [2026.10.3-20261025011050827978557]
  -> LowlandTech.Components.Core [2026.10.1-20261024235136415]
```

Both sides name the same identity of `LowlandTech.Components.Core`, so the
downgrade of the exact pin is impossible.

**Step 4 — this repository, done on 2026-10-25.** An external consumer with no
`ProjectReference` restores the published feature package from the shared feed
alone:

```
dotnet restore Consumer.csproj --configfile NuGet.Config   # exit 0
dotnet build Consumer.csproj -c Release --no-restore         # exit 0
dotnet bin/Release/net10.0/SharedFeatureConsumer.dll         # exit 0
```

The consumer references `LowlandTech.Components.Features.Session
[2026.10.1-20261024235136415]` only, and it reads the feature registration the
published package carries and resolves a symbolic identity through the
published `ComponentsStartupMap`:

```
registration: LowlandTech.Components.Core.ComponentsStartupRegistration
feature key: lowlandtech.components.session
component identities: LowlandTech.Components.SessionPluginHost
resolved: True LowlandTech.Components.SessionPluginHost
```

The browser consumer of `CPAN0001` restores, builds and publishes from the same
shared feed with the same exit code 0, starts and serves the page with HTTP
200, and the deployed stylesheet of the package is served with HTTP 200. The
two-circuit browser proof of `CPAN0001` runs again on the new closure —
`dotnet test src/e2e/LowlandTech.Components.E2E.csproj --filter "SPEC=CPAN0001&SC=01"`
exits 0 with 1 passed — because the packaging harness resolves the upstream
Core of the shared feed by the dependency it declares, and that is now the Core
of step 2.

## What is closed and what remains

The defect of this document is closed: the shared feed resolves the whole
closure without a downgrade, and an external consumer restores, compiles,
starts and reads the feature registration from it.

One sub-item of step 4 remains and is not part of the defect: the two-circuit
browser proof of `CPAN0001` still restores from a private feed that the
harness packs, although it packs exactly the identities the shared feed now
carries. Moving the proof onto the shared feed itself would make the gate of a
closed scope depend on the state of a local feed, so it is left as a separate
action for the coordinator to release rather than folded into that gate: the
`CPKG0001` harness would need a switch that copies the published identities of
the shared feed into the scenario feed instead of packing them, and the
`CPAN0001` scenario would need to request it.

No package, cache entry, producer source, version constraint or compiler
warning was overwritten or weakened in this triage, and the consumer acceptance
gates stay separate: the `CPAN0001`, `CTHE0001` and `CPGS0001` proofs keep
their own scenarios and their own exit codes.
