namespace LowlandTech.Components;

public sealed record SessionNodeInput(
    string Id,
    string NodeTypeKey,
    string Label,
    string? ComponentBlock = null,
    IReadOnlyDictionary<string, object?>? Properties = null);

public sealed record SessionContextNode(
    SessionNodeInput Node,
    IReadOnlyList<SessionContextNode>? Children = null,
    bool IsLoaded = false);

/// <summary>
/// Fired from <see cref="SessionBlock"/> when the user submits a create command
/// while a header node is selected, so the host page can materialise the child
/// node in the graph and refresh the context tree.
/// </summary>
public sealed record SessionHeaderCreateRequest(string ParentNodeId, string ParentNodeTypeKey, string Label);

/// <summary>
/// Fired from <see cref="SessionBlock"/> when the user submits an edit command
/// while a context node is selected, so the host page can persist the renamed
/// node in the graph and refresh the context tree.
/// </summary>
public sealed record SessionNodeRenameRequest(string NodeId, string NodeTypeKey, string OldLabel, string NewLabel);

/// <summary>
/// Marker interface for actions that represent a system side-effect
/// (create, update, delete, send mail, etc.) to be surfaced in the outbox.
/// </summary>
public interface ISessionSideEffect
{
}

/// <summary>Side-effect: schedule creation of an entity.</summary>
public sealed record ScheduleCreateEffect(string EntityType, string EntityId, string Label) : ISessionSideEffect;

/// <summary>Side-effect: schedule an update to an entity property.</summary>
public sealed record ScheduleUpdateEffect(string EntityType, string EntityId, string PropertyName, string NewValue) : ISessionSideEffect;

/// <summary>Side-effect: schedule deletion of an entity.</summary>
public sealed record ScheduleDeleteEffect(string EntityType, string EntityId, string Label) : ISessionSideEffect;

/// <summary>Side-effect: send an email.</summary>
public sealed record SendMailEffect(string To, string Subject, string Body) : ISessionSideEffect;

public sealed record SessionAgentType(
    string Key,
    string Label,
    bool Disabled = false,
    string? Tooltip = null,
    EventCallback<SessionNodeInput> OnSelected = default,
    EventCallback<SessionCommandSubmitted> OnSubmit = default);

public sealed record SessionListEntry(
    string Id,
    string Label,
    int Count = 0,
    string NodeTypeKey = "session");

public sealed record SessionCardAction(
    string Key,
    string Label,
    EventCallback<SessionNodeInput> OnSelected = default);

public sealed record SessionContextProjectionModel(
    SessionNodeInput Node);

public sealed record SessionContextPinModel(
    string Id,
    SessionNodeInput Node,
    SessionNodeInput SessionNode);

public sealed record SessionContextBlockModel(
    string Id,
    SessionNodeInput Context,
    object? Data = null,
    [property: JsonIgnore]
    Type? ComponentType = null,
    bool IncludeInContextWindow = true)
{
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    [JsonIgnore]
    public Type? EffectiveComponentType => ComponentType;
}

public interface ISessionBlock
{
}

public interface ISessionContextBlock : ISessionBlock
{
}

public sealed record SessionUiState(
    string ActiveAgentTypeKey,
    string Input,
    SessionNodeInput? SelectedNode,
    IReadOnlyList<SessionNodeInput> OpenNodes,
    SessionContextProjectionModel? ContextProjection = null,
    IReadOnlyList<SessionContextPinModel>? ContextPins = null,
    IReadOnlyList<SessionContextBlockModel>? ContextBlocks = null);

public enum SessionBlockPage
{
    Feed,
    EventLog,
    Settings,
}

public sealed record SessionClientKeyEntry(
    string Id,
    string Name,
    string KeyPreview,
    string CreatedBy,
    DateTimeOffset CreatedAt,
    bool IsRevoked = false);

public sealed record SessionClientKeyIssued(
    SessionClientKeyEntry Entry,
    string Secret);

