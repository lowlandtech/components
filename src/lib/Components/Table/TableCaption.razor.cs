using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableCaption component for table captions.
/// Renders a semantic caption element.
/// </summary>
public partial class TableCaption : ComponentBase
{
    /// <summary>
    /// The content to render inside the table caption.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the caption element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the caption element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "table-caption";
        return CssClassUtils.Merge(baseClasses, Class);
    }
}
