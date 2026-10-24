# Scheduled, issue-linked workitem delivery

Components delivers work through one scheduled pass every hour. The pass resumes
the scope it owns or takes the next eligible scope from the board, refines or
executes exactly one scope, proves it, commits only its own changes, and closes
its coordination loop with the other lanes. This document is the contract the
scheduled task reads on every run. It does not replace [README.md](../README.md)
(the issue workflow), [WORKITEMS.md](../WORKITEMS.md) (the imported migration
inventory), the [board](workitems/README.md) or the
[compatibility review](plugin-recommendations.md).

## Authority

The work item file and its board row are one logical state. The issue workflow
in [README.md](../README.md) names the issue labels and states. GitHub issues
mirror the board; a mirror proves the documents agree, never that behavior is
accepted. Acceptance is proven by an executed scenario.

Imported work keeps its source references and its original completion state. An
imported issue is never reopened, re-estimated or closed by this lane.

## Selection

One scope per pass. The pass selects in this order:

1. Hub work released to this lane that it already owns in `doing` or `review`. A
   `review` scope is finished work waiting for its delivery gate, not work to
   re-implement.
2. Otherwise the board row with the lowest scope id whose state is `backlog` and
   whose dependencies are satisfied.

A `blocked` row is never eligible. `CPKG0001-US01-SC01` stays blocked until a
compatible upstream `LowlandTech.Core` package exists; `CPLG0001-US01-SC01`
waits for it; `CPAN`, `CBLK`, `CTHE` and `CPGS` wait for `CPLG0001` and for
Plugins `BLAZ0001-US01-SC01`. The blocked row of the board names its blocker
and the work item file names the same blocker; a disagreement stops the pass.

Another owner's scope is never taken, never re-owned and never unstaged. When
nothing is eligible, the pass says so plainly and changes nothing.

## Refinement before execution

Every row on the board currently reads "needs triage; needs estimate; no sprint
assigned", so the first pass over a scope is normally a refinement pass, and a
refinement pass implements no production behavior:

- the outcome is recorded in the work item file and the board row together; an
  owner, a sprint or an estimate is never invented
- the executable inventory is written at
  `src/test/<SPECID>.<Area>/<US##>.<Story>/SC##_<Name>.cs`, and real-browser
  scenarios live under `src/e2e/<SPECID>.<Area>/`
- one Given, one When and one expression-bodied Shouldly Fact per unique
  acceptance criterion, carrying the repository's `SPEC`, `US`, `SC` and `UAC`
  traits and its `UserStory` trait, one named type per file, imports in
  `GlobalUsings.cs`
- a Fact that cannot run yet is skipped with
  `Fact(Skip = "TODO: <the specific missing behavior, fixture, contract or dependency>")`;
  `TODO`, `later`, `blocked` and `pending` alone are invalid
- the `Compile Remove` exclusions in
  `src/test/LowlandTech.Components.Tests.csproj` quarantine unfinished files that
  other owners are still writing; this lane never edits those exclusions to
  hide or to promote its own scope

A skipped Fact is unfinished inventory, never acceptance.

## Execution

When the inventory exists, the pass implements the smallest behavior the selected
scenario covers, then removes that scenario's skips. It never weakens or deletes
a UAC and never accepts skipped, scaffolded or unverified behavior.

The repository boundaries stay in force: `src/core` owns shared component
contracts, `src/lib` owns the controls, `src/blocks` owns the higher-level
compositions, `src/stories` owns the examples, `src/app` is the demo host, and
`src/test` and `src/e2e` own acceptance. `src/core` and `src/lib` stay free of
host-framework concerns and the public component contracts are preserved.

## Verification

The selected scope must exit 0 on every gate it touches:

```powershell
dotnet build components.slnx -m:1 -p:SkipTailwindBuild=true -v:minimal
dotnet test src/test/LowlandTech.Components.Tests.csproj
dotnet test src/test/LowlandTech.Components.Tests.csproj --filter "SPEC=<number>&SC=<sc>"
dotnet test src/e2e/LowlandTech.Components.E2E.csproj
./eng/build-local.ps1
./scripts/Sync-PluginWorkitems.ps1
```

The browser gate runs only for a real-browser scope and needs the Playwright
browsers installed. `./eng/build-local.ps1` runs only for a packaging scope; it
refuses to overwrite an existing package version and never publishes a partial
batch, so that refusal is never worked around.

The baseline is recorded, never hidden. On 2026-10-24 the standalone build
exited 1 with one error and no warning in under a second: the
`RequireCompatibleCorePackage` target in `src/lib/LowlandTech.Components.csproj`
fails before compilation while `LowlandTechCoreVersion` is unset or still
`2026.5.29`, because that archive duplicates `Icons`, `BlockIntent` and related
types that now belong to `Components.Core`. `CPKG0001-US01-SC01` owns that
prerequisite. The pass records the failing command and its exit code as baseline
evidence; it does not weaken the guard, add an exclusion to hide its scope, or
claim acceptance for a scope whose scenario did not execute.

## Commit and closure

The pass commits on `develop` with a Conventional Commit subject that carries the
scope, for example `feat(CPKG0001): restore independent package consumption`. The
body records the scope, the work item, the commands and their exit codes, and any
limitation.

