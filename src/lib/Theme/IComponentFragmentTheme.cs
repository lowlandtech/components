namespace LowlandTech.Components;

/// <summary>
/// Optional capability that an <see cref="IComponentTheme"/> can implement to
/// supply <see cref="RenderFragment"/> replacements for named component regions.
/// Use only when class overrides via <see cref="IComponentClassTheme"/> are
/// insufficient — fragments change markup structure, not just styling.
/// </summary>
public interface IComponentFragmentTheme
{
    /// <summary>
    /// Per-slot fragment overrides.  The delegate receives a
    /// <see cref="ThemeFragmentContext"/> and returns a
    /// <see cref="RenderFragment"/>.
    /// </summary>
    IReadOnlyDictionary<ComponentSlotKey, RenderFragment<ThemeFragmentContext>> Fragments { get; }
}
