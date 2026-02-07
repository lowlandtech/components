using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace LowlandTech.Components.Components.SwipeArea;

/// <summary>
/// Direction of swipe movement.
/// </summary>
public enum SwipeDirection
{
    None,
    Left,
    Right,
    Up,
    Down
}

/// <summary>
/// Event args for swipe events containing direction and distance information.
/// </summary>
public class SwipeEventArgs : EventArgs
{
    public SwipeDirection Direction { get; set; }
    public double Distance { get; set; }
    public double DeltaX { get; set; }
    public double DeltaY { get; set; }
}

/// <summary>
/// A component that detects swipe gestures on touch and pointer devices.
/// </summary>
public class SwipeAreaBase : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets or sets the child content to be rendered inside the swipe area.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the swipe area.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the sensitivity threshold in pixels before OnSwipeEnd fires.
    /// Default is 50 pixels.
    /// </summary>
    [Parameter]
    public double Sensitivity { get; set; } = 50.0;

    /// <summary>
    /// Gets or sets whether to prevent default browser behavior during swipe.
    /// </summary>
    [Parameter]
    public bool PreventDefault { get; set; } = false;

    /// <summary>
    /// Event fired during swipe movement with direction data.
    /// </summary>
    [Parameter]
    public EventCallback<SwipeEventArgs> OnSwipeMove { get; set; }

    /// <summary>
    /// Event fired when swipe movement exceeds the sensitivity threshold.
    /// </summary>
    [Parameter]
    public EventCallback<SwipeEventArgs> OnSwipeEnd { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Reference to the root element.
    /// </summary>
    protected ElementReference ElementRef { get; set; }

    private double _startX = 0;
    private double _startY = 0;
    private bool _isSwiping = false;
    private IJSObjectReference? _swipeModule;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && PreventDefault)
        {
            // Initialize JS interop for preventDefault if needed
            _swipeModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/LowlandTech.Components/js/components.js");
            
            await _swipeModule.InvokeAsync<object>("swipeUtils.init", ElementRef);
        }
    }

    /// <summary>
    /// Handles pointer down event to start tracking swipe.
    /// </summary>
    protected async Task HandlePointerDown(PointerEventArgs e)
    {
        _startX = e.ClientX;
        _startY = e.ClientY;
        _isSwiping = true;

        if (PreventDefault && _swipeModule != null)
        {
            await _swipeModule.InvokeAsync<object>("swipeUtils.preventDefault", ElementRef);
        }
    }

    /// <summary>
    /// Handles pointer move event to track swipe direction and distance.
    /// </summary>
    protected async Task HandlePointerMove(PointerEventArgs e)
    {
        if (!_isSwiping) return;

        var deltaX = e.ClientX - _startX;
        var deltaY = e.ClientY - _startY;
        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        var direction = GetSwipeDirection(deltaX, deltaY);

        if (OnSwipeMove.HasDelegate)
        {
            await OnSwipeMove.InvokeAsync(new SwipeEventArgs
            {
                Direction = direction,
                Distance = distance,
                DeltaX = deltaX,
                DeltaY = deltaY
            });
        }
    }

    /// <summary>
    /// Handles pointer up event to complete swipe if threshold exceeded.
    /// </summary>
    protected async Task HandlePointerUp(PointerEventArgs e)
    {
        if (!_isSwiping) return;

        var deltaX = e.ClientX - _startX;
        var deltaY = e.ClientY - _startY;
        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        if (distance >= Sensitivity && OnSwipeEnd.HasDelegate)
        {
            var direction = GetSwipeDirection(deltaX, deltaY);

            await OnSwipeEnd.InvokeAsync(new SwipeEventArgs
            {
                Direction = direction,
                Distance = distance,
                DeltaX = deltaX,
                DeltaY = deltaY
            });
        }

        _isSwiping = false;
    }

    /// <summary>
    /// Handles pointer cancel event to stop tracking.
    /// </summary>
    protected void HandlePointerCancel(PointerEventArgs e)
    {
        _isSwiping = false;
    }

    /// <summary>
    /// Determines swipe direction from delta values.
    /// </summary>
    private SwipeDirection GetSwipeDirection(double deltaX, double deltaY)
    {
        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            return deltaX > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else if (Math.Abs(deltaY) > 0)
        {
            return deltaY > 0 ? SwipeDirection.Down : SwipeDirection.Up;
        }

        return SwipeDirection.None;
    }

    /// <summary>
    /// Gets the combined CSS classes for the swipe area.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "swipe-area";
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }

    public async ValueTask DisposeAsync()
    {
        if (_swipeModule != null)
        {
            await _swipeModule.DisposeAsync();
        }
    }
}
