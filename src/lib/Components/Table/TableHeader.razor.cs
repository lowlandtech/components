using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Table;

/// <summary>
/// TableHeader component for table header section.
/// Renders a semantic thead element.
/// </summary>
public partial class TableHeader : ComponentBase
{
    /// <summary>
    /// The content to render inside the table header.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the thead element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the thead element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string? GetClasses()
    {
        return CssClassUtils.Merge(Class);
    }
}
