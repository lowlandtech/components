# Components repository instructions

Build and maintain the component library, higher-level Blocks and Showcode demo.
Read README.md, docs/workflow.md, docs/workitems/README.md and the selected
workitem before changing it. Root workspace coordination instructions apply.

## Boundaries and acceptance

src/core owns host-independent contracts; src/lib owns controls; src/blocks
owns compositions; src/stories and src/app own examples and the demo. Tests live
in src/test and real-browser acceptance in src/e2e. Preserve public contracts
and keep host-specific behavior outside base libraries.

Use the source workitem and milestone roadmap as one state. Owner in the roadmap
is the components lane; the shared hub retains runner claims and phase releases.
Execute only explicitly released scope; refinement skips are unfinished behavior.
Keep imported issue #1-#15 history and source-owned external dependencies intact.
Only the assigning coordinator accepts results. Do not infer acceptance from
a successful build, closed issue or skipped scenario.

Select a verified compatible Core package as docs/local-build.md documents;
never weaken RequireCompatibleCorePackage to bypass missing package inputs.
Run checks appropriate to the scope: solution build, focused acceptance tests,
browser acceptance for browser scope, local packaging for package scope and the
roadmap validator. Record baseline failures without claiming they passed.

## Shared branch and checkpoints

Stay on the current working branch. Preserve other owners' dirty paths and
claims. Wait while the shared index is occupied; stage only named owned paths
and commit immediately. Never stash, reset, unstage or discard others' changes.
Commit every owned change before ending a turn; report exact blockers if a safe
checkpoint cannot be created. Do not deploy the demo or publish remote packages
under source delivery authority.

## Required remote delivery

The user's 2026-10-24 instruction grants standing authorization to push verified,
owned commits to this repository's current origin working branch and reconcile
its managed workitem issues. Routine delivery does not need fresh permission.
This applies to implementation, refinement, planning, documentation and safe
partial checkpoints; a local commit alone is not a completed handoff.

Before ending an authorized work turn:

1. Run the checks appropriate to the changed scope and record exact commands,
   exits, executed/skipped counts and remaining blockers in its source evidence.
   Keep the workitem, roadmap and file lane consistent with actual acceptance.
2. Under the repository's concurrency rules, commit only explicit owned paths.
   Inspect the branch/upstream, fetch origin and review every outgoing commit.
   Push the verified delivery to the same working branch. A push sends the entire
   outgoing ancestry: never publish unidentified or unverified commits belonging
   to another runner. Resolve that ownership with the runner first.
3. Reconcile changed managed workitems using the native commands below: online
   preview, Apply, then a final online verification. If LinkLocal or evidence
   updates change source documents, commit/push them and reconcile again before
   reporting completion. An offline check is not GitHub synchronization.
4. Verify the delivered checkpoint is reachable from the fetched origin branch;
   verify the managed issue's identity, source/body, labels and state agree with
   the final workitem. Use an unchanged online preview or VerifyOnly, plus the
   tool's supported milestone projection where applicable.
5. Report the pushed checkpoint, issue URL and verification result in the hub
   handoff. Only the assigning coordinator accepts the hub result. Refinement
   remains unfinished product behavior; its issue stays open. Final done requires
   passing executable acceptance, coordinator acceptance where required, pushed
   source and a closed counterpart with the repository's done label.

If tests, authentication, networking, push, reconciliation, branch divergence or
another owner's lease/index prevents completion, commit a safe owned checkpoint,
keep the item review or blocked as appropriate, and send the exact failure and
recovery action. Explicitly identify any unpushed commits or outstanding drift.
Do not call it delivered/done, silently leave it local, or select new work to
avoid finishing the delivery. Retry recoverable failures within the turn. A
rejected push requires coordination; never force-push, switch branches, stash,
reset or rewrite another owner's work. Already-done items with delivery drift
need repair before being reported complete.

This authorization covers source delivery and managed issue reconciliation. Main
promotion, deployment, remote package publication and excluded product scope
retain their separate gates. Preserve unmanaged issues, comments, assignees,
labels and milestone fields outside the native tool's managed scope.

### Native reconciliation commands

```powershell
pwsh -NoProfile -File scripts/Sync-PluginWorkitems.ps1 -TraceId <scope>
pwsh -NoProfile -File scripts/Sync-PluginWorkitems.ps1 -TraceId <scope> -Apply
pwsh -NoProfile -File scripts/Sync-PluginWorkitems.ps1 -TraceId <scope>
```
Use the scoped mirror for a single delivery; preserve imported issues #1-#15 and external reference rows.
