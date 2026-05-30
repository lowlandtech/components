namespace LowlandTech.Components.Session;

public partial class SessionList : TinyComponentBase
{
    [Parameter] public SessionUiState State { get; set; } = SessionUiDefaults.Empty;
    [Parameter] public EventCallback<SessionUiState> StateChanged { get; set; }
    [Parameter] public IReadOnlyList<SessionListEntry> Items { get; set; } = [];
    [Parameter] public SessionNodeInput? SelectedNode { get; set; }
    [Parameter] public string Title { get; set; } = "Sessions";
    [Parameter] public string CreateTitle { get; set; } = "New session";
    [Parameter] public string EmptyTitle { get; set; } = "No sessions yet";
    [Parameter] public string EmptyDescription { get; set; } = "Start a session to collect work, messages, and generated blocks.";
    [Parameter] public string PageLabel { get; set; } = "Page 1 / 1";
    [Parameter] public string RowsLabel { get; set; } = "Rows 10";
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool ShowFooter { get; set; } = true;
    [Parameter] public EventCallback<SessionNodeSelected> OnSelected { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnCreateRequested { get; set; }

    protected override void OnParametersSet()
    {
        SelectedNode ??= State.SelectedNode;
    }

    private string GetClass()
    {
        var baseClass = "w-full overflow-hidden rounded-lg border border-border bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private static SessionNodeInput ToNode(SessionListEntry entry) =>
        new(entry.Id, entry.NodeTypeKey, entry.Label);

    private async Task SelectAsync(SessionNodeInput node)
    {
        SelectedNode = node;
        State = SessionUiReducer.Reduce(State, new SelectSessionNode(node));
        await OnSelected.InvokeAsync(new SessionNodeSelected(node));
        await StateChanged.InvokeAsync(State);
    }

    private async Task CreateAsync()
    {
        var node = new SessionNodeInput("new-session", "session", Title);
        await OnCreateRequested.InvokeAsync(node);
    }
}