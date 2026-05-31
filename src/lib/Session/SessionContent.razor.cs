namespace LowlandTech.Components.Session;

public partial class SessionContent : TinyComponentBase
{
    [Parameter] public SessionUiState State { get; set; } = SessionUiDefaults.Empty;
    [Parameter] public EventCallback<SessionUiState> StateChanged { get; set; }
    [Parameter] public IReadOnlyList<SessionNodeInput> OpenNodes { get; set; } = [];
    [Parameter] public SessionNodeInput? SelectedNode { get; set; }
    [Parameter] public string EmptyTitle { get; set; } = "Select a session";
    [Parameter] public string EmptyDescription { get; set; } = "SessionContent stays idle until a session tab is active.";
    [Parameter] public bool CloseableTabs { get; set; } = true;
    [Parameter] public bool PinnableTabs { get; set; }
    [Parameter] public bool RenameableTabs { get; set; }
    [Parameter] public bool DraggableTabs { get; set; }
    [Parameter] public IReadOnlySet<string>? PinnedTabIds { get; set; }
    [Parameter] public RenderFragment? HeaderLeading { get; set; }
    [Parameter] public EventCallback<SessionNodeSelected> OnSelected { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnClosed { get; set; }
    [Parameter] public EventCallback<TabsItem> OnTabPinnedChanged { get; set; }

    private IReadOnlyList<TabsItem> TabItems =>
        [.. OpenNodes.Select(node => new TabsItem
        {
            Value = node.Id,
            Title = node.Label,
            Closeable = CloseableTabs && !(PinnedTabIds?.Contains(node.Id) ?? false),
            Pinnable = PinnableTabs,
            Pinned = PinnedTabIds?.Contains(node.Id) ?? false,
            Renameable = RenameableTabs,
            Draggable = DraggableTabs
        })];

    protected override void OnParametersSet()
    {
        if (OpenNodes.Count == 0)
            OpenNodes = State.OpenNodes;

        SelectedNode ??= State.SelectedNode ?? OpenNodes.FirstOrDefault();
    }

    private string GetClass()
    {
        var baseClass = "flex w-full flex-col overflow-hidden rounded-lg border border-border bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private async Task SelectAsync(SessionNodeInput node)
    {
        SelectedNode = node;
        await OnSelected.InvokeAsync(new SessionNodeSelected(node));

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                SelectedNode = node,
                OpenNodes = UpsertOpenNode(OpenNodes.Count == 0 ? State.OpenNodes : OpenNodes, node)
            });
        }
    }

    private Task SelectByValueAsync(string? nodeId)
    {
        var node = OpenNodes.FirstOrDefault(candidate => candidate.Id == nodeId);
        return node is null ? Task.CompletedTask : SelectAsync(node);
    }

    private async Task CloseTabAsync(TabsItem item)
    {
        var currentNodes = OpenNodes.Count == 0 ? State.OpenNodes : OpenNodes;
        var nextOpenNodes = currentNodes.Where(node => node.Id != item.Value).ToList();
        var nextSelected = SelectedNode?.Id == item.Value
            ? nextOpenNodes.FirstOrDefault()
            : SelectedNode;
        var closedNode = currentNodes.FirstOrDefault(node => node.Id == item.Value);

        OpenNodes = nextOpenNodes;
        SelectedNode = nextSelected;

        if (closedNode is not null)
            await OnClosed.InvokeAsync(closedNode);

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                OpenNodes = nextOpenNodes,
                SelectedNode = nextSelected
            });
        }
    }

    private Task OnTabPinnedChangedAsync(TabsItem item)
    {
        return OnTabPinnedChanged.InvokeAsync(item);
    }

    private static IReadOnlyList<SessionNodeInput> UpsertOpenNode(IReadOnlyList<SessionNodeInput> openNodes, SessionNodeInput node)
    {
        if (openNodes.Any(existing => existing.Id == node.Id))
            return openNodes;

        var next = openNodes.ToList();
        next.Add(node);
        return next;
    }
}