namespace LowlandTech.Components;

/// <summary>
/// Designable parameter extensions for <see cref="ThemeProvider"/>.
/// </summary>
[Properties(Group = "Misc", Icon = "palette",
    Description = "Scoped theme boundary — applies a named token set to its subtree")]
public partial class ThemeProvider
{
    /// <summary>
    /// Unique identifier for this provider. Used by <see cref="ComponentThemeService"/>
    /// to track which theme is active at this boundary.
    /// </summary>
    [Parameter, EditorRequired]
    public string Id { get; set; } = "";

    /// <summary>Name of the initial <see cref="IComponentTheme"/> to apply.</summary>
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "Light", "Dark", "Ocean", "Rose", "Forest", "Glass", "ChadcnBlack", "CorporateGrey" })]
    public string? Theme { get; set; }
}
