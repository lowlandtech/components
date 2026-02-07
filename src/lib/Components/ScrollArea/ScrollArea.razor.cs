using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ScrollArea;

/// <summary>
/// ScrollArea component that provides a scrollable container with custom scrollbars.
/// Based on shadcn/ui ScrollArea component with dark theme styling.
/// </summary>
public partial class ScrollArea : ComponentBase
{
    /// <summary>
    /// The content to render inside the scroll area.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the scroll area container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Whether to show the horizontal scrollbar.
    /// </summary>
    [Parameter]
    public bool ShowScrollBarHorizontal { get; set; }

    /// <summary>
    /// Whether to show the vertical scrollbar.
    /// </summary>
    [Parameter]
    public bool ShowScrollBarVertical { get; set; } = true;

    /// <summary>
    /// Additional attributes to apply to the scroll area container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "scroll-area",
            Class
        );
    }
}
