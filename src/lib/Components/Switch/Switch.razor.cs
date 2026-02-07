using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Switch;

/// <summary>
/// Switch component for toggling binary states.
/// Based on shadcn/ui Switch component.
/// </summary>
public partial class Switch
{
    /// <summary>
    /// Whether the switch is checked.
    /// </summary>
    [Parameter]
    public bool Checked { get; set; }

    /// <summary>
    /// Event callback for when the checked state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// Whether the switch is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to forward to the wrapper element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("switch-wrapper", Class);
    }

    private string GetButtonClasses()
    {
        return CssClassUtils.Merge(
            "switch",
            Checked ? "switch-checked" : "",
            Disabled ? "switch-disabled" : ""
        );
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (Disabled) return;
        var newValue = !Checked;
        Checked = newValue;
        await CheckedChanged.InvokeAsync(newValue);
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (Disabled) return;
        var newValue = !Checked;
        Checked = newValue;
        await CheckedChanged.InvokeAsync(newValue);
    }
}
