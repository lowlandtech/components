namespace LowlandTech.Components;

public partial class SessionContextExplorer : TinyComponentBase
{
    private const string SessionPinNodeType = "session.pin";
    private const string ProjectedContextBlockId = "projected-context-block";

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
    [Parameter] public EventCallback<SessionNodeInput> OnLoadChildren { get; set; }

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
        var baseClass = "flex w-full flex-col overflow-hidden rounded-lg border border-border bg-background";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private bool IsPinned(string nodeId) => Pins.Any(p => p.Node.Id == nodeId);

    private async Task ProjectAsync(SessionNodeInput node)
    {
        Projection = new SessionContextProjectionModel(node);
        await OnProjected.InvokeAsync(new SessionContextProjected(node));

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with { ContextProjection = Projection });
        }
    }

    private Task HandleNewAsync(SessionNodeInput node) => OnNodeNew.InvokeAsync(node);

    private Task HandleRenameAsync(SessionNodeInput node) => OnNodeRename.InvokeAsync(node);

    private Task HandleMoveUpAsync(SessionNodeInput node) => OnNodeMoveUp.InvokeAsync(node);

    private Task HandleMoveDownAsync(SessionNodeInput node) => OnNodeMoveDown.InvokeAsync(node);

    private Task HandleDeleteAsync(SessionNodeInput node) => OnNodeDelete.InvokeAsync(node);

    private Task HandlePinAsync(SessionNodeInput node) => PinAsync(new SessionContextProjectionModel(node));

    private async Task PinAsync(SessionContextProjectionModel projection)
    {
        var pin = CreatePin(projection);
        Pins = UpsertPin(Pins, pin);
        Projection = null;
        await OnPinned.InvokeAsync(new SessionContextPinned(projection.Node, pin));

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                ContextProjection = null,
                ContextPins = Pins,
                ContextBlocks = UpsertBlock(RemoveProjectedBlock(State.ContextBlocks ?? []), CreateBlock(projection.Node))
            });
        }
    }

    private async Task UnpinAsync(SessionContextPinModel pin)
    {
        Pins = Pins.Where(existing => existing.Id != pin.Id).ToList();
        await OnUnpinned.InvokeAsync(new SessionContextUnpinned(pin.Node, pin.Id));

        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(State with
            {
                ContextPins = Pins,
                ContextBlocks = RemoveBlocksForPin(State.ContextBlocks ?? [], State.ContextPins ?? [], pin.Id)
            });
        }
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

    private static IReadOnlyList<SessionContextBlockModel> RemoveProjectedBlock(IReadOnlyList<SessionContextBlockModel> blocks) =>
        blocks.Where(existing => existing.Id != ProjectedContextBlockId).ToList();

    private static IReadOnlyList<SessionContextBlockModel> RemoveBlocksForPin(
        IReadOnlyList<SessionContextBlockModel> blocks,
        IReadOnlyList<SessionContextPinModel> pins,
        string pinId)
    {
        var contextNodeId = pins.FirstOrDefault(existing => existing.Id == pinId)?.Node.Id;
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