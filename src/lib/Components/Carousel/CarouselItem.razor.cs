using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Carousel;

/// <summary>
/// A component that represents an individual item in a carousel.
/// </summary>
public class CarouselItemBase : ComponentBase
{
    private int _index = -1;

    /// <summary>
    /// Gets or sets the child content to be rendered inside the carousel item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the carousel item wrapper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the parent carousel component.
    /// </summary>
    [CascadingParameter]
    public CarouselBase? Carousel { get; set; }

    /// <summary>
    /// Gets the index of this item in the carousel.
    /// </summary>
    public int Index => _index;

    protected override void OnInitialized()
    {
        if (Carousel != null)
        {
            _index = Carousel.TotalItems;
            Carousel.RegisterItem(this);
        }
    }

    /// <summary>
    /// Gets whether this item is currently active in the carousel.
    /// </summary>
    public bool IsActive => Carousel?.IsActive(_index) == true;

    /// <summary>
    /// Gets the combined CSS classes for the carousel item wrapper.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "carousel-item";

        if (IsActive)
        {
            classes += " carousel-item-active";
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }

    public void Dispose()
    {
        Carousel?.UnregisterItem(this);
    }
}
