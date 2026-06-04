namespace LowlandTech.Components.Session;

public sealed class SessionStateService
{
    private SessionAppState _state;

    private static readonly ISessionSurfaceDiscovery DefaultSurfaceDiscovery = new SessionSurfaceDiscovery([]);

    private readonly ISessionSurfaceDiscovery _surfaceDiscovery;

    public SessionStateService()
        : this(SessionAppDefaults.Empty, null)
    {
    }

    public SessionStateService(SessionAppState initialState)
        : this(initialState, null)
    {
    }

    public SessionStateService(ISessionSurfaceDiscovery? surfaceDiscovery)
        : this(SessionAppDefaults.Empty, surfaceDiscovery)
    {
    }

    public SessionStateService(SessionAppState initialState, ISessionSurfaceDiscovery? surfaceDiscovery)
    {
        _state = initialState;
        _surfaceDiscovery = surfaceDiscovery ?? DefaultSurfaceDiscovery;
    }

    public SessionAppState State => _state;

    public event Action<SessionAppState>? StateChanged;

    public SessionAppState Dispatch(object action)
    {
        var next = SessionAppReducer.Reduce(_state, action);
        if (EqualityComparer<SessionAppState>.Default.Equals(_state, next))
            return _state;

        _state = next;
        StateChanged?.Invoke(_state);
        return _state;
    }

    public void Reset(SessionAppState? state = null)
    {
        _state = state ?? SessionAppDefaults.Empty;
        StateChanged?.Invoke(_state);
    }

    public void ShowSessions()
    {
        var activeSessionId = _state.ActiveSessionId;
        PersistCurrentSessionState();
        Dispatch(new SetActiveSession(null));
        Dispatch(new RestoreSessionUiState(SessionUiDefaults.Empty));
        Dispatch(new SetOpenSessionNodes(_state.OpenSessionNodes.Where(node => _state.PinnedNodeIds.Contains(node.Id)).ToList()));
        Dispatch(new SetSessionPresentationState(null, null, null, null, null, null));
        Dispatch(new ShowSessionFeedPage());
        if (!string.IsNullOrWhiteSpace(activeSessionId))
            Dispatch(new SetPendingScrollSection(null));
    }

    public void SelectSession(SessionNodeInput node, IReadOnlyList<SessionListEntry> sessions)
    {
        PersistCurrentSessionState();

        var openNodes = _state.OpenSessionNodes.Where(existing => _state.PinnedNodeIds.Contains(existing.Id)).ToList();
        if (openNodes.All(existing => existing.Id != node.Id))
            openNodes.Add(node);

        Dispatch(new SetActiveSession(node.Id));
        Dispatch(new SetSessionPresentationState(null, null, null, null, null, null));
        Dispatch(new SetOpenSessionNodes(openNodes));
        Dispatch(new RestoreSessionUiState(GetSessionState(node.Id, sessions)));
        Dispatch(new SelectSessionNode(node));
        Dispatch(new ShowSessionFeedPage());
    }

    public void SetInput(string value)
    {
        Dispatch(new SetSessionInput(value));
        PersistCurrentSessionState();
    }

    public void SetAgentType(string agentTypeKey)
    {
        Dispatch(new SelectSessionAgentType(agentTypeKey));
        PersistCurrentSessionState();
    }

    public void ProjectContext(SessionNodeInput node)
    {
        Dispatch(new ProjectSessionContext(node));
        PersistCurrentSessionState();
        MoveFeedItemToEnd(GetFeedItemKey(CurrentSessionId, "block", "projected-context-block"));
        Dispatch(new SetPendingScrollSection("projected-context-block"));
    }

    public void PinContext(SessionNodeInput node)
    {
        Dispatch(new PinSessionContext(new SessionContextProjectionModel(node)));
        PersistCurrentSessionState();
        var blockId = $"context-block:{node.Id}";
        EnsureFeedSequence(GetFeedItemKey(CurrentSessionId, "block", blockId));
        Dispatch(new SetPendingScrollSection(blockId));
    }

    public void UnpinContext(string pinId)
    {
        Dispatch(new UnpinSessionContext(pinId));
        PersistCurrentSessionState();
    }

    public void SetTabPinned(string nodeId, bool isPinned)
    {
        var next = new HashSet<string>(_state.PinnedNodeIds, StringComparer.Ordinal);
        if (isPinned)
            next.Add(nodeId);
        else
            next.Remove(nodeId);

        Dispatch(new SetPinnedSessionNodes(next));
    }

