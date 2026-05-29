namespace LowlandTech.Components.Session;

public sealed record SessionNodeInput(
    string Id,
    string NodeTypeKey,
    string Label,
    IReadOnlyDictionary<string, object?>? Properties = null);

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

public sealed record SessionUiState(
    string ActiveAgentTypeKey,
    string Input,
    SessionNodeInput? SelectedNode,
    IReadOnlyList<SessionNodeInput> OpenNodes);

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

public static class SessionUiReducer
{
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
        _ => state
    };

    private static IReadOnlyList<SessionNodeInput> Upsert(IReadOnlyList<SessionNodeInput> nodes, SessionNodeInput node)
    {
        var next = nodes.Where(existing => existing.Id != node.Id).ToList();
        next.Add(node);
        return next;
    }
}

public abstract record SessionUiEvent(SessionNodeInput Node);

public sealed record SessionNodeSelected(SessionNodeInput Node) : SessionUiEvent(Node);

public sealed record SessionAgentTypeSelected(SessionNodeInput Node, string AgentTypeKey) : SessionUiEvent(Node);

public sealed record SessionCommandSubmitted(SessionNodeInput Node, string AgentTypeKey, string Input) : SessionUiEvent(Node);

public sealed record SessionCardActionSelected(SessionNodeInput Node, string ActionKey) : SessionUiEvent(Node);
