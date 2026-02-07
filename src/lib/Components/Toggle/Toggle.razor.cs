using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Toggle;

/// <summary>
/// Toggle component for toggling states.
/// Based on shadcn/ui Toggle component.
/// </summary>
public partial class Toggle
{
    /// <summary>
    /// Whether the toggle is pressed.
    /// </summary>
    [Parameter]
    public bool Pressed { get; set; }

    /// <summary>
    /// Event callback for when the pressed state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>
    /// Whether the toggle is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Child content to render inside the toggle.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to forward to the button element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "toggle",
            Pressed ? "toggle-pressed" : "",
            Disabled ? "toggle-disabled" : "",
            Class
        );
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (Disabled) return;
        var newValue = !Pressed;
        Pressed = newValue;
        await ValueChanged.InvokeAsync(newValue);
    }
}
