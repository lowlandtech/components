using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Breadcrumb;

/// <summary>
/// BreadcrumbList component that contains breadcrumb items.
/// Based on shadcn/ui Breadcrumb component with dark theme styling.
/// </summary>
public partial class BreadcrumbList : ComponentBase
{
    /// <summary>
    /// The content to render inside the breadcrumb list (typically BreadcrumbItem components).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the breadcrumb list.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the breadcrumb list element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "breadcrumb-list",
            Class
        );
    }
}
