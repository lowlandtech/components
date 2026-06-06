namespace LowlandTech.Components;

public enum WorkItemState
{
    Enqueued,
    Scheduled,
    Processing,
    Succeeded,
    Failed,
    Deleted,
    Awaiting,
    Retry
}

public enum WorkView
{
    Dashboard,
    Queues,
    Enqueued,
    Scheduled,
    Processing,
    Succeeded,
    Failed,
    Deleted,
    Awaiting,
    Retries,
    Recurring,
    Servers,
    Detail
}

public enum WorkSortColumn
{
    Id,
    State,
    Job,
    CreatedAt,
    EnqueuedAt,
    ScheduledAt,
    StartedAt,
    SucceededAt,
    FailedAt,
    DeletedAt,
    Duration,
    Latency,
    Server,
    Queue,
    Cron,
    NextExecution,
    LastExecution
}

public enum WorkMetricStyle
{
    Default,
    Success,
    Warning,
    Danger,
    Info,
    Null
}

public sealed record WorkItem(
    string Id,
    string JobDisplayName,
    WorkItemState State,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EnqueuedAt = null,
    DateTimeOffset? ScheduledAt = null,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? SucceededAt = null,
    DateTimeOffset? FailedAt = null,
    DateTimeOffset? DeletedAt = null,
    DateTimeOffset? ExpireAt = null,
    string? ServerName = null,
    string? QueueName = null,
    string? Reason = null,
    string? ExceptionType = null,
    string? ExceptionMessage = null,
    string? ExceptionDetails = null,
    long? Latency = null,
    long? Duration = null,
    int? RetryAttempt = null,
    string? ParentId = null,
    string? ContinuationOptions = null,
    IReadOnlyDictionary<string, string>? Parameters = null,
    IReadOnlyDictionary<string, string>? StateData = null,
    bool IsExpired = false);

public sealed record WorkQueueItem(
    string Name,
    long Length,
    long Fetched,
    IReadOnlyList<WorkItem> TopJobs);

public sealed record WorkServerItem(
    string Name,
    int Workers,
    IReadOnlyList<string> Queues,
    DateTimeOffset StartedAt,
    DateTimeOffset? HeartbeatAt = null);

public sealed record WorkRecurringJobItem(
    string Id,
    string? Cron,
    string? TimeZoneId,
    string JobDisplayName,
    DateTimeOffset? NextExecutionAt = null,
    DateTimeOffset? LastExecutionAt = null,
    string? LastJobId = null,
    WorkItemState? LastJobState = null,
    string? Error = null,
    int RetryAttempt = 0,
    DateTimeOffset CreatedAt = default);

public sealed record WorkMetricItem(
    string Title,
    string? Value,
    string? Url = null,
    WorkMetricStyle Style = WorkMetricStyle.Default,
    bool IsHighlighted = false);

public sealed record WorkStateHistory(
    string StateName,
    DateTimeOffset CreatedAt,
    string? Reason = null,
    long? DurationMs = null,
    IReadOnlyDictionary<string, string>? Data = null);

public sealed record WorkStatistics(
    long SucceededCount,
    long FailedCount,
    long ProcessingCount,
    long EnqueuedCount,
    long ScheduledCount,
    long AwaitingCount,
    long DeletedCount,
    long RecurringCount,
    long RetriesCount,
    long ServersCount,
    long QueuesCount,
    long FetchedCount);

public sealed record WorkPager(
    int PageNumber,
    int PageSize,
    int TotalItems,
    int PageCount);

public sealed record WorkUiState(
    WorkView ActiveView,
    string? SelectedJobId,
    string FilterText,
    int PageNumber,
    int PageSize,
    IReadOnlyList<string> SelectedJobIds,
    WorkSortColumn SortColumn,
    bool SortAscending,
    bool IsLoading);

public sealed record WorkStateDeltaItem(
    string Path,
    string Kind,
    string Label,
    string? Before,
    string? After,
    string Description);

public sealed record WorkEventLogItem(
    Guid Id,
    string ActionType,
    DateTimeOffset Timestamp,
    string BeforeStateJson,
    string AfterStateJson,
    IReadOnlyList<WorkStateDeltaItem> Deltas);

public sealed record WorkGraphData(
    IReadOnlyList<int> DaySucceeded,
    IReadOnlyList<int> DayFailed,
    IReadOnlyList<int> DayDeleted,
    IReadOnlyList<int> WeekSucceeded,
    IReadOnlyList<int> WeekFailed,
    IReadOnlyList<int> WeekDeleted);

