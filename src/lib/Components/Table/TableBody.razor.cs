using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableBody component for table body section.
/// Renders a semantic tbody element.
/// </summary>
public partial class TableBody : ComponentBase
{
    /// <summary>
    /// The content to render inside the table body.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the tbody element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tbody element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string? GetClasses()
    {
        return CssClassUtils.Merge(Class);
    }
}
