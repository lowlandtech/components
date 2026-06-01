namespace LowlandTech.Components;

/// <summary>
/// Resolves theme class overrides and fragment replacements for a given
/// <c>ThemeProvider</c> boundary.  Components call this instead of building
/// class strings by hand so that active theme packs can intercept and
/// modify the output.
/// </summary>
public interface IComponentThemeResolver
{
    /// <summary>
    /// Returns the effective CSS class string for a component slot.
    /// </summary>
    /// <param name="providerId">The enclosing <c>ThemeProvider</c> boundary ID.</param>
    /// <param name="key">The slot to resolve.</param>
    /// <param name="defaultClass">The component's built-in class string.</param>
    /// <param name="consumerClass">
    /// The <c>Class</c> parameter supplied by the component consumer.
    /// Always appended last regardless of merge mode.
    /// </param>
    /// <param name="designClass">
    /// Design-mode classes injected by <c>TinyComponentBase</c>.
    /// Always appended last, after <paramref name="consumerClass"/>.
    /// </param>
    /// <returns>The resolved, merged class string.</returns>
    string Class(
        string? providerId,
        ComponentSlotKey key,
        string defaultClass,
        string? consumerClass = null,
        string? designClass = null);

    /// <summary>
    /// Returns a theme-supplied fragment for the given slot, or <c>null</c>
    /// when the active theme does not override that slot.
    /// </summary>
    RenderFragment<ThemeFragmentContext>? Fragment(
        string? providerId,
        ComponentSlotKey key);
}
