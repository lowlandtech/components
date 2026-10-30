namespace LowlandTech.Components;

// The Razor render tree feeds DynamicComponent a component type that the descriptor// registry discovered. The renderer assigns that parameter through reflection, so the// trimmer cannot verify the DynamicallyAccessedMembers requirement of the parameter;// the host that publishes trimmed preserves the discovered component types, and the// trimmed-host preservation probe executes this render path.
[UnconditionalSuppressMessage("Trimming", "IL2110", Justification = "The generated render tree assigns a component type that the descriptor registry discovered to DynamicComponent.Type, whose setter and backing field carry a DynamicallyAccessedMembers requirement that the reflection-based parameter assignment of the renderer cannot satisfy statically; the host that publishes trimmed preserves the discovered component types and their members, which the trimmed-host preservation probe checks.")]
[UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "The generated render tree assigns a component type that the descriptor registry discovered to DynamicComponent.Type, whose setter and backing field carry a DynamicallyAccessedMembers requirement that the reflection-based parameter assignment of the renderer cannot satisfy statically; the host that publishes trimmed preserves the discovered component types and their members, which the trimmed-host preservation probe checks.")]
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
