using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ButtonGroup;

/// <summary>
/// ButtonGroupSeparator component that provides a visual separator within a ButtonGroup.
/// </summary>
public partial class ButtonGroupSeparator : ComponentBase
{
    /// <summary>
    /// Optional CSS classes to apply to the button group separator.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the button group separator element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "button-group-separator",
            Class
        );
    }
}
