using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.AspectRatio;

/// <summary>
/// A container that preserves a specific aspect ratio for its content.
/// </summary>
public class AspectRatioBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the aspect ratio (width / height).
    /// Default is 16/9.
    /// </summary>
    [Parameter]
    public double Ratio { get; set; } = 16.0 / 9.0;

    /// <summary>
    /// Gets or sets the child content to be rendered inside the aspect ratio container.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the aspect ratio container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets the combined CSS classes for the aspect ratio container.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "aspect-ratio";
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
