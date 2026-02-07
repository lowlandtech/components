using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Breadcrumb;

/// <summary>
/// BreadcrumbLink component that provides clickable links in breadcrumb navigation.
/// Based on shadcn/ui Breadcrumb component with dark theme styling.
/// </summary>
public partial class BreadcrumbLink : ComponentBase
{
    /// <summary>
    /// The content to render inside the breadcrumb link.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The href attribute for the link.
    /// </summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the breadcrumb link.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the breadcrumb link element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "breadcrumb-link",
            Class
        );
    }
}
