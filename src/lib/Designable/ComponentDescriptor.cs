using System.Reflection;

namespace LowlandTech.Components;

/// <summary>
/// Discovered metadata for a designable component (SPEC2100.US45).
/// Built once per component type at registry load and cached forever —
/// reflection happens at startup, not on every Properties-panel render.
/// </summary>
public sealed record ComponentDescriptor(
    Type ComponentType,
    string Element,
    string Group,
    string? Icon,
    string? Description,
    IReadOnlyList<PropertyDescriptor> Properties)
{
    /// <summary>
    /// Canonical name used for class-attribute lookup —
    /// <see cref="Element"/> is what <c>ContainerItem.Element</c> stores
    /// when the component is dropped, so the canvas Properties panel
    /// uses it to resolve "what descriptor drives this item's editor".
    /// </summary>
    public string LookupKey => Element;

    /// <summary>
    /// Properties grouped by <see cref="PropertyDescriptor.Group"/> in
    /// declaration order, with each group's properties sorted by
    /// <see cref="PropertyDescriptor.Order"/> then declaration index.
    /// The Properties panel renders one accordion section per group.
    /// </summary>
    public IReadOnlyList<IGrouping<string, PropertyDescriptor>> GroupedProperties =>
        Properties
            .OrderBy(p => p.Order)
            .ThenBy(p => p.DeclarationIndex)
            .GroupBy(p => p.Group, StringComparer.OrdinalIgnoreCase)
            .ToList();
}

/// <summary>
/// Discovered metadata for one designable parameter on a component.
/// </summary>
public sealed record PropertyDescriptor(
    PropertyInfo PropertyInfo,
    string Name,
    string Label,
    string Group,
    PropertyEditor Editor,
    Type ValueType,
    object? DefaultValue,
    IReadOnlyList<string> Options,
    double? Min,
    double? Max,
    double? Step,
    string? Placeholder,
    string? Description,
    int Order,
    int DeclarationIndex);
