namespace LowlandTech.Components;

/// <summary>
/// Optional capability that an <see cref="IComponentTheme"/> can implement to
/// supply <see cref="RenderFragment"/> replacements for named component regions.
/// Use only when class overrides via <see cref="IComponentClassTheme"/> are
/// insufficient — fragments change markup structure, not just styling.
/// </summary>
/// <remarks>
/// <b>Fragment system: Phase 5 — deferred.</b>
/// The interface, resolver method, and context wrapper are in place but no
/// production component renders a fragment slot yet.  Do not add fragment
/// slots to components until a concrete UX requirement exists that class
/// overrides cannot satisfy.  See <c>ThemingArchitecturePlan.md § Fragment System</c>.
/// </remarks>
public interface IComponentFragmentTheme
{
    /// <summary>
    /// Per-slot fragment overrides.  The delegate receives a
    /// <see cref="ThemeFragmentContext"/> and returns a
    /// <see cref="RenderFragment"/>.
    /// </summary>
    IReadOnlyDictionary<ComponentSlotKey, RenderFragment<ThemeFragmentContext>> Fragments { get; }
}
