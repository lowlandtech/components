using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableRow component for table rows.
/// Renders a semantic tr element with optional selection state.
/// </summary>
public partial class TableRow : ComponentBase
{
    /// <summary>
    /// The content to render inside the table row.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the tr element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Whether the row is selected.
    /// Applies a visual highlight when true.
    /// </summary>
    [Parameter]
    public bool Selected { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tr element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "table-row";
        if (Selected)
        {
            baseClasses += " table-row-selected";
        }
        return CssClassUtils.Merge(baseClasses, Class);
    }
}
