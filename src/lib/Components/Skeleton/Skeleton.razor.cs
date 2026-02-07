using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Skeleton;

/// <summary>
/// A placeholder component for loading states with optional animations.
/// </summary>
public partial class Skeleton : ComponentBase
{
    /// <summary>
    /// Optional child content to render inside the skeleton.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the skeleton.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the skeleton.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The width of the skeleton (e.g., "100px", "50%", "10rem").
    /// </summary>
    [Parameter]
    public string? Width { get; set; }

    /// <summary>
    /// The height of the skeleton (e.g., "20px", "2rem").
    /// </summary>
    [Parameter]
    public string? Height { get; set; }

    /// <summary>
    /// The shape variant of the skeleton.
    /// </summary>
    /// <remarks>
    /// Defaults to "text". Options: "text", "rect", "circle".
    /// </remarks>
    [Parameter]
    public string Variant { get; set; } = "text";

    /// <summary>
    /// The animation style for the skeleton.
    /// </summary>
    /// <remarks>
    /// Defaults to "pulse". Options: "pulse", "wave", "none".
    /// </remarks>
    [Parameter]
    public string Animation { get; set; } = "pulse";

    private string GetClasses()
    {
        var variant = $"skeleton-{Variant}";
        var animation = Animation != "none" ? $"skeleton-{Animation}" : "";
        
        return CssClassUtils.Merge("skeleton", variant, animation, Class);
    }

    private string? GetStyles()
    {
        var styles = new List<string>();
        
        if (!string.IsNullOrEmpty(Width))
            styles.Add($"width: {Width}");
            
        if (!string.IsNullOrEmpty(Height))
            styles.Add($"height: {Height}");
            
        return styles.Count > 0 ? string.Join("; ", styles) : null;
    }
}
