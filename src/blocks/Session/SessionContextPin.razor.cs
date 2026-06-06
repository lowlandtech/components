namespace LowlandTech.Components;

public partial class SessionContextPin : TinyComponentBase
{
    [Parameter, EditorRequired] public SessionContextPinModel Pin { get; set; } = new(
        "pin:context",
        new("context", "context", "Context"),
        new("pin:context", "session.pin", "Context", "SessionContextPin"));

    [Parameter] public string RemoveTitle { get; set; } = "Unpin context";
    [Parameter] public EventCallback<SessionContextPinModel> OnRemove { get; set; }

    private Task RemoveAsync(MouseEventArgs _) => OnRemove.InvokeAsync(Pin);
}