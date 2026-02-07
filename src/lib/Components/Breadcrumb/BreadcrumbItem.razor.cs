using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Breadcrumb;

/// <summary>
/// BreadcrumbItem component that wraps individual breadcrumb entries.
/// Based on shadcn/ui Breadcrumb component with dark theme styling.
/// </summary>
public partial class BreadcrumbItem : ComponentBase
{
    /// <summary>
    /// The content to render inside the breadcrumb item (typically BreadcrumbLink and BreadcrumbSeparator).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the breadcrumb item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the breadcrumb item element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "breadcrumb-item",
            Class
        );
    }
}
