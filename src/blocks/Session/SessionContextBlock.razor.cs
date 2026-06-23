namespace LowlandTech.Components;

public partial class SessionContextBlock : TinyComponentBase
{
    [Inject] private IPageOverlayService PageOverlay { get; set; } = default!;
    [Inject] private ISessionBlockDiscovery BlockDiscovery { get; set; } = default!;
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    public const string ModeMinimized = "minimized";
    public const string ModeNormal = "normal";
    public const string ModeMaximized = "maximized";
    public const string ModeOverlayPage = "overlay-page";
    public const string ModeOverlayApp = "overlay-app";

    [Parameter, EditorRequired] public SessionContextBlockModel Block { get; set; } =
        new("context-block", new("context", "context", "Context"));

    [Parameter] public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    [Parameter] public string Mode { get; set; } = ModeNormal;
    [Parameter] public bool IsPinned { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnUnpin { get; set; }
    [Parameter] public EventCallback<string> ModeChanged { get; set; }
    [Parameter] public EventCallback<BlockIntent> OnIntentSelected { get; set; }
    [Parameter] public string? PageOverlayContainerClass { get; set; }
    [Parameter] public string? AppOverlayContainerClass { get; set; }
    [Parameter] public bool HostHandlesPageOverlay { get; set; }
    [Parameter] public bool IsPageOverlayProjection { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    private string NormalizedMode => Mode?.ToLowerInvariant() switch
    {
        ModeMinimized => ModeMinimized,
        ModeMaximized => ModeMaximized,
        ModeOverlayPage => ModeOverlayPage,
        ModeOverlayApp => ModeOverlayApp,
        _ => ModeNormal
    };

    private bool ShowsContent =>
        NormalizedMode != ModeMinimized &&
        (NormalizedMode != ModeOverlayPage || IsPageOverlayProjection);

    private bool IsSquareMode =>
        NormalizedMode is ModeMaximized or ModeOverlayApp;

    private string GetHeaderClass() => NormalizedMode switch
    {
        ModeMaximized or ModeOverlayApp => "border-b border-border border-x-transparent border-t-transparent bg-muted/30 px-3 py-2",
        _ => IsPageOverlayProjection
            ? "h-10 shrink-0 gap-2 border-b border-border bg-background px-3 py-0"
            : "border-b border-border bg-muted/30 px-3 py-2"
    };

    private string? GetHeaderStyle() => IsPageOverlayProjection
        ? "grid-template-rows:1fr;"
        : null;

    private string GetClass()
    {
        var baseClass = NormalizedMode switch
        {
            ModeMaximized => "cn-card-maximized flex h-full min-h-0 w-full flex-col overflow-hidden bg-background gap-0 py-0 !rounded-none",
            ModeOverlayPage when IsPageOverlayProjection => "flex min-h-full w-full flex-col overflow-hidden bg-background gap-0 py-0 !rounded-none",
            ModeOverlayPage => "w-full overflow-hidden bg-background gap-0 py-0 opacity-60",
            ModeOverlayApp when !string.IsNullOrWhiteSpace(AppOverlayContainerClass) => $"{AppOverlayContainerClass} !rounded-none",
            ModeOverlayApp => "fixed inset-0 z-50 flex h-dvh w-dvw flex-col overflow-hidden bg-background gap-0 py-0 !rounded-none",
            _ => "w-full overflow-hidden bg-background gap-0 py-0"
        };

        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private Type? ResolvedComponentType =>
        Block.ComponentType ?? BlockDiscovery.ResolveComponentType(Block.Context.ComponentBlock ?? Block.Context.NodeTypeKey);

    private string GetContentClass()
    {
        var baseClass = NormalizedMode switch
        {
            ModeMaximized => "min-h-0 flex-1 overflow-auto p-3 text-sm",
            ModeOverlayPage => "min-h-0 flex-1 overflow-auto p-3 text-sm",
            ModeOverlayApp when !string.IsNullOrWhiteSpace(AppOverlayContainerClass) => "min-h-0 flex-1 overflow-auto p-3 text-sm",
            ModeOverlayApp => "h-[calc(100dvh-3rem)] overflow-auto p-3 text-sm",
            _ => "p-3 text-sm"
        };

        return baseClass;
    }

    private Dictionary<string, object?> GetParameters()
    {
        var parameters = Parameters?.ToDictionary(pair => pair.Key, pair => pair.Value) ?? [];
        parameters.TryAdd("Block", Block);
        parameters.TryAdd("Context", Block.Context);
        parameters.TryAdd("Data", Block.Data);
        if (ResolvedComponentType?.GetProperty("Mode")?.GetCustomAttribute<ParameterAttribute>() is not null)
            parameters.TryAdd("Mode", NormalizedMode);
        if (ResolvedComponentType?.GetProperty("OnIntentSelected")?.GetCustomAttribute<ParameterAttribute>() is not null)
            parameters.TryAdd("OnIntentSelected", OnIntentSelected);
        return parameters;
    }

    private string GetFallbackText() =>
        Block.Data is null
            ? "Context data is available to the session."
            : Block.Data.ToString() ?? "Context data is available to the session.";

    private Task UnpinAsync(MouseEventArgs _) => OnUnpin.InvokeAsync(Block.Context);

    private async Task OnModeChangedAsync(string mode)
    {
        if (mode == ModeOverlayPage)
        {
            if (!HostHandlesPageOverlay)
            {
                PageOverlay.Set(BuildOverlayFragment(), Block.Context.Label, PageOverlayContainerClass);
            }
        }
        else if (NormalizedMode == ModeOverlayPage)
        {
            PageOverlay.Clear();
        }

        Mode = mode;
        await ModeChanged.InvokeAsync(NormalizedMode);
        StateHasChanged();
    }

    private RenderFragment BuildOverlayFragment() => builder =>
    {
        builder.OpenComponent<SessionContextBlockOverlayContent>(0);
        builder.AddAttribute(1, nameof(SessionContextBlockOverlayContent.Block), Block);
        builder.AddAttribute(2, nameof(SessionContextBlockOverlayContent.IsPinned), IsPinned);
        builder.AddAttribute(3, nameof(SessionContextBlockOverlayContent.Parameters), Parameters);
        builder.AddAttribute(4, nameof(SessionContextBlockOverlayContent.ChildContent), ChildContent);
        builder.AddAttribute(5, nameof(SessionContextBlockOverlayContent.RestoreRequested), EventCallback.Factory.Create(this, () => OnModeChangedAsync(ModeNormal)));
        builder.AddAttribute(6, nameof(SessionContextBlockOverlayContent.OnIntentSelected), OnIntentSelected);
        builder.CloseComponent();
    };
}
