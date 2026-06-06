namespace LowlandTech.Components;

public partial class SessionContextBlockOverlayContent : TinyComponentBase
{
    [Inject] private ISessionBlockDiscovery BlockDiscovery { get; set; } = default!;

    [Parameter, EditorRequired] public SessionContextBlockModel Block { get; set; } =
        new("context-block", new("context", "context", "Context"));

    [Parameter] public bool IsPinned { get; set; }
    [Parameter] public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    [Parameter] public EventCallback RestoreRequested { get; set; }

    private Type? ResolvedComponentType =>
        Block.ComponentType ?? BlockDiscovery.ResolveComponentType(Block.Context.ComponentBlock ?? Block.Context.NodeTypeKey);

    private Task RestoreAsync() => RestoreRequested.InvokeAsync();

    private Dictionary<string, object?> GetParameters()
    {
        var parameters = Parameters?.ToDictionary(pair => pair.Key, pair => pair.Value) ?? [];
        parameters.TryAdd("Block", Block);
        parameters.TryAdd("Context", Block.Context);
        parameters.TryAdd("Data", Block.Data);
        return parameters;
    }

    private string GetFallbackText() =>
        Block.Data is null
            ? "Context data is available to the session."
            : Block.Data.ToString() ?? "Context data is available to the session.";
}