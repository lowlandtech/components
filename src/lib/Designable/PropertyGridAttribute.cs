namespace LowlandTech.Components;

/// <summary>
/// Decorates a <c>[Parameter]</c> on a <see cref="PropertiesAttribute"/>-
/// marked component to drive its editor in the canvas Properties panel
/// (SPEC2100.US45).
///
/// The Properties panel reflects on the parameter's <see cref="Editor"/>
/// + <see cref="Group"/> + (for Select) <see cref="Options"/> /
/// (for Slider) <see cref="Min"/> / <see cref="Max"/> / <see cref="Step"/>,
/// reads the parameter's default value as the row's seed, and writes
/// edits back as <c>ContainerAttribute(Source="parameter")</c> rows so
/// the existing storage pipe handles persistence.
///
/// All fields are optional — a bare <c>[PropertyGrid]</c> infers the
/// editor from the parameter type:
/// <list type="bullet">
///   <item>bool → <see cref="PropertyEditor.Switch"/></item>
///   <item>int / double / decimal → <see cref="PropertyEditor.Number"/></item>
///   <item>string → <see cref="PropertyEditor.Text"/></item>
///   <item>enum → <see cref="PropertyEditor.Select"/> with the enum names as Options</item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class PropertyGridAttribute : Attribute
{
    /// <summary>Section name within the Properties panel
    /// (Appearance / Behavior / Sizing / etc.). Each distinct group
    /// renders as its own collapsible accordion. Defaults to
    /// "Properties".</summary>
    public string Group { get; init; } = "Properties";

    /// <summary>Override the type-inferred editor. Use this when the
    /// parameter type doesn't uniquely determine the editor (e.g.
    /// a string CSS color = <see cref="PropertyEditor.Color"/>; a
    /// string from a known enumeration = <see cref="PropertyEditor.Select"/>).
    /// Defaults to <see cref="PropertyEditor.Inferred"/> so the registry
    /// picks the editor from the parameter type. Note: nullable enum is
    /// not a valid attribute parameter type in C#, hence the sentinel
    /// rather than <c>PropertyEditor?</c>.</summary>
    public PropertyEditor Editor { get; init; } = PropertyEditor.Inferred;

    /// <summary>Allowed values for <see cref="PropertyEditor.Select"/>
    /// editors. Ignored for other editors. Each entry becomes one
    /// SelectItem in the rendered dropdown.</summary>
    public string[]? Options { get; init; }

    /// <summary>Range minimum for <see cref="PropertyEditor.Slider"/>
    /// or <see cref="PropertyEditor.Number"/> editors. <see cref="double.NaN"/>
    /// (the default) means "no minimum specified" — nullable double is
    /// not a valid attribute parameter type in C# so we sentinel with
    /// NaN.</summary>
    public double Min { get; init; } = double.NaN;

    /// <summary>Range maximum for <see cref="PropertyEditor.Slider"/>
    /// or <see cref="PropertyEditor.Number"/> editors. NaN-sentinel
    /// like <see cref="Min"/>.</summary>
    public double Max { get; init; } = double.NaN;

    /// <summary>Step increment for <see cref="PropertyEditor.Slider"/>
    /// or <see cref="PropertyEditor.Number"/> editors. NaN-sentinel
    /// like <see cref="Min"/>.</summary>
    public double Step { get; init; } = double.NaN;

    /// <summary>Friendly label for the row. Defaults to the property
    /// name humanised ("FontSize" → "Font size").</summary>
    public string? Label { get; init; }

    /// <summary>Placeholder shown in empty Text / Textarea / Json
    /// editors.</summary>
    public string? Placeholder { get; init; }

    /// <summary>One-line tooltip shown on hover of the row label —
    /// surfaces a parameter's intent without bloating the panel.</summary>
    public string? Description { get; init; }

    /// <summary>Sort order within a group. Lower numbers render first;
    /// ties resolve by reflection order (which is property-declaration
    /// order). Defaults to 0.</summary>
    public int Order { get; init; }
}
