namespace LowlandTech.Components;

/// <summary>
/// Optional capability that an <see cref="IComponentTheme"/> can implement to
/// supply per-slot CSS class overrides.  Components check for this interface
/// when building their class strings via the theme resolver.
/// </summary>
/// <remarks>
/// Existing themes that only implement <see cref="IComponentTheme"/> continue
/// to work unchanged — class slots are purely additive.
/// </remarks>
public interface IComponentClassTheme
{
    /// <summary>
    /// Per-slot class overrides keyed by <see cref="ComponentSlotKey"/>.
    /// </summary>
    IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; }
}
