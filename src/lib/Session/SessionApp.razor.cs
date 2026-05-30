namespace LowlandTech.Components.Session;

public partial class SessionApp : TinyComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private ISonnerService Sonner { get; set; } = default!;
    [Inject] private IGraphDiscoveryService Discovery { get; set; } = default!;

    private SessionUiState _state;
    private string? _activeSessionId;
    private string? _pendingScrollSectionId;
    private string? _maximizedBlockId;
    private string? _maximizedCommandId;
    private string? _pageOverlayBlockId;
    private string? _pageOverlayCommandId;
    private string? _appOverlayBlockId;
    private string? _appOverlayCommandId;
    private readonly List<SessionNodeInput> _openSessionNodes = [];
    private readonly HashSet<string> _pinnedNodeIds = [];
    private readonly Dictionary<string, long> _feedSequenceByItemKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _blockModesByKey = new(StringComparer.Ordinal);
    private long _nextFeedSequence;

    private readonly Dictionary<string, SessionNodeInput> _requestNodes = new()
    {
        ["orders-session"] = new("request-orders", "session-request", "Order summary"),
        ["support-session"] = new("request-support", "session-request", "Support ticket summary"),
        ["release-session"] = new("request-release", "session-request", "Release draft")
    };

    private readonly Dictionary<string, SessionNodeInput> _responseNodes = new()
    {
        ["orders-session"] = new("response-orders", "session-response", "Orders context"),
        ["support-session"] = new("response-support", "session-response", "Support context"),
        ["release-session"] = new("response-release", "session-response", "Release context")
    };

    private readonly Dictionary<string, List<SessionCommandSubmitted>> _sessionCommands = new();
    private readonly Dictionary<string, SessionUiState> _sessionStates = new();

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

    public SessionApp()
    {
        _state = SessionUiDefaults.Empty;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (string.IsNullOrWhiteSpace(_pendingScrollSectionId))
            return;

        var sectionId = _pendingScrollSectionId;
        _pendingScrollSectionId = null;
        await ScrollSectionIntoViewAsync(sectionId);
    }

    private string CurrentSessionId => _activeSessionId ?? "orders-session";

    private bool HasActiveSession => _activeSessionId is not null;

    private SessionNodeInput? CurrentSessionNode =>
        _activeSessionId is null
            ? null
            : FindSessionNode(_activeSessionId);

    private SessionNodeInput? CurrentCommandContextNode =>
        ViewState.ContextProjection?.Node ?? CurrentSessionNode;

    private SessionUiState ViewState =>
        CurrentSessionNode is null
            ? SessionUiDefaults.Empty with { OpenNodes = [.. _openSessionNodes] }
            : _state with
            {
                SelectedNode = CurrentSessionNode,
                OpenNodes = [.. _openSessionNodes]
            };

    private SessionNodeInput CurrentRequestNode => _requestNodes.GetValueOrDefault(CurrentSessionId) ?? _requestNodes["orders-session"];

    private SessionNodeInput CurrentResponseNode => _responseNodes.GetValueOrDefault(CurrentSessionId) ?? _responseNodes["orders-session"];

    private List<SessionCommandSubmitted> CurrentSubmittedCommands => GetSessionCommands(CurrentSessionId);

    private IReadOnlyList<SessionListEntry> DisplaySessions =>
        [.. Sessions.Select(session => session with { Count = GetSessionSectionCount(session.Id) })];

    private IReadOnlyList<SessionAppSection> VisibleSections
    {
        get
        {
            if (!HasActiveSession)
                return [];

            var sections = new List<SessionAppSection>
            {
                new("session-turn-request", CurrentRequestNode.Label, "turn"),
                new("session-turn-response", CurrentResponseNode.Label, "turn")
            };

            foreach (var item in FeedItems)
            {
                if (item.Kind == SessionFeedItemKind.Response)
                {
                    sections.Add(new(GetCommandTurnId(item.Command!), CreateResponseNode(item.Command!).Label, "turn"));
                }
                else
                {
                    sections.Add(new SessionAppSection(item.Block!.Id, item.Block.Context.Label, item.Block.Context.NodeTypeKey));
                }
            }

            return sections;
        }
    }

    private SessionContextBlockModel? MaximizedBlock =>
        string.IsNullOrWhiteSpace(_maximizedBlockId)
            ? null
            : (ViewState.ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, _maximizedBlockId, StringComparison.Ordinal));

    private SessionCommandSubmitted? MaximizedCommand =>
        string.IsNullOrWhiteSpace(_maximizedCommandId)
            ? null
            : CurrentSubmittedCommands.FirstOrDefault(command => string.Equals(command.Id, _maximizedCommandId, StringComparison.Ordinal));

    private SessionContextBlockModel? PageOverlayBlock =>
        string.IsNullOrWhiteSpace(_pageOverlayBlockId)
            ? null
            : (ViewState.ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, _pageOverlayBlockId, StringComparison.Ordinal));

    private SessionCommandSubmitted? PageOverlayCommand =>
        string.IsNullOrWhiteSpace(_pageOverlayCommandId)
            ? null
            : CurrentSubmittedCommands.FirstOrDefault(command => string.Equals(command.Id, _pageOverlayCommandId, StringComparison.Ordinal));

    private SessionContextBlockModel? AppOverlayBlock =>
        string.IsNullOrWhiteSpace(_appOverlayBlockId)
            ? null
            : (ViewState.ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, _appOverlayBlockId, StringComparison.Ordinal));

    private SessionCommandSubmitted? AppOverlayCommand =>
        string.IsNullOrWhiteSpace(_appOverlayCommandId)
            ? null
            : CurrentSubmittedCommands.FirstOrDefault(command => string.Equals(command.Id, _appOverlayCommandId, StringComparison.Ordinal));

    private IEnumerable<SessionFeedItem> FeedItems
    {
        get
        {
            var responses = CurrentSubmittedCommands
                .Select(cmd =>
                {
                    var key = GetFeedItemKey(CurrentSessionId, "command", cmd.Id);
                    return new SessionFeedItem(key, SessionFeedItemKind.Response, EnsureFeedSequence(key), Command: cmd);
                });

            var blocks = (ViewState.ContextBlocks ?? [])
                .Select(block =>
                {
                    var key = GetFeedItemKey(CurrentSessionId, "block", block.Id);
                    return new SessionFeedItem(key, SessionFeedItemKind.Block, EnsureFeedSequence(key), Block: block);
                });

            return responses.Concat(blocks).OrderBy(item => item.Sequence);
        }
    }

    private int GetSessionSectionCount(string sessionId) =>
        GetSessionTurnSectionCount(sessionId) + GetSessionBlockSectionCount(sessionId);

    private int GetSessionTurnSectionCount(string sessionId)
    {
        var commandCount = _sessionCommands.TryGetValue(sessionId, out var commands)
            ? commands.Count
            : 0;

        return commandCount + 2;
    }

    private int GetSessionBlockSectionCount(string sessionId)
    {
        var state = string.Equals(_activeSessionId, sessionId, StringComparison.Ordinal)
            ? _state
            : _sessionStates.TryGetValue(sessionId, out var storedState)
                ? storedState
                : SessionUiDefaults.Empty;

        return state.ContextBlocks?.Count ?? 0;
    }

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

    private Task SetStateAsync(SessionUiState state)
    {
        var selectedSessionNode = ResolveSessionNode(state.SelectedNode);

        if (selectedSessionNode is not null && !string.Equals(_activeSessionId, selectedSessionNode.Id, StringComparison.Ordinal))
        {
            PersistCurrentSessionState();

            var previousActiveId = _activeSessionId;
            _activeSessionId = selectedSessionNode.Id;
            _maximizedBlockId = null;
            _maximizedCommandId = null;
            _pageOverlayBlockId = null;
            _pageOverlayCommandId = null;
            _appOverlayBlockId = null;
            _appOverlayCommandId = null;

            SyncOpenSessionNodes(state.OpenNodes, previousActiveId);

            _state = GetSessionState(selectedSessionNode.Id);
        }
        else if (_activeSessionId is not null && state.OpenNodes.All(n => n.Id != _activeSessionId))
        {
            PersistCurrentSessionState();

            var newSelectedNode = ResolveSessionNode(state.SelectedNode);
            _activeSessionId = newSelectedNode?.Id;
            _maximizedBlockId = null;
            _maximizedCommandId = null;
            _pageOverlayBlockId = null;
            _pageOverlayCommandId = null;
            _appOverlayBlockId = null;
            _appOverlayCommandId = null;

            SyncOpenSessionNodes(state.OpenNodes, null);

            _state = _activeSessionId is null
                ? SessionUiDefaults.Empty
                : GetSessionState(_activeSessionId);
        }
        else
        {
            _state = StripNavigationState(state);
            PersistCurrentSessionState();
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task ShowSessionsAsync()
    {
        PersistCurrentSessionState();

        _activeSessionId = null;
        _state = SessionUiDefaults.Empty;
        _maximizedBlockId = null;
        _maximizedCommandId = null;
        _pageOverlayBlockId = null;
        _pageOverlayCommandId = null;
        _appOverlayBlockId = null;
        _appOverlayCommandId = null;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task SelectSessionAsync(SessionNodeInput node) =>
        SetStateAsync(SessionUiReducer.Reduce(ViewState, new SelectSessionNode(node)));

    private Task CreateSessionAsync()
    {
        return Task.CompletedTask;
    }

    private Task SetInputAsync(string value)
    {
        _state = SessionUiReducer.Reduce(_state, new SetSessionInput(value));
        PersistCurrentSessionState();
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleProjectedAsync(SessionContextProjected _)
    {
        MoveFeedItemToEnd(GetFeedItemKey(CurrentSessionId, "block", "projected-context-block"));
        _pendingScrollSectionId = "projected-context-block";
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandlePinnedAsync(SessionContextPinned pinned)
    {
        var blockId = $"context-block:{pinned.Node.Id}";
        EnsureFeedSequence(GetFeedItemKey(CurrentSessionId, "block", blockId));
        _pendingScrollSectionId = blockId;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleUnpinnedAsync(SessionContextUnpinned _)
    {
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleTabPinnedChangedAsync(TabsItem item)
    {
        if (item.Pinned)
        {
            if (!_pinnedNodeIds.Contains(item.Value))
            {
                _pinnedNodeIds.Add(item.Value);
            }
        }
        else
        {
            _pinnedNodeIds.Remove(item.Value);
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task HandleSubmitAsync(SessionCommandSubmitted command)
    {
        var sessionId = CurrentSessionId;

        if (IsTest)
        {
            var streamingCommand = command with { Response = string.Empty, IsStreaming = true };
            EnsureFeedSequence(GetFeedItemKey(sessionId, "command", streamingCommand.Id));
            GetSessionCommands(sessionId).Add(streamingCommand);
            MoveProjectedBlockToEnd(sessionId);
            _pendingScrollSectionId = GetCommandTurnId(streamingCommand);
            StateHasChanged();
            await StreamTestResponseAsync(sessionId, streamingCommand);
            return;
        }

        var service = Discovery.Get(command.AgentTypeKey);
        if (service is not null)
        {
            var context = new SessionContext();
            await service.ExecuteAsync(context);
            if (!string.IsNullOrWhiteSpace(context.Response))
            {
                command = command with { Response = context.Response };
            }
        }
        else
        {
            Sonner.Show($"No agent service registered for '{command.AgentTypeKey}'.", ToastType.Warning, description: "Install a plugin that provides this agent type or enable test mode.");
        }

        EnsureFeedSequence(GetFeedItemKey(sessionId, "command", command.Id));
        GetSessionCommands(sessionId).Add(command);
        MoveProjectedBlockToEnd(sessionId);
        _pendingScrollSectionId = GetCommandTurnId(command);

        StateHasChanged();

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

    private static SessionNodeInput CreateResponseNode(SessionCommandSubmitted command) =>
        new($"response:{command.Id}", "session-response", $"{command.AgentTypeKey} response");

    private static string GetResponseStatus(SessionCommandSubmitted command) =>
        command.IsStreaming ? "Streaming" : command.AgentTypeKey;

    private static string GetResponseStatusVariant(SessionCommandSubmitted command) =>
        command.IsStreaming ? "secondary" : "outline";

    private string CreateResponseDescription(SessionCommandSubmitted command)
    {
        if (!string.IsNullOrWhiteSpace(command.Response))
            return command.Response;

        if (command.IsStreaming)
            return "Thinking...";

        var contextCount = ViewState.ContextBlocks?.Count ?? 0;
        return $"{command.AgentTypeKey} processed \"{command.Input}\" with {contextCount} context block(s).";
    }

    private async Task StreamTestResponseAsync(string sessionId, SessionCommandSubmitted command)
    {
        var response = string.Empty;

        await foreach (var chunk in DummyAskAgentGraphService.StreamReplyAsync(command.Input, TestStreamDelayMilliseconds))
        {
            response += chunk;
            UpdateCommand(sessionId, command.Id, existing => existing with
            {
                Response = response,
                IsStreaming = true
            });

            await InvokeAsync(StateHasChanged);
        }

        UpdateCommand(sessionId, command.Id, existing => existing with
        {
            Response = response,
            IsStreaming = false
        });

        await InvokeAsync(StateHasChanged);
    }

    private void UpdateCommand(string sessionId, string commandId, Func<SessionCommandSubmitted, SessionCommandSubmitted> update)
    {
        var commands = GetSessionCommands(sessionId);
        var index = commands.FindIndex(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
        if (index < 0)
            return;

        commands[index] = update(commands[index]);
    }

    private SessionNodeInput? FindSessionNode(string sessionId)
    {
        var entry = Sessions.FirstOrDefault(candidate => string.Equals(candidate.Id, sessionId, StringComparison.Ordinal));
        return entry is null ? null : new SessionNodeInput(entry.Id, entry.NodeTypeKey, entry.Label);
    }

    private SessionNodeInput? ResolveSessionNode(SessionNodeInput? node) =>
        node is null ? null : FindSessionNode(node.Id);

    private void PersistCurrentSessionState()
    {
        if (_activeSessionId is null)
            return;

        _sessionStates[_activeSessionId] = StripNavigationState(_state);
    }

    private SessionUiState GetSessionState(string sessionId)
    {
        if (_sessionStates.TryGetValue(sessionId, out var state))
            return state;

        var sessionNode = FindSessionNode(sessionId);
        return sessionNode is null
            ? SessionUiDefaults.Empty
            : SessionUiDefaults.Empty with { SelectedNode = sessionNode };
    }

    private SessionUiState StripNavigationState(SessionUiState state)
    {
        var currentSessionNode = CurrentSessionNode ?? FindSessionNode(_activeSessionId ?? string.Empty);
        return currentSessionNode is null
            ? state with { OpenNodes = [] }
            : state with
            {
                SelectedNode = currentSessionNode,
                OpenNodes = []
            };
    }

    private void SyncOpenSessionNodes(IReadOnlyList<SessionNodeInput> openNodes, string? previousActiveId)
    {
        var incomingNodes = openNodes
            .Select(ResolveSessionNode)
            .Where(node => node is not null)
            .Cast<SessionNodeInput>()
            .DistinctBy(node => node.Id)
            .ToList();

        var newOpenNodes = new List<SessionNodeInput>();

        foreach (var pinnedId in _pinnedNodeIds)
        {
            var pinnedNode = incomingNodes.FirstOrDefault(n => n.Id == pinnedId)
                ?? FindSessionNode(pinnedId);

            if (pinnedNode is not null)
            {
                newOpenNodes.Add(pinnedNode);
            }
        }

        if (_activeSessionId is not null && !_pinnedNodeIds.Contains(_activeSessionId))
        {
            var activeNode = incomingNodes.FirstOrDefault(n => n.Id == _activeSessionId);
            if (activeNode is not null)
            {
                newOpenNodes.Add(activeNode);
            }
        }

        _openSessionNodes.Clear();
        _openSessionNodes.AddRange(newOpenNodes);
    }

    private List<SessionCommandSubmitted> GetSessionCommands(string sessionId)
    {
        if (_sessionCommands.TryGetValue(sessionId, out var commands))
            return commands;

        commands = [];
        _sessionCommands[sessionId] = commands;
        return commands;
    }

    private bool IsBlockPinned(SessionContextBlockModel block) =>
        (ViewState.ContextPins ?? []).Any(pin => pin.Node.Id == block.Context.Id);

    private Task UnpinBlockAsync(SessionContextBlockModel block)
    {
        var pinId = $"pin:{block.Context.Id}";
        _state = SessionUiReducer.Reduce(_state, new UnpinSessionContext(pinId));
        ClearBlockMode(block);
        if (string.Equals(_maximizedBlockId, block.Id, StringComparison.Ordinal))
        {
            _maximizedBlockId = null;
        }

        if (string.Equals(_pageOverlayBlockId, block.Id, StringComparison.Ordinal))
        {
            _pageOverlayBlockId = null;
        }

        if (string.Equals(_appOverlayBlockId, block.Id, StringComparison.Ordinal))
        {
            _appOverlayBlockId = null;
        }

        PersistCurrentSessionState();
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleBlockModeChangedAsync(SessionContextBlockModel block, string mode)
    {
        if (string.Equals(mode, SessionContextBlock.ModeMaximized, StringComparison.Ordinal))
        {
            ClearBlockMode(block);
            _maximizedBlockId = block.Id;
            _maximizedCommandId = null;
            _pageOverlayBlockId = null;
            _pageOverlayCommandId = null;
            _appOverlayBlockId = null;
            _appOverlayCommandId = null;
        }
        else if (string.Equals(mode, SessionContextBlock.ModeOverlayPage, StringComparison.Ordinal))
        {
            ClearBlockMode(block);
            _pageOverlayBlockId = block.Id;
            _pageOverlayCommandId = null;
            _maximizedBlockId = null;
            _maximizedCommandId = null;
            _appOverlayBlockId = null;
            _appOverlayCommandId = null;
        }
        else if (string.Equals(mode, SessionContextBlock.ModeOverlayApp, StringComparison.Ordinal))
        {
            ClearBlockMode(block);
            _appOverlayBlockId = block.Id;
            _appOverlayCommandId = null;
            _pageOverlayBlockId = null;
            _pageOverlayCommandId = null;
            _maximizedBlockId = null;
            _maximizedCommandId = null;
        }
        else
        {
            SetBlockMode(block, mode);

            if (string.Equals(_maximizedBlockId, block.Id, StringComparison.Ordinal))
            {
                _maximizedBlockId = null;
            }

            if (string.Equals(_pageOverlayBlockId, block.Id, StringComparison.Ordinal))
            {
                _pageOverlayBlockId = null;
            }

            if (string.Equals(_appOverlayBlockId, block.Id, StringComparison.Ordinal))
            {
                _appOverlayBlockId = null;
            }
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleCommandModeChangedAsync(SessionCommandSubmitted command, string mode)
    {
        if (string.Equals(mode, SessionContextBlock.ModeMaximized, StringComparison.Ordinal))
        {
            _maximizedCommandId = command.Id;
            _maximizedBlockId = null;
            _pageOverlayCommandId = null;
            _pageOverlayBlockId = null;
            _appOverlayCommandId = null;
            _appOverlayBlockId = null;
        }
        else if (string.Equals(mode, SessionContextBlock.ModeOverlayPage, StringComparison.Ordinal))
        {
            _pageOverlayCommandId = command.Id;
            _pageOverlayBlockId = null;
            _maximizedCommandId = null;
            _maximizedBlockId = null;
            _appOverlayCommandId = null;
            _appOverlayBlockId = null;
        }
        else if (string.Equals(mode, SessionContextBlock.ModeOverlayApp, StringComparison.Ordinal))
        {
            _appOverlayCommandId = command.Id;
            _appOverlayBlockId = null;
            _pageOverlayCommandId = null;
            _pageOverlayBlockId = null;
            _maximizedCommandId = null;
            _maximizedBlockId = null;
        }
        else if (string.Equals(_maximizedCommandId, command.Id, StringComparison.Ordinal))
        {
            _maximizedCommandId = null;
        }
        else if (string.Equals(_pageOverlayCommandId, command.Id, StringComparison.Ordinal))
        {
            _pageOverlayCommandId = null;
        }
        else if (string.Equals(_appOverlayCommandId, command.Id, StringComparison.Ordinal))
        {
            _appOverlayCommandId = null;
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private static string GetCommandTurnId(SessionCommandSubmitted command) => $"session-turn-response:{command.Id}";

    private static string GetFeedItemKey(string sessionId, string itemKind, string itemId) =>
        $"{sessionId}:{itemKind}:{itemId}";

    private static string GetBlockModeKey(string sessionId, SessionContextBlockModel block) =>
        $"{sessionId}:block-mode:{block.Id}";

    private string GetBlockMode(SessionContextBlockModel block)
    {
        if (string.Equals(_maximizedBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeMaximized;

        if (string.Equals(_pageOverlayBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeOverlayPage;

        if (string.Equals(_appOverlayBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeOverlayApp;

        return _blockModesByKey.TryGetValue(GetBlockModeKey(CurrentSessionId, block), out var mode)
            ? mode
            : SessionContextBlock.ModeNormal;
    }

    private void ClearBlockMode(SessionContextBlockModel block) =>
        _blockModesByKey.Remove(GetBlockModeKey(CurrentSessionId, block));

    private void SetBlockMode(SessionContextBlockModel block, string mode)
    {
        if (string.Equals(mode, SessionContextBlock.ModeMinimized, StringComparison.Ordinal))
        {
            _blockModesByKey[GetBlockModeKey(CurrentSessionId, block)] = SessionContextBlock.ModeMinimized;
            return;
        }

        ClearBlockMode(block);
    }

    private long EnsureFeedSequence(string key)
    {
        if (_feedSequenceByItemKey.TryGetValue(key, out var sequence))
            return sequence;

        sequence = ++_nextFeedSequence;
        _feedSequenceByItemKey[key] = sequence;
        return sequence;
    }

    private long MoveFeedItemToEnd(string key)
    {
        var sequence = ++_nextFeedSequence;
        _feedSequenceByItemKey[key] = sequence;
        return sequence;
    }

    private void MoveProjectedBlockToEnd(string sessionId)
    {
        if (_state.ContextProjection is null)
            return;

        MoveFeedItemToEnd(GetFeedItemKey(sessionId, "block", "projected-context-block"));
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

    private sealed record SessionAppSection(string Id, string Label, string Kind);

    private enum SessionFeedItemKind { Response, Block }

    private sealed record SessionFeedItem(
        string Key,
        SessionFeedItemKind Kind,
        long Sequence,
        SessionCommandSubmitted? Command = null,
        SessionContextBlockModel? Block = null);
}