namespace LowlandTech.Components.Session;

public partial class SessionContextExplorer : TinyComponentBase
{
    [Parameter] public SessionUiState State { get; set; } = SessionUiDefaults.Empty;
    [Parameter] public EventCallback<SessionUiState> StateChanged { get; set; }
    [Parameter] public IReadOnlyList<SessionContextNode> RootNodes { get; set; } = [];
    [Parameter] public SessionContextProjectionModel? Projection { get; set; }
    [Parameter] public IReadOnlyList<SessionContextPinModel> Pins { get; set; } = [];
    [Parameter] public string Title { get; set; } = "Context";
    [Parameter] public string EmptyTitle { get; set; } = "No context nodes";
    [Parameter] public string EmptyDescription { get; set; } = "Context nodes appear here when application entities are available.";
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public EventCallback<SessionContextProjected> OnProjected { get; set; }
    [Parameter] public EventCallback<SessionContextPinned> OnPinned { get; set; }
    [Parameter] public EventCallback<SessionContextUnpinned> OnUnpinned { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeNew { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeRename { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveUp { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveDown { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeDelete { get; set; }

    private IReadOnlyList<SessionContextNode> VisibleRoots =>
        [.. RootNodes.Where(node => !IsPinned(node.Node.Id))];

    protected override void OnParametersSet()
    {
        Projection ??= State.ContextProjection;
        if (Pins.Count == 0 && State.ContextPins is { Count: > 0 })
            Pins = State.ContextPins;
    }

    private string GetClass()
    {
        var baseClass = "w-full overflow-hidden rounded-lg border border-border bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private bool IsPinned(string nodeId) => Pins.Any(p => p.Node.Id == nodeId);

    private async Task ProjectAsync(SessionNodeInput node)
    {
        State = SessionUiReducer.Reduce(State, new ProjectSessionContext(node));
        Projection = State.ContextProjection;
        await OnProjected.InvokeAsync(new SessionContextProjected(node));
        await StateChanged.InvokeAsync(State);
    }

    private Task HandleNewAsync(SessionNodeInput node) => OnNodeNew.InvokeAsync(node);

    private Task HandleRenameAsync(SessionNodeInput node) => OnNodeRename.InvokeAsync(node);

    private Task HandleMoveUpAsync(SessionNodeInput node) => OnNodeMoveUp.InvokeAsync(node);

    private Task HandleMoveDownAsync(SessionNodeInput node) => OnNodeMoveDown.InvokeAsync(node);

    private Task HandleDeleteAsync(SessionNodeInput node) => OnNodeDelete.InvokeAsync(node);

    private Task HandlePinAsync(SessionNodeInput node) => PinAsync(new SessionContextProjectionModel(node));

    private async Task PinAsync(SessionContextProjectionModel projection)
    {
        State = SessionUiReducer.Reduce(State, new PinSessionContext(projection));
        Projection = State.ContextProjection;
        Pins = State.ContextPins ?? [];
        var pin = Pins.First(candidate => candidate.Node.Id == projection.Node.Id);
        await OnPinned.InvokeAsync(new SessionContextPinned(projection.Node, pin));
        await StateChanged.InvokeAsync(State);
    }

    private async Task UnpinAsync(SessionContextPinModel pin)
    {
        State = SessionUiReducer.Reduce(State, new UnpinSessionContext(pin.Id));
        Pins = State.ContextPins ?? [];
        await OnUnpinned.InvokeAsync(new SessionContextUnpinned(pin.Node, pin.Id));
        await StateChanged.InvokeAsync(State);
    }
}