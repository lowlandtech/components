namespace LowlandTech.Components;

public partial class SessionBlock : TinyComponentBase, IDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IGraphDiscoveryService Discovery { get; set; } = default!;
    [Inject] private IEdgeTypeService EdgeTypeService { get; set; } = default!;
    [Inject] private IExecutionEngine ExecutionEngine { get; set; } = default!;
    [Inject] private IServiceProvider Services { get; set; } = default!;

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
    private bool _isCommandBarVisible;
    private bool _isHydratingCommandBar;
    private int _commandBarChangeVersion;
    private string? _hydratedCommandBarPersistKey;
    private readonly List<SessionListEntry> _createdSessions = [];
    private List<SessionContextNode>? _localContextNodes;

    private IReadOnlyList<SessionContextNode> DisplayContextNodes => _localContextNodes ?? ContextNodes;

    [Parameter] public IReadOnlyList<SessionListEntry> Sessions { get; set; } =
    [
        new("orders-session", "Orders"),
        new("support-session", "Support triage"),
        new("release-session", "Release notes")
    ];

    [Parameter] public IReadOnlyList<SessionContextNode> ContextNodes { get; set; } =
    [
        new(
            new("orders", "workitem", "Orders", "NodeBlock"),
            [
                new(new("invoice-approval", "invoice", "Invoice approval", "NodeBlock"), IsLoaded: true),
                new(new("shipment-delay", "shipment", "Shipment delay", "NodeBlock"), IsLoaded: true)
            ],
            true),
        new(
            new("customers", "table", "Customers", "NodeBlock"),
            [
                new(new("northwind", "customer", "Northwind Traders", "NodeBlock"), IsLoaded: true),
                new(new("contoso", "customer", "Contoso", "NodeBlock"), IsLoaded: true)
            ],
            true)
    ];

    [Parameter] public bool IsTest { get; set; }
    [Parameter] public int TestStreamDelayMilliseconds { get; set; } = 35;
    [Parameter] public bool HasSidebar { get; set; }
    [Parameter] public string SidebarPersistKey { get; set; } = "session-block:sidebar-open";
    [Parameter] public string RightSidebarPersistKey { get; set; } = "session-block:right-sidebar-open";
    [Parameter] public string CommandBarVisiblePersistKey { get; set; } = "session-block:command-bar-visible";
    [Parameter] public int? SidebarWidth { get; set; }
    [Parameter] public string Variant { get; set; } = "outlined";
    [Parameter] public string? PageOverlayContainerClass { get; set; }
    [Parameter] public string? AppOverlayContainerClass { get; set; }
    [Parameter] public SessionStateService? StateService { get; set; }
    [Parameter] public string? Language { get; set; }
    [Parameter] public string LogoSrc { get; set; } = "_content/LowlandTech.Components/img/session-block-logo.svg";
    [Parameter] public string? Title { get; set; }
    [Parameter] public EventCallback<SessionHeaderCreateRequest> OnHeaderCreateRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeRenameRequest> OnNodeRenameRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeDeleteRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnLoadChildrenRequested { get; set; }

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
    }

    private string L(string key) => _localizer?[key] ?? key;

    private string EffectiveTitle => Title ?? L("Title");

    private string GetCommandPlaceholder() =>
        string.Equals(ViewState.ActiveAgentTypeKey, "create", StringComparison.OrdinalIgnoreCase)
            ? L("CreateCommandPlaceholder")
            : string.Equals(ViewState.ActiveAgentTypeKey, "edit", StringComparison.OrdinalIgnoreCase)
                ? L("EditCommandPlaceholder")
                : L("CommandPlaceholder");

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

    private IReadOnlyList<SessionAgentType> LocalizedAgentTypes =>
    [
        new("ask", L("AgentAsk")),
        new("plan", L("AgentPlan")),
        new("edit", L("AgentEdit")),
        new("create", L("AgentCreate")),
        new("run", L("AgentRun"), Disabled: true)
    ];

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
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "data-testid", "session-block-sidebar-open");
        builder.AddAttribute(3, "data-slot", "tabs-trigger");
        builder.AddAttribute(4, "data-state", "inactive");
        builder.AddAttribute(5, "data-orientation", "horizontal");
        builder.AddAttribute(6, "class", "cn-tabs-trigger cn-tabs-trigger-ide inline-flex h-10 w-10 min-w-0 cursor-default items-center justify-center gap-1 whitespace-nowrap border-r border-b border-border px-0 text-sm text-muted-foreground transition-colors hover:bg-muted/40 hover:text-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring");
        builder.AddAttribute(7, "title", L("OpenSidebar"));
        builder.AddAttribute(8, "aria-label", L("OpenSidebar"));
        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, OpenStandaloneSidebarAsync));

        builder.AddMarkupContent(10, "<svg data-slot=\"icon\" xmlns=\"http://www.w3.org/2000/svg\" width=\"14\" height=\"14\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" class=\"cn-icon shrink-0\" aria-hidden=\"true\"><rect width=\"18\" height=\"18\" x=\"3\" y=\"3\" rx=\"2\"></rect><path d=\"M9 3v18\"></path><path d=\"m13 15 3-3-3-3\"></path></svg>");

        builder.CloseElement();
    };

    private RenderFragment RightSidebarOpenButton => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "data-testid", "session-block-right-sidebar-open");
        builder.AddAttribute(3, "data-slot", "tabs-trigger");
        builder.AddAttribute(4, "data-state", "inactive");
        builder.AddAttribute(5, "data-orientation", "horizontal");
        builder.AddAttribute(6, "class", "cn-tabs-trigger cn-tabs-trigger-ide inline-flex h-10 w-10 min-w-0 cursor-default items-center justify-center gap-1 whitespace-nowrap border-l border-b border-border px-0 text-sm text-muted-foreground transition-colors hover:bg-muted/40 hover:text-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring");
        builder.AddAttribute(7, "title", L("OpenSections"));
        builder.AddAttribute(8, "aria-label", L("OpenSections"));
        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, OpenRightSidebarAsync));
        builder.AddMarkupContent(10, "<svg data-slot=\"icon\" xmlns=\"http://www.w3.org/2000/svg\" width=\"14\" height=\"14\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" class=\"cn-icon shrink-0\" aria-hidden=\"true\"><rect width=\"18\" height=\"18\" x=\"3\" y=\"3\" rx=\"2\"></rect><path d=\"M15 3v18\"></path><path d=\"m11 9-3 3 3 3\"></path></svg>");
        builder.CloseElement();
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

    private async Task HandleNodeNewAsync(SessionNodeInput node)
    {
        await PrepareCommandBarAccessAsync(node);
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
        || string.Equals(node.NodeTypeKey, "header", StringComparison.OrdinalIgnoreCase);

    private static string ResolveLocalChildNodeType(SessionNodeInput parentNode) =>
        parentNode.NodeTypeKey.ToLowerInvariant() switch
        {
            "workspaces.header" => "workspace",
            "workitem.header" => "workitem",
            "flow.header" => "flow",
            "package.header" => "package",
            "step.header" => "step",
            "header" => "node",
            _ => parentNode.NodeTypeKey
        };

    private async Task<bool> TryCreateViaEdgeToolAsync(SessionCommandSubmitted command)
    {
        var edge = EdgeTypeService.FindCreateEdge(command.Node.NodeTypeKey);
        if (edge is null || string.IsNullOrWhiteSpace(edge.Tool))
            return false;

        var creator = Discovery.GetAsync<IGraphToolService>(edge.Tool);
        if (creator is null)
            return false;

        var props = new Dictionary<string, object?> { ["Title"] = command.Input };
        var entity = await creator.ExecuteAsync(ExecutionEngine, props);
        var label = edge.TargetDescriptor?.ExtractLabel(entity) ?? command.Input;

        _localContextNodes ??= [.. ContextNodes];
        var childNode = new SessionContextNode(
            new SessionNodeInput(
                Guid.NewGuid().ToString(),
                edge.TargetNodeTypeKey,
                label,
                command.Node.ComponentBlock ?? "NodeBlock"),
            null,
            true);
        _localContextNodes = AddChildNode(_localContextNodes, command.Node.Id, childNode).ToList();
        SessionState.Dispatch(new ScheduleCreateEffect(edge.TargetNodeTypeKey, childNode.Node.Id, label));
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

        // Non-session node create: prefer an edge-descriptor-driven tool, then the host callback, then a local fallback.
        if (string.Equals(command.AgentTypeKey, "create", StringComparison.OrdinalIgnoreCase))
        {
            var createdViaTool = await TryCreateViaEdgeToolAsync(command);

            if (!createdViaTool)
            {
                if (OnHeaderCreateRequested.HasDelegate)
                {
                    await OnHeaderCreateRequested.InvokeAsync(new SessionHeaderCreateRequest(
                        command.Node.Id, command.Node.NodeTypeKey, command.Input));
                    SessionState.Dispatch(new ScheduleCreateEffect(
                        ResolveLocalChildNodeType(command.Node),
                        Guid.NewGuid().ToString(),
                        command.Input));
                }
                else
                {
                    _localContextNodes ??= [.. ContextNodes];
                    var childTypeKey = ResolveLocalChildNodeType(command.Node);
                    var childNode = new SessionContextNode(
                        new SessionNodeInput(
                            Guid.NewGuid().ToString(),
                            childTypeKey,
                            command.Input,
                            command.Node.ComponentBlock ?? "NodeBlock"),
                        null,
                        true);
                    _localContextNodes = AddChildNode(_localContextNodes, command.Node.Id, childNode).ToList();
                    SessionState.Dispatch(new ScheduleCreateEffect(childTypeKey, childNode.Node.Id, command.Input));
                }
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
