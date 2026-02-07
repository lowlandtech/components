using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ToggleGroup;

/// <summary>
/// ToggleGroupItem component representing an individual toggle within a ToggleGroup.
/// Based on shadcn/ui ToggleGroup component.
/// </summary>
public partial class ToggleGroupItem : ComponentBase
{
    /// <summary>
    /// The value of this toggle item.
    /// </summary>
    [Parameter, EditorRequired]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Whether this item is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Child content to render inside the toggle item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to forward to the button element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The parent ToggleGroup component.
    /// </summary>
    [CascadingParameter]
    public ToggleGroup? Group { get; set; }

    private bool IsPressed => Group?.IsSelected(Value) == true;

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "toggle-group-item",
            IsPressed ? "toggle-pressed" : "",
            (Disabled || Group?.Disabled == true) ? "toggle-disabled" : "",
            Class
        );
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (Disabled || Group?.Disabled == true) return;
        if (Group != null)
        {
            await Group.ToggleItem(Value);
        }
    }
}
