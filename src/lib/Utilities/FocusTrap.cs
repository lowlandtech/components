using Microsoft.JSInterop;

namespace LowlandTech.Components.Utilities;

/// <summary>
/// C# wrapper for JavaScript focus trap utilities.
/// Manages keyboard focus within a container element, useful for modals and dialogs.
/// </summary>
public class FocusTrap : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _containerId;
    private bool _isActive;
    private bool _disposed;

    /// <summary>
    /// Event fired when the focus trap is released (e.g., via Escape key)
    /// </summary>
    public event EventHandler? Released;

    /// <summary>
    /// Creates a new focus trap for the specified container
    /// </summary>
    /// <param name="jsRuntime">The JS runtime instance</param>
    /// <param name="containerId">The ID of the container element</param>
    public FocusTrap(IJSRuntime jsRuntime, string containerId)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _containerId = !string.IsNullOrWhiteSpace(containerId) 
            ? containerId 
            : throw new ArgumentException("Container ID cannot be null or empty", nameof(containerId));
    }

    /// <summary>
    /// Activates the focus trap, moving focus to the first focusable element
    /// </summary>
    /// <param name="restoreFocusOnRelease">Whether to restore focus to the trigger element on release</param>
    public async Task ActivateAsync(bool restoreFocusOnRelease = true)
    {
        if (_isActive) return;

        try
        {
            await _jsRuntime.InvokeAsync<object?>(
                "LowlandTechComponents.focusTrapUtils.activate",
                _containerId,
                restoreFocusOnRelease);
            
            _isActive = true;
        }
        catch (JSException)
        {
            // JSInterop not available (e.g., during SSR) - fail silently
        }
        catch (InvalidOperationException)
        {
            // JSRuntime not available - fail silently
        }
    }

    /// <summary>
    /// Deactivates the focus trap, optionally restoring focus to the trigger element
    /// </summary>
    public async Task DeactivateAsync()
    {
        if (!_isActive) return;

        try
        {
            await _jsRuntime.InvokeAsync<object?>(
                "LowlandTechComponents.focusTrapUtils.deactivate",
                _containerId);
            
            _isActive = false;
            Released?.Invoke(this, EventArgs.Empty);
        }
        catch (JSException)
        {
            // JSInterop not available - fail silently
        }
        catch (InvalidOperationException)
        {
            // JSRuntime not available - fail silently
        }
    }

    /// <summary>
    /// Gets whether the focus trap is currently active
    /// </summary>
    public bool IsActive => _isActive;

    /// <summary>
    /// Deactivates the focus trap on disposal
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_isActive)
        {
            await DeactivateAsync();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