public sealed record WorkAppState
{
    public WorkUiState Ui { get; init; } = WorkUiDefaults.Empty;
    public IReadOnlyList<WorkItem> Jobs { get; init; } = [];
    public IReadOnlyList<WorkQueueItem> Queues { get; init; } = [];
    public IReadOnlyList<WorkServerItem> Servers { get; init; } = [];
    public IReadOnlyList<WorkRecurringJobItem> RecurringJobs { get; init; } = [];
    public IReadOnlyList<WorkMetricItem> Metrics { get; init; } = [];
    public WorkStatistics? Statistics { get; init; }
    public WorkGraphData? GraphData { get; init; }
    public string? LastPollingError { get; init; }
    public bool IsReadOnly { get; init; }
    public string? SelectedQueueName { get; init; }
    public WorkItem? SelectedJob { get; init; }
    public IReadOnlyList<WorkStateHistory> SelectedJobHistory { get; init; } = [];
    public IReadOnlyList<WorkItem> SelectedJobContinuations { get; init; } = [];
    public IReadOnlyList<WorkEventLogItem> EventLogEntries { get; init; } = [];
}

public static class WorkUiDefaults
{
    public static WorkUiState Empty { get; } = new(
        WorkView.Dashboard,
        null,
        string.Empty,
        1,
        20,
        [],
        WorkSortColumn.CreatedAt,
        false,
        false);
}

public static class WorkAppDefaults
{
    public static WorkAppState Empty { get; } = new();
}

public interface IWorkAction { }

public interface IWorkEvent { }

public abstract record WorkUiAction : IWorkAction;

public abstract record WorkAppAction : IWorkAction;

public sealed record SelectWorkItem(string? JobId) : WorkUiAction;

public sealed record SetWorkFilter(string FilterText) : WorkUiAction;

public sealed record SetWorkPage(int PageNumber) : WorkUiAction;

public sealed record SetWorkPageSize(int PageSize) : WorkUiAction;

public sealed record ToggleWorkSelection(string JobId) : WorkUiAction;

public sealed record SelectAllWork(IReadOnlyList<string> JobIds) : WorkUiAction;

public sealed record ClearWorkSelection : WorkUiAction;

public sealed record SetWorkView(WorkView View) : WorkUiAction;

public sealed record SetWorkSort(WorkSortColumn Column, bool Ascending) : WorkUiAction;

public sealed record SetWorkLoading(bool IsLoading) : WorkUiAction;

public sealed record LoadWorkItems(WorkView View, string? FilterText, int PageNumber, int PageSize) : WorkAppAction;

public sealed record SetWorkItems(IReadOnlyList<WorkItem> Jobs) : WorkAppAction;

public sealed record SetWorkQueues(IReadOnlyList<WorkQueueItem> Queues) : WorkAppAction;

public sealed record SetWorkServers(IReadOnlyList<WorkServerItem> Servers) : WorkAppAction;

public sealed record SetWorkRecurringJobs(IReadOnlyList<WorkRecurringJobItem> Jobs) : WorkAppAction;

public sealed record SetWorkMetrics(IReadOnlyList<WorkMetricItem> Metrics) : WorkAppAction;

public sealed record SetWorkStatistics(WorkStatistics Statistics) : WorkAppAction;

public sealed record SetSelectedWorkJob(WorkItem? Job, IReadOnlyList<WorkStateHistory> History, IReadOnlyList<WorkItem> Continuations) : WorkAppAction;

public sealed record SetWorkPollingError(string? Error) : WorkAppAction;

public sealed record SetWorkQueueName(string? QueueName) : WorkAppAction;

public sealed record SetWorkReadOnly(bool IsReadOnly) : WorkAppAction;

public sealed record RequeueWorkJobs(IReadOnlyList<string> JobIds) : WorkAppAction;

public sealed record DeleteWorkJobs(IReadOnlyList<string> JobIds) : WorkAppAction;

public sealed record EnqueueWorkJobs(IReadOnlyList<string> JobIds) : WorkAppAction;

public sealed record TriggerWorkRecurring(IReadOnlyList<string> JobIds) : WorkAppAction;

public sealed record ShowWorkDashboard : WorkAppAction;

public sealed record ShowWorkQueues : WorkAppAction;

public sealed record ShowWorkServers : WorkAppAction;

public sealed record ShowWorkRecurring : WorkAppAction;

public sealed record ClearWorkEventLog : WorkAppAction;

public static class WorkUiReducer
{
    public static WorkUiState Reduce(WorkUiState state, WorkUiAction action) => action switch
    {
        SelectWorkItem select => state with { SelectedJobId = select.JobId },
        SetWorkFilter filter => state with { FilterText = filter.FilterText, PageNumber = 1 },
        SetWorkPage page => state with { PageNumber = page.PageNumber },
        SetWorkPageSize size => state with { PageSize = size.PageSize, PageNumber = 1 },
        ToggleWorkSelection toggle => state with
        {
            SelectedJobIds = ToggleSelection(state.SelectedJobIds, toggle.JobId)
        },
        SelectAllWork all => state with { SelectedJobIds = all.JobIds.ToList() },
        ClearWorkSelection => state with { SelectedJobIds = [] },
        SetWorkView view => state with { ActiveView = view.View, PageNumber = 1, SelectedJobId = null, SelectedJobIds = [] },
        SetWorkSort sort => state with { SortColumn = sort.Column, SortAscending = sort.Ascending },
        SetWorkLoading loading => state with { IsLoading = loading.IsLoading },
        _ => state
    };