public interface ISessionClientSettingsService
{
    Task<IReadOnlyList<SessionClientKeyEntry>> ListAsync(CancellationToken ct = default);
    Task<SessionClientKeyIssued> CreateAsync(string name, CancellationToken ct = default);
    Task RevokeAsync(string id, CancellationToken ct = default);
}

public interface ISessionAction
{
}

public interface ISessionEvent
{
}

public sealed record SessionStateDeltaItem(
    string Path,
    string Kind,
    string Label,
    string? Before,
    string? After,
    string Description);

public sealed record SessionEventLogItem(
    Guid Id,
    string ActionType,
    DateTimeOffset Timestamp,
    string BeforeStateJson,
    string AfterStateJson,
    IReadOnlyList<SessionStateDeltaItem> Deltas) : ISessionEvent;

public sealed record SessionSectionItem(string Id, string Label, string Kind);

public enum SessionFeedItemKind
{
    Response,
    Block,
}

public sealed record SessionFeedItemDescriptor(
    string Key,
    SessionFeedItemKind Kind,
    long Sequence,
    SessionCommandSubmitted? Command = null,
    SessionContextBlockModel? Block = null);

public sealed record SessionResponseProjection(
    string Status,
    string StatusVariant,
    string Description);

public sealed record SessionSurfaceDescriptor(
    string SessionId,
    SessionNodeInput RequestNode,
    SessionNodeInput ResponseNode);

public sealed record SessionContextPinSnapshot(
    string Id,
    string NodeId,
    string Label);

public sealed record SessionContextBlockSnapshot(
    string Id,
    string ContextId,
    string ContextLabel,
    string NodeTypeKey,
    string? ComponentBlock,
    bool IncludeInContextWindow,
    string? DataPreview);

public sealed record SessionUiStateSnapshot(
    string ActiveAgentTypeKey,
    string Input,
    string? SelectedNodeId,
    IReadOnlyList<string> OpenNodeIds,
    string? ContextProjectionNodeId,
    IReadOnlyList<SessionContextPinSnapshot> ContextPins,
    IReadOnlyList<SessionContextBlockSnapshot> ContextBlocks);

public sealed record SessionCommandSnapshot(
    string Id,
    string AgentTypeKey,
    string Input,
    bool IsStreaming,
    string? Response);

public sealed record SessionBlockStateSnapshot(
    SessionUiStateSnapshot Ui,
    string ActivePage,
    string? ActiveSessionId,
    string? PendingScrollSectionId,
    string? MaximizedBlockId,
    string? MaximizedCommandId,
    string? PageOverlayBlockId,
    string? PageOverlayCommandId,
    string? AppOverlayBlockId,
    string? AppOverlayCommandId,
    IReadOnlyList<string> OpenSessionNodeIds,
    IReadOnlyList<string> PinnedNodeIds,
    IReadOnlyDictionary<string, long> FeedSequenceByItemKey,
    IReadOnlyDictionary<string, string> BlockModesByKey,
    IReadOnlyDictionary<string, string> CommandModesByKey,
    long NextFeedSequence,
    IReadOnlyDictionary<string, IReadOnlyList<SessionCommandSnapshot>> SessionCommandsBySessionId,
    IReadOnlyDictionary<string, SessionUiStateSnapshot> SessionStatesBySessionId);

public interface ISessionSurfaceRegistry
{
    int Priority => 0;

    SessionSurfaceDescriptor? GetSurface(string sessionId);
}

public interface ISessionSurfaceDiscovery
{
    SessionSurfaceDescriptor Find(string sessionId);
}

