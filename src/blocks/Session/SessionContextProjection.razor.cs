namespace LowlandTech.Components;

public partial class SessionContextProjection : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    [Parameter] public SessionContextProjectionModel? Projection { get; set; }
    [Parameter] public string PinTitle { get; set; } = "Pin context";

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(PinTitle))
        {
            PinTitle = L("PinContext");
        }
    }
    [Parameter] public EventCallback<SessionContextProjectionModel> OnPin { get; set; }

    private string GetClass()
    {
        var baseClass = "flex w-full items-center justify-between gap-3 border-b border-border bg-primary/5 px-3 py-2";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private async Task PinAsync(MouseEventArgs _)
    {
        if (Projection is not null)
            await OnPin.InvokeAsync(Projection);
    }
}