namespace LowlandTech.Components;

/// <summary>
/// Cascading context published by <c>ThemeProvider</c> so that child components
/// can resolve theme slot classes and fragments without injecting services
/// directly.
/// </summary>
public sealed class ThemeProviderContext
{
    public string ProviderId { get; }

    /// <summary>
    /// The resolver backed by the scoped <see cref="ComponentThemeService"/>.
    /// Components call <see cref="IComponentThemeResolver.Class"/> and
    /// <see cref="IComponentThemeResolver.Fragment"/> through this reference.
    /// </summary>
    public IComponentThemeResolver Resolver { get; }

    /// <summary>The currently active theme for this provider boundary, if any.</summary>
    public IComponentTheme? ActiveTheme { get; }

    /// <summary>
    /// Kept for backwards-compatibility — existing code that reads
    /// <c>ThemeService</c> directly from the context continues to compile.
    /// </summary>
    public ComponentThemeService ThemeService { get; }

    public ThemeProviderContext(
        string providerId,
        ComponentThemeService themeService)
    {
        ProviderId = providerId;
        ThemeService = themeService;
        Resolver = themeService;
        ActiveTheme = themeService.GetForProvider(providerId);
    }
}

