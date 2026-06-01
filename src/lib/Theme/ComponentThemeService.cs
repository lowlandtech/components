namespace LowlandTech.Components;

/// <summary>
/// Scoped service that maps <c>ThemeProvider</c> instance IDs to their active
/// <see cref="IComponentTheme"/>.  Multiple providers can be active simultaneously,
/// each with a different theme.
/// </summary>
/// <remarks>
/// Also implements <see cref="IComponentThemeResolver"/> so that components
/// with a <see cref="ThemeProviderContext"/> can resolve slot classes and
/// fragments without injecting an extra service.
/// </remarks>
public sealed class ComponentThemeService : IComponentThemeResolver
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

    // ── IComponentThemeResolver ──────────────────────────────────────────────

    /// <inheritdoc/>
    public string Class(
        string? providerId,
        ComponentSlotKey key,
        string defaultClass,
        string? consumerClass = null,
        string? designClass = null)
    {
        var theme = providerId is null ? null : GetForProvider(providerId);
        var classTheme = theme as IComponentClassTheme;

        string resolved;
        if (classTheme is not null && TryResolveSlot(classTheme, key, out var slot))
        {
            resolved = slot!.MergeMode switch
            {
                ThemeClassMergeMode.Replace  => slot.Class,
                ThemeClassMergeMode.Prepend  => $"{slot.Class} {defaultClass}",
                _                            => $"{defaultClass} {slot.Class}"
            };
        }
        else
        {
            resolved = defaultClass;
        }

        return Combine(resolved, consumerClass, designClass);
    }

    /// <inheritdoc/>
    public RenderFragment<ThemeFragmentContext>? Fragment(
        string? providerId,
        ComponentSlotKey key)
    {
        if (providerId is null) return null;
        var theme = GetForProvider(providerId) as IComponentFragmentTheme;
        if (theme is null) return null;

        // Exact match first, then wildcard (no variant/size/state).
        if (theme.Fragments.TryGetValue(key, out var fragment))
            return fragment;

        var wildcard = new ComponentSlotKey(key.Component, key.Slot);
        return theme.Fragments.TryGetValue(wildcard, out var wf) ? wf : null;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Tries to find a matching slot in <paramref name="classTheme"/>.
    /// Checks the full key first, then falls back to a wildcard key that omits
    /// variant/size/state, so a theme can override all variants of a slot with
    /// a single entry.
    /// </summary>
    private static bool TryResolveSlot(
        IComponentClassTheme classTheme,
        ComponentSlotKey key,
        out ThemeClassSlot? slot)
    {
        if (classTheme.ClassSlots.TryGetValue(key, out slot))
            return true;

        var wildcard = new ComponentSlotKey(key.Component, key.Slot);
        if (classTheme.ClassSlots.TryGetValue(wildcard, out slot))
            return true;

        return false;
    }

    private static string Combine(string main, string? extra1, string? extra2)
    {
        if (string.IsNullOrEmpty(extra1) && string.IsNullOrEmpty(extra2))
            return main;

        var parts = new[] { main, extra1 ?? string.Empty, extra2 ?? string.Empty };
        return string.Join(" ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
}
