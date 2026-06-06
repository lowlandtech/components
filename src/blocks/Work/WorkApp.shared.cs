namespace LowlandTech.Components;

public partial class WorkApp
{
    [Parameter] public WorkStateService? StateService { get; set; }
    [Parameter] public bool IsTest { get; set; }
    [Parameter] public int TestStreamDelayMilliseconds { get; set; } = 0;

    private WorkStateService SessionState => StateService ?? (_ownedStateService ??= new WorkStateService());
    private WorkStateService? _ownedStateService;
    private WorkStateService? _subscribedStateService;
    private WorkAppContext? _context;

    private WorkAppContext Context
    {
        get
        {
            if (_context is null || !ReferenceEquals(_context.StateService, SessionState))
            {
                _context = new WorkAppContext(SessionState);
            }

            return _context;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        EnsureSubscription();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        EnsureSubscription();
    }

    private void EnsureSubscription()
    {
        if (ReferenceEquals(_subscribedStateService, SessionState))
            return;

        if (_subscribedStateService is not null)
        {
            _subscribedStateService.StateChanged -= OnStateChanged;
        }

        _subscribedStateService = SessionState;
        _subscribedStateService.StateChanged += OnStateChanged;
        _context = new WorkAppContext(SessionState);
    }

    private void OnStateChanged(WorkAppState _) => InvokeAsync(StateHasChanged);

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? $"grid h-full min-h-[42rem] w-full overflow-hidden rounded-lg border border-border bg-card {GetLayoutClass()}"
        : $"grid h-full min-h-[42rem] w-full overflow-hidden rounded-lg border border-border bg-card {GetLayoutClass()} {Class}";

    private string GetLayoutClass()
    {
        if (Context.ShowSidebar && Context.ShowDetailPanel)
        {
            return "grid-cols-1 lg:grid-cols-[14rem_minmax(0,1fr)] xl:grid-cols-[14rem_minmax(0,1fr)_22rem]";
        }

        if (Context.ShowSidebar)
        {
            return "grid-cols-1 lg:grid-cols-[14rem_minmax(0,1fr)]";
        }

        if (Context.ShowDetailPanel)
        {
            return "grid-cols-1 xl:grid-cols-[minmax(0,1fr)_22rem]";
        }

        return "grid-cols-1";
    }

    public void Dispose()
    {
        if (_subscribedStateService is not null)
        {
            _subscribedStateService.StateChanged -= OnStateChanged;
        }
    }
}

public sealed class WorkAppContext(WorkStateService stateService)
{
    public WorkStateService StateService { get; } = stateService;

    public WorkAppState State => StateService.State;

    public bool ShowSidebar => State.Ui.ActiveView != WorkView.Servers
        && State.Ui.ActiveView != WorkView.Recurring;

    public bool ShowDetailPanel => State.Ui.ActiveView != WorkView.Detail
        && State.SelectedJob is not null;

    public bool ShowCommandBar => State.Ui.ActiveView is WorkView.Enqueued or WorkView.Scheduled
        or WorkView.Processing or WorkView.Succeeded or WorkView.Failed
        or WorkView.Deleted or WorkView.Awaiting or WorkView.Retries
        or WorkView.Recurring;

    public bool ShowFilterBar => State.Ui.ActiveView is WorkView.Enqueued or WorkView.Scheduled
        or WorkView.Processing or WorkView.Succeeded or WorkView.Failed
        or WorkView.Deleted or WorkView.Awaiting or WorkView.Retries;

    public bool ShowPaginator => ShowFilterBar;

    public bool ShowServerColumn => State.Ui.ActiveView == WorkView.Processing;

    public bool ShowQueueColumn => State.Ui.ActiveView is WorkView.Enqueued or WorkView.Queues;

    public bool IsAllSelected()
    {
        var jobs = GetPagedJobs();
        return jobs.Count > 0 && jobs.All(j => State.Ui.SelectedJobIds.Contains(j.Id));
    }

    public void HandleViewChanged(WorkViewChanged e) => StateService.SetView(e.View);

    public void HandleFilterChanged(string filter) => StateService.SetFilter(filter);

    public void HandlePageChanged(int page) => StateService.SetPage(page);

    public void HandlePageSizeChanged(int size) => StateService.SetPageSize(size);

    public void HandleJobSelected(WorkItemSelected e) => StateService.SelectJob(e.JobId);

    public void HandleQueueSelected(WorkQueueSelected e) => StateService.SelectQueue(e.QueueName);

    public void HandleToggleSelection(ToggleWorkSelection e) => StateService.ToggleSelection(e.JobId);

    public void HandleSelectAll(bool select)
    {
        if (select)
        {
            StateService.SelectAll();
        }
        else
        {
            StateService.ClearSelection();
        }
    }

    public void HandleCommand(WorkCommandSubmitted e) => StateService.ExecuteCommand(e.Command);

    public void HandleDismissError() => StateService.Dispatch(new SetWorkPollingError(null));

    public void HandleHomeClick() => StateService.SetView(WorkView.Dashboard);

    public IReadOnlyList<WorkItem> GetPagedJobs() => StateService.GetPagedJobs();

    public WorkPager GetPager() => StateService.GetPager();

    public string GetBreadcrumbTitle() => State.Ui.ActiveView switch
    {
        WorkView.Dashboard => "Dashboard",
        WorkView.Queues => "Queues",
        WorkView.Enqueued => "Enqueued",
        WorkView.Scheduled => "Scheduled",
        WorkView.Processing => "Processing",
        WorkView.Succeeded => "Succeeded",
        WorkView.Failed => "Failed",
        WorkView.Deleted => "Deleted",
        WorkView.Awaiting => "Awaiting",
        WorkView.Retries => "Retries",
        WorkView.Recurring => "Recurring Jobs",
        WorkView.Servers => "Servers",
        WorkView.Detail => "Job Details",
        _ => "Work"
    };

    public IReadOnlyDictionary<string, string> GetBreadcrumbCrumbs()
    {
        var crumbs = new Dictionary<string, string>();
        if (State.Ui.ActiveView != WorkView.Dashboard)
        {
            crumbs["Dashboard"] = "";
        }

        if (!string.IsNullOrWhiteSpace(State.SelectedQueueName))
        {
            crumbs["Queues"] = "";
            crumbs[State.SelectedQueueName] = "";
        }

        return crumbs;
    }
}
