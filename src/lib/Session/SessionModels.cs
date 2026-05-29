namespace LowlandTech.Components.Session;

public sealed record SessionNodeInput(
    string Id,
    string NodeTypeKey,
    string Label,
    string? ComponentBlock = null,
    IReadOnlyDictionary<string, object?>? Properties = null);

public sealed record SessionContextNode(
    SessionNodeInput Node,
    IReadOnlyList<SessionContextNode>? Children = null);

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
    Type? ComponentType = null,
    bool IncludeInContextWindow = true);

public interface IBlock
{
}

public interface ISessionContextBlock : IBlock
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

public abstract record SessionUiAction;

public sealed record SelectSessionNode(SessionNodeInput Node) : SessionUiAction;

public sealed record SetSessionInput(string Input) : SessionUiAction;

public sealed record SelectSessionAgentType(string AgentTypeKey) : SessionUiAction;

public sealed record SubmitSessionCommand(SessionNodeInput Node, string AgentTypeKey, string Input) : SessionUiAction;

public sealed record CloseSessionNode(string NodeId) : SessionUiAction;

public sealed record ProjectSessionContext(SessionNodeInput Node) : SessionUiAction;

public sealed record PinSessionContext(SessionContextProjectionModel Projection) : SessionUiAction;

public sealed record UnpinSessionContext(string PinId) : SessionUiAction;

public sealed record AddSessionContextBlock(SessionContextBlockModel Block) : SessionUiAction;

public sealed record RemoveSessionContextBlock(string BlockId) : SessionUiAction;

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
            node.Properties,
            ResolveComponentType(node.ComponentBlock));

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

    private static Type? ResolveComponentType(string? componentBlock)
    {
        if (string.IsNullOrWhiteSpace(componentBlock))
            return null;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(type => type is not null).Cast<Type>().ToArray();
            }
            catch
            {
                continue;
            }

            var match = types.FirstOrDefault(type =>
                !type.IsAbstract &&
                typeof(ComponentBase).IsAssignableFrom(type) &&
                string.Equals(type.Name, componentBlock, StringComparison.Ordinal));

            if (match is not null)
                return match;
        }

        return null;
    }
}

public abstract record SessionUiEvent(SessionNodeInput Node);

public sealed record SessionNodeSelected(SessionNodeInput Node) : SessionUiEvent(Node);

public sealed record SessionAgentTypeSelected(SessionNodeInput Node, string AgentTypeKey) : SessionUiEvent(Node);

public sealed record SessionCommandSubmitted(SessionNodeInput Node, string AgentTypeKey, string Input, string? Response = null) : SessionUiEvent(Node);

public sealed record SessionCardActionSelected(SessionNodeInput Node, string ActionKey) : SessionUiEvent(Node);

public sealed record SessionContextProjected(SessionNodeInput Node) : SessionUiEvent(Node);

public sealed record SessionContextPinned(SessionNodeInput Node, SessionContextPinModel Pin) : SessionUiEvent(Node);

public sealed record SessionContextUnpinned(SessionNodeInput Node, string PinId) : SessionUiEvent(Node);

public sealed record SessionContextBlockAdded(SessionNodeInput Node, SessionContextBlockModel Block) : SessionUiEvent(Node);

public sealed record SessionContextBlockRemoved(SessionNodeInput Node, string BlockId) : SessionUiEvent(Node);