public sealed record SessionBlockState
{
    public SessionUiState Ui { get; init; } = SessionUiDefaults.Empty;
    public SessionBlockPage ActivePage { get; init; } = SessionBlockPage.Feed;
    public IReadOnlyList<SessionEventLogItem> EventLogEntries { get; init; } = [];
    public string? ActiveSessionId { get; init; }
    public string? PendingScrollSectionId { get; init; }
    public string? MaximizedBlockId { get; init; }
    public string? MaximizedCommandId { get; init; }
    public string? PageOverlayBlockId { get; init; }
    public string? PageOverlayCommandId { get; init; }
    public string? AppOverlayBlockId { get; init; }
    public string? AppOverlayCommandId { get; init; }
    public IReadOnlyList<SessionNodeInput> OpenSessionNodes { get; init; } = [];
    public HashSet<string> PinnedNodeIds { get; init; } = new(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, long> FeedSequenceByItemKey { get; init; } = new Dictionary<string, long>(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, string> BlockModesByKey { get; init; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, string> CommandModesByKey { get; init; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public long NextFeedSequence { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyList<SessionCommandSubmitted>> SessionCommandsBySessionId { get; init; } = new Dictionary<string, IReadOnlyList<SessionCommandSubmitted>>(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, SessionUiState> SessionStatesBySessionId { get; init; } = new Dictionary<string, SessionUiState>(StringComparer.Ordinal);
}

public static class SessionUiDefaults
{
    public static readonly IReadOnlyList<SessionAgentType> AgentTypes =
    [
        new("ask", "Ask"),
        new("plan", "Plan"),
        new("edit", "Edit"),
        new("run", "Run", Disabled: true)
    ];

    public static SessionUiState Empty { get; } = new("ask", string.Empty, null, []);
}

public static class SessionBlockDefaults
{
    public static SessionBlockState Empty { get; } = new();
}

public abstract record SessionUiAction : ISessionAction;

public abstract record SessionBlockAction : ISessionAction;

public sealed record SelectSessionNode(SessionNodeInput Node) : SessionUiAction;

public sealed record SetSessionInput(string Input) : SessionUiAction;

public sealed record SelectSessionAgentType(string AgentTypeKey) : SessionUiAction;

public sealed record SubmitSessionCommand(SessionNodeInput Node, string AgentTypeKey, string Input) : SessionUiAction;

public sealed record CloseSessionNode(string NodeId) : SessionUiAction;

public sealed record ProjectSessionContext(SessionNodeInput Node) : SessionUiAction;

public sealed record ClearSessionContextProjection : SessionUiAction;

public sealed record PinSessionContext(SessionContextProjectionModel Projection) : SessionUiAction;

public sealed record UnpinSessionContext(string PinId) : SessionUiAction;

public sealed record AddSessionContextBlock(SessionContextBlockModel Block) : SessionUiAction;

public sealed record RemoveSessionContextBlock(string BlockId) : SessionUiAction;

public sealed record RestoreSessionUiState(SessionUiState State) : SessionBlockAction;

public sealed record ShowSessionFeedPage : SessionBlockAction;

public sealed record ShowSessionEventLogPage : SessionBlockAction;

public sealed record ShowSessionSettingsPage : SessionBlockAction;

public sealed record ClearSessionEventLog : SessionBlockAction;

public sealed record SetSessionTabPinned(string NodeId, bool IsPinned) : SessionBlockAction;

public sealed record SetSessionBlockMode(string BlockId, string Mode) : SessionBlockAction;

public sealed record SetSessionCommandMode(string CommandId, string? Mode) : SessionBlockAction;

public sealed record SetActiveSession(string? SessionId) : SessionBlockAction;

public sealed record SetPendingScrollSection(string? SectionId) : SessionBlockAction;

public sealed record SetOpenSessionNodes(IReadOnlyList<SessionNodeInput> Nodes) : SessionBlockAction;

public sealed record SetPinnedSessionNodes(IReadOnlySet<string> NodeIds) : SessionBlockAction;

public sealed record SetSessionPresentationState(
    string? MaximizedBlockId,
    string? MaximizedCommandId,
    string? PageOverlayBlockId,
    string? PageOverlayCommandId,
    string? AppOverlayBlockId,
    string? AppOverlayCommandId) : SessionBlockAction;

public sealed record SetFeedSequence(string Key, long Sequence, long NextFeedSequence) : SessionBlockAction;

public sealed record SetBlockModeEntry(string Key, string? Mode) : SessionBlockAction;

public sealed record SetSessionCommandHistory(string SessionId, IReadOnlyList<SessionCommandSubmitted> Commands) : SessionBlockAction;

public sealed record SetStoredSessionState(string SessionId, SessionUiState State) : SessionBlockAction;

public sealed record PersistSessionCommands(string SessionId) : SessionBlockAction;

public sealed record RestoreSessionCommandHistory(string SessionId, IReadOnlyList<SessionCommandSubmitted> Commands) : SessionBlockAction;

public static class SessionUiReducer
{
    private const string SessionPinNodeType = "session.pin";
    private const string ProjectedContextBlockId = "projected-context-block";

    public static SessionUiState Reduce(SessionUiState state, SessionUiAction action) => action switch
    {
        SelectSessionNode select => state with
        {
            SelectedNode = select.Node,
            OpenNodes = Upsert(state.OpenNodes, select.Node)
        },
        SetSessionInput setInput => state with { Input = setInput.Input },
        SelectSessionAgentType selectAgent => state with { ActiveAgentTypeKey = selectAgent.AgentTypeKey },
        SubmitSessionCommand => state with { Input = string.Empty },
        CloseSessionNode close => state with
        {
            OpenNodes = state.OpenNodes.Where(node => node.Id != close.NodeId).ToList(),
            SelectedNode = state.SelectedNode?.Id == close.NodeId ? null : state.SelectedNode
        },
        ProjectSessionContext project => state with
        {
            ContextProjection = new SessionContextProjectionModel(project.Node),
            ContextBlocks = ReplaceProjectedBlock(
                state.ContextBlocks ?? [],
                CreateBlock(project.Node, ProjectedContextBlockId))
        },
        ClearSessionContextProjection => state with
        {
            ContextProjection = null,
            ContextBlocks = RemoveProjectedBlock(state.ContextBlocks ?? [])
        },
        PinSessionContext pin => state with
        {
            ContextPins = UpsertPin(state.ContextPins ?? [], CreatePin(pin.Projection)),
            ContextBlocks = UpsertBlock(
                RemoveProjectedBlock(state.ContextBlocks ?? []),
                CreateBlock(pin.Projection.Node)),
            ContextProjection = null
        },
        UnpinSessionContext unpin => state with
        {
            ContextPins = (state.ContextPins ?? []).Where(pin => pin.Id != unpin.PinId).ToList(),
            ContextBlocks = RemoveBlocksForPin(state.ContextBlocks ?? [], state.ContextPins ?? [], unpin.PinId)
        },
        AddSessionContextBlock add => state with
        {
            ContextBlocks = UpsertBlock(state.ContextBlocks ?? [], add.Block)
        },
        RemoveSessionContextBlock remove => state with
        {
            ContextBlocks = (state.ContextBlocks ?? []).Where(block => block.Id != remove.BlockId).ToList()
        },
        _ => state
    };

    private static IReadOnlyList<SessionNodeInput> Upsert(IReadOnlyList<SessionNodeInput> nodes, SessionNodeInput node)
    {
        var found = nodes.Select((n, i) => (n, i)).FirstOrDefault(x => x.n.Id == node.Id);
        if (found.n is not null)
            return nodes;

        var next = nodes.ToList();
        next.Add(node);
        return next;
    }

    private static SessionContextPinModel CreatePin(SessionContextProjectionModel projection)
    {
        var pinId = $"pin:{projection.Node.Id}";
        var pinNode = new SessionNodeInput(
            pinId,
            SessionPinNodeType,
            projection.Node.Label,
            "SessionContextPin",
            new Dictionary<string, object?>
            {
                ["contextNodeId"] = projection.Node.Id,
                ["contextNodeTypeKey"] = projection.Node.NodeTypeKey
            });

        return new SessionContextPinModel(pinId, projection.Node, pinNode);
    }

    private static SessionContextBlockModel CreateBlock(SessionNodeInput node, string? blockId = null) =>
        new(
            blockId ?? $"context-block:{node.Id}",
            node,
            node.Properties);

    private static IReadOnlyList<SessionContextPinModel> UpsertPin(IReadOnlyList<SessionContextPinModel> pins, SessionContextPinModel pin)
    {
        var next = pins.Where(existing => existing.Id != pin.Id).ToList();
        next.Add(pin);
        return next;
    }

    private static IReadOnlyList<SessionContextBlockModel> UpsertBlock(IReadOnlyList<SessionContextBlockModel> blocks, SessionContextBlockModel block)
    {
        var next = blocks.Where(existing => existing.Id != block.Id).ToList();
        next.Add(block);
        return next;
    }

    private static IReadOnlyList<SessionContextBlockModel> ReplaceProjectedBlock(
        IReadOnlyList<SessionContextBlockModel> blocks,
        SessionContextBlockModel block)
    {
        var next = RemoveProjectedBlock(blocks).ToList();
        next.Add(block);
        return next;
    }

    private static IReadOnlyList<SessionContextBlockModel> RemoveProjectedBlock(IReadOnlyList<SessionContextBlockModel> blocks) =>
        blocks.Where(existing => existing.Id != ProjectedContextBlockId).ToList();

    private static IReadOnlyList<SessionContextBlockModel> RemoveBlocksForPin(
        IReadOnlyList<SessionContextBlockModel> blocks,
        IReadOnlyList<SessionContextPinModel> pins,
        string pinId)
    {
        var contextNodeId = pins.FirstOrDefault(pin => pin.Id == pinId)?.Node.Id;
        return string.IsNullOrWhiteSpace(contextNodeId)
            ? blocks
            : blocks.Where(block => block.Context.Id != contextNodeId).ToList();
    }

}

public static class SessionBlockReducer
{
    private static readonly JsonSerializerOptions EventLogJsonOptions = new() { WriteIndented = true };

    public static SessionBlockState Reduce(SessionBlockState state, object action)
    {
        var next = action switch
        {
            RestoreSessionUiState restore => RestoreUiState(state, restore),
            SessionUiAction uiAction => state with { Ui = SessionUiReducer.Reduce(state.Ui, uiAction) },
            ShowSessionFeedPage => state with { ActivePage = SessionBlockPage.Feed },
            ShowSessionEventLogPage => state with { ActivePage = SessionBlockPage.EventLog },
            ShowSessionSettingsPage => state with { ActivePage = SessionBlockPage.Settings },
            ClearSessionEventLog => state with { EventLogEntries = [] },
            SetSessionTabPinned => state,
            SetSessionBlockMode => state,
            SetSessionCommandMode a => state with
            {
                CommandModesByKey = string.IsNullOrWhiteSpace(a.Mode)
                    ? RemoveDictionaryEntry(state.CommandModesByKey, a.CommandId)
                    : SetDictionaryEntry(state.CommandModesByKey, a.CommandId, a.Mode),
            },
            SetActiveSession a => state with { ActiveSessionId = a.SessionId },
            SetPendingScrollSection a => state with { PendingScrollSectionId = a.SectionId },
            SetOpenSessionNodes a => state with { OpenSessionNodes = a.Nodes.ToList() },
            SetPinnedSessionNodes a => state with { PinnedNodeIds = new HashSet<string>(a.NodeIds, StringComparer.Ordinal) },
            SetSessionPresentationState a => state with
            {
                MaximizedBlockId = a.MaximizedBlockId,
                MaximizedCommandId = a.MaximizedCommandId,
                PageOverlayBlockId = a.PageOverlayBlockId,
                PageOverlayCommandId = a.PageOverlayCommandId,
                AppOverlayBlockId = a.AppOverlayBlockId,
                AppOverlayCommandId = a.AppOverlayCommandId,
            },
            SetFeedSequence a => state with
            {
                FeedSequenceByItemKey = SetDictionaryEntry(state.FeedSequenceByItemKey, a.Key, a.Sequence),
                NextFeedSequence = a.NextFeedSequence,
            },
            SetBlockModeEntry a => state with
            {
                BlockModesByKey = string.IsNullOrWhiteSpace(a.Mode)
                    ? RemoveDictionaryEntry(state.BlockModesByKey, a.Key)
                    : SetDictionaryEntry(state.BlockModesByKey, a.Key, a.Mode),
            },
            SetSessionCommandHistory a => SetCommandHistory(state, a.SessionId, a.Commands),
            RestoreSessionCommandHistory a => SetCommandHistory(state, a.SessionId, a.Commands),
            PersistSessionCommands => state,
            SetStoredSessionState a => state with
            {
                SessionStatesBySessionId = SetDictionaryEntry(state.SessionStatesBySessionId, a.SessionId, a.State),
            },
            _ => state
        };

        return ShouldAppendEventLog(action)
            ? next with
            {
                EventLogEntries = next.EventLogEntries
                    .Append(new SessionEventLogItem(
                        Guid.NewGuid(),
                        action.GetType().Name,
                        DateTimeOffset.UtcNow,
                        SerializeEventLogState(state),
                        SerializeEventLogState(next),
                        BuildStateDelta(state, next)))
                    .ToList()
            }
            : next;
    }

    public static string SerializeEventLogState(SessionBlockState state) =>
        JsonSerializer.Serialize(CreateSnapshot(state), EventLogJsonOptions);

    private static SessionBlockState RestoreUiState(SessionBlockState state, RestoreSessionUiState restore)
    {
        var nextSequence = state.NextFeedSequence;
        var sequences = state.FeedSequenceByItemKey;

        if (!string.IsNullOrWhiteSpace(state.ActiveSessionId) && restore.State.ContextBlocks?.Count > 0)
        {
            var sessionId = state.ActiveSessionId;
            foreach (var block in restore.State.ContextBlocks)
            {
                var key = SessionStateService.GetFeedItemKey(sessionId, "block", block.Id);
                if (!sequences.ContainsKey(key))
                {
                    sequences = SetDictionaryEntry(sequences, key, ++nextSequence);
                }
            }
        }

        return state with
        {
            Ui = restore.State,
            FeedSequenceByItemKey = sequences,
            NextFeedSequence = nextSequence,
        };
    }

    private static SessionBlockState SetCommandHistory(SessionBlockState state, string sessionId, IReadOnlyList<SessionCommandSubmitted> commands)
    {
        var nextSequence = state.NextFeedSequence;
        var sequences = state.FeedSequenceByItemKey;

        foreach (var command in commands)
        {
            var key = SessionStateService.GetFeedItemKey(sessionId, "command", command.Id);
            if (!sequences.ContainsKey(key))
            {
                sequences = SetDictionaryEntry(sequences, key, ++nextSequence);
            }
        }

        return state with
        {
            SessionCommandsBySessionId = SetDictionaryEntry(state.SessionCommandsBySessionId, sessionId, commands),
            FeedSequenceByItemKey = sequences,
            NextFeedSequence = nextSequence,
        };
    }

    private static IReadOnlyList<SessionStateDeltaItem> BuildStateDelta(SessionBlockState before, SessionBlockState after)
    {
        var beforeNode = JsonSerializer.SerializeToNode(CreateSnapshot(before));
        var afterNode = JsonSerializer.SerializeToNode(CreateSnapshot(after));
        var deltas = new List<SessionStateDeltaItem>();
        AppendDelta(string.Empty, beforeNode, afterNode, deltas);
        return deltas;
    }

    private static void AppendDelta(string path, JsonNode? before, JsonNode? after, List<SessionStateDeltaItem> deltas)
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

        deltas.Add(CreateDeltaItem(path, before, after));
    }

    private static string? ToDeltaValue(JsonNode? node)
        => node is null ? null : node.ToJsonString();

    private static SessionStateDeltaItem CreateDeltaItem(string path, JsonNode? before, JsonNode? after)
    {
        var beforeValue = ToDeltaValue(before);
        var afterValue = ToDeltaValue(after);

        return path switch
        {
            "ActiveSessionId" => new SessionStateDeltaItem(path, "session", "Active session", FormatValue(before), FormatValue(after), $"Session changed from {FormatValue(before)} to {FormatValue(after)}."),
            "ActivePage" => new SessionStateDeltaItem(path, "page", "Active page", FormatValue(before), FormatValue(after), $"Page changed from {FormatValue(before)} to {FormatValue(after)}."),
            "Ui.Input" => new SessionStateDeltaItem(path, "draft", "Draft input", FormatValue(before), FormatValue(after), $"Draft input changed from {FormatValue(before)} to {FormatValue(after)}."),
            "Ui.ContextPins" => new SessionStateDeltaItem(path, "context", "Pinned context", FormatCollectionValue(before), FormatCollectionValue(after), $"Pinned context changed from {FormatCollectionValue(before)} to {FormatCollectionValue(after)}."),
            "Ui.ContextBlocks" => new SessionStateDeltaItem(path, "context", "Context blocks", FormatCollectionValue(before), FormatCollectionValue(after), $"Context blocks changed from {FormatCollectionValue(before)} to {FormatCollectionValue(after)}."),
            "PinnedNodeIds" => new SessionStateDeltaItem(path, "tabs", "Pinned tabs", FormatCollectionValue(before), FormatCollectionValue(after), $"Pinned tabs changed from {FormatCollectionValue(before)} to {FormatCollectionValue(after)}."),
            _ when path.StartsWith("SessionCommandsBySessionId.", StringComparison.Ordinal) =>
                new SessionStateDeltaItem(path, "commands", "Command history", FormatCollectionValue(before), FormatCollectionValue(after), $"Command history changed from {FormatCollectionValue(before)} to {FormatCollectionValue(after)}."),
            _ => new SessionStateDeltaItem(path, "state", HumanizePath(path), FormatValue(before), FormatValue(after), $"{HumanizePath(path)} changed from {FormatValue(before)} to {FormatValue(after)}.")
        };
    }

    private static string FormatValue(JsonNode? node)
    {
        if (node is null)
            return "none";

        return node switch
        {
            JsonValue value => value.ToJsonString().Trim('"'),
            JsonArray array => $"{array.Count} item(s)",
            JsonObject => "object",
            _ => node.ToJsonString()
        };
    }

    private static string FormatCollectionValue(JsonNode? node)
        => node is JsonArray array
            ? array.Count == 0 ? "empty" : $"{array.Count} item(s)"
            : FormatValue(node);

    private static string HumanizePath(string path)
    {
        var segment = path.Split('.').LastOrDefault() ?? path;
        return Regex.Replace(segment, "([a-z])([A-Z])", "$1 $2");
    }

    private static SessionBlockStateSnapshot CreateSnapshot(SessionBlockState state)
        => new(
            CreateUiSnapshot(state.Ui),
            state.ActivePage.ToString(),
            state.ActiveSessionId,
            state.PendingScrollSectionId,
            state.MaximizedBlockId,
            state.MaximizedCommandId,
            state.PageOverlayBlockId,
            state.PageOverlayCommandId,
            state.AppOverlayBlockId,
            state.AppOverlayCommandId,
            [.. state.OpenSessionNodes.Select(node => node.Id)],
            [.. state.PinnedNodeIds.OrderBy(id => id, StringComparer.Ordinal)],
            new Dictionary<string, long>(state.FeedSequenceByItemKey, StringComparer.Ordinal),
            new Dictionary<string, string>(state.BlockModesByKey, StringComparer.Ordinal),
            new Dictionary<string, string>(state.CommandModesByKey, StringComparer.Ordinal),
            state.NextFeedSequence,
            state.SessionCommandsBySessionId.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<SessionCommandSnapshot>)[.. pair.Value.Select(CreateCommandSnapshot)],
                StringComparer.Ordinal),
            state.SessionStatesBySessionId.ToDictionary(
                pair => pair.Key,
                pair => CreateUiSnapshot(pair.Value),
                StringComparer.Ordinal));

    private static SessionUiStateSnapshot CreateUiSnapshot(SessionUiState state)
        => new(
            state.ActiveAgentTypeKey,
            state.Input,
            state.SelectedNode?.Id,
            [.. state.OpenNodes.Select(node => node.Id)],
            state.ContextProjection?.Node.Id,
            [.. (state.ContextPins ?? []).Select(pin => new SessionContextPinSnapshot(pin.Id, pin.Node.Id, pin.Node.Label))],
            [.. (state.ContextBlocks ?? []).Select(block => new SessionContextBlockSnapshot(
                block.Id,
                block.Context.Id,
                block.Context.Label,
                block.Context.NodeTypeKey,
                block.Context.ComponentBlock,
                block.IncludeInContextWindow,
                block.Data?.ToString()))]);

    public static SessionCommandSnapshot CreateCommandSnapshot(SessionCommandSubmitted command)
        => new(command.Id, command.AgentTypeKey, command.Input, command.IsStreaming, command.Response);

    private static bool ShouldAppendEventLog(object action) => action switch
    {
        RestoreSessionUiState => false,
        ClearSessionEventLog => false,
        RestoreSessionCommandHistory => false,
        _ => true
    };

    private static IReadOnlyDictionary<TKey, TValue> SetDictionaryEntry<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source, TKey key, TValue value)
        where TKey : notnull
    {
        var next = source.ToDictionary(pair => pair.Key, pair => pair.Value);
        next[key] = value;
        return next;
    }

    private static IReadOnlyDictionary<TKey, TValue> RemoveDictionaryEntry<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source, TKey key)
        where TKey : notnull
    {
        if (!source.ContainsKey(key))
            return source;

        var next = source.ToDictionary(pair => pair.Key, pair => pair.Value);
        next.Remove(key);
        return next;
    }
}

public abstract record SessionUiEvent(SessionNodeInput Node) : ISessionEvent;

public sealed record SessionNodeSelected(SessionNodeInput Node) : SessionUiEvent(Node);

public sealed record SessionAgentTypeSelected(SessionNodeInput Node, string AgentTypeKey) : SessionUiEvent(Node);

public sealed record SessionCommandSubmitted(
    SessionNodeInput Node,
    string AgentTypeKey,
    string Input,
    string? Response = null,
    SessionCompletionSelection? Completion = null,
    bool IsControlEnter = false) : SessionUiEvent(Node)
{
    public string Id { get; init; } = $"session-command:{Guid.NewGuid():N}";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsStreaming { get; init; }
}

public sealed record SessionCardActionSelected(SessionNodeInput Node, string ActionKey) : SessionUiEvent(Node);

public sealed record SessionContextProjected(SessionNodeInput Node) : SessionUiEvent(Node);

public sealed record SessionContextPinned(SessionNodeInput Node, SessionContextPinModel Pin) : SessionUiEvent(Node);

public sealed record SessionContextUnpinned(SessionNodeInput Node, string PinId) : SessionUiEvent(Node);

public sealed record SessionContextBlockAdded(SessionNodeInput Node, SessionContextBlockModel Block) : SessionUiEvent(Node);

public sealed record SessionContextBlockRemoved(SessionNodeInput Node, string BlockId) : SessionUiEvent(Node);
