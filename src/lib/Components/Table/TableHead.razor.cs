using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableHead component for table header cells.
/// Renders a semantic th element with optional text alignment.
/// </summary>
public partial class TableHead : ComponentBase
{
    /// <summary>
    /// The content to render inside the table header cell.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the th element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Text alignment for the header cell content.
    /// Options: "left", "center", "right".
    /// </summary>
    [Parameter]
    public string? Align { get; set; }

    /// <summary>
    /// Additional attributes to apply to the th element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "table-head";

        // Alignment handling
        if (!string.IsNullOrEmpty(Align))
        {
            baseClasses += $" table-head-align-{Align.ToLower()}";
        }

        return CssClassUtils.Merge(baseClasses, Class);
    }
}
