using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Timeline;

/// <summary>
/// An individual item within a timeline.
/// </summary>
public partial class TimelineItem : ComponentBase
{
    /// <summary>
    /// The content to display in the timeline item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional content for the timeline dot/icon.
    /// </summary>
    [Parameter]
    public RenderFragment? DotContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the timeline item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the timeline item.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The alignment of the timeline item.
    /// </summary>
    /// <remarks>
    /// Options: "start", "center", "end".
    /// </remarks>
    [Parameter]
    public string? Align { get; set; }

    private string GetClasses()
    {
        var alignment = !string.IsNullOrEmpty(Align) ? $"timeline-{Align}" : "";
        return CssClassUtils.Merge("timeline-item", alignment, Class);
    }
}
