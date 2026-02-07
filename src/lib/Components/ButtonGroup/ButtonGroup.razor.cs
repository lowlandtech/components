using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ButtonGroup;

/// <summary>
/// ButtonGroup component that groups related buttons together.
/// Based on shadcn/ui ButtonGroup component with dark theme styling.
/// </summary>
public partial class ButtonGroup : ComponentBase
{
    /// <summary>
    /// The content to render inside the button group.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The orientation of the button group.
    /// Defaults to Horizontal.
    /// </summary>
    [Parameter]
    public ButtonGroupOrientation Orientation { get; set; } = ButtonGroupOrientation.Horizontal;

    /// <summary>
    /// Optional CSS classes to apply to the button group.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the button group element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "button-group",
            Orientation == ButtonGroupOrientation.Vertical ? "button-group-vertical" : null,
            Class
        );
    }
}
