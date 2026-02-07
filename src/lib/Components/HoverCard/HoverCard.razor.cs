using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.HoverCard;

/// <summary>
/// Root HoverCard component that manages the open/closed state on hover.
/// Provides cascading state to child components (HoverCardTrigger, HoverCardContent).
/// </summary>
public partial class HoverCard : ComponentBase
{
    private bool _isOpen;

    /// <summary>
    /// The content to render inside the hover card context.
    /// Typically contains HoverCardTrigger and HoverCardContent components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Controls the open/closed state of the hover card.
    /// </summary>
    [Parameter]
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            if (_isOpen != value)
            {
                _isOpen = value;
                IsOpenChanged.InvokeAsync(_isOpen);
            }
        }
    }

    /// <summary>
    /// Event callback when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Opens the hover card.
    /// </summary>
    internal void OpenHoverCard()
    {
        IsOpen = true;
        StateHasChanged();
    }

    /// <summary>
    /// Closes the hover card.
    /// </summary>
    internal void CloseHoverCard()
    {
        IsOpen = false;
        StateHasChanged();
    }
}
