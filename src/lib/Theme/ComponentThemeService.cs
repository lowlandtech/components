namespace LowlandTech.Components;

/// <summary>
/// Scoped service that maps <c>ThemeProvider</c> instance IDs to their active
/// <see cref="IComponentTheme"/>.  Multiple providers can be active simultaneously,
/// each with a different theme.
/// </summary>
public sealed class ComponentThemeService
{
    private readonly List<IComponentTheme> _themes;
    private readonly Dictionary<string, string> _providerThemes = new();

    /// <summary>Raised after a provider's active theme changes.  Argument is the provider ID.</summary>
    public event Action<string>? ThemeChanged;

    public ComponentThemeService(IEnumerable<IComponentTheme> themes)
    {
        _themes = [..themes];
    }

    /// <summary>All registered themes, in registration order.</summary>
    public IReadOnlyList<IComponentTheme> All => _themes;

    /// <summary>
    /// Returns the active theme for <paramref name="providerId"/>, or the first
    /// registered theme when none has been set yet.
    /// </summary>
    public IComponentTheme? GetForProvider(string providerId)
    {
        var name = _providerThemes.GetValueOrDefault(providerId);
        return name is null
            ? _themes.FirstOrDefault()
            : _themes.FirstOrDefault(t => t.Name == name);
    }

    /// <summary>Returns the active theme name for <paramref name="providerId"/>.</summary>
    public string? GetNameForProvider(string providerId) =>
        _providerThemes.GetValueOrDefault(providerId);

    /// <summary>
    /// Called by <c>ThemeProvider.OnInitialized</c> to establish the initial theme.
    /// Does nothing if <paramref name="providerId"/> is already registered.
    /// </summary>
    public void Register(string providerId, string? initialThemeName = null)
    {
        if (!_providerThemes.ContainsKey(providerId))
            _providerThemes[providerId] = initialThemeName ?? _themes.FirstOrDefault()?.Name ?? "";
    }

    /// <summary>Called by <c>ThemeProvider.Dispose</c> to clean up state.</summary>
    public void Unregister(string providerId) =>
        _providerThemes.Remove(providerId);

    /// <summary>Sets the active theme for <paramref name="providerId"/> and fires <see cref="ThemeChanged"/>.</summary>
    public void Set(string providerId, string themeName)
    {
        _providerThemes[providerId] = themeName;
        ThemeChanged?.Invoke(providerId);
    }

    /// <summary>
    /// Advances <paramref name="providerId"/> to the next theme in registration order,
    /// wrapping around to the first theme after the last.
    /// </summary>
    public void Next(string providerId)
    {
        if (_themes.Count == 0) return;
        var current = GetNameForProvider(providerId);
        var idx = _themes.FindIndex(t => t.Name == current);
        var next = _themes[(idx + 1) % _themes.Count];
        Set(providerId, next.Name);
    }
}
