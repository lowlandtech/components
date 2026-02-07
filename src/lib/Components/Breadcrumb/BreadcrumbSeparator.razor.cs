using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Breadcrumb;

/// <summary>
/// BreadcrumbSeparator component that displays a separator between breadcrumb items.
/// Based on shadcn/ui Breadcrumb component with dark theme styling.
/// </summary>
public partial class BreadcrumbSeparator : ComponentBase
{
    /// <summary>
    /// The custom separator content to render. Defaults to "/" if not provided.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the breadcrumb separator.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the breadcrumb separator element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "breadcrumb-separator",
            Class
        );
    }
}