    private static IReadOnlyList<string> ToggleSelection(IReadOnlyList<string> selected, string jobId)
    {
        var next = selected.ToList();
        if (next.Contains(jobId))
            next.Remove(jobId);
        else
            next.Add(jobId);
        return next;
    }
}

public static class WorkAppReducer
{
    private static readonly JsonSerializerOptions EventLogJsonOptions = new() { WriteIndented = true };

    public static WorkAppState Reduce(WorkAppState state, object action)
    {
        var next = action switch
        {
            WorkUiAction uiAction => state with { Ui = WorkUiReducer.Reduce(state.Ui, uiAction) },
            SetWorkItems set => state with { Jobs = set.Jobs },
            SetWorkQueues set => state with { Queues = set.Queues },
            SetWorkServers set => state with { Servers = set.Servers },
            SetWorkRecurringJobs set => state with { RecurringJobs = set.Jobs },
            SetWorkMetrics set => state with { Metrics = set.Metrics },
            SetWorkStatistics set => state with { Statistics = set.Statistics },
            SetSelectedWorkJob set => state with { SelectedJob = set.Job, SelectedJobHistory = set.History, SelectedJobContinuations = set.Continuations },
            SetWorkPollingError error => state with { LastPollingError = error.Error },
            SetWorkQueueName queue => state with { SelectedQueueName = queue.QueueName },
            SetWorkReadOnly readOnly => state with { IsReadOnly = readOnly.IsReadOnly },
            ShowWorkDashboard => state with { Ui = state.Ui with { ActiveView = WorkView.Dashboard } },
            ShowWorkQueues => state with { Ui = state.Ui with { ActiveView = WorkView.Queues } },
            ShowWorkServers => state with { Ui = state.Ui with { ActiveView = WorkView.Servers } },
            ShowWorkRecurring => state with { Ui = state.Ui with { ActiveView = WorkView.Recurring } },
            ClearWorkEventLog => state with { EventLogEntries = [] },
            _ => state
        };

        return ShouldAppendEventLog(action)
            ? next with
            {
                EventLogEntries = next.EventLogEntries
                    .Append(new WorkEventLogItem(
                        Guid.NewGuid(),
                        action.GetType().Name,
                        DateTimeOffset.UtcNow,
                        SerializeEventLogState(state),
                        SerializeEventLogState(next),
                        BuildStateDelta(state, next)))
                    .TakeLast(100)
                    .ToList()
            }
            : next;
    }

    public static string SerializeEventLogState(WorkAppState state)
    {
        var trimmed = state with { EventLogEntries = [] };
        return JsonSerializer.Serialize(trimmed, EventLogJsonOptions);
    }

    private static IReadOnlyList<WorkStateDeltaItem> BuildStateDelta(WorkAppState before, WorkAppState after)
    {
        var beforeTrimmed = before with { EventLogEntries = [] };
        var afterTrimmed = after with { EventLogEntries = [] };
        var beforeNode = JsonSerializer.SerializeToNode(beforeTrimmed, EventLogJsonOptions);
        var afterNode = JsonSerializer.SerializeToNode(afterTrimmed, EventLogJsonOptions);
        var deltas = new List<WorkStateDeltaItem>();
        AppendDelta(string.Empty, beforeNode, afterNode, deltas);
        return deltas;
    }

    private static void AppendDelta(string path, JsonNode? before, JsonNode? after, List<WorkStateDeltaItem> deltas)
    {
        if (JsonNode.DeepEquals(before, after))
            return;

        if (before is JsonObject beforeObject && after is JsonObject afterObject)
        {
            var propertyNames = beforeObject.Select(pair => pair.Key)
                .Concat(afterObject.Select(pair => pair.Key))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(name => name, StringComparer.Ordinal);

            foreach (var propertyName in propertyNames)
            {
                var nextPath = string.IsNullOrWhiteSpace(path) ? propertyName : $"{path}.{propertyName}";
                AppendDelta(nextPath, beforeObject[propertyName], afterObject[propertyName], deltas);
            }
            return;
        }

        deltas.Add(new WorkStateDeltaItem(
            path,
            "state",
            HumanizePath(path),
            ToDeltaValue(before),
            ToDeltaValue(after),
            $"{HumanizePath(path)} changed."));
    }

    private static string? ToDeltaValue(JsonNode? node) => node?.ToJsonString();

    private static string HumanizePath(string path)
    {
        var segment = path.Split('.').LastOrDefault() ?? path;
        return System.Text.RegularExpressions.Regex.Replace(segment, "([a-z])([A-Z])", "$1 $2");
    }

    private static bool ShouldAppendEventLog(object action) => action is not ClearWorkEventLog;
}

public abstract record WorkUiEvent : IWorkEvent;

public sealed record WorkItemSelected(string JobId) : WorkUiEvent;

public sealed record WorkQueueSelected(string QueueName) : WorkUiEvent;

public sealed record WorkViewChanged(WorkView View) : WorkUiEvent;

public sealed record WorkCommandSubmitted(string Command, IReadOnlyList<string> SelectedJobIds) : WorkUiEvent;

public sealed record WorkPageChanged(int PageNumber, int PageSize) : WorkUiEvent;
