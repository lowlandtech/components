using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.ScrollArea;

/// <summary>
/// Orientation for ScrollBar component.
/// </summary>
public enum ScrollBarOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// ScrollBar component that provides a custom scrollbar for ScrollArea.
/// Based on shadcn/ui ScrollArea scrollbar with dark theme styling.
/// </summary>
public partial class ScrollBar : ComponentBase
{
    /// <summary>
    /// The orientation of the scrollbar (Horizontal or Vertical).
    /// </summary>
    [Parameter]
    public ScrollBarOrientation Orientation { get; set; } = ScrollBarOrientation.Vertical;

    /// <summary>
    /// Optional CSS classes to apply to the scrollbar.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the scrollbar element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "scrollbar",
            Orientation == ScrollBarOrientation.Horizontal ? "scrollbar-horizontal" : "scrollbar-vertical",
            Class
        );
    }
}
