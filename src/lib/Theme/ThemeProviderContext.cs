namespace LowlandTech.Components;

/// <summary>
/// Cascading context published by <c>ThemeProvider</c> so that child components
/// (e.g. <c>ThemeToggle</c>) can interact with the enclosing provider's theme
/// without taking a direct component reference.
/// </summary>
public sealed class ThemeProviderContext
{
    public string ProviderId { get; }
    public ComponentThemeService ThemeService { get; }

    public ThemeProviderContext(string providerId, ComponentThemeService themeService)
    {
        ProviderId = providerId;
        ThemeService = themeService;
    }
}
