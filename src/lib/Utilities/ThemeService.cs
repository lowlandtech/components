using Microsoft.JSInterop;

namespace LowlandTech.Components.Utilities;

/// <summary>
/// Service for managing dark theme with localStorage persistence.
/// Dark theme only - aligns with .cursorrules requirement.
/// </summary>
public class ThemeService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;
    private const string DarkTheme = "dark";

    /// <summary>
    /// Raised when the theme changes.
    /// </summary>
    public event EventHandler<string>? OnThemeChanged;

    /// <summary>
    /// Gets the current theme (always "dark").
    /// </summary>
    public string Theme => DarkTheme;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the theme service by loading the persisted theme.
    /// Must be called after JSRuntime is available (not during SSR).
    /// Only honors "dark" theme - any other value is ignored.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            // Load persisted theme from localStorage
            var storedTheme = await _jsRuntime.InvokeAsync<string?>("LowlandTechComponents.themeUtils.getTheme");
            
            // Only honor "dark" theme - ignore any other value
            if (storedTheme == DarkTheme)
            {
                // Apply dark theme
                await _jsRuntime.InvokeAsync<object?>("LowlandTechComponents.themeUtils.applyTheme", DarkTheme);
            }
            else
            {
                // Default to dark theme and persist it
                await SetThemeAsync(DarkTheme);
            }

            _isInitialized = true;
        }
        catch (JSException)
        {
            // JSInterop not available (e.g., during SSR) - graceful degradation
        }
        catch (InvalidOperationException)
        {
            // JSRuntime not available - graceful degradation
        }
    }

    /// <summary>
    /// Sets the theme to dark and persists it to localStorage.
    /// Only accepts "dark" theme - rejects any other value.
    /// </summary>
    /// <param name="theme">The theme to set (must be "dark")</param>
    public async Task SetThemeAsync(string theme)
    {
        if (theme != DarkTheme)
        {
            // Reject or ignore non-dark theme values (UAC37)
            throw new ArgumentException($"Invalid theme: {theme}. Only 'dark' theme is supported.", nameof(theme));
        }

        try
        {
            // Persist to localStorage and apply
            await _jsRuntime.InvokeAsync<object?>("LowlandTechComponents.themeUtils.setTheme", DarkTheme);
        }
        catch (JSException)
        {
            // JSInterop not available - continue without persistence
        }
        catch (InvalidOperationException)
        {
            // JSRuntime not available - continue without persistence
        }

        // Notify listeners (UAC34)
        OnThemeChanged?.Invoke(this, DarkTheme);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Task.CompletedTask;
    }
}
