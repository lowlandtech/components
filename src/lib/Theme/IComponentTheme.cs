namespace LowlandTech.Components;

/// <summary>
/// Provides a named set of CSS custom-property tokens that a <c>ThemeProvider</c>
/// injects as inline style onto its root element, scoping the design tokens to
/// that component subtree.
/// </summary>
public interface IComponentTheme
{
    /// <summary>Unique name used to identify and address the theme.</summary>
    string Name { get; }

    /// <summary>
    /// CSS custom-property overrides keyed by variable name (e.g. <c>"--background"</c>).
    /// Values are written verbatim into the element's inline <c>style</c>.
    /// </summary>
    IReadOnlyDictionary<string, string> Tokens { get; }
}
