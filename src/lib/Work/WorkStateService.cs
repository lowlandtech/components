namespace LowlandTech.Components;

public sealed class WorkStateService
{
    private WorkAppState _state;
    private readonly IReadOnlyList<WorkItem> _allJobs;

    public WorkStateService()
        : this(WorkUseCaseData.HealthyCluster())
    {
        LoadDashboard();
    }

    public WorkStateService(WorkAppState initialState)
    {
        _state = initialState;
        _allJobs = initialState.Jobs.ToList();
    }

    public WorkAppState State => _state;

    public event Action<WorkAppState>? StateChanged;

    public WorkAppState Dispatch(object action)
    {
        var next = WorkAppReducer.Reduce(_state, action);
        if (EqualityComparer<WorkAppState>.Default.Equals(_state, next))
            return _state;

        _state = next;
        StateChanged?.Invoke(_state);
        return _state;
    }

    public void Reset(WorkAppState? state = null)
    {
        _state = state ?? WorkAppDefaults.Empty;
        StateChanged?.Invoke(_state);
    }

    public void SelectJob(string? jobId)
    {
        Dispatch(new SelectWorkItem(jobId));
        if (!string.IsNullOrWhiteSpace(jobId))
        {
            var job = _state.Jobs.FirstOrDefault(j => j.Id == jobId);
            var history = job is not null ? WorkUseCaseData.BuildHistory(job) : [];
            var continuations = _allJobs.Where(j => j.ParentId == jobId).ToList();
            Dispatch(new SetSelectedWorkJob(job, history, continuations));
        }
        else
        {
            Dispatch(new SetSelectedWorkJob(null, [], []));
        }
    }

    public void SetView(WorkView view)
    {
        Dispatch(new SetWorkView(view));
        Dispatch(new SetWorkLoading(true));
        Dispatch(new SetSelectedWorkJob(null, [], []));

        var filtered = FilterJobsForView(view);
        Dispatch(new SetWorkItems(filtered));
        Dispatch(new SetWorkLoading(false));
    }

    public void SetFilter(string filterText)
    {
        Dispatch(new SetWorkFilter(filterText));
    }

    public void SetPage(int pageNumber)
    {
        Dispatch(new SetWorkPage(pageNumber));
    }

    public void SetPageSize(int pageSize)
    {
        Dispatch(new SetWorkPageSize(pageSize));
    }

    public void ToggleSelection(string jobId)
    {
        Dispatch(new ToggleWorkSelection(jobId));
    }

    public void SelectAll()
    {
        var visibleIds = GetPagedJobs().Select(j => j.Id).ToList();
        Dispatch(new SelectAllWork(visibleIds));
    }

    public void ClearSelection()
    {
        Dispatch(new ClearWorkSelection());
    }

    public void SetSort(WorkSortColumn column, bool ascending)
    {
        Dispatch(new SetWorkSort(column, ascending));
    }

    public void SelectQueue(string? queueName)
    {
        Dispatch(new SetWorkQueueName(queueName));
        if (!string.IsNullOrWhiteSpace(queueName))
        {
            Dispatch(new SetWorkView(WorkView.Enqueued));
            var filtered = _allJobs
                .Where(j => j.QueueName == queueName)
                .ToList();
            Dispatch(new SetWorkItems(filtered));
        }
    }

    public void LoadDashboard()
    {
        Dispatch(new SetWorkView(WorkView.Dashboard));
        Dispatch(new SetWorkMetrics(WorkUseCaseData.BuildMetrics(_allJobs)));
        Dispatch(new SetWorkStatistics(WorkUseCaseData.BuildStatistics(
            _allJobs, _state.Queues, _state.Servers, _state.RecurringJobs)));
    }

    public void LoadQueues()
    {
        Dispatch(new SetWorkView(WorkView.Queues));
        if (_state.Queues.Count == 0)
        {
            Dispatch(new SetWorkQueues(WorkUseCaseData.BuildQueues(_allJobs)));
        }
    }

    public void LoadServers()
    {
        Dispatch(new SetWorkView(WorkView.Servers));
        if (_state.Servers.Count == 0)
        {
            Dispatch(new SetWorkServers(WorkUseCaseData.BuildServers()));
        }
    }

    public void LoadRecurringJobs()
    {
        Dispatch(new SetWorkView(WorkView.Recurring));
        if (_state.RecurringJobs.Count == 0)
        {
            Dispatch(new SetWorkRecurringJobs(WorkUseCaseData.BuildRecurringJobs()));
        }
    }

    public void ShowDashboard() => Dispatch(new ShowWorkDashboard());

    public void ShowQueues() => Dispatch(new ShowWorkQueues());

    public void ShowServers() => Dispatch(new ShowWorkServers());

    public void ShowRecurring() => Dispatch(new ShowWorkRecurring());

