namespace LowlandTech.Components;

/// <summary>
/// A class override for a named component slot provided by an
/// <see cref="IComponentClassTheme"/>.
/// </summary>
/// <param name="Class">
/// The CSS class string to apply.  May be a Tailwind class list or any
/// valid <c>class</c> attribute value.
/// </param>
/// <param name="MergeMode">
/// Specifies whether the theme classes are appended to, replace, or prepend
/// the component's built-in default class string.
/// </param>
public sealed record ThemeClassSlot(
    string Class,
    ThemeClassMergeMode MergeMode = ThemeClassMergeMode.Append);
