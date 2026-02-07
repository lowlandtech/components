using Microsoft.JSInterop;

namespace LowlandTech.Components.Utilities;

/// <summary>
/// Provides positioning utilities for floating elements (popovers, tooltips, dropdowns).
/// Handles optimal positioning with viewport collision detection and auto-adjustment.
/// </summary>
public class PositionUtils : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _elementId;
    private readonly string _referenceId;
    private bool _disposed;

    /// <summary>
    /// Creates a new PositionUtils instance for managing floating element positions.
    /// </summary>
    /// <param name="jsRuntime">The JS runtime for interop calls</param>
    /// <param name="elementId">ID of the floating element to position</param>
    /// <param name="referenceId">ID of the reference element to position relative to</param>
    /// <exception cref="ArgumentNullException">Thrown when jsRuntime is null</exception>
    /// <exception cref="ArgumentException">Thrown when elementId or referenceId is null or whitespace</exception>
    public PositionUtils(IJSRuntime jsRuntime, string elementId, string referenceId)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Cannot be null or whitespace", nameof(elementId));
        
        if (string.IsNullOrWhiteSpace(referenceId))
            throw new ArgumentException("Cannot be null or whitespace", nameof(referenceId));

        _elementId = elementId;
        _referenceId = referenceId;
    }

    /// <summary>
    /// Calculates and applies optimal position for the floating element.
    /// Supports placements: top, bottom, left, right.
    /// Returns actual coordinates applied (x=left, y=top in pixels).
    /// </summary>
    /// <param name="placement">Desired placement relative to reference (default: bottom)</param>
    /// <param name="offset">Offset in pixels from reference element (default: 8)</param>
    /// <returns>Position result with x, y coordinates and final placement</returns>
    public async Task<PositionResult> CalculatePositionAsync(
        string placement = "bottom",
        int offset = 8)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                _elementId,
                _referenceId,
                placement,
                offset);
            
            return result ?? new PositionResult();
        }
        catch (JSException)
        {
            // SSR or JS not available - return default
            return new PositionResult();
        }
        catch (InvalidOperationException)
        {
            // JSRuntime not available
            return new PositionResult();
        }
    }

    /// <summary>
    /// Checks if the floating element overflows the viewport.
    /// Used for auto-positioning to avoid viewport edges.
    /// </summary>
    /// <returns>Overflow status for each edge (top, bottom, left, right)</returns>
    public async Task<OverflowResult> CheckOverflowAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                _elementId);
            
            return result ?? new OverflowResult();
        }
        catch (JSException)
        {
            return new OverflowResult();
        }
        catch (InvalidOperationException)
        {
            return new OverflowResult();
        }
    }

    /// <summary>
    /// Calculates optimal position with auto-adjustment to avoid viewport overflow.
    /// If primary placement causes overflow, tries opposite placement.
    /// </summary>
    /// <param name="placement">Desired placement (top, bottom, left, right)</param>
    /// <param name="offset">Offset in pixels (default: 8)</param>
    /// <returns>Position result with auto-adjusted placement if needed</returns>
    public async Task<PositionResult> CalculateWithAutoPositionAsync(
        string placement = "bottom",
        int offset = 8)
    {
        // Try primary placement
        var result = await CalculatePositionAsync(placement, offset);
        
        // Check if overflow occurs
        var overflow = await CheckOverflowAsync();
        
        // Auto-adjust if overflow detected
        if (ShouldAdjustPlacement(placement, overflow))
        {
            var adjustedPlacement = GetOppositePlacement(placement);
            result = await CalculatePositionAsync(adjustedPlacement, offset);
        }
        
        return result;
    }

    private static bool ShouldAdjustPlacement(string placement, OverflowResult overflow)
    {
        return placement switch
        {
            "top" => overflow.Top,
            "bottom" => overflow.Bottom,
            "left" => overflow.Left,
            "right" => overflow.Right,
            _ => false
        };
    }

    private static string GetOppositePlacement(string placement)
    {
        return placement switch
        {
            "top" => "bottom",
            "bottom" => "top",
            "left" => "right",
            "right" => "left",
            _ => "bottom"
        };
    }

    /// <summary>
    /// Disposes the PositionUtils instance.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        _disposed = true;
        await Task.CompletedTask;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Result of position calculation containing coordinates and placement.
/// </summary>
public class PositionResult
{
    /// <summary>
    /// Horizontal position (left) in pixels
    /// </summary>
    public double Left { get; set; }
    
    /// <summary>
    /// Vertical position (top) in pixels
    /// </summary>
    public double Top { get; set; }
}

/// <summary>
/// Result of overflow check indicating which edges overflow the viewport.
/// </summary>
public class OverflowResult
{
    /// <summary>
    /// Whether the element overflows the top edge of viewport
    /// </summary>
    public bool Top { get; set; }
    
    /// <summary>
    /// Whether the element overflows the bottom edge of viewport
    /// </summary>
    public bool Bottom { get; set; }
    
    /// <summary>
    /// Whether the element overflows the left edge of viewport
    /// </summary>
    public bool Left { get; set; }
    
    /// <summary>
    /// Whether the element overflows the right edge of viewport
    /// </summary>
    public bool Right { get; set; }
}