    public void ClearEventLog() => Dispatch(new ClearWorkEventLog());

    public string GetCurrentEventLogStateJson() => WorkAppReducer.SerializeEventLogState(_state);

    public string GetInitialEventLogStateJson() => _state.EventLogEntries.FirstOrDefault()?.BeforeStateJson ?? GetCurrentEventLogStateJson();

    public void ExecuteCommand(string command)
    {
        var selectedIds = _state.Ui.SelectedJobIds;
        Dispatch(new SetWorkLoading(true));

        switch (command)
        {
            case "requeue":
                Dispatch(new RequeueWorkJobs(selectedIds));
                break;
            case "delete":
                Dispatch(new DeleteWorkJobs(selectedIds));
                break;
            case "enqueue":
                Dispatch(new EnqueueWorkJobs(selectedIds));
                break;
            case "trigger":
                Dispatch(new TriggerWorkRecurring(selectedIds));
                break;
        }

        Dispatch(new ClearWorkSelection());
        Dispatch(new SetWorkLoading(false));
    }

    public WorkItem? GetJob(string jobId) => _allJobs.FirstOrDefault(j => j.Id == jobId);

    public IReadOnlyList<WorkItem> GetVisibleJobs()
    {
        var jobs = _state.Jobs;
        if (!string.IsNullOrWhiteSpace(_state.Ui.FilterText))
        {
            jobs = jobs.Where(j => j.JobDisplayName.Contains(_state.Ui.FilterText, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return ApplySort(jobs);
    }

    public WorkPager GetPager()
    {
        var total = GetVisibleJobs().Count;
        var pageCount = (int)Math.Ceiling((double)total / _state.Ui.PageSize);
        return new WorkPager(_state.Ui.PageNumber, _state.Ui.PageSize, total, Math.Max(1, pageCount));
    }

    public IReadOnlyList<WorkItem> GetPagedJobs()
    {
        var visible = GetVisibleJobs();
        var skip = (_state.Ui.PageNumber - 1) * _state.Ui.PageSize;
        return visible.Skip(skip).Take(_state.Ui.PageSize).ToList();
    }

    private IReadOnlyList<WorkItem> ApplySort(IReadOnlyList<WorkItem> jobs)
    {
        var sorted = _state.Ui.SortColumn switch
        {
            WorkSortColumn.Id => jobs.OrderBy(j => j.Id),
            WorkSortColumn.State => jobs.OrderBy(j => j.State),
            WorkSortColumn.Job => jobs.OrderBy(j => j.JobDisplayName),
            WorkSortColumn.CreatedAt => jobs.OrderBy(j => j.CreatedAt),
            WorkSortColumn.EnqueuedAt => jobs.OrderBy(j => j.EnqueuedAt),
            WorkSortColumn.ScheduledAt => jobs.OrderBy(j => j.ScheduledAt),
            WorkSortColumn.StartedAt => jobs.OrderBy(j => j.StartedAt),
            WorkSortColumn.SucceededAt => jobs.OrderBy(j => j.SucceededAt),
            WorkSortColumn.FailedAt => jobs.OrderBy(j => j.FailedAt),
            WorkSortColumn.DeletedAt => jobs.OrderBy(j => j.DeletedAt),
            WorkSortColumn.Duration => jobs.OrderBy(j => j.Duration),
            WorkSortColumn.Latency => jobs.OrderBy(j => j.Latency),
            WorkSortColumn.Server => jobs.OrderBy(j => j.ServerName),
            WorkSortColumn.Queue => jobs.OrderBy(j => j.QueueName),
            _ => jobs.OrderBy(j => j.CreatedAt)
        };

        return (_state.Ui.SortAscending ? sorted : sorted.Reverse()).ToList();
    }

    private IReadOnlyList<WorkItem> FilterJobsForView(WorkView view)
    {
        if (_allJobs.Count == 0) return [];

        return view switch
        {
            WorkView.Enqueued => _allJobs.Where(j => j.State == WorkItemState.Enqueued).ToList(),
            WorkView.Scheduled => _allJobs.Where(j => j.State == WorkItemState.Scheduled).ToList(),
            WorkView.Processing => _allJobs.Where(j => j.State == WorkItemState.Processing).ToList(),
            WorkView.Succeeded => _allJobs.Where(j => j.State == WorkItemState.Succeeded).ToList(),
            WorkView.Failed => _allJobs.Where(j => j.State == WorkItemState.Failed).ToList(),
            WorkView.Deleted => _allJobs.Where(j => j.State == WorkItemState.Deleted).ToList(),
            WorkView.Awaiting => _allJobs.Where(j => j.State == WorkItemState.Awaiting).ToList(),
            WorkView.Retries => _allJobs.Where(j => j.State == WorkItemState.Retry).ToList(),
            _ => _allJobs.ToList()
        };
    }
}
