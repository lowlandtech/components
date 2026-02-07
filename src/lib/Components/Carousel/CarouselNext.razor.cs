using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Carousel;

/// <summary>
/// A button component for navigating to the next carousel item.
/// </summary>
public class CarouselNextBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the child content to be rendered inside the button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the button.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the button element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the parent carousel component.
    /// </summary>
    [CascadingParameter]
    public CarouselBase? Carousel { get; set; }

    /// <summary>
    /// Gets whether the button should be disabled.
    /// </summary>
    protected bool IsDisabled => Carousel?.CanGoNext != true;

    /// <summary>
    /// Handles the button click event.
    /// </summary>
    protected async Task HandleClick()
    {
        if (Carousel != null)
        {
            await Carousel.Next();
        }
    }

    /// <summary>
    /// Gets the combined CSS classes for the button.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "carousel-next";

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
