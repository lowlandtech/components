namespace LowlandTech.Components.Session;

public partial class SessionCardOverlayContent : TinyComponentBase
{
    [Parameter, EditorRequired] public SessionNodeInput Node { get; set; } = new("session-card", "session-card", "Session card");
    [Parameter] public string Initials { get; set; } = "AI";
    [Parameter] public string? Description { get; set; }
    [Parameter] public string? Status { get; set; }
    [Parameter] public string StatusVariant { get; set; } = "outline";
    [Parameter] public IReadOnlyList<SessionCardAction> Actions { get; set; } = [];
    [Parameter] public EventCallback RestoreRequested { get; set; }
    [Parameter] public EventCallback<SessionCardAction> ActionSelected { get; set; }

    private Task RestoreAsync() => RestoreRequested.InvokeAsync();

    private Task SelectActionAsync(SessionCardAction action) => ActionSelected.InvokeAsync(action);
}