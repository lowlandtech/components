namespace LowlandTech.Components;

/// <summary>
/// One theme a plugin owner contributes to a provider boundary.
/// </summary>
/// <param name="OwnerKey">
/// The key of the owner that contributes the theme. The owner decides whether
/// the theme is selectable: a theme of an owner that is not active is never
/// published.
/// </param>
/// <param name="ProviderId">
/// The identifier of the <c>ThemeProvider</c> boundary the theme belongs to.
/// </param>
/// <param name="ThemeKey">
/// The key of the theme inside the owner. The host selects the theme of a
/// provider by that key.
/// </param>
/// <param name="Theme">
/// The theme itself, in the theme contracts the resolver of the host already
/// uses: <see cref="IComponentTheme"/> carries the tokens,
/// <see cref="IComponentClassTheme"/> the class slots and
/// <see cref="IComponentFragmentTheme"/> the fragments.
/// </param>
/// <param name="Stylesheets">
/// The stylesheets the theme needs, as local package CSS paths. The selection
/// belongs to the boundary and follows the owner: a withdrawn owner loses the
/// selection, while the deployed files stay untouched.
/// </param>
public sealed record ComponentThemeBinding(
    string OwnerKey,
    string ProviderId,
    string ThemeKey,
    IComponentTheme Theme,
    IReadOnlyList<string> Stylesheets);

/// <summary>
/// The validation of a theme binding before it is published.
/// </summary>
/// <param name="Binding">The binding that was validated.</param>
/// <param name="IsValid">Whether the binding may be published.</param>
/// <param name="Errors">The named refusals of the binding.</param>
public sealed record ComponentThemeBindingValidation(
    ComponentThemeBinding Binding,
    bool IsValid,
    IReadOnlyList<ComponentThemeBindingError> Errors);

/// <summary>
/// One named refusal of a theme binding.
/// </summary>
/// <param name="Code">The reason of the refusal.</param>
/// <param name="Subject">The owner, the theme or the path the refusal names.</param>
/// <param name="Message">What the refusal says.</param>
public sealed record ComponentThemeBindingError(
    ComponentThemeBindingErrorCode Code,
    string Subject,
    string Message);

/// <summary>
/// The named reasons a theme binding is refused before publication.
/// </summary>
public enum ComponentThemeBindingErrorCode
{
    /// <summary>
    /// The owner of the binding is not active, so the binding is excluded
    /// before publication.
    /// </summary>
    InactiveOwner,

    /// <summary>The theme has no name, so a provider cannot select it.</summary>
    MissingThemeName,

    /// <summary>The style family of the theme is empty.</summary>
    InvalidStyleFamily,

    /// <summary>A class slot of the theme uses a merge mode that does not exist.</summary>
    UndefinedMergeMode,

    /// <summary>A stylesheet of the theme is not a local package CSS path.</summary>
    InvalidStylesheetPath,

    /// <summary>
    /// Another published binding already claims the same provider and theme
    /// key.
    /// </summary>
    DuplicateBinding,
}
