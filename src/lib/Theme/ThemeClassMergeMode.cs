namespace LowlandTech.Components;

/// <summary>
/// Describes how a theme's class override should be combined with a component's
/// default class string.
/// </summary>
public enum ThemeClassMergeMode
{
    /// <summary>Append the theme classes after the component defaults.</summary>
    Append,

    /// <summary>Replace the component default class entirely with the theme classes.</summary>
    Replace,

    /// <summary>Insert the theme classes before the component defaults.</summary>
    Prepend
}
