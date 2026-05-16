namespace LowlandTech.Components;

/// <summary>
/// Marks a component class as designable — the canvas Properties panel
/// will introspect its <see cref="PropertyGridAttribute"/>-decorated
/// parameters and render typed editors. SPEC2100.US45.
///
/// <example>
/// <code>
/// [Properties(Group = "Forms", Icon = "form-input", Description = "Single-line input")]
/// public partial class Input : TinyComponentBase
/// {
///     [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
///         Options = new[] { "default", "sm", "lg" })]
///     public string Size { get; set; } = "default";
/// }
/// </code>
/// </example>
///
/// Components without this attribute are still droppable on the canvas,
/// but expose only the raw <c>Attributes</c> accordion for power-user
/// overrides — they don't get a typed Properties panel.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PropertiesAttribute : Attribute
{
    /// <summary>Sidebar grouping in the Components palette
    /// (Forms / Layout / Display / Feedback / …). Free-form string;
    /// the palette renders one section per distinct group value.</summary>
    public string Group { get; init; } = "Components";

    /// <summary>Icon name resolved through the existing Icons registry
    /// (e.g. <c>"form-input"</c>, <c>"square"</c>). Optional — null
    /// renders a generic placeholder icon.</summary>
    public string? Icon { get; init; }

    /// <summary>One-line description shown beneath the component name
    /// in the palette + as the tooltip on the dragged ghost.</summary>
    public string? Description { get; init; }

    /// <summary>Element name the component publishes as
    /// <see cref="ContainerItem.Element"/> when dropped on the canvas.
    /// Defaults to the class name (e.g. <c>"Input"</c>) so consumers
    /// can rely on type-name matching unless explicitly overridden.</summary>
    public string? Element { get; init; }
}
