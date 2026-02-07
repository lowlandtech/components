using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableCell component for table data cells.
/// Renders a semantic td element with optional alignment.
/// </summary>
public partial class TableCell : ComponentBase
{
    /// <summary>
    /// The content to render inside the table cell.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the td element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Horizontal text alignment for the cell content.
    /// Options: "left", "center", "right".
    /// </summary>
    [Parameter]
    public string? Align { get; set; }

    /// <summary>
    /// Align content to the top of the cell.
    /// </summary>
    [Parameter]
    public bool AlignTop { get; set; }

    /// <summary>
    /// Align content to the middle of the cell.
    /// </summary>
    [Parameter]
    public bool AlignMiddle { get; set; }

    /// <summary>
    /// Align content to the bottom of the cell.
    /// </summary>
    [Parameter]
    public bool AlignBottom { get; set; }

    /// <summary>
    /// Additional attributes to apply to the td element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "table-cell";

        // Alignment handling
        if (!string.IsNullOrEmpty(Align))
        {
            baseClasses += $" table-cell-align-{Align.ToLower()}";
        }
        else
        {
            if (AlignTop) baseClasses += " table-cell-align-top";
            if (AlignMiddle) baseClasses += " table-cell-align-middle";
            if (AlignBottom) baseClasses += " table-cell-align-bottom";
        }

        return CssClassUtils.Merge(baseClasses, Class);
    }
}
