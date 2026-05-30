namespace LowlandTech.Components.Session;

public partial class SessionApp : TinyComponentBase, IDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IGraphDiscoveryService Discovery { get; set; } = default!;
    [Inject] private IServiceProvider Services { get; set; } = default!;

    private SessionStateService? _ownedStateService;
    private SessionStateService? _subscribedStateService;

    [Parameter] public IReadOnlyList<SessionListEntry> Sessions { get; set; } =
    [
        new("orders-session", "Orders"),
        new("support-session", "Support triage"),
        new("release-session", "Release notes")
    ];

    [Parameter] public IReadOnlyList<SessionContextNode> ContextNodes { get; set; } =
    [
        new(
            new("orders", "workitem", "Orders", "OrdersBlock"),
            [
                new(new("invoice-approval", "invoice", "Invoice approval", "InvoiceBlock")),
                new(new("shipment-delay", "shipment", "Shipment delay", "ShipmentBlock"))
            ]),
        new(
            new("customers", "table", "Customers", "DataGridBlock"),
            [
                new(new("northwind", "customer", "Northwind Traders", "CustomerBlock")),
                new(new("contoso", "customer", "Contoso", "CustomerBlock"))
            ])
    ];

    [Parameter] public bool IsTest { get; set; }
    [Parameter] public int TestStreamDelayMilliseconds { get; set; } = 35;
    [Parameter] public bool HasSidebar { get; set; }
    [Parameter] public string? PageOverlayContainerClass { get; set; }
    [Parameter] public string? AppOverlayContainerClass { get; set; }
    [Parameter] public SessionStateService? StateService { get; set; }

    private SessionStateService SessionState => StateService ?? (_ownedStateService ??= new SessionStateService(ResolveSessionSurfaceDiscovery()));

    private SessionAppState AppState => SessionState.State;

    protected override void OnInitialized()
    {
        EnsureStateServiceSubscription();
    }

    protected override void OnParametersSet()
    {
        EnsureStateServiceSubscription();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (string.IsNullOrWhiteSpace(AppState.PendingScrollSectionId))
            return;

        var sectionId = AppState.PendingScrollSectionId;
        await ScrollSectionIntoViewAsync(sectionId);
        SessionState.Dispatch(new SetPendingScrollSection(null));
    }

    private bool HasActiveSession => !string.IsNullOrWhiteSpace(AppState.ActiveSessionId);

    private bool IsEventLogPage => HasActiveSession && AppState.ActivePage == SessionAppPage.EventLog;

    private string CurrentSessionId => SessionState.CurrentSessionId;

    private SessionNodeInput? CurrentSessionNode => SessionState.GetCurrentSessionNode(Sessions);

    private SessionUiState ViewState => SessionState.GetViewState(Sessions);

    private SessionNodeInput? CurrentCommandContextNode => ViewState.ContextProjection?.Node ?? CurrentSessionNode;

    private SessionNodeInput CurrentRequestNode => SessionState.GetRequestNode(CurrentSessionId);

    private SessionNodeInput CurrentResponseNode => SessionState.GetResponseNode(CurrentSessionId);

    private IReadOnlyList<SessionCommandSubmitted> CurrentSubmittedCommands => SessionState.GetSessionCommands(CurrentSessionId);

    private IReadOnlyList<SessionListEntry> DisplaySessions => SessionState.GetDisplaySessions(Sessions);

    private IReadOnlyList<SessionSectionItem> VisibleSections => SessionState.GetVisibleSections(Sessions);

    private SessionContextBlockModel? MaximizedBlock => SessionState.GetMaximizedBlock(Sessions);

    private SessionCommandSubmitted? MaximizedCommand => SessionState.GetMaximizedCommand();

    private SessionContextBlockModel? PageOverlayBlock => SessionState.GetPageOverlayBlock(Sessions);

    private SessionCommandSubmitted? PageOverlayCommand => SessionState.GetPageOverlayCommand();

    private SessionContextBlockModel? AppOverlayBlock => SessionState.GetAppOverlayBlock(Sessions);

    private SessionCommandSubmitted? AppOverlayCommand => SessionState.GetAppOverlayCommand();

    private IReadOnlyList<SessionFeedItemDescriptor> FeedItems => SessionState.GetFeedItems(Sessions);

    private string GetClass()
    {
        var baseClass = "relative min-h-0 w-full overflow-hidden rounded-lg border border-border bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private static string GetStyle() => "height:42rem; min-height:42rem;";

    private string EffectiveAppOverlayContainerClass =>
        AppOverlayContainerClass ??
        (HasSidebar
            ? "absolute inset-0 z-50 flex h-full w-full flex-col overflow-hidden rounded-none bg-background gap-0 py-0"
            : "fixed inset-0 z-50 flex h-dvh w-dvw flex-col overflow-hidden rounded-none bg-background gap-0 py-0");

    private string EffectivePageOverlayContainerClass =>
        PageOverlayContainerClass ??
        "absolute inset-0 z-40 flex h-full w-full flex-col overflow-hidden bg-background";

    private Task ShowSessionsAsync()
    {
        SessionState.ShowSessions();
        return Task.CompletedTask;
    }

    private Task SelectSessionAsync(SessionNodeInput node)
    {
        SessionState.SelectSession(node, Sessions);
        return Task.CompletedTask;
    }

    private Task SetInputAsync(string value)
    {
        SessionState.SetInput(value);
        return Task.CompletedTask;
    }

    private Task SetAgentTypeAsync(string agentTypeKey)
    {
        SessionState.SetAgentType(agentTypeKey);
        return Task.CompletedTask;
    }

    private Task HandleProjectedAsync(SessionContextProjected projected)
    {
        SessionState.ProjectContext(projected.Node);
        return Task.CompletedTask;
    }

    private Task HandlePinnedAsync(SessionContextPinned pinned)
    {
        SessionState.PinContext(pinned.Node);
        return Task.CompletedTask;
    }

    private Task HandleUnpinnedAsync(SessionContextUnpinned unpinned)
    {
        SessionState.UnpinContext(unpinned.PinId);
        return Task.CompletedTask;
    }

    private Task HandleTabPinnedChangedAsync(TabsItem item)
    {
        SessionState.SetTabPinned(item.Value, item.Pinned);
        return Task.CompletedTask;
    }

    private Task HandleTabClosedAsync(SessionNodeInput node)
    {
        SessionState.CloseTab(node, Sessions);
        return Task.CompletedTask;
    }

    private async Task HandleSubmitAsync(SessionCommandSubmitted command)
    {
        await SessionState.SubmitAsync(command, IsTest, TestStreamDelayMilliseconds, Discovery);

        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (input) { input.value = ''; input.focus(); }
                })();
                """);
        }
        catch
        {
        }
    }

    private SessionResponseProjection GetResponseProjection(SessionCommandSubmitted command) => SessionState.GetResponseProjection(command);

    private string GetResponseStatus(SessionCommandSubmitted command) => GetResponseProjection(command).Status;

    private string GetResponseStatusVariant(SessionCommandSubmitted command) => GetResponseProjection(command).StatusVariant;

    private string CreateResponseDescription(SessionCommandSubmitted command) => GetResponseProjection(command).Description;

    private static SessionNodeInput CreateResponseNode(SessionCommandSubmitted command) => SessionStateService.CreateResponseNode(command);

    private static string GetCommandTurnId(SessionCommandSubmitted command) => SessionStateService.GetCommandTurnId(command);

    private bool IsBlockPinned(SessionContextBlockModel block) => (ViewState.ContextPins ?? []).Any(pin => pin.Node.Id == block.Context.Id);

    private Task UnpinBlockAsync(SessionContextBlockModel block)
    {
        SessionState.UnpinBlock(block);
        return Task.CompletedTask;
    }

    private Task HandleBlockModeChangedAsync(SessionContextBlockModel block, string mode)
    {
        SessionState.SetBlockMode(block, mode);
        return Task.CompletedTask;
    }

    private Task HandleCommandModeChangedAsync(SessionCommandSubmitted command, string mode)
    {
        SessionState.SetCommandMode(command, mode);
        return Task.CompletedTask;
    }

    private string GetBlockMode(SessionContextBlockModel block) => SessionState.GetBlockMode(CurrentSessionId, block);

    private Task ShowEventLogPageAsync()
    {
        if (IsEventLogPage)
        {
            SessionState.ShowFeedPage();
        }
        else
        {
            SessionState.ShowEventLogPage();
        }

        return Task.CompletedTask;
    }

    private Task ShowFeedPageAsync()
    {
        SessionState.ShowFeedPage();
        return Task.CompletedTask;
    }

    private Task ClearEventLogAsync()
    {
        SessionState.ClearEventLog();
        return Task.CompletedTask;
    }

    private async Task ScrollSectionIntoViewAsync(string sectionId)
    {
        try
        {
            await JS.InvokeVoidAsync("eval",
                $$"""
                (() => {
                    const target = document.getElementById('{{EscapeJsString(sectionId)}}');
                    const scroller = target?.closest('[data-slot="session-content-body"]');
                    if (!target || !scroller) return;

                    const inset = 10;
                    const targetTop = target.offsetTop - scroller.offsetTop;
                    const targetBottom = targetTop + target.offsetHeight;
                    const visibleTop = scroller.scrollTop + inset;
                    const visibleBottom = scroller.scrollTop + scroller.clientHeight - inset;

                    if (targetTop < visibleTop) {
                        scroller.scrollTo({ top: Math.max(0, targetTop - inset), behavior: 'smooth' });
                    } else if (targetBottom > visibleBottom) {
                        scroller.scrollTo({ top: targetBottom - scroller.clientHeight + inset, behavior: 'smooth' });
                    }
                })();
                """);
        }
        catch
        {
        }
    }

    private static string EscapeJsString(string value) =>
        value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("'", "\\'", StringComparison.Ordinal);

    private void EnsureStateServiceSubscription()
    {
        var service = SessionState;
        if (ReferenceEquals(_subscribedStateService, service))
            return;

        if (_subscribedStateService is not null)
            _subscribedStateService.StateChanged -= OnStateChanged;

        _subscribedStateService = service;
        _subscribedStateService.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(SessionAppState _) => InvokeAsync(StateHasChanged);

    private ISessionSurfaceDiscovery ResolveSessionSurfaceDiscovery() =>
        Services.GetService<ISessionSurfaceDiscovery>() ?? new SessionSurfaceDiscovery([]);

    public void Dispose()
    {
        if (_subscribedStateService is not null)
            _subscribedStateService.StateChanged -= OnStateChanged;
    }
}
