# Published dependency closure repair

Components lane triage, 2026-10-24. This follows the coordinator's defect
report in [feature-feed-closure.md](feature-feed-closure.md). It reproduces the
defect from this repository, derives the exact closure the published feed
needs, names the owner of every step and states what is still blocked.

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

**Step 2 — the producer of `LowlandTech.Core`, not this lane.** A new upstream
Core `C` has to be published that

- depends on `LowlandTech.Components.Core` exactly
  `[2026.10.1-20261024235136415]`, and
- is built against that published package instead of the stale local copy in
  `src/shared/components-core`, so the Core assembly it ships is compiled
  against the assembly that carries the startup contracts.

**Step 3 — this repository, after step 2.** The remaining three packages of the
batch are published under the same version, with the upstream Core as their
exact dependency:

```
./eng/build-local.ps1 -Version 2026.10.1-20261024235136415 -Projects `
    src/lib/LowlandTech.Components.csproj `
    src/blocks/LowlandTech.Components.Blocks.csproj `
    src/features/LowlandTech.Components.Features.Session/LowlandTech.Components.Features.Session.csproj
```

with `LowlandTechCoreVersion` set to `C`. The three packages then pin the
upstream Core exactly and the Components.Core of the batch exactly, and the
closure of the feed resolves without a downgrade.

**Step 4 — this repository, after step 3.** The external consumer restores
`LowlandTech.Components.Features.Session [2026.10.1-20261024235136415]` from the
shared feed, compiles, starts and reads the feature registration, and the
CPAN0001 two-circuit consumer proof runs against the same published feed
instead of a locally packed one.

## What is still blocked

Steps 2 and 3 are blocked on the upstream Core producer, which this lane does
not own and must not publish from another repository. The evidence of this
scope in
[../workitems/CPAN0001.US01.SC01.active-session-panels.md](../workitems/CPAN0001.US01.SC01.active-session-panels.md)
records the two-circuit consumer proof that already passes against a locally
packed feed; the shared feed proof of step 4 follows the closure above.

No package, cache entry, producer source, version constraint or compiler
warning was overwritten or weakened in this triage.
