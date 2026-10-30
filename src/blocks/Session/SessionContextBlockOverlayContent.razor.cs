namespace LowlandTech.Components;

public partial class SessionContextBlockOverlayContent : TinyComponentBase
{
    [Inject] private ISessionBlockDiscovery BlockDiscovery { get; set; } = default!;
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    [Parameter, EditorRequired] public SessionContextBlockModel Block { get; set; } =
        new("context-block", new("context", "context", "Context"));

    [Parameter] public bool IsPinned { get; set; }
    [Parameter] public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    [Parameter] public EventCallback RestoreRequested { get; set; }
    [Parameter] public EventCallback<BlockIntent> OnIntentSelected { get; set; }

    private Type? ResolvedComponentType =>
        Block.ComponentType ?? BlockDiscovery.ResolveComponentType(Block.Context.ComponentBlock ?? Block.Context.NodeTypeKey);

    private Task RestoreAsync() => RestoreRequested.InvokeAsync();

    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "The entity or component is supplied by the host and a DynamicallyAccessedMembers requirement cannot flow from object.GetType(); the host preserves the reflected members, which the trimmed-host preservation probe checks.")]
    private Dictionary<string, object?> GetParameters()
    {
        var parameters = Parameters?.ToDictionary(pair => pair.Key, pair => pair.Value) ?? [];
        parameters.TryAdd("Block", Block);
        parameters.TryAdd("Context", Block.Context);
        parameters.TryAdd("Data", Block.Data);
        if (ResolvedComponentType?.GetProperty("OnIntentSelected")?.GetCustomAttribute<ParameterAttribute>() is not null)
            parameters.TryAdd("OnIntentSelected", OnIntentSelected);
        return parameters;
    }

    private string GetFallbackText() =>
        Block.Data is null
            ? "Context data is available to the session."
            : Block.Data.ToString() ?? "Context data is available to the session.";
}
