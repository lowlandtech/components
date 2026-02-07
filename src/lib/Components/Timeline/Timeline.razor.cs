using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Timeline;

/// <summary>
/// A container component for displaying timeline items in vertical or horizontal orientation.
/// Based on MudBlazor Timeline component.
/// </summary>
public partial class Timeline : ComponentBase
{
    /// <summary>
    /// The timeline items to display.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the timeline.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the timeline.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The orientation of the timeline.
    /// </summary>
    /// <remarks>
    /// Defaults to "vertical". Options: "vertical", "horizontal".
    /// </remarks>
    [Parameter]
    public string Orientation { get; set; } = "vertical";

    private string GetClasses()
    {
        var orientation = $"timeline-{Orientation}";
        return CssClassUtils.Merge("timeline", orientation, Class);
    }
}
