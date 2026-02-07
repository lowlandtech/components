using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Avatar;

/// <summary>
/// AvatarImage component that displays the user's profile image.
/// </summary>
public partial class AvatarImage : ComponentBase
{
    /// <summary>
    /// The source URL of the image.
    /// </summary>
    [Parameter]
    public string? Src { get; set; }

    /// <summary>
    /// The alt text for the image.
    /// </summary>
    [Parameter]
    public string? Alt { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the image.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the image element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "avatar-image",
            Class
        );
    }
}
