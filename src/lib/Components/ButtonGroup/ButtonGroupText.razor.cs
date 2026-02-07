using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ButtonGroup;

/// <summary>
/// ButtonGroupText component that provides text content within a ButtonGroup.
/// </summary>
public partial class ButtonGroupText : ComponentBase
{
    /// <summary>
    /// The content to render inside the button group text.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the button group text.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the button group text element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "button-group-text",
            Class
        );
    }
}
