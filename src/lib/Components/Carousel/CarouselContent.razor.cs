using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Carousel;

/// <summary>
/// A component that provides a container for carousel items.
/// </summary>
public class CarouselContentBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the child content to be rendered inside the carousel content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the carousel content wrapper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the combined CSS classes for the carousel content wrapper.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "carousel-content";

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
