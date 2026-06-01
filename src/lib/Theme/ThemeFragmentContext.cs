namespace LowlandTech.Components;

/// <summary>
/// Contextual information passed to fragment render delegates so they can
/// adapt their output to the component's current state.
/// </summary>
/// <param name="Component">The component name (e.g. <c>"Card"</c>).</param>
/// <param name="Slot">The slot name (e.g. <c>"Decoration"</c>).</param>
/// <param name="Variant">The active variant, or <c>null</c> if none.</param>
/// <param name="Size">The active size, or <c>null</c> if none.</param>
/// <param name="Parameters">
/// A snapshot of the component's public parameters at render time, allowing
/// fragments to adapt (e.g. show different decoration for different sizes).
/// </param>
public sealed record ThemeFragmentContext(
    string Component,
    string Slot,
    string? Variant,
    string? Size,
    IReadOnlyDictionary<string, object?> Parameters);
