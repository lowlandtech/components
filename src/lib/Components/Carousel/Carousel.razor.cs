using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.Carousel;

/// <summary>
/// A component that provides a carousel container with navigation controls.
/// </summary>
public class CarouselBase : ComponentBase
{
    private int _currentIndex = 0;
    private List<CarouselItemBase> _items = new();

    /// <summary>
    /// Gets or sets the child content to be rendered inside the carousel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the carousel wrapper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets whether the carousel should loop from last to first item.
    /// </summary>
    [Parameter]
    public bool Loop { get; set; } = false;

    /// <summary>
    /// Gets or sets the default starting index.
    /// </summary>
    [Parameter]
    public int DefaultIndex { get; set; } = 0;

    /// <summary>
    /// Event fired when the active index changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> OnIndexChanged { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the current active index.
    /// </summary>
    public int CurrentIndex => _currentIndex;

    /// <summary>
    /// Gets the total number of items in the carousel.
    /// </summary>
    public int TotalItems => _items.Count;

    /// <summary>
    /// Gets whether the carousel can navigate to the previous item.
    /// </summary>
    public bool CanGoPrevious => Loop || _currentIndex > 0;

    /// <summary>
    /// Gets whether the carousel can navigate to the next item.
    /// </summary>
    public bool CanGoNext => Loop || _currentIndex < TotalItems - 1;

    protected override void OnInitialized()
    {
        _currentIndex = DefaultIndex;
    }

    /// <summary>
    /// Registers a carousel item with this carousel.
    /// </summary>
    public void RegisterItem(CarouselItemBase item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
        }
    }

    /// <summary>
    /// Unregisters a carousel item from this carousel.
    /// </summary>
    public void UnregisterItem(CarouselItemBase item)
    {
        _items.Remove(item);
    }

    /// <summary>
    /// Checks if the specified index is the active index.
    /// </summary>
    public bool IsActive(int index) => index == _currentIndex;

    /// <summary>
    /// Navigates to the next item in the carousel.
    /// </summary>
    public async Task Next()
    {
        if (!CanGoNext)
            return;

        var newIndex = _currentIndex + 1;
        if (newIndex >= TotalItems && Loop)
        {
            newIndex = 0;
        }

        await GoToIndex(newIndex);
    }

    /// <summary>
    /// Navigates to the previous item in the carousel.
    /// </summary>
    public async Task Previous()
    {
        if (!CanGoPrevious)
            return;

        var newIndex = _currentIndex - 1;
        if (newIndex < 0 && Loop)
        {
            newIndex = TotalItems - 1;
        }

        await GoToIndex(newIndex);
    }

    /// <summary>
    /// Navigates to a specific index in the carousel.
    /// </summary>
    public async Task GoToIndex(int index)
    {
        if (index < 0 || index >= TotalItems)
            return;

        _currentIndex = index;
        await OnIndexChanged.InvokeAsync(_currentIndex);
        StateHasChanged();
    }

    /// <summary>
    /// Handles keyboard navigation events.
    /// </summary>
    protected async Task HandleKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowLeft":
                await Previous();
                break;
            case "ArrowRight":
                await Next();
                break;
            case "Home":
                await GoToIndex(0);
                break;
            case "End":
                await GoToIndex(TotalItems - 1);
                break;
        }
    }

    /// <summary>
    /// Gets the combined CSS classes for the carousel wrapper.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "carousel";

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