    public void CloseTab(SessionNodeInput node, IReadOnlyList<SessionListEntry> sessions)
    {
        Dispatch(new CloseSessionNode(node.Id));
        var openNodes = _state.OpenSessionNodes.Where(existing => !string.Equals(existing.Id, node.Id, StringComparison.Ordinal)).ToList();
        Dispatch(new SetOpenSessionNodes(openNodes));

        if (!string.Equals(_state.ActiveSessionId, node.Id, StringComparison.Ordinal))
        {
            PersistCurrentSessionState();
            return;
        }

        var nextNode = openNodes.LastOrDefault();
        if (nextNode is null)
        {
            ShowSessions();
            return;
        }

        SelectSession(nextNode, sessions);
    }

    public async Task SubmitAsync(SessionCommandSubmitted command, bool isTest, int testStreamDelayMilliseconds, IGraphDiscoveryService discovery)
    {
        var sessionId = CurrentSessionId;
        Dispatch(new SubmitSessionCommand(command.Node, command.AgentTypeKey, command.Input));
        PersistCurrentSessionState();

        if (isTest)
        {
            var testCommand = command with { Response = string.Empty, IsStreaming = true };
            EnsureFeedSequence(GetFeedItemKey(sessionId, "command", testCommand.Id));
            SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(testCommand).ToList());
            MoveProjectedBlockToEnd(sessionId);
            Dispatch(new SetPendingScrollSection(GetCommandTurnId(testCommand)));
            await StreamTestResponseAsync(sessionId, testCommand, testStreamDelayMilliseconds);
            return;
        }

