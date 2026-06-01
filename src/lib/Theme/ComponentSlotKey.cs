namespace LowlandTech.Components;

/// <summary>
/// Identifies a named visual slot within a component, optionally scoped to a
/// specific variant, size, or state combination.
/// </summary>
/// <remarks>
/// Used as the dictionary key in <see cref="IComponentClassTheme.ClassSlots"/>
/// and <see cref="IComponentFragmentTheme.Fragments"/> so that theme overrides
/// can target, for example, only the <c>Root</c> slot of a <c>Button</c> when
/// its variant is <c>"default"</c>.
/// </remarks>
public readonly record struct ComponentSlotKey(
    string Component,
    string Slot,
    string? Variant = null,
    string? Size = null,
    string? State = null);
