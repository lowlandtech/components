using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Avatar;

/// <summary>
/// AvatarFallback component that displays fallback content when the image is not available.
/// </summary>
public partial class AvatarFallback : ComponentBase
{
    /// <summary>
    /// The fallback content to render (typically initials or an icon).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the fallback container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the fallback element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "avatar-fallback",
            Class
        );
    }
}
