using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Breadcrumb;

/// <summary>
/// Breadcrumb component that provides navigation hierarchy.
/// Based on shadcn/ui Breadcrumb component with dark theme styling.
/// </summary>
public partial class Breadcrumb : ComponentBase
{
    /// <summary>
    /// The content to render inside the breadcrumb (typically BreadcrumbList).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the breadcrumb.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the breadcrumb element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "breadcrumb",
            Class
        );
    }
}
