namespace LowlandTech.Components;

/// <summary>
/// Discovery + lookup of <see cref="PropertiesAttribute"/>-decorated
/// components. Singleton-scoped — reflection happens once at startup,
/// the registry is queried thousands of times per Properties-panel
/// render after that.
///
/// Plugins extend the toolbox by shipping new components with
/// <c>[Properties]</c>; the registry sweeps every loaded assembly so
/// no DI registration per component is needed.
/// </summary>
public interface IComponentDescriptorRegistry
{
    /// <summary>All discovered component descriptors, in stable order
    /// (by Group then Element, case-insensitive).</summary>
    IReadOnlyList<ComponentDescriptor> All { get; }

    /// <summary>
    /// Resolve a descriptor by the element name a <c>ContainerItem</c>
    /// stores (matches <see cref="ComponentDescriptor.Element"/>). Case-
    /// insensitive. Returns null when no decorated component matches —
    /// the canvas Properties panel falls back to the raw Attributes
    /// editor in that case.
    /// </summary>
    ComponentDescriptor? GetByElement(string? element);

    /// <summary>
    /// Resolve a descriptor by the component <see cref="Type"/> itself.
    /// Returns null when the type isn't decorated with
    /// <see cref="PropertiesAttribute"/>.
    /// </summary>
    ComponentDescriptor? GetByType(Type componentType);

    /// <summary>
    /// All descriptors whose Group matches the supplied palette section
    /// (e.g. "Forms", "Layout"). Used by the Components sidebar to
    /// render one collapsible group per section.
    /// </summary>
    IReadOnlyList<ComponentDescriptor> GetByGroup(string group);
}
