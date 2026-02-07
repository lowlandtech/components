using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// Table component for displaying tabular data with semantic HTML structure.
/// Based on shadcn/ui Table component with dark theme styling.
/// </summary>
public partial class Table : ComponentBase
{
    /// <summary>
    /// The content to render inside the table.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the table element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the table element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetWrapperClasses()
    {
        return "table-wrapper";
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "table",
            Class
        );
    }
}
