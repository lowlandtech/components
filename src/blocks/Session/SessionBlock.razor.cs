namespace LowlandTech.Components;

public partial class SessionBlock : TinyComponentBase, IDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IGraphDiscoveryService Discovery { get; set; } = default!;
    [Inject] private IServiceProvider Services { get; set; } = default!;

    private IEdgeTypeService? EdgeTypeService => Services.GetService<IEdgeTypeService>();
    private IExecutionEngine? ExecutionEngine => Services.GetService<IExecutionEngine>();

    private SessionStateService? _ownedStateService;
    private SessionStateService? _subscribedStateService;
    private bool _isStandaloneSidebarOpen = true;
    private bool _isHydratingStandaloneSidebar;
    private int _standaloneSidebarChangeVersion;
    private string? _hydratedSidebarPersistKey;
    private bool _isRightSidebarOpen = true;
    private bool _isHydratingRightSidebar;
    private int _rightSidebarChangeVersion;
    private string? _hydratedRightSidebarPersistKey;
    private string _activeRightSidebarTab = RightSidebarSectionsTabKey;
    private bool _isCommandBarVisible;
    private bool _isHydratingCommandBar;
    private int _commandBarChangeVersion;
    private string? _hydratedCommandBarPersistKey;
    private readonly List<SessionListEntry> _createdSessions = [];
    private readonly HashSet<string> _expandedContextNodeIds = new(StringComparer.Ordinal);
    private List<SessionContextNode>? _localContextNodes;
    private bool _showAddToListDialog;
    private SessionNodeInput? _addToListNode;
    private IReadOnlyList<SessionNodeInput> _addToListCandidates = [];
    private IReadOnlySet<string> _addToListMemberIds = new HashSet<string>();
    private const string RightSidebarPluginTabKey = "plugins";
    private const string RightSidebarSectionsTabKey = "sections";

    private IReadOnlyList<SessionContextNode> DisplayContextNodes => _localContextNodes ?? ContextNodes;

    [Parameter] public IReadOnlyList<SessionListEntry> Sessions { get; set; } =
    [
        new("workspaces-session", "Workspaces"),
        new("flows-session", "Flows"),
        new("workitems-session", "Workitems")
    ];

    [Parameter] public IReadOnlyList<SessionContextNode> ContextNodes { get; set; } =
    [
        new(
            new("workspaces", "workspaces.header", "Workspaces", "NodeBlock"),
            [
                new(new("foundry", "workspace", "Foundry", "NodeBlock"), IsLoaded: true),
                new(new("acme", "workspace", "Acme", "NodeBlock"), IsLoaded: true)
            ],
            true),
        new(
            new("flows", "flow.header", "Flows", "NodeBlock"),
            [
                new(new("onboarding", "flow", "Onboarding", "NodeBlock"), IsLoaded: true),
                new(new("approval", "flow", "Approval", "NodeBlock"), IsLoaded: true)
            ],
            true),
        new(
            new("workitems", "workitems.header", "Workitems", "NodeBlock"),
            [
                new(new("setup-ci", "workitem", "Set up CI", "NodeBlock"), IsLoaded: true),
                new(new("write-docs", "workitem", "Write docs", "NodeBlock"), IsLoaded: true)
            ],
            true)
    ];

    [Parameter] public bool IsTest { get; set; }
    [Parameter] public bool IsContentLoading { get; set; }
    [Parameter] public int TestStreamDelayMilliseconds { get; set; } = 35;
    [Parameter] public bool HasSidebar { get; set; }
    [Parameter] public string SidebarPersistKey { get; set; } = "session-block:sidebar-open";
    [Parameter] public string RightSidebarPersistKey { get; set; } = "session-block:right-sidebar-open";
    [Parameter] public string CommandBarVisiblePersistKey { get; set; } = "session-block:command-bar-visible";
    [Parameter] public string ResizablePersistKey { get; set; } = "session-block:panel-sizes";
    [Parameter] public int? SidebarWidth { get; set; }
    [Parameter] public string Variant { get; set; } = "outlined";
    [Parameter] public string? PageOverlayContainerClass { get; set; }
    [Parameter] public string? AppOverlayContainerClass { get; set; }
    [Parameter] public RenderFragment? RightSidebarContent { get; set; }
    [Parameter] public string RightSidebarContentLabel { get; set; } = "Plugins";
    [Parameter] public SessionStateService? StateService { get; set; }
    [Parameter] public string? Language { get; set; }
    [Parameter] public string LogoSrc { get; set; } = "_content/LowlandTech.Components/img/session-block-logo.svg";
    [Parameter] public string? Title { get; set; }
    [Parameter] public EventCallback<SessionHeaderCreateRequest> OnHeaderCreateRequested { get; set; }
    [Parameter] public EventCallback<SessionListCreateRequest> OnListCreateRequested { get; set; }
    [Parameter] public EventCallback<SessionAddToListRequest> OnAddToListRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeRenameRequest> OnNodeRenameRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveUpRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveDownRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeDeleteRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnLoadChildrenRequested { get; set; }
    [Parameter] public EventCallback<BlockIntent> OnNodeIntentSelected { get; set; }

    private IStringLocalizer<SessionBlockStrings>? _localizer;

    private SessionStateService SessionState => StateService ?? (_ownedStateService ??= CreateOwnedStateService());

    private SessionStateService CreateOwnedStateService()
    {
        var outbox = new InMemorySessionOutbox();
        var service = new SessionStateService(ResolveSessionSurfaceDiscovery(), outbox: outbox);
        service.RegisterEffect((state, action, ct) =>
        {
            if (action is ISessionSideEffect sideEffect)
            {
                outbox.Append(new SessionOutboxEntry(sideEffect.GetType().Name, sideEffect, DateTimeOffset.UtcNow));
            }
            return Task.CompletedTask;
        });
        return service;
    }

    private SessionBlockState AppState => SessionState.State;

    protected override void OnInitialized()
    {
        _localizer = Services.GetService<IStringLocalizer<SessionBlockStrings>>();
        EnsureStateServiceSubscription();
    }

    protected override void OnParametersSet()
    {
        EnsureStateServiceSubscription();

        if (!string.IsNullOrWhiteSpace(Language))
        {
            var culture = new CultureInfo(Language);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        if (!HasSidebar)
        {
            _isStandaloneSidebarOpen = true;
            _hydratedSidebarPersistKey = null;
        }

        if (!HasRightSidebarPluginContent)
        {
            _activeRightSidebarTab = RightSidebarSectionsTabKey;
        }
        else if (!string.Equals(_activeRightSidebarTab, RightSidebarPluginTabKey, StringComparison.Ordinal)
            && !string.Equals(_activeRightSidebarTab, RightSidebarSectionsTabKey, StringComparison.Ordinal))
        {
            _activeRightSidebarTab = RightSidebarSectionsTabKey;
        }
    }

    private string L(string key) => _localizer?[key] ?? key;

    private string EffectiveTitle => Title ?? L("Title");

    private string GetCommandPlaceholder()
    {
        if (string.Equals(ViewState.ActiveAgentTypeKey, "create", StringComparison.OrdinalIgnoreCase))
            return L("CreateCommandPlaceholder");

        var node = CurrentCommandContextNode;
        if (node is not null && EdgeTypeService?.FindEdgeByAgentTypeKey(node.NodeTypeKey, ViewState.ActiveAgentTypeKey) is not null)
            return L("CreateCommandPlaceholder");

        if (string.Equals(ViewState.ActiveAgentTypeKey, "edit", StringComparison.OrdinalIgnoreCase))
            return L("EditCommandPlaceholder");

        return L("CommandPlaceholder");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await HydrateStandaloneSidebarAsync();
        await HydrateRightSidebarAsync();
        await HydrateCommandBarAsync();

        if (!string.IsNullOrWhiteSpace(AppState.PendingScrollSectionId))
        {
            var sectionId = AppState.PendingScrollSectionId;
            await ScrollSectionIntoViewAsync(sectionId);
            SessionState.Dispatch(new SetPendingScrollSection(null));
        }
        else if (HasStreamingCommand)
        {
            await ScrollStreamingResponseIntoViewAsync();
        }
    }

    private bool HasActiveSession => !string.IsNullOrWhiteSpace(AppState.ActiveSessionId);

    private bool HasStreamingCommand =>
        MaximizedCommand?.IsStreaming == true
        || PageOverlayCommand?.IsStreaming == true
        || AppOverlayCommand?.IsStreaming == true
        || FeedItems.Any(item => item.Kind == SessionFeedItemKind.Response && item.Command?.IsStreaming == true);

    private bool IsEventLogPage => HasActiveSession && AppState.ActivePage == SessionBlockPage.EventLog;

    private bool IsSettingsPage => HasActiveSession && AppState.ActivePage == SessionBlockPage.Settings;

    private bool IsStandaloneSidebarOpen => !HasSidebar || _isStandaloneSidebarOpen;

    private string CurrentSessionId => SessionState.CurrentSessionId;

    private SessionNodeInput? CurrentSessionNode => SessionState.GetCurrentSessionNode(DisplaySessions);

    private SessionUiState ViewState => SessionState.GetViewState(DisplaySessions);

    private SessionNodeInput? CurrentCommandContextNode => ViewState.ContextProjection?.Node ?? CurrentSessionNode;

    private IReadOnlyList<SessionCommandSubmitted> CurrentSubmittedCommands => SessionState.GetSessionCommands(CurrentSessionId);

    private IReadOnlyList<SessionListEntry> DisplaySessions => SessionState.GetDisplaySessions([.. Sessions, .. _createdSessions]);

    private IReadOnlyList<SessionSectionItem> VisibleSections => SessionState.GetVisibleSections(Sessions);

    private SessionContextBlockModel? MaximizedBlock => SessionState.GetMaximizedBlock(Sessions);

    private SessionCommandSubmitted? MaximizedCommand => SessionState.GetMaximizedCommand();

    private bool HasMaximizedContent => MaximizedBlock is not null || MaximizedCommand is not null;

    private SessionContextBlockModel? PageOverlayBlock => SessionState.GetPageOverlayBlock(Sessions);

    private SessionCommandSubmitted? PageOverlayCommand => SessionState.GetPageOverlayCommand();

    private SessionContextBlockModel? AppOverlayBlock => SessionState.GetAppOverlayBlock(Sessions);

    private SessionCommandSubmitted? AppOverlayCommand => SessionState.GetAppOverlayCommand();

    private IReadOnlyList<SessionFeedItemDescriptor> FeedItems => SessionState.GetFeedItems(Sessions);

    private IReadOnlyList<SessionAgentType> LocalizedAgentTypes
    {
        get
        {
            var agentTypes = new List<SessionAgentType>
            {
                new("ask", L("AgentAsk")),
                new("plan", L("AgentPlan")),
                new("edit", L("AgentEdit")),
                new("create", L("AgentCreate")),
                new("run", L("AgentRun"), Disabled: true)
            };

            var node = CurrentCommandContextNode;
            if (node is null || EdgeTypeService is null)
                return agentTypes;

            foreach (var edge in EdgeTypeService.GetOutgoingEdges(node.NodeTypeKey))
            {
                if (!string.Equals(edge.Surface, "command-bar", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(edge.Intent)
                    || agentTypes.Any(agent => string.Equals(agent.Key, edge.Intent, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                agentTypes.Add(new SessionAgentType(edge.Intent, edge.Label));
            }

            return agentTypes;
        }
    }

    private string GetClass()
    {
        var frameClass = IsNoneVariant
            ? string.Empty
            : "rounded-lg border border-border";
        var bgClass = IsNoneVariant ? "bg-sidebar" : "bg-background";
        var baseClass = $"relative min-h-0 w-full overflow-hidden {bgClass} {frameClass}".Trim();
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private bool IsNoneVariant => string.Equals(Variant, "none", StringComparison.OrdinalIgnoreCase);

    private string GetLeftSidebarClass() => IsNoneVariant
        ? "h-full min-h-0 border-r-0"
        : "h-full min-h-0 border-r-0";

    private string GetLeftPanelClass() => IsNoneVariant
        ? "flex h-full min-h-0 w-full flex-col overflow-hidden bg-sidebar"
        : "flex h-full min-h-0 w-full flex-col overflow-hidden border-r border-border bg-sidebar/40";

    private string GetRightSidebarClass() => IsNoneVariant
        ? "flex h-full min-h-0 min-w-0 w-full flex-col overflow-hidden bg-sidebar"
        : "flex h-full min-h-0 min-w-0 w-full flex-col overflow-hidden border-l border-border bg-sidebar/40";

    private string GetRightSidebarBodyStyle() => IsNoneVariant
        ? "padding:10px 10px 10px 4px;"
        : "padding:10px;";

    private bool HasRegisteredSessionPlugins => Services.GetServices<ISessionPlugin>().Any();

    private bool HasRightSidebarPluginContent => RightSidebarContent is not null || HasRegisteredSessionPlugins;

    private RenderFragment? EffectiveRightSidebarContent => RightSidebarContent ?? (HasRegisteredSessionPlugins ? RenderRegisteredSessionPlugins : null);

    private RenderFragment RenderRegisteredSessionPlugins => builder =>
    {
        builder.OpenComponent<SessionPluginHost>(0);
        builder.AddAttribute(1, nameof(SessionPluginHost.ActiveSessionId), HasActiveSession ? CurrentSessionId : null);
        builder.AddAttribute(2, nameof(SessionPluginHost.Sessions), DisplaySessions);
        builder.AddAttribute(3, nameof(SessionPluginHost.ContextNodes), DisplayContextNodes);
        builder.AddAttribute(4, nameof(SessionPluginHost.BlockContext), Services.GetService<BlockContext>() ?? new BlockContext());
        builder.CloseComponent();
    };

    private bool IsRightSidebarPluginTabActive =>
        HasRightSidebarPluginContent
        && string.Equals(_activeRightSidebarTab, RightSidebarPluginTabKey, StringComparison.Ordinal);

    private bool IsRightSidebarSectionsTabActive =>
        !IsRightSidebarPluginTabActive;

    private string GetRightSidebarTabClass(string tabKey)
    {
        var isActive = string.Equals(_activeRightSidebarTab, tabKey, StringComparison.Ordinal);
        return isActive
            ? "inline-flex h-7 items-center rounded-md bg-accent px-2 text-xs font-semibold text-foreground"
            : "inline-flex h-7 items-center rounded-md px-2 text-xs font-medium text-muted-foreground transition-colors hover:bg-muted hover:text-foreground";
    }

    private Task SelectRightSidebarTabAsync(string tabKey)
    {
        _activeRightSidebarTab = tabKey;
        return Task.CompletedTask;
    }

    private string GetMainContentStyle(bool isLeftOpen)
    {
        var (left, right) = GetMainContentHorizontalSpacing(isLeftOpen);
        return $"padding:10px {right} 10px {left};";
    }

    private (string Left, string Right) GetMainContentHorizontalSpacing(bool isLeftOpen)
    {
        var left = isLeftOpen ? (IsNoneVariant ? "4px" : "6px") : "10px";
        var right = _isRightSidebarOpen
            ? IsNoneVariant ? "4px" : "6px"
            : "10px";
        return (left, right);
    }

    private string GetSidebarInsetClass() => IsNoneVariant
        ? "h-full min-h-0 overflow-hidden bg-sidebar"
        : "h-full min-h-0 overflow-hidden bg-background";

    private static string GetStyle() => "width:100%; height:100%; min-height:0;";

    private string EffectiveAppOverlayContainerClass =>
        AppOverlayContainerClass ??
        (HasSidebar
            ? "absolute inset-0 z-50 flex h-full w-full flex-col overflow-hidden rounded-none bg-background gap-0 py-0"
            : "fixed inset-0 z-50 flex h-dvh w-dvw flex-col overflow-hidden rounded-none bg-background gap-0 py-0");

    private string EffectivePageOverlayContainerClass =>
        $"{PageOverlayContainerClass ?? "absolute inset-0 z-40 flex h-full w-full flex-col bg-background"} overflow-hidden rounded-lg";

    private string? GetPageOverlayContainerStyle(bool isLeftOpen)
    {
        if (!IsNoneVariant)
            return null;

        var (left, right) = GetMainContentHorizontalSpacing(isLeftOpen);
        return $"top:10px; right:{right}; bottom:10px; left:{left}; width:auto; height:auto;";
    }

    private RenderFragment SidebarOpenButton => builder =>
    {
        builder.OpenComponent<PaneToggleButton>(0);
        builder.AddAttribute(1, nameof(PaneToggleButton.Side), "left");
        builder.AddAttribute(2, nameof(PaneToggleButton.Action), "open");
        builder.AddAttribute(3, nameof(PaneToggleButton.BorderSide), "right");
        builder.AddAttribute(4, nameof(PaneToggleButton.TestId), "session-block-sidebar-open");
        builder.AddAttribute(5, nameof(PaneToggleButton.Title), L("OpenSidebar"));
        builder.AddAttribute(6, nameof(PaneToggleButton.AriaLabel), L("OpenSidebar"));
        builder.AddAttribute(7, nameof(PaneToggleButton.OnClick), EventCallback.Factory.Create(this, OpenStandaloneSidebarAsync));
        builder.CloseComponent();
    };

    private RenderFragment RightSidebarOpenButton => builder =>
    {
        builder.OpenComponent<PaneToggleButton>(0);
        builder.AddAttribute(1, nameof(PaneToggleButton.Side), "right");
        builder.AddAttribute(2, nameof(PaneToggleButton.Action), "open");
        builder.AddAttribute(3, nameof(PaneToggleButton.BorderSide), "left");
        builder.AddAttribute(4, nameof(PaneToggleButton.TestId), "session-block-right-sidebar-open");
        builder.AddAttribute(5, nameof(PaneToggleButton.Title), L("OpenSections"));
        builder.AddAttribute(6, nameof(PaneToggleButton.AriaLabel), L("OpenSections"));
        builder.AddAttribute(7, nameof(PaneToggleButton.OnClick), EventCallback.Factory.Create(this, OpenRightSidebarAsync));
        builder.CloseComponent();
    };

    private Task ShowSessionsAsync()
    {
        SessionState.ShowSessions();
        return Task.CompletedTask;
    }

    private async Task HandleStandaloneSidebarOpenChanged(bool isOpen)
    {
        _isStandaloneSidebarOpen = isOpen;
        _standaloneSidebarChangeVersion++;
        await PersistStandaloneSidebarAsync();
    }

    private async Task OpenStandaloneSidebarAsync()
    {
        _isStandaloneSidebarOpen = true;
        _standaloneSidebarChangeVersion++;
        await PersistStandaloneSidebarAsync();
    }

    private async Task HydrateStandaloneSidebarAsync()
    {
        if (!HasSidebar ||
            _isHydratingStandaloneSidebar ||
            string.Equals(_hydratedSidebarPersistKey, SidebarPersistKey, StringComparison.Ordinal))
        {
            return;
        }

        _isHydratingStandaloneSidebar = true;
        var changeVersion = _standaloneSidebarChangeVersion;

        try
        {
            var storedValue = await JS.InvokeAsync<string?>("localStorage.getItem", SidebarPersistKey);
            if (changeVersion == _standaloneSidebarChangeVersion &&
                bool.TryParse(storedValue, out var isOpen) &&
                isOpen != _isStandaloneSidebarOpen)
            {
                _isStandaloneSidebarOpen = isOpen;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best effort when browser storage is unavailable.
        }
        finally
        {
            _hydratedSidebarPersistKey = SidebarPersistKey;
            _isHydratingStandaloneSidebar = false;
        }
    }

    private async Task PersistStandaloneSidebarAsync()
    {
        try
        {
            await JS.InvokeVoidAsync(
                "localStorage.setItem",
                SidebarPersistKey,
                _isStandaloneSidebarOpen.ToString().ToLowerInvariant());
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best effort when browser storage is unavailable.
        }
    }

    private async Task CloseRightSidebarAsync()
    {
        _isRightSidebarOpen = false;
        _rightSidebarChangeVersion++;
        await PersistRightSidebarAsync();
    }

    private async Task OpenRightSidebarAsync()
    {
        _isRightSidebarOpen = true;
        _rightSidebarChangeVersion++;
        await PersistRightSidebarAsync();
    }

    private async Task HydrateRightSidebarAsync()
    {
        if (_isHydratingRightSidebar ||
            string.Equals(_hydratedRightSidebarPersistKey, RightSidebarPersistKey, StringComparison.Ordinal))
        {
            return;
        }

        _isHydratingRightSidebar = true;
        var changeVersion = _rightSidebarChangeVersion;

        try
        {
            var storedValue = await JS.InvokeAsync<string?>("localStorage.getItem", RightSidebarPersistKey);
            if (changeVersion == _rightSidebarChangeVersion &&
                bool.TryParse(storedValue, out var isOpen) &&
                isOpen != _isRightSidebarOpen)
            {
                _isRightSidebarOpen = isOpen;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best effort when browser storage is unavailable.
        }
        finally
        {
            _hydratedRightSidebarPersistKey = RightSidebarPersistKey;
            _isHydratingRightSidebar = false;
        }
    }

    private async Task PersistRightSidebarAsync()
    {
        try
        {
            await JS.InvokeVoidAsync(
                "localStorage.setItem",
                RightSidebarPersistKey,
                _isRightSidebarOpen.ToString().ToLowerInvariant());
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best effort when browser storage is unavailable.
        }
    }

    private async Task ToggleCommandBarAsync()
    {
        var wasVisible = _isCommandBarVisible;
        _isCommandBarVisible = !_isCommandBarVisible;
        _commandBarChangeVersion++;
        await PersistCommandBarAsync();

        if (!wasVisible)
        {
            PromoteActiveCardToMaximized();
        }
        else
        {
            DemoteActiveCardToPageOverlay();
        }
    }

    private async Task ShowCommandBarAsync()
    {
        if (_isCommandBarVisible) return;
        _isCommandBarVisible = true;
        _commandBarChangeVersion++;
        await PersistCommandBarAsync();
    }

    private void PromoteActiveCardToMaximized()
    {
        if (PageOverlayBlock is { } pageOverlayBlock)
        {
            SessionState.SetBlockMode(pageOverlayBlock, SessionContextBlock.ModeMaximized);
            return;
        }

        if (PageOverlayCommand is { } pageOverlayCommand)
        {
            SessionState.SetCommandMode(pageOverlayCommand, SessionContextBlock.ModeMaximized);
            return;
        }

        if (AppOverlayBlock is { } appOverlayBlock)
        {
            SessionState.SetBlockMode(appOverlayBlock, SessionContextBlock.ModeMaximized);
            return;
        }

        if (AppOverlayCommand is { } appOverlayCommand)
        {
            SessionState.SetCommandMode(appOverlayCommand, SessionContextBlock.ModeMaximized);
            return;
        }

        if (GetProjectedBlock() is { } projectedBlock)
        {
            SessionState.SetBlockMode(projectedBlock, SessionContextBlock.ModeMaximized);
        }
    }

    private void DemoteActiveCardToPageOverlay()
    {
        if (MaximizedBlock is { } maximizedBlock && IsProjectedBlock(maximizedBlock))
        {
            SessionState.SetBlockMode(maximizedBlock, SessionContextBlock.ModeOverlayPage);
            return;
        }

        if (MaximizedCommand is { } maximizedCommand)
        {
            SessionState.SetCommandMode(maximizedCommand, SessionContextBlock.ModeOverlayPage);
            return;
        }

        if (AppOverlayBlock is { } appOverlayBlock && IsProjectedBlock(appOverlayBlock))
        {
            SessionState.SetBlockMode(appOverlayBlock, SessionContextBlock.ModeOverlayPage);
            return;
        }

        if (AppOverlayCommand is { } appOverlayCommand)
        {
            SessionState.SetCommandMode(appOverlayCommand, SessionContextBlock.ModeOverlayPage);
            return;
        }

        if (GetProjectedBlock() is { } projectedBlock)
        {
            SessionState.SetBlockMode(projectedBlock, SessionContextBlock.ModeOverlayPage);
        }
    }

    private SessionContextBlockModel? GetProjectedBlock() =>
        ViewState.ContextBlocks?.FirstOrDefault(candidate =>
            string.Equals(candidate.Id, "projected-context-block", StringComparison.Ordinal));

    private static bool IsProjectedBlock(SessionContextBlockModel block) =>
        string.Equals(block.Id, "projected-context-block", StringComparison.Ordinal);

    private async Task HydrateCommandBarAsync()
    {
        if (_isHydratingCommandBar ||
            string.Equals(_hydratedCommandBarPersistKey, CommandBarVisiblePersistKey, StringComparison.Ordinal))
        {
            return;
        }

        _isHydratingCommandBar = true;
        var changeVersion = _commandBarChangeVersion;

        try
        {
            var storedValue = await JS.InvokeAsync<string?>("localStorage.getItem", CommandBarVisiblePersistKey);
            if (changeVersion == _commandBarChangeVersion &&
                bool.TryParse(storedValue, out var isVisible) &&
                isVisible != _isCommandBarVisible)
            {
                _isCommandBarVisible = isVisible;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best effort when browser storage is unavailable.
        }
        finally
        {
            _hydratedCommandBarPersistKey = CommandBarVisiblePersistKey;
            _isHydratingCommandBar = false;
        }
    }

    private async Task PersistCommandBarAsync()
    {
        try
        {
            await JS.InvokeVoidAsync(
                "localStorage.setItem",
                CommandBarVisiblePersistKey,
                _isCommandBarVisible.ToString().ToLowerInvariant());
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best effort when browser storage is unavailable.
        }
    }

    private Task SelectSessionAsync(SessionNodeInput node)
    {
        SessionState.SelectSession(node, DisplaySessions);
        return Task.CompletedTask;
    }

    private async Task HandleContextCreateAsync()
    {
        var userContextNode = DisplayContextNodes.FirstOrDefault();
        if (userContextNode is null)
            return;

        // Only treat the root as a user-context parent when it really is one.
        // In standalone/storybook the roots are headers (workspaces, flows, ...)
        // and Lists should be created as a root-level header instead.
        var isUserContext = string.Equals(
            userContextNode.Node.NodeTypeKey,
            "user-context",
            StringComparison.OrdinalIgnoreCase);

        var listsContextNode = new SessionNodeInput(
            $"lists-context:{userContextNode.Node.Id}",
            "lists",
            L("Lists"),
            "NodeBlock",
            new Dictionary<string, object?>
            {
                ["userContextId"] = isUserContext ? userContextNode.Node.Id : null
            });

        await PrepareCommandBarAccessAsync(listsContextNode);
        SessionState.SetAgentType("create");

        await Task.Yield();

        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (input) { input.focus({ preventScroll: true }); }
                })();
                """);
        }
        catch { }
    }

    private async Task CreateSessionAsync()
    {
        var id = Guid.NewGuid().ToString();
        var entry = new SessionListEntry(id, L("NewSession"), 0, "session");
        _createdSessions.Add(entry);
        var node = new SessionNodeInput(id, "session", entry.Label);
        SessionState.SelectSession(node, DisplaySessions);
        SessionState.SetAgentType("create");
        SessionState.Dispatch(new ScheduleCreateEffect("session", id, entry.Label));

        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (input) { input.focus({ preventScroll: true }); }
                })();
                """);
        }
        catch
        {
        }
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
        var block = ViewState.ContextBlocks?
            .FirstOrDefault(candidate => string.Equals(candidate.Id, "projected-context-block", StringComparison.Ordinal));
        if (block is not null)
        {
            var targetMode = _isCommandBarVisible
                ? SessionContextBlock.ModeMaximized
                : SessionContextBlock.ModeOverlayPage;
            SessionState.SetBlockMode(block, targetMode);
        }

        return Task.CompletedTask;
    }

    private Task ScrollToPinnedContextAsync(SessionNodeInput node)
    {
        SessionState.Dispatch(new SetPendingScrollSection($"context-block:{node.Id}"));
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

    private async Task HandleNodeMoveUpAsync(SessionNodeInput node)
        => await OnNodeMoveUpRequested.InvokeAsync(node);

    private async Task HandleNodeMoveDownAsync(SessionNodeInput node)
        => await OnNodeMoveDownRequested.InvokeAsync(node);

    private async Task HandleNodeNewAsync(SessionNodeInput node)
    {
        var agentTypeKey = node.Properties?.GetValueOrDefault("agentTypeKey") as string ?? "create";
        EdgeTypeInfo? edge = null;
        if (node.Properties?.TryGetValue("createEdgeKey", out var edgeKeyValue) == true
            && edgeKeyValue is string edgeKey
            && !string.IsNullOrWhiteSpace(edgeKey))
        {
            if (node.Properties?.ContainsKey("agentTypeKey") != true)
                agentTypeKey = $"create:{edgeKey}";
            edge = EdgeTypeService?.FindEdgeByAgentTypeKey(node.NodeTypeKey, agentTypeKey);
        }

        // Navigation-header children (e.g. workspace -> Pages -> page) should be
        // created under their header, not directly under the root. Project the
        // header into the command bar so submission targets the right parent.
        if (edge?.IsNavigationHeaderChild == true && !IsHeaderNode(node))
        {
            var headerNode = await EnsureNavigationHeaderAsync(node, edge);
            if (headerNode is not null)
            {
                _expandedContextNodeIds.Add(node.Id);
                _expandedContextNodeIds.Add(headerNode.Id);
                await PrepareCommandBarAccessAsync(headerNode);
                SessionState.SetAgentType(agentTypeKey);
                await FocusCommandInputAsync();
                return;
            }
        }

        await PrepareCommandBarAccessAsync(node);

        SessionState.SetAgentType(agentTypeKey);

        await FocusCommandInputAsync();
    }

    private async Task HandleNodeIntentSelectedAsync(BlockIntent intent)
    {
        var edgeKey = intent.Payload?.GetValueOrDefault("edgeKey")?.ToString();
        var sourceNodeId = intent.Payload?.GetValueOrDefault("sourceNodeId")?.ToString();
        if (string.IsNullOrWhiteSpace(edgeKey) || string.IsNullOrWhiteSpace(sourceNodeId))
        {
            await OnNodeIntentSelected.InvokeAsync(intent);
            return;
        }

        var sourceNode = FindNodeAcrossTree(DisplayContextNodes, sourceNodeId)?.Node
            ?? ViewState.ContextProjection?.Node;
        if (sourceNode is null)
        {
            await OnNodeIntentSelected.InvokeAsync(intent);
            return;
        }

        var properties = new Dictionary<string, object?>(
            sourceNode.Properties ?? new Dictionary<string, object?>(),
            StringComparer.Ordinal)
        {
            ["createEdgeKey"] = edgeKey,
            ["agentTypeKey"] = intent.Key
        };

        await HandleNodeNewAsync(sourceNode with { Properties = properties });
        await OnNodeIntentSelected.InvokeAsync(intent);
    }

    private async Task FocusCommandInputAsync()
    {
        await Task.Yield();

        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (input) { input.focus({ preventScroll: true }); }
                })();
                """);
        }
        catch { }
    }

    private async Task HandleListCreateSubmitAsync(SessionCommandSubmitted command)
    {
        var userContextId = command.Node.Properties?.GetValueOrDefault("userContextId") as string;

        if (SessionState.HasGraphService)
        {
            var result = await SessionState.CreateListAsync(userContextId, command.Input);
            if (result.Success)
            {
                await FinishListCreateSubmitAsync(command);
            }
            return;
        }

        if (OnListCreateRequested.HasDelegate)
        {
            await OnListCreateRequested.InvokeAsync(new SessionListCreateRequest(userContextId!, command.Input));
            await FinishListCreateSubmitAsync(command);
            return;
        }

        // Local fallback: ensure a Lists header exists and append the new list
        // node under it. When a real user-context root is present the header lives
        // under it; otherwise it is added as a new root node (standalone/storybook).
        _localContextNodes ??= [.. ContextNodes];
        var targetUserContextId = userContextId ?? command.Node.Id;
        var userContextRoot = _localContextNodes.FirstOrDefault(n =>
            string.Equals(n.Node.Id, targetUserContextId, StringComparison.Ordinal));
        var isUserContextRoot = userContextRoot is not null &&
            string.Equals(userContextRoot.Node.NodeTypeKey, "user-context", StringComparison.OrdinalIgnoreCase);

        SessionContextNode? listsHeader;
        if (isUserContextRoot)
        {
            _localContextNodes = EnsureLocalListsHeader(_localContextNodes, targetUserContextId).ToList();
            listsHeader = FindLocalListsHeader(_localContextNodes, targetUserContextId);
        }
        else
        {
            listsHeader = FindLocalRootListsHeader(_localContextNodes);
            if (listsHeader is null)
            {
                listsHeader = new SessionContextNode(
                    new SessionNodeInput(
                        Guid.NewGuid().ToString(),
                        "lists",
                        "Lists",
                        "NodeBlock"),
                    [],
                    true);
                _localContextNodes = [.. _localContextNodes, listsHeader];
            }
        }

        if (listsHeader is not null)
        {
            var listNode = new SessionContextNode(
                new SessionNodeInput(
                    Guid.NewGuid().ToString(),
                    "list",
                    command.Input,
                    "NodeBlock"),
                null,
                true);
            _localContextNodes = AddChildNode(_localContextNodes, listsHeader.Node.Id, listNode).ToList();
            SessionState.Dispatch(new ScheduleCreateEffect("list", listNode.Node.Id, command.Input));
        }

        await FinishListCreateSubmitAsync(command);
    }

    private async Task FinishListCreateSubmitAsync(SessionCommandSubmitted command)
    {
        SessionState.SetInput(string.Empty);

        if (command.IsControlEnter)
        {
            try
            {
                await JS.InvokeVoidAsync("eval", """
                    (() => {
                        const input = document.querySelector('[data-slot="session-command-input"]');
                        if (input) { input.value = ''; input.focus({ preventScroll: true }); }
                    })();
                    """);
            }
            catch { }
        }
        else
        {
            SessionState.SetAgentType("ask");
        }
    }

    private static IReadOnlyList<SessionContextNode> EnsureLocalListsHeader(
        IReadOnlyList<SessionContextNode> nodes, string userContextId)
    {
        return nodes.Select(n => EnsureLocalListsHeader(n, userContextId)).ToList();
    }

    private static SessionContextNode EnsureLocalListsHeader(SessionContextNode node, string userContextId)
    {
        if (!string.Equals(node.Node.Id, userContextId, StringComparison.Ordinal))
            return node;

        var children = node.Children?.ToList() ?? [];
        if (children.Any(c => string.Equals(c.Node.NodeTypeKey, "lists", StringComparison.OrdinalIgnoreCase)))
            return node;

        var listsHeader = new SessionContextNode(
            new SessionNodeInput(
                Guid.NewGuid().ToString(),
                "lists",
                "Lists",
                "NodeBlock"),
            [],
            true);
        children.Add(listsHeader);
        return new SessionContextNode(node.Node, children, node.IsLoaded);
    }

    private static SessionContextNode? FindLocalListsHeader(
        IReadOnlyList<SessionContextNode> nodes, string userContextId)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Node.Id, userContextId, StringComparison.Ordinal))
            {
                return node.Children?.FirstOrDefault(c =>
                    string.Equals(c.Node.NodeTypeKey, "lists", StringComparison.OrdinalIgnoreCase));
            }

            if (node.Children is { Count: > 0 })
            {
                var found = FindLocalListsHeader(node.Children, userContextId);
                if (found is not null) return found;
            }
        }

        return null;
    }

    private static SessionContextNode? FindLocalRootListsHeader(IReadOnlyList<SessionContextNode> nodes)
    {
        return nodes.FirstOrDefault(n => string.Equals(n.Node.NodeTypeKey, "lists", StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<SessionContextNode> EnsureLocalNavigationHeader(
        IReadOnlyList<SessionContextNode> nodes,
        string parentId,
        string headerNodeTypeKey,
        string headerLabel)
    {
        return nodes.Select(n => EnsureLocalNavigationHeader(n, parentId, headerNodeTypeKey, headerLabel)).ToList();
    }

    private static SessionContextNode EnsureLocalNavigationHeader(
        SessionContextNode node,
        string parentId,
        string headerNodeTypeKey,
        string headerLabel)
    {
        if (string.Equals(node.Node.Id, parentId, StringComparison.Ordinal))
        {
            var children = node.Children?.ToList() ?? [];
            if (children.Any(c =>
                    string.Equals(c.Node.NodeTypeKey, headerNodeTypeKey, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(c.Node.Label, headerLabel, StringComparison.Ordinal)))
            {
                return node;
            }

            var header = new SessionContextNode(
                new SessionNodeInput(
                    Guid.NewGuid().ToString(),
                    headerNodeTypeKey,
                    headerLabel,
                    "NodeBlock"),
                [],
                true);
            children.Add(header);
            return new SessionContextNode(node.Node, children, node.IsLoaded);
        }

        if (node.Children is null || node.Children.Count == 0)
            return node;

        return new SessionContextNode(
            node.Node,
            node.Children.Select(c => EnsureLocalNavigationHeader(c, parentId, headerNodeTypeKey, headerLabel)).ToList(),
            node.IsLoaded);
    }

    private static SessionContextNode? FindLocalNavigationHeader(
        IReadOnlyList<SessionContextNode> nodes,
        string parentId,
        string headerNodeTypeKey,
        string headerLabel)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Node.Id, parentId, StringComparison.Ordinal))
            {
                return node.Children?.FirstOrDefault(c =>
                    string.Equals(c.Node.NodeTypeKey, headerNodeTypeKey, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(c.Node.Label, headerLabel, StringComparison.Ordinal));
            }

            if (node.Children is { Count: > 0 })
            {
                var found = FindLocalNavigationHeader(node.Children, parentId, headerNodeTypeKey, headerLabel);
                if (found is not null) return found;
            }
        }

        return null;
    }

    private async Task<SessionNodeInput?> EnsureNavigationHeaderAsync(SessionNodeInput sourceNode, EdgeTypeInfo edge)
    {
        var headerNodeTypeKey = edge.HeaderNodeTypeKey ?? "header";
        var headerLabel = edge.HeaderLabel ?? edge.TargetDescriptor?.Label ?? "Header";

        if (SessionState.HasGraphService)
        {
            var headerNode = await SessionState.GetOrCreateNavigationHeaderAsync(sourceNode.Id, headerLabel, headerNodeTypeKey);
            if (headerNode is not null)
                return headerNode;
        }

        _localContextNodes = EnsureLocalNavigationHeader(DisplayContextNodes, sourceNode.Id, headerNodeTypeKey, headerLabel).ToList();
        var header = FindLocalNavigationHeader(_localContextNodes, sourceNode.Id, headerNodeTypeKey, headerLabel);
        return header?.Node;
    }

    private async Task HandleNodeRenameAsync(SessionNodeInput node)
    {
        await PrepareCommandBarAccessAsync(node);
        SessionState.SetAgentType("edit");
        SessionState.SetInput(node.Label);

        await Task.Yield();

        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (input) {
                        input.focus({ preventScroll: true });
                        input.setSelectionRange(0, input.value.length);
                    }
                })();
                """);
        }
        catch { }
    }

    private async Task HandleNodeDeleteAsync(SessionNodeInput node)
    {
        if (OnNodeDeleteRequested.HasDelegate)
        {
            await OnNodeDeleteRequested.InvokeAsync(node);
            ClearActiveNodeState(node);
            return;
        }

        _localContextNodes ??= [.. ContextNodes];
        _localContextNodes = RemoveNode(_localContextNodes, node.Id).ToList();

        ClearActiveNodeState(node);
        SessionState.Dispatch(new ScheduleDeleteEffect(node.NodeTypeKey, node.Id, node.Label));
    }

    private async Task HandleAddToListAsync(SessionNodeInput node)
    {
        _addToListNode = node;
        _addToListCandidates = SessionStateService.GetAllLists(DisplayContextNodes).ToList();
        _addToListMemberIds = SessionState.HasGraphService
            ? await SessionState.GetListIdsForNodeAsync(node.Id)
            : GetLocalListIdsForNode(node.Id);
        _showAddToListDialog = true;
    }

    private async Task ConfirmAddToListAsync(SessionAddToListRequest request)
    {
        if (OnAddToListRequested.HasDelegate)
        {
            await OnAddToListRequested.InvokeAsync(request);
        }
        else if (SessionState.HasGraphService)
        {
            foreach (var listId in request.ListIdsToAdd)
                await SessionState.ToggleListMembershipAsync(request.NodeId, listId, true);

            foreach (var listId in request.ListIdsToRemove)
                await SessionState.ToggleListMembershipAsync(request.NodeId, listId, false);
        }
        else
        {
            ApplyLocalListMemberships(request);
        }

        _showAddToListDialog = false;
        _addToListNode = null;
    }

    private void CancelAddToListAsync()
    {
        _showAddToListDialog = false;
        _addToListNode = null;
    }

    private IReadOnlySet<string> GetLocalListIdsForNode(string nodeId)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        CollectLocalListIds(DisplayContextNodes, nodeId, result);
        return result;
    }

    private static void CollectLocalListIds(IReadOnlyList<SessionContextNode> nodes, string nodeId, HashSet<string> result)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Node.NodeTypeKey, "list", StringComparison.OrdinalIgnoreCase)
                && node.Children?.Any(c => string.Equals(c.Node.Id, nodeId, StringComparison.Ordinal)) == true)
            {
                result.Add(node.Node.Id);
            }

            if (node.Children is { Count: > 0 })
                CollectLocalListIds(node.Children, nodeId, result);
        }
    }

    private void ApplyLocalListMemberships(SessionAddToListRequest request)
    {
        _localContextNodes ??= [.. ContextNodes];

        foreach (var listId in request.ListIdsToAdd)
        {
            var listHeader = FindLocalList(_localContextNodes, listId);
            if (listHeader is null) continue;

            if (listHeader.Children?.Any(c => string.Equals(c.Node.Id, request.NodeId, StringComparison.Ordinal)) != true)
            {
                var nodeToAdd = FindNodeAcrossTree(_localContextNodes, request.NodeId);
                if (nodeToAdd is not null)
                {
                    _localContextNodes = AddChildNode(_localContextNodes, listId,
                        new SessionContextNode(nodeToAdd.Node, null, true)).ToList();
                }
            }
        }

        foreach (var listId in request.ListIdsToRemove)
        {
            _localContextNodes = RemoveNodeFromParent(_localContextNodes, listId, request.NodeId).ToList();
        }

        SessionState.Dispatch(new ScheduleUpdateEffect("list", request.NodeId, "memberships", string.Join(",", request.ListIdsToAdd.Concat(request.ListIdsToRemove))));
    }

    private static SessionContextNode? FindLocalList(IReadOnlyList<SessionContextNode> nodes, string listId)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Node.Id, listId, StringComparison.Ordinal))
                return node;

            if (node.Children is { Count: > 0 })
            {
                var found = FindLocalList(node.Children, listId);
                if (found is not null) return found;
            }
        }
        return null;
    }

    private static SessionContextNode? FindNodeAcrossTree(IReadOnlyList<SessionContextNode> nodes, string nodeId)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Node.Id, nodeId, StringComparison.Ordinal))
                return node;

            if (node.Children is { Count: > 0 })
            {
                var found = FindNodeAcrossTree(node.Children, nodeId);
                if (found is not null) return found;
            }
        }
        return null;
    }

    private static IReadOnlyList<SessionContextNode> RemoveNodeFromParent(
        IReadOnlyList<SessionContextNode> nodes, string parentId, string childId)
    {
        return nodes.Select(n => RemoveNodeFromParent(n, parentId, childId)).ToList();
    }

    private static SessionContextNode RemoveNodeFromParent(
        SessionContextNode node, string parentId, string childId)
    {
        if (string.Equals(node.Node.Id, parentId, StringComparison.Ordinal))
        {
            var children = node.Children?.Where(c =>
                !string.Equals(c.Node.Id, childId, StringComparison.Ordinal)).ToList() ?? [];
            return new SessionContextNode(node.Node, children, node.IsLoaded);
        }

        if (node.Children is null || node.Children.Count == 0)
            return node;

        return new SessionContextNode(
            node.Node,
            node.Children.Select(c => RemoveNodeFromParent(c, parentId, childId)).ToList(),
            node.IsLoaded);
    }

    private void ClearActiveNodeState(SessionNodeInput node)
    {
        if (ViewState.ContextProjection?.Node.Id == node.Id)
        {
            SessionState.Dispatch(new ClearSessionContextProjection());
        }

        if (ViewState.ContextPins?.Any(pin => string.Equals(pin.Node.Id, node.Id, StringComparison.Ordinal)) == true)
        {
            SessionState.Dispatch(new UnpinSessionContext($"pin:{node.Id}"));
        }
    }

    private async Task HandleNodeRenameSubmitAsync(SessionCommandSubmitted command)
    {
        var renamedNode = command.Node with { Label = command.Input };

        if (OnNodeRenameRequested.HasDelegate)
        {
            await OnNodeRenameRequested.InvokeAsync(new SessionNodeRenameRequest(
                command.Node.Id,
                command.Node.NodeTypeKey,
                command.Node.Label,
                command.Input));
        }
        else
        {
            _localContextNodes ??= [.. ContextNodes];
            _localContextNodes = RenameNode(_localContextNodes, renamedNode).ToList();
            SessionState.ProjectContext(renamedNode);
        }

        SessionState.SetInput(string.Empty);
        SessionState.SetAgentType("ask");
        SessionState.Dispatch(new ScheduleUpdateEffect(
            command.Node.NodeTypeKey,
            command.Node.Id,
            "Label",
            command.Input));

        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const input = document.querySelector('[data-slot="session-command-input"]');
                    if (input) { input.value = ''; input.focus({ preventScroll: true }); }
                })();
                """);
        }
        catch { }
    }

    private async Task PrepareCommandBarAccessAsync(SessionNodeInput node)
    {
        ResetPageOverlay();
        await ShowCommandBarAsync();
        SessionState.ProjectContext(node);

        var projectedBlock = ViewState.ContextBlocks?
            .FirstOrDefault(candidate => string.Equals(candidate.Id, "projected-context-block", StringComparison.Ordinal));
        if (projectedBlock is not null)
            SessionState.SetBlockMode(projectedBlock, SessionContextBlock.ModeNormal);
    }

    private void ResetPageOverlay()
    {
        if (PageOverlayBlock is { } block)
            SessionState.SetBlockMode(block, SessionContextBlock.ModeNormal);
        else if (PageOverlayCommand is { } command)
            SessionState.SetCommandMode(command, SessionContextBlock.ModeNormal);
    }

    private static bool IsHeaderNode(SessionNodeInput node) =>
        node.NodeTypeKey.EndsWith(".header", StringComparison.OrdinalIgnoreCase)
        || string.Equals(node.NodeTypeKey, "header", StringComparison.OrdinalIgnoreCase)
        || string.Equals(node.NodeTypeKey, "lists", StringComparison.OrdinalIgnoreCase);

    private async Task<bool> TryCreateViaEdgeToolAsync(SessionCommandSubmitted command, EdgeTypeInfo edge)
    {
        if (string.IsNullOrWhiteSpace(edge.Tool) || ExecutionEngine is null)
            return false;

        var creator = Discovery.GetAsync<IGraphToolService>(edge.Tool);
        if (creator is null)
            return false;

        var props = new Dictionary<string, object?>
        {
            ["Title"] = command.Input,
            ["SourceNodeId"] = command.Node.Id,
            ["SourceNodeTypeKey"] = command.Node.NodeTypeKey,
            ["SourceLabel"] = command.Node.Label,
            ["EdgeKey"] = edge.EdgeKey,
            ["Intent"] = edge.Intent,
        };
        var entity = await creator.ExecuteAsync(ExecutionEngine, props);
        var extracted = edge.TargetDescriptor?.ExtractLabel(entity);
        var label = string.IsNullOrWhiteSpace(extracted) ? command.Input : extracted;
        var extractedId = edge.TargetDescriptor?.ExtractId(entity) ?? Guid.Empty;

        _localContextNodes ??= [.. ContextNodes];
        var childNode = new SessionContextNode(
            new SessionNodeInput(
                extractedId == Guid.Empty ? Guid.NewGuid().ToString() : extractedId.ToString(),
                edge.TargetNodeTypeKey,
                label,
                edge.TargetDescriptor?.BlockComponent ?? "NodeBlock"),
            null,
            true);
        _localContextNodes = AddChildNode(_localContextNodes, command.Node.Id, childNode).ToList();

        // Properly persist via the host's header callback when available.
        // ScheduleCreateEffect alone writes fact patches to a non-existent
        // NodeEntity and gets silently dropped by ReceiveFactAsync.
        if (OnHeaderCreateRequested.HasDelegate)
        {
            await OnHeaderCreateRequested.InvokeAsync(new SessionHeaderCreateRequest(
                command.Node.Id, command.Node.NodeTypeKey, command.Input));
        }
        else
        {
            SessionState.Dispatch(new ScheduleCreateEffect(edge.TargetNodeTypeKey, childNode.Node.Id, label));
        }

        return true;
    }

    private static IReadOnlyList<SessionContextNode> AddChildNode(
        IReadOnlyList<SessionContextNode> nodes,
        string parentId,
        SessionContextNode child)
    {
        return nodes.Select(n => AddChildNode(n, parentId, child)).ToList();
    }

    private static SessionContextNode AddChildNode(
        SessionContextNode node,
        string parentId,
        SessionContextNode child)
    {
        if (string.Equals(node.Node.Id, parentId, StringComparison.Ordinal))
        {
            var children = node.Children?.ToList() ?? [];
            children.Add(child);
            return new SessionContextNode(node.Node, children, true);
        }

        if (node.Children is null || node.Children.Count == 0)
            return node;

        return new SessionContextNode(
            node.Node,
            node.Children.Select(c => AddChildNode(c, parentId, child)).ToList(),
            node.IsLoaded);
    }

    private static IReadOnlyList<SessionContextNode> RemoveNode(
        IReadOnlyList<SessionContextNode> nodes,
        string nodeId)
    {
        return nodes
            .Where(n => !string.Equals(n.Node.Id, nodeId, StringComparison.Ordinal))
            .Select(n => RemoveNode(n, nodeId))
            .ToList();
    }

    private static SessionContextNode RemoveNode(SessionContextNode node, string nodeId)
    {
        if (node.Children is null || node.Children.Count == 0)
            return node;

        return new SessionContextNode(
            node.Node,
            node.Children
                .Where(c => !string.Equals(c.Node.Id, nodeId, StringComparison.Ordinal))
                .Select(c => RemoveNode(c, nodeId))
                .ToList(),
            node.IsLoaded);
    }

    private static IReadOnlyList<SessionContextNode> RenameNode(
        IReadOnlyList<SessionContextNode> nodes,
        SessionNodeInput renamedNode)
    {
        return nodes.Select(n => RenameNode(n, renamedNode)).ToList();
    }

    private static SessionContextNode RenameNode(SessionContextNode node, SessionNodeInput renamedNode)
    {
        if (string.Equals(node.Node.Id, renamedNode.Id, StringComparison.Ordinal))
        {
            return new SessionContextNode(renamedNode, node.Children, node.IsLoaded);
        }

        if (node.Children is null || node.Children.Count == 0)
            return node;

        return new SessionContextNode(
            node.Node,
            node.Children.Select(c => RenameNode(c, renamedNode)).ToList(),
            node.IsLoaded);
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
        // Rename the selected context node when in edit mode.
        if (string.Equals(command.AgentTypeKey, "edit", StringComparison.OrdinalIgnoreCase))
        {
            await HandleNodeRenameSubmitAsync(command);
            return;
        }

        // When in "create" mode with a session node, the input is the session title, not a command.
        if (string.Equals(command.AgentTypeKey, "create", StringComparison.OrdinalIgnoreCase)
            && string.Equals(command.Node.NodeTypeKey, "session", StringComparison.OrdinalIgnoreCase))
        {
            var updatedNode = command.Node with { Label = command.Input };

            var index = _createdSessions.FindIndex(s => s.Id == command.Node.Id);
            if (index >= 0)
            {
                _createdSessions[index] = _createdSessions[index] with { Label = command.Input };
            }

            SessionState.SelectSession(updatedNode, DisplaySessions);
            SessionState.SetAgentType("ask");
            SessionState.Dispatch(new ScheduleUpdateEffect(
                updatedNode.NodeTypeKey,
                updatedNode.Id,
                "Label",
                updatedNode.Label));

            await SessionState.DispatchAsync(new PersistSessionCommands(command.Node.Id), CancellationToken.None);

            try
            {
                await JS.InvokeVoidAsync("eval", """
                    (() => {
                        const input = document.querySelector('[data-slot="session-command-input"]');
                        if (input) { input.value = ''; input.focus(); }
                    })();
                    """);
            }
            catch { }

            return;
        }

        // Non-session node create: resolve the target type and tool from the registered
        // edge descriptors. The default Create agent maps to the first create edge; future
        // dynamic intents are discovered by matching the agent key against edge intents.
        var createEdge = EdgeTypeService?.FindEdgeByAgentTypeKey(command.Node.NodeTypeKey, command.AgentTypeKey);
        if (string.Equals(command.AgentTypeKey, "create", StringComparison.OrdinalIgnoreCase) || createEdge is not null)
        {
            // Convenience edges from a root to a navigation-header child (e.g. workspace -> page)
            // must target the intermediate header, not the root itself.
            if (createEdge?.IsNavigationHeaderChild == true && !IsHeaderNode(command.Node))
            {
                var headerNode = await EnsureNavigationHeaderAsync(command.Node, createEdge);
                if (headerNode is not null)
                {
                    command = command with { Node = headerNode };
                    createEdge = EdgeTypeService?.FindEdgeByAgentTypeKey(headerNode.NodeTypeKey, command.AgentTypeKey);
                }
            }

            if (string.Equals(command.Node.NodeTypeKey, "lists", StringComparison.OrdinalIgnoreCase))
            {
                await HandleListCreateSubmitAsync(command);
                return;
            }

            if (createEdge is not null)
            {
                var createdViaTool = await TryCreateViaEdgeToolAsync(command, createEdge);
                if (createdViaTool)
                {
                    SessionState.SetInput(string.Empty);

                    if (command.IsControlEnter)
                    {
                        try
                        {
                            await JS.InvokeVoidAsync("eval", """
                                (() => {
                                    const input = document.querySelector('[data-slot="session-command-input"]');
                                    if (input) { input.value = ''; input.focus({ preventScroll: true }); }
                                })();
                                """);
                        }
                        catch { }
                    }
                    else
                    {
                        SessionState.SetAgentType("ask");
                    }

                    return;
                }
            }

            if (OnHeaderCreateRequested.HasDelegate)
            {
                await OnHeaderCreateRequested.InvokeAsync(new SessionHeaderCreateRequest(
                    command.Node.Id, command.Node.NodeTypeKey, command.Input));
            }
            else if (!IsHeaderNode(command.Node))
            {
                _localContextNodes ??= [.. ContextNodes];
                var childNode = new SessionContextNode(
                    new SessionNodeInput(
                        Guid.NewGuid().ToString(),
                        command.Node.NodeTypeKey,
                        command.Input,
                        command.Node.ComponentBlock ?? "NodeBlock"),
                    null,
                    true);
                _localContextNodes = AddChildNode(_localContextNodes, command.Node.Id, childNode).ToList();
                SessionState.Dispatch(new ScheduleCreateEffect(childNode.Node.NodeTypeKey, childNode.Node.Id, command.Input));
            }

            SessionState.SetInput(string.Empty);

            if (command.IsControlEnter)
            {
                try
                {
                    await JS.InvokeVoidAsync("eval", """
                        (() => {
                            const input = document.querySelector('[data-slot="session-command-input"]');
                            if (input) { input.value = ''; input.focus({ preventScroll: true }); }
                        })();
                        """);
                }
                catch { }
            }
            else
            {
                SessionState.SetAgentType("ask");
            }

            return;
        }

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

    private string GetCommandMode(SessionCommandSubmitted command) => SessionState.GetCommandMode(command);

    private Task ShowSettingsPageAsync()
    {
        if (IsSettingsPage)
        {
            SessionState.ShowFeedPage();
        }
        else
        {
            SessionState.ShowSettingsPage();
        }

        return Task.CompletedTask;
    }

    private Task ShowEventLogPageAsync()
    {
        SessionState.ShowEventLogPage();
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

    private async Task ScrollStreamingResponseIntoViewAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("eval", """
                (() => {
                    const scroller = document.querySelector('[data-slot="session-content-body"]');
                    if (!scroller) return;

                    const streaming = scroller.querySelector('[data-slot="session-turn-response-text"][data-streaming="true"]');
                    if (!streaming) return;

                    const streamingRect = streaming.getBoundingClientRect();
                    const scrollerRect = scroller.getBoundingClientRect();
                    const targetBottom = streamingRect.bottom - scrollerRect.top + scroller.scrollTop;
                    const visibleBottom = scroller.scrollTop + scroller.clientHeight - 20;

                    if (targetBottom > visibleBottom) {
                        scroller.scrollTo({ top: targetBottom - scroller.clientHeight + 20, behavior: 'instant' });
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

    private void OnStateChanged(SessionBlockState _) => InvokeAsync(StateHasChanged);

    private ISessionSurfaceDiscovery ResolveSessionSurfaceDiscovery() =>
        Services.GetService<ISessionSurfaceDiscovery>() ?? new SessionSurfaceDiscovery([]);

    public void Dispose()
    {
        if (_subscribedStateService is not null)
            _subscribedStateService.StateChanged -= OnStateChanged;
    }
}
