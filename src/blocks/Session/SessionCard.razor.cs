namespace LowlandTech.Components;

public partial class SessionCard : TinyComponentBase
{
    [Inject] private IPageOverlayService PageOverlay { get; set; } = default!;
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    [Parameter, EditorRequired] public SessionNodeInput Node { get; set; } = new("session-card", "session-card", "Session card");
    [Parameter] public string Initials { get; set; } = "AI";
    [Parameter] public string? Description { get; set; }
    [Parameter] public string? Status { get; set; }
    [Parameter] public string StatusVariant { get; set; } = "outline";
    [Parameter] public string? Mode { get; set; }
    [Parameter] public EventCallback<string> ModeChanged { get; set; }
    [Parameter] public IReadOnlyList<SessionCardAction> Actions { get; set; } = [];
    [Parameter] public EventCallback<SessionCardActionSelected> OnActionSelected { get; set; }
    [Parameter] public string? PageOverlayContainerClass { get; set; }
    [Parameter] public string? AppOverlayContainerClass { get; set; }
    [Parameter] public bool HostHandlesPageOverlay { get; set; }
    [Parameter] public bool IsPageOverlayProjection { get; set; }

    private const string ModeNormal = "normal";
    private const string ModeMinimized = "minimized";
    private const string ModeMaximized = "maximized";
    private const string ModeOverlayPage = "overlay-page";
    private const string ModeOverlayApp = "overlay-app";

    private string _mode = ModeNormal;

    private string CardMode =>
        EffectiveMode == ModeOverlayApp && !string.IsNullOrWhiteSpace(AppOverlayContainerClass)
            ? ModeNormal
            : EffectiveMode;

    private string EffectiveMode => (Mode ?? _mode)?.ToLowerInvariant() switch
    {
        ModeMinimized => ModeMinimized,
        ModeMaximized => ModeMaximized,
        ModeOverlayPage => ModeOverlayPage,
        ModeOverlayApp => ModeOverlayApp,
        _ => ModeNormal
    };

    private string L(string key) => Localizer?[key] ?? key;

    private string GetHeaderClass() => IsPageOverlayProjection
        ? "h-10 shrink-0 gap-2 border-b border-border bg-background px-3 py-0"
        : "border-b border-border bg-muted/30 px-3 py-2";

    private string GetClass()
    {
        var baseClass = EffectiveMode switch
        {
            ModeMaximized => "flex min-h-full w-full flex-col gap-0 overflow-hidden py-0",
            ModeOverlayPage when IsPageOverlayProjection => "flex min-h-full w-full flex-col gap-0 overflow-hidden py-0",
            ModeOverlayPage => "w-full gap-0 overflow-hidden py-0 opacity-60",
            ModeOverlayApp when !string.IsNullOrWhiteSpace(AppOverlayContainerClass) => AppOverlayContainerClass,
            _ => "w-full gap-0 overflow-hidden py-0"
        };
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private async Task OnModeChangedAsync(string mode)
    {
        if (mode == ModeOverlayPage)
        {
            if (!HostHandlesPageOverlay)
            {
                PageOverlay.Set(BuildOverlayFragment(), Node.Label, PageOverlayContainerClass);
            }
        }
        else if (EffectiveMode == ModeOverlayPage)
        {
            PageOverlay.Clear();
        }

        Mode = mode;
        _mode = mode;
        await ModeChanged.InvokeAsync(EffectiveMode);
        StateHasChanged();
    }

    private RenderFragment BuildOverlayFragment() => builder =>
    {
        builder.OpenComponent<SessionCardOverlayContent>(0);
        builder.AddAttribute(1, nameof(SessionCardOverlayContent.Node), Node);
        builder.AddAttribute(2, nameof(SessionCardOverlayContent.Initials), Initials);
        builder.AddAttribute(3, nameof(SessionCardOverlayContent.Status), Status);
        builder.AddAttribute(4, nameof(SessionCardOverlayContent.StatusVariant), StatusVariant);
        builder.AddAttribute(5, nameof(SessionCardOverlayContent.Description), Description);
        builder.AddAttribute(6, nameof(SessionCardOverlayContent.Actions), Actions);
        builder.AddAttribute(7, nameof(SessionCardOverlayContent.ChildContent), ChildContent);
        builder.AddAttribute(8, nameof(SessionCardOverlayContent.RestoreRequested), EventCallback.Factory.Create(this, () => OnModeChangedAsync(ModeNormal)));
        builder.AddAttribute(9, nameof(SessionCardOverlayContent.ActionSelected), EventCallback.Factory.Create<SessionCardAction>(this, SelectActionAsync));
        builder.CloseComponent();
    };

    private async Task SelectActionAsync(SessionCardAction action)
    {
        await action.OnSelected.InvokeAsync(Node);
        await OnActionSelected.InvokeAsync(new SessionCardActionSelected(Node, action.Key));
    }
}