Only owned paths are staged, explicitly. The pass never runs `git add .` or
`git commit -a`, never stages, resets, stashes, restores or reverts a path it did
not write, never switches branches, never amends a pushed commit, never changes
the git identity and never force-pushes. Exiting a pass with its work
uncommitted is forbidden.

Closure sets the `**State:**` field in the work item file and the State cell of
its board row in the same change; they must never disagree. The acceptance
criteria that actually executed are ticked and the Evidence section is replaced
with the commands that ran and their exit codes. A scope that is not accepted
stays `review` with its blocker evidence committed. Nothing is closed without an
executed scenario.

## Mirror

The board mirrors onto the managed GitHub issues through the existing tool; a
second synchronizer is never added.

```powershell
./scripts/Sync-PluginWorkitems.ps1
./scripts/Sync-PluginWorkitems.ps1 -Apply
```

The preview always runs. The apply touches only the proposals this lane owns or
created: never the imported issues `#1`-`#15`, never another lane's issue, and
never a scope whose acceptance did not execute. Unmarked issues, comments,
labels, assignees and milestones are preserved, and a managed issue is never
edited by hand.

## Coordination with the other lanes

The lane is a participant in the shared hub at `D:\lowlandtech`. Its identity is
`components` and its inbox is `D:\lowlandtech\.chatbox\components\inbox`.

```powershell
python D:\lowlandtech\eng\agent_hub.py inbox --actor components
python D:\lowlandtech\eng\agent_hub.py queue
python D:\lowlandtech\eng\agent_hub.py read <id> --actor components
python D:\lowlandtech\eng\agent_hub.py send --actor components --to <peer> --kind feature --repo D:\lowlandtech\components --source docs/workitems/<scope>.md --body '...'
```

Inbox and queue are inspected at the start and at the end of every pass. A
feature request from another lane is triaged the same way as a board row: it
becomes a proposal in `docs/workitems/` with its own scope id, its board row and
its acceptance criteria, and it stays `backlog` until it is refined. The
requester receives the scope id, the board row and the issue link; `coordinator-1`
receives a `feature` message so that milestone coverage sees the new scope.

A claim only ever takes a phase that a coordinator explicitly released. A
finished scope moves to `review` with evidence; a real blocker moves to
`blocked` with the precise evidence and the required action, after the owned
work is committed as a safe checkpoint. Only the assigning coordinator accepts
`done`. Peer coordination never transfers ownership and never turns a message
into a permission barrier.

Messages follow `D:\lowlandtech\docs\agent-chat.md`: at most 150 words including
the frontmatter, one or two short body sentences, and references to existing
documents, work items and evidence instead of copies. A needs-work or blocker
notice is sent once per episode and the lane stays quiet while nothing changed.

## Report

The pass ends with the scope it took, what it refined or implemented, the exit
code of every verification command, the commit subject and hash, the mirror
result, the feature requests it triaged with the answer given to each requester,
and the named remaining backlog and blockers. A pass that cannot finish stops at
a clean verified boundary and reports what remains for the next run. The loop
stops only when every work item is closed or when a decision genuinely needs the
user; in that case the pass says exactly what the decision is.

## Not authorized

The schedule does not authorize implementing another scope because a plan
exists, claiming blocked work, weakening a gate, closing an imported issue,
inventing an owner, estimate or sprint, publishing to a remote feed, deleting
data, or mutating an external issue beyond this lane's own proposals.

## Milestone roadmap pilot

docs/workitems/README.md is the current lane roadmap, grouped by outcome
milestones M1-M3. Work contains only IDs such as M1-001; the Title column links
to the existing stable SPEC/US/SC workitem. Source metadata and roadmap rows
agree on Work, Milestone, Owner, State and dependencies. Milestones are planning
groups, not sprint assignments or implementation releases; no estimates are invented.

The accountable Owner defaults to the components lane. This records the existing
agent assignment and does not transfer a live claim or launch a second runner.
Hub claims and released phases remain authoritative for active execution.

External M#-X## rows link to source-owned workitems. They are references to
concrete consumed technical contracts, never local assignments, new Components
issues, or cross-coordinator acknowledgement/completion gates. Recheck their
linked source and package compatibility before executing a consuming item.

Use scripts/Sync-PluginWorkitems.ps1 -Offline for the local roadmap consistency
check before the normal online preview and Apply. Dependencies use Work aliases
on the roadmap and retain canonical scope identities in the workitems.
The mirror validates the mapping and never mirrors external rows. State changes
still update the source and its row together; backlog/review/done are preserved
by this formatting pilot. Imported issues retain their original inventory.


## Standing source delivery authority

The user instruction of 2026-10-24 and ../AGENTS.md require verified owned
commits to be pushed to the current origin working branch and managed issue
reconciliation to be verified before a delivery is complete. This supersedes
the earlier schedule's prohibition on pushing. Refinement and documentation
checkpoints are pushed without falsely closing unfinished behavior. Use
Sync-PluginWorkitems.ps1 -TraceId for the selected managed scope; only close
after executable acceptance and coordinator acceptance. Package publication
and demo deployment remain separately gated.
