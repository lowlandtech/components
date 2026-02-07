using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Pagination;

/// <summary>
/// Pagination component that provides navigation for paginated content.
/// Based on shadcn/ui Pagination component with dark theme styling.
/// </summary>
public partial class Pagination : ComponentBase
{
    /// <summary>
    /// The content to render inside the pagination.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the pagination.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the pagination element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "pagination",
            Class
        );
    }
}
