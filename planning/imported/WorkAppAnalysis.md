# WorkApp Component Analysis

## Purpose

This document is a handoff for continuing `WorkApp` as a component-layer experience in `src/components/lib/Work`. The goal is to make it feel like a modernized Hangfire dashboard while keeping it reusable, deterministic, and in-memory capable before promoting any domain-owned behavior into `src/plugins/work`.

## Current State

`WorkApp` already has the right dashboard vocabulary:

- dashboard metrics
- job lists by state
- queues
- servers
- recurring jobs
- detail panel
- state history
- batch commands
- event log snapshots and deltas

The current model is mostly in `src/components/lib/Work/WorkModels.cs` and `WorkStateService.cs`. The components are tested under `src/components/test/SPEC1080.Work`.

The current weak spot is data realism. `WorkStateService` still generates synthetic jobs directly with `Guid.NewGuid()` and `DateTimeOffset.UtcNow`. That is useful for first render, but it is not strong enough to model operational behavior or write stable Given/When/Then-style specs.

## Boundary Decision

Keep the next phase in the component layer.

Use `src/components/lib/Work` for:

- reusable Hangfire-like UI components
- in-memory `WorkAppState`
- deterministic scenario data
- component-level reducer/service behavior
- Storybook/showcase scenarios
- bUnit specs for rendering, filtering, navigation, selection, and command dispatch

Do not move this phase into `src/plugins/work` yet. The plugin already owns workitem/pipeline domain behavior, which is related but not the same as a Hangfire-style job execution dashboard.

Promote behavior into the plugin layer only when it becomes domain-owned:

- persisted jobs
- queue storage
- worker heartbeats
- retry policy execution
- recurring job scheduling
- backend adapters
- API contracts for live job operations

## Immediate Recommendation

Add a deterministic use case data layer:

- `src/components/lib/Work/WorkUseCaseData.cs`
- optionally `WorkUseCaseScenario`, `WorkScenarioId`, or `WorkScenarioFactory`

This should replace ad hoc generation inside `WorkStateService` with named, repeatable scenarios.

Suggested entry points:

```csharp
public static class WorkUseCaseData
{
    public static WorkAppState HealthyCluster();
    public static WorkAppState RetryStorm();
    public static WorkAppState DeadServerWithFetchedJobs();
    public static WorkAppState RecurringJobFailure();
    public static WorkAppState ContinuationChain();
}
```

`WorkStateService` can default to `HealthyCluster()` while tests can choose specific scenarios.

## Scenario Shape

Each scenario should include coherent data across all surfaces:

- jobs with stable IDs and display names
- queues whose counts match job state
- servers whose queue subscriptions match processing jobs
- recurring jobs linked to last job IDs
- state histories for selected jobs
- continuations for awaiting jobs
- metrics and statistics derived from the jobs
- graph data that matches dashboard history

Use fixed timestamps anchored to one value:

```csharp
private static readonly DateTimeOffset Now =
    new(2026, 06, 01, 12, 00, 00, TimeSpan.Zero);
```

Avoid `Guid.NewGuid()`, `Random.Shared`, and direct `DateTimeOffset.UtcNow` in use case data. Determinism is the point.

## Suggested Scenarios

### Healthy Cluster

Purpose: default dashboard and navigation baseline.

Include:

- queues: `default`, `critical`, `emails`, `imports`, `agents`
- servers: three alive workers with recent heartbeats
- mostly succeeded jobs
- a small number of enqueued, scheduled, processing, and failed jobs
- recurring jobs with successful last executions

Tests:

- dashboard metrics match statistics
- queue counts match jobs
- server list shows healthy heartbeats
- graph renders supplied data, not random fallback values

### Retry Storm

Purpose: failed/retry operational pressure.

Include:

- repeated failures for one job family
- retry attempts 1, 2, 3
- exception type/message/details
- latency and duration spikes
- retry queue with meaningful counts

Tests:

- Failed and Retries views show the right jobs
- detail panel shows exception data
- requeue command records selected failed jobs
- event log delta captures selection and command action

### Dead Server With Fetched Jobs

Purpose: worker health and stuck job diagnosis.

Include:

- one server with stale heartbeat
- fetched jobs assigned to that server
- processing jobs older than expected timeout
- queues showing fetched count

Tests:

- server view marks stale heartbeat
- processing view includes server column
- queue fetched counts are visible
- detail history makes stale processing legible

### Recurring Job Failure

Purpose: recurring schedule management.

Include:

- recurring job with invalid cron/error
- recurring job with failed last execution
- recurring job with next execution
- last job IDs linked to job details

Tests:

- recurring view shows errors
- trigger command records selected recurring job IDs
- failed recurring job exposes last job state

### Continuation Chain

Purpose: parent/child and awaiting job behavior.

Include:

- parent job succeeded
- awaiting continuation
- continuation options
- selected job continuations list

Tests:

- Awaiting view shows parent/continuation data
- detail panel shows state history
- continuation list renders when selected

## Component Fixes Already Applied

The following cleanup was applied during analysis:

- Work component buttons now use `OnClick` instead of raw `@onclick` where the component is `LowlandTech.Components.Button`.
- `WorkApp` keeps sidebar navigation visible on dashboard.
- generated sample job names use zero-padded names for stable filtering.
- `SelectAll()` selects paged visible jobs rather than every filtered job.
- current `SPEC1080.Work` tests pass.

Verify with:

```powershell
dotnet test src/components/test/LowlandTech.Components.Tests.csproj --filter "FullyQualifiedName~SPEC1080.Work" -m:1 --no-restore
```

## Risks To Avoid

- Do not turn `WorkApp` into a plugin-specific workitem page.
- Do not couple component scenarios to EF, API, or plugin services yet.
- Do not hide operational semantics inside markup helpers.
- Do not keep random fallback data in visual components such as `WorkGraph`; make scenario data explicit.
- Do not duplicate the existing `src/plugins/work` workitem/pipeline model unless the job dashboard becomes a real plugin domain.

## Good Next Task

Start with `HealthyCluster()` and wire it into `WorkStateService`.

Acceptance target:

- `WorkStateService()` loads deterministic healthy data.
- dashboard metrics derive from the scenario state.
- `WorkGraph` receives deterministic history series.
- `SPEC1080.Work` still passes.
- add one new test proving queue counts match the generated jobs.

After that, add `RetryStorm()` because it will put the most useful pressure on commands, detail panels, exceptions, and event-log attribution.

---

## Navigation

[[components/_index|Back to Index]]