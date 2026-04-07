using Microsoft.JSInterop;

namespace LowlandTech.Components.App.Services;

/// <summary>
/// Service for managing theme (light/dark mode) with localStorage persistence.
/// </summary>
public class ThemeService
{
    private const string ThemeKey = "app-theme";
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;
    private bool _isDark;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public bool IsDark => _isDark;

    public event Action? ThemeChanged;

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            // Check if there's a stored theme preference
            var storedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", ThemeKey);
            
            if (storedTheme == "dark")
            {
                _isDark = true;
            }
            else if (storedTheme == "light")
            {
                _isDark = false;
            }
            else
            {
                // No stored preference, check system preference
                _isDark = await _jsRuntime.InvokeAsync<bool>("eval", 
                    "window.matchMedia('(prefers-color-scheme: dark)').matches");
            }

            await ApplyThemeAsync();
            _isInitialized = true;
        }
        catch
        {
            // If JS interop fails, default to light theme
            _isDark = false;
        }
    }

    public async Task ToggleThemeAsync()
    {
        _isDark = !_isDark;
        await SaveAndApplyThemeAsync();
    }

    public async Task SetThemeAsync(bool isDark)
    {
        if (_isDark != isDark)
        {
            _isDark = isDark;
            await SaveAndApplyThemeAsync();
        }
    }

    private async Task SaveAndApplyThemeAsync()
    {
        try
        {
            var value = _isDark ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ThemeKey, value);
            await ApplyThemeAsync();
            ThemeChanged?.Invoke();
        }
        catch
        {
            // Handle JS interop failures gracefully
        }
    }

    private async Task ApplyThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", 
                $"document.documentElement.classList.toggle('dark', {(_isDark ? "true" : "false")})");
        }
        catch
        {
            // Handle JS interop failures gracefully
        }
    }
}
