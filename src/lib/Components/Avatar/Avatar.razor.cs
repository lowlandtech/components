using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Avatar;

/// <summary>
/// Avatar component that displays user profile images with fallback support.
/// Based on shadcn/ui Avatar component with dark theme styling.
/// </summary>
public partial class Avatar : ComponentBase
{
    /// <summary>
    /// The content to render inside the avatar (typically AvatarImage and AvatarFallback).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the avatar.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the avatar element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "avatar",
            Class
        );
    }
}
