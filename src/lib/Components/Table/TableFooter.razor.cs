using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableFooter component for table footer section.
/// Renders a semantic tfoot element.
/// </summary>
public partial class TableFooter : ComponentBase
{
    /// <summary>
    /// The content to render inside the table footer.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the tfoot element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tfoot element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string? GetClasses()
    {
        return CssClassUtils.Merge(Class);
    }
}
