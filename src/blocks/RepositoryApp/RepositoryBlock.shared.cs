namespace LowlandTech.Components;

public partial class RepositoryBlock
{
    [Parameter] public RepositoryStateService? StateService { get; set; }
    [Parameter] public bool IsTest { get; set; }
    [Parameter] public int TestStreamDelayMilliseconds { get; set; } = 0;

    private RepositoryStateService SessionState => StateService ?? (_ownedStateService ??= new RepositoryStateService());
    private RepositoryStateService? _ownedStateService;
    private RepositoryStateService? _subscribedStateService;
    private RepositoryAppContext? _context;

    private RepositoryAppContext Context
    {
        get
        {
            if (_context is null || !ReferenceEquals(_context.StateService, SessionState))
            {
                _context = new RepositoryAppContext(SessionState);
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
        _context = new RepositoryAppContext(SessionState);
    }

    private void OnStateChanged(RepositoryAppState _) => InvokeAsync(StateHasChanged);

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex h-full min-h-[42rem] w-full flex-col overflow-hidden rounded-lg border border-border bg-background text-foreground"
        : $"flex h-full min-h-[42rem] w-full flex-col overflow-hidden rounded-lg border border-border bg-background text-foreground {Class}";

    public void Dispose()
    {
        if (_subscribedStateService is not null)
        {
            _subscribedStateService.StateChanged -= OnStateChanged;
        }
    }
}

public sealed class RepositoryAppContext(RepositoryStateService stateService)
{
    public RepositoryStateService StateService { get; } = stateService;
    public RepositoryAppState State => StateService.State;
    public RepositoryItem? Current => State.Current;

    public bool ShowSidebar => State.Ui.ActiveView == RepositoryView.Code
        && State.Ui.ActiveCodeView == RepositoryCodeView.Files
        && State.Current is not null
        && string.IsNullOrEmpty(State.Ui.CurrentPath)
        && State.Ui.SelectedFilePaths.Count > 0;

    public bool ShowSubMenu => State.Ui.ActiveView == RepositoryView.Code
        && State.Current is not null;

    public bool ShowPaginator => State.Ui.ActiveView == RepositoryView.Code
        && State.Current is not null
        && State.Ui.ActiveCodeView is RepositoryCodeView.Files or RepositoryCodeView.Commits;

    public bool ShowClonePanel => State.Ui.ShowClonePanel && State.CloneInfo is not null;

    public bool HasOpenIssues => State.Current?.OpenIssues > 0;
    public bool HasOpenPulls => State.Current?.OpenPulls > 0;
    public bool HasReleases => State.Releases.Count > 0;
    public bool HasLanguages => State.Current?.Languages.Count > 0;

    public void HandleViewChanged(RepositoryViewChanged e) => StateService.SetView(e.View);

    public void HandleCodeViewChanged(RepositoryCodeViewChanged e) => StateService.SetCodeView(e.CodeView);

    public void HandleBranchChanged(RepositoryBranchChanged e) => StateService.SetBranch(e.Branch);

    public void HandlePathChanged(RepositoryPathChanged e) => StateService.SetPath(e.Path);

    public void HandleFilterChanged(RepositoryFilterChanged e) => StateService.SetFilter(e.FilterText);

    public void HandlePageChanged(int page) => StateService.SetPage(page);

    public void HandlePageSizeChanged(int size) => StateService.SetPageSize(size);

    public void HandleFileSelected(RepositoryFileSelected e) => StateService.ToggleFileSelection(e.Path);

    public void HandleSortChanged(RepositorySortChanged e) => StateService.SetSort(e.Column, e.Ascending);

    public void HandleFileTreeToggled() => StateService.ToggleFileTree();

    public void HandleClonePanelToggled() => StateService.ToggleClonePanel();

    public void HandleWatchToggled() => StateService.ToggleWatch();

    public void HandleStarToggled() => StateService.ToggleStar();

    public void HandleForkRequested() => StateService.IncrementFork();

    public void HandleDismissError() => StateService.Dispatch(new SetRepositoryPollingError(null));

    public void HandleHomeClick() => StateService.SetView(RepositoryView.Code);

    public IReadOnlyList<RepositoryFileItem> GetPagedFiles() => StateService.GetPagedFiles();

    public IReadOnlyList<RepositoryBreadcrumbCrumb> GetPathBreadcrumbs() => StateService.GetPathBreadcrumbs();

    public IReadOnlyList<RepositoryFileItem> GetRootFiles() => StateService.GetRootFiles();

    public RepositoryPager GetPager() => StateService.GetPager();

    public string GetOwnerName() => State.Current?.Owner.Name ?? string.Empty;

    public string GetRepoName() => State.Current?.Name ?? string.Empty;

    public string GetRepoDescription() => State.Current?.Description ?? string.Empty;

    public bool IsRepoArchived() => State.Current?.IsArchived ?? false;

    public bool IsRepoPrivate() => State.Current?.Visibility == RepositoryVisibility.Private;

    public bool IsRepoInternal() => State.Current?.Visibility == RepositoryVisibility.Internal;

    public bool IsRepoTemplate() => State.Current?.IsTemplate ?? false;

    public bool IsRepoFork() => State.Current?.IsFork ?? false;

    public bool IsRepoMirror() => State.Current?.IsMirror ?? false;
}
