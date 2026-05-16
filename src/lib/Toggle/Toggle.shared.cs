namespace LowlandTech.Components.Toggle;

public partial class Toggle
{
    [Parameter] public bool Pressed { get; set; }
    [Parameter] public EventCallback<bool> PressedChanged { get; set; }

    private string VariantValue => string.IsNullOrWhiteSpace(variant) ? "default" : variant!;
    private string SizeValue => string.IsNullOrWhiteSpace(size) ? "default" : size!;

    private const string BaseClass =
        "cn-toggle group/toggle inline-flex items-center justify-center whitespace-nowrap outline-none " +
        "hover:bg-muted focus-visible:ring-[3px] disabled:pointer-events-none disabled:opacity-50 " +
        "[&_svg]:pointer-events-none [&_svg]:shrink-0";

    private string CssClass => string.IsNullOrEmpty(Class) ? BaseClass : $"{BaseClass} {Class}";

    private async Task ToggleAsync()
    {
        Pressed = !Pressed;
        await PressedChanged.InvokeAsync(Pressed);
        StateHasChanged();
    }
}
