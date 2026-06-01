namespace LowlandTech.Components;

/// <summary>
/// Optional marker that a theme can implement to declare its broad visual
/// style family.  <c>ThemeProvider</c> emits the value as
/// <c>data-theme-style</c> on its root element, enabling CSS to target
/// entire style families rather than individual named themes.
/// </summary>
/// <example>
/// Well-known family names: <c>"default"</c>, <c>"neutral"</c>,
/// <c>"glass"</c>, <c>"high-contrast"</c>, <c>"brand"</c>.
/// </example>
public interface IThemeStyleFamily
{
    /// <summary>
    /// The style family name written to the <c>data-theme-style</c>
    /// attribute, e.g. <c>"glass"</c>.
    /// </summary>
    string StyleFamily { get; }
}