        var service = discovery.Get(command.AgentTypeKey);
        if (service is null)
        {
            EnsureFeedSequence(GetFeedItemKey(sessionId, "command", command.Id));
            SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(command).ToList());
            MoveProjectedBlockToEnd(sessionId);
            Dispatch(new SetPendingScrollSection(GetCommandTurnId(command)));
            return;
        }

        var context = new SessionContext { Input = command.Input };

        // Prefer streaming when the agent supports it. The default interface
        // implementation throws NotSupportedException; catch it at the call site
        // to fall back to ExecuteAsync for agents that haven't opted in.
        IAsyncEnumerable<string>? stream = null;
        try
        {
            stream = service.StreamAsync(context);
        }
        catch (NotSupportedException)
        {
            stream = null;
        }

        if (stream is null)
        {
            await service.ExecuteAsync(context);
            if (!string.IsNullOrWhiteSpace(context.Response))
                command = command with { Response = context.Response };

            EnsureFeedSequence(GetFeedItemKey(sessionId, "command", command.Id));
            SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(command).ToList());
            MoveProjectedBlockToEnd(sessionId);
            Dispatch(new SetPendingScrollSection(GetCommandTurnId(command)));
            return;
        }

        var streamingCommand = command with { Response = string.Empty, IsStreaming = true };
        EnsureFeedSequence(GetFeedItemKey(sessionId, "command", streamingCommand.Id));
        SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(streamingCommand).ToList());
        MoveProjectedBlockToEnd(sessionId);
        Dispatch(new SetPendingScrollSection(GetCommandTurnId(streamingCommand)));

        var response = string.Empty;
        await foreach (var chunk in stream)
        {
            response += chunk;
            UpdateCommand(sessionId, streamingCommand.Id, existing => existing with { Response = response, IsStreaming = true });
        }

        UpdateCommand(sessionId, streamingCommand.Id, existing => existing with { Response = response, IsStreaming = false });
    }

    public void UnpinBlock(SessionContextBlockModel block)
    {
        Dispatch(new UnpinSessionContext($"pin:{block.Context.Id}"));
        ClearBlockMode(block);
        ClearPresentationIds(block.Id, null);
        PersistCurrentSessionState();
    }

    public void SetBlockMode(SessionContextBlockModel block, string mode)
    {
        var modeKey = GetBlockModeKey(CurrentSessionId, block);
        if (string.Equals(mode, SessionContextBlock.ModeMaximized, StringComparison.Ordinal))
        {
            Dispatch(new SetBlockModeEntry(modeKey, null));
            Dispatch(new SetSessionPresentationState(block.Id, null, null, null, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayPage, StringComparison.Ordinal))
        {
            Dispatch(new SetBlockModeEntry(modeKey, null));
            Dispatch(new SetSessionPresentationState(null, null, block.Id, null, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayApp, StringComparison.Ordinal))
        {
            Dispatch(new SetBlockModeEntry(modeKey, null));
            Dispatch(new SetSessionPresentationState(null, null, null, null, block.Id, null));
            return;
        }

        Dispatch(new SetBlockModeEntry(modeKey, string.Equals(mode, SessionContextBlock.ModeMinimized, StringComparison.Ordinal) ? SessionContextBlock.ModeMinimized : null));
        ClearPresentationIds(block.Id, null);
    }

    public void SetCommandMode(SessionCommandSubmitted command, string mode)
    {
        if (string.Equals(mode, SessionContextBlock.ModeMaximized, StringComparison.Ordinal))
        {
            Dispatch(new SetSessionPresentationState(null, command.Id, null, null, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayPage, StringComparison.Ordinal))
        {
            Dispatch(new SetSessionPresentationState(null, null, null, command.Id, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayApp, StringComparison.Ordinal))
        {
            Dispatch(new SetSessionPresentationState(null, null, null, null, null, command.Id));
            return;
        }

        ClearPresentationIds(null, command.Id);
    }

    public void ShowEventLogPage() => Dispatch(new ShowSessionEventLogPage());

    public void ShowSettingsPage() => Dispatch(new ShowSessionSettingsPage());

    public void ShowFeedPage() => Dispatch(new ShowSessionFeedPage());

    public void ClearEventLog() => Dispatch(new ClearSessionEventLog());

    public string GetCurrentEventLogStateJson() => SessionAppReducer.SerializeEventLogState(_state);

    public string GetInitialEventLogStateJson() => _state.EventLogEntries.FirstOrDefault()?.BeforeStateJson ?? GetCurrentEventLogStateJson();

    public SessionUiState GetViewState(IReadOnlyList<SessionListEntry> sessions)
    {
        var currentNode = GetCurrentSessionNode(sessions);
        return currentNode is null
            ? SessionUiDefaults.Empty with { OpenNodes = _state.OpenSessionNodes }
            : _state.Ui with { SelectedNode = currentNode, OpenNodes = _state.OpenSessionNodes };
    }

    public IReadOnlyList<SessionListEntry> GetDisplaySessions(IReadOnlyList<SessionListEntry> sessions) =>
        [.. sessions.Select(session => session with { Count = GetSessionSectionCount(session.Id) })];

    public IReadOnlyList<SessionSectionItem> GetVisibleSections(IReadOnlyList<SessionListEntry> sessions)
    {
        if (string.IsNullOrWhiteSpace(_state.ActiveSessionId))
            return [];

        var sessionId = CurrentSessionId;
        var sections = new List<SessionSectionItem>
        {
            new("session-turn-request", GetRequestNode(sessionId).Label, "turn"),
            new("session-turn-response", GetResponseNode(sessionId).Label, "turn")
        };

        foreach (var item in GetFeedItems(sessions))
        {
            if (item.Kind == SessionFeedItemKind.Response)
            {
                sections.Add(new SessionSectionItem(GetCommandTurnId(item.Command!), CreateResponseNode(item.Command!).Label, "turn"));
            }
            else
            {
                sections.Add(new SessionSectionItem(item.Block!.Id, item.Block.Context.Label, item.Block.Context.NodeTypeKey));
            }
        }

        return sections;
    }

    public IReadOnlyList<SessionFeedItemDescriptor> GetFeedItems(IReadOnlyList<SessionListEntry> sessions)
    {
        var sessionId = CurrentSessionId;
        var viewState = GetViewState(sessions);

        var responses = GetSessionCommands(sessionId).Select(cmd =>
        {
            var key = GetFeedItemKey(sessionId, "command", cmd.Id);
            var sequence = _state.FeedSequenceByItemKey.TryGetValue(key, out var value) ? value : long.MaxValue;
            return new SessionFeedItemDescriptor(key, SessionFeedItemKind.Response, sequence, Command: cmd);
        });

        var blocks = (viewState.ContextBlocks ?? []).Select(block =>
        {
            var key = GetFeedItemKey(sessionId, "block", block.Id);
            var sequence = _state.FeedSequenceByItemKey.TryGetValue(key, out var value) ? value : long.MaxValue;
            return new SessionFeedItemDescriptor(key, SessionFeedItemKind.Block, sequence, Block: block);
        });

        return [.. responses.Concat(blocks).OrderBy(item => item.Sequence).ThenBy(item => item.Key, StringComparer.Ordinal)];
    }

    public int GetSessionSectionCount(string sessionId) => GetSessionTurnSectionCount(sessionId) + GetSessionBlockSectionCount(sessionId);

    public int GetSessionTurnSectionCount(string sessionId) => GetSessionCommands(sessionId).Count + 2;

    public int GetSessionBlockSectionCount(string sessionId)
    {
        var state = string.Equals(_state.ActiveSessionId, sessionId, StringComparison.Ordinal)
            ? _state.Ui
            : _state.SessionStatesBySessionId.TryGetValue(sessionId, out var storedState)
                ? storedState
                : SessionUiDefaults.Empty;

        return state.ContextBlocks?.Count ?? 0;
    }

    public SessionContextBlockModel? GetMaximizedBlock(IReadOnlyList<SessionListEntry> sessions)
    {
        var blockId = _state.MaximizedBlockId;
        if (string.IsNullOrWhiteSpace(blockId))
            return null;

        return (GetViewState(sessions).ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, blockId, StringComparison.Ordinal));
    }

    public SessionCommandSubmitted? GetMaximizedCommand()
    {
        var commandId = _state.MaximizedCommandId;
        if (string.IsNullOrWhiteSpace(commandId))
            return null;

        return GetSessionCommands(CurrentSessionId).FirstOrDefault(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
    }

    public SessionContextBlockModel? GetPageOverlayBlock(IReadOnlyList<SessionListEntry> sessions)
    {
        var blockId = _state.PageOverlayBlockId;
        if (string.IsNullOrWhiteSpace(blockId))
            return null;

        return (GetViewState(sessions).ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, blockId, StringComparison.Ordinal));
    }

    public SessionCommandSubmitted? GetPageOverlayCommand()
    {
        var commandId = _state.PageOverlayCommandId;
        if (string.IsNullOrWhiteSpace(commandId))
            return null;

        return GetSessionCommands(CurrentSessionId).FirstOrDefault(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
    }

    public SessionContextBlockModel? GetAppOverlayBlock(IReadOnlyList<SessionListEntry> sessions)
    {
        var blockId = _state.AppOverlayBlockId;
        if (string.IsNullOrWhiteSpace(blockId))
            return null;

        return (GetViewState(sessions).ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, blockId, StringComparison.Ordinal));
    }

    public SessionCommandSubmitted? GetAppOverlayCommand()
    {
        var commandId = _state.AppOverlayCommandId;
        if (string.IsNullOrWhiteSpace(commandId))
            return null;

        return GetSessionCommands(CurrentSessionId).FirstOrDefault(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
    }

    public SessionResponseProjection GetResponseProjection(SessionCommandSubmitted command)
    {
        var contextCount = GetSessionBlockSectionCount(command.Node.Id);
        return new SessionResponseProjection(
            GetResponseStatus(command),
            GetResponseStatusVariant(command),
            CreateResponseDescription(command, contextCount));
    }

    public static string GetResponseStatus(SessionCommandSubmitted command) => command.IsStreaming ? "Streaming" : command.AgentTypeKey;

    public static string GetResponseStatusVariant(SessionCommandSubmitted command) => command.IsStreaming ? "secondary" : "outline";

    private static string CreateResponseDescription(SessionCommandSubmitted command, int contextCount)
    {
        if (!string.IsNullOrWhiteSpace(command.Response))
            return command.Response;

        if (command.IsStreaming)
            return "Thinking...";

        return $"{command.AgentTypeKey} processed \"{command.Input}\" with {contextCount} context block(s).";
    }

    public SessionNodeInput? GetCurrentSessionNode(IReadOnlyList<SessionListEntry> sessions) =>
        _state.ActiveSessionId is null ? null : FindSessionNode(_state.ActiveSessionId, sessions);

    public string CurrentSessionId => _state.ActiveSessionId ?? "orders-session";

    public IReadOnlyList<SessionCommandSubmitted> GetSessionCommands(string sessionId)
    {
        return _state.SessionCommandsBySessionId.TryGetValue(sessionId, out var commands)
            ? commands
            : [];
    }

    public SessionUiState GetSessionState(string sessionId, IReadOnlyList<SessionListEntry> sessions)
    {
        if (_state.SessionStatesBySessionId.TryGetValue(sessionId, out var state))
            return state;

        var sessionNode = FindSessionNode(sessionId, sessions);
        return sessionNode is null
            ? SessionUiDefaults.Empty
            : SessionUiDefaults.Empty with { SelectedNode = sessionNode };
    }

    public long EnsureFeedSequence(string key)
    {
        if (_state.FeedSequenceByItemKey.TryGetValue(key, out var sequence))
            return sequence;

        sequence = _state.NextFeedSequence + 1;
        Dispatch(new SetFeedSequence(key, sequence, sequence));
        return sequence;
    }

    public long MoveFeedItemToEnd(string key)
    {
        var sequence = _state.NextFeedSequence + 1;
        Dispatch(new SetFeedSequence(key, sequence, sequence));
        return sequence;
    }

    public string GetBlockMode(string sessionId, SessionContextBlockModel block)
    {
        if (string.Equals(_state.MaximizedBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeMaximized;

        if (string.Equals(_state.PageOverlayBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeOverlayPage;

        if (string.Equals(_state.AppOverlayBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeOverlayApp;

        return _state.BlockModesByKey.TryGetValue(GetBlockModeKey(sessionId, block), out var mode)
            ? mode
            : SessionContextBlock.ModeNormal;
    }

    public SessionNodeInput GetRequestNode(string sessionId) => _surfaceDiscovery.Find(sessionId).RequestNode;

    public SessionNodeInput GetResponseNode(string sessionId) => _surfaceDiscovery.Find(sessionId).ResponseNode;

    public static SessionNodeInput CreateResponseNode(SessionCommandSubmitted command) =>
        new($"response:{command.Id}", "session-response", $"{command.AgentTypeKey} response");

    public static string GetCommandTurnId(SessionCommandSubmitted command) => $"session-turn-response:{command.Id}";

    public static string GetFeedItemKey(string sessionId, string itemKind, string itemId) =>
        $"{sessionId}:{itemKind}:{itemId}";

    public static string GetBlockModeKey(string sessionId, SessionContextBlockModel block) =>
        $"{sessionId}:block-mode:{block.Id}";

    private void SetCommandHistory(string sessionId, IReadOnlyList<SessionCommandSubmitted> commands)
    {
        Dispatch(new SetSessionCommandHistory(sessionId, commands));
    }

    private async Task StreamTestResponseAsync(string sessionId, SessionCommandSubmitted command, int delay)
    {
        var response = string.Empty;

        await foreach (var chunk in DummyAskAgentGraphService.StreamReplyAsync(command.Input, delay))
        {
            response += chunk;
            UpdateCommand(sessionId, command.Id, existing => existing with { Response = response, IsStreaming = true });
        }

        UpdateCommand(sessionId, command.Id, existing => existing with { Response = response, IsStreaming = false });
    }

    private void UpdateCommand(string sessionId, string commandId, Func<SessionCommandSubmitted, SessionCommandSubmitted> update)
    {
        var commands = GetSessionCommands(sessionId).ToList();
        var index = commands.FindIndex(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
        if (index < 0)
            return;

        commands[index] = update(commands[index]);
        SetCommandHistory(sessionId, commands);
    }

    private void PersistCurrentSessionState()
    {
        if (_state.ActiveSessionId is null)
            return;

        Dispatch(new SetStoredSessionState(_state.ActiveSessionId, StripNavigationState(_state.Ui)));
    }

    private SessionUiState StripNavigationState(SessionUiState state)
    {
        var currentSessionNode = _state.ActiveSessionId is null ? null : new SessionNodeInput(_state.ActiveSessionId, "session", _state.ActiveSessionId);
        return currentSessionNode is null
            ? state with { OpenNodes = [] }
            : state with { SelectedNode = currentSessionNode, OpenNodes = [] };
    }

    private void MoveProjectedBlockToEnd(string sessionId)
    {
        if (_state.Ui.ContextProjection is null)
            return;

        MoveFeedItemToEnd(GetFeedItemKey(sessionId, "block", "projected-context-block"));
    }

    private void ClearBlockMode(SessionContextBlockModel block)
    {
        Dispatch(new SetBlockModeEntry(GetBlockModeKey(CurrentSessionId, block), null));
    }

    private void ClearPresentationIds(string? blockId, string? commandId)
    {
        var next = new SetSessionPresentationState(
            string.Equals(_state.MaximizedBlockId, blockId, StringComparison.Ordinal) ? null : _state.MaximizedBlockId,
            string.Equals(_state.MaximizedCommandId, commandId, StringComparison.Ordinal) ? null : _state.MaximizedCommandId,
            string.Equals(_state.PageOverlayBlockId, blockId, StringComparison.Ordinal) ? null : _state.PageOverlayBlockId,
            string.Equals(_state.PageOverlayCommandId, commandId, StringComparison.Ordinal) ? null : _state.PageOverlayCommandId,
            string.Equals(_state.AppOverlayBlockId, blockId, StringComparison.Ordinal) ? null : _state.AppOverlayBlockId,
            string.Equals(_state.AppOverlayCommandId, commandId, StringComparison.Ordinal) ? null : _state.AppOverlayCommandId);
        Dispatch(next);
    }

    private static SessionNodeInput? FindSessionNode(string sessionId, IReadOnlyList<SessionListEntry> sessions)
    {
        var entry = sessions.FirstOrDefault(candidate => string.Equals(candidate.Id, sessionId, StringComparison.Ordinal));
        return entry is null ? null : new SessionNodeInput(entry.Id, entry.NodeTypeKey, entry.Label);
    }
}
