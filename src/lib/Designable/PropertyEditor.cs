namespace LowlandTech.Components;

/// <summary>
/// Editor styles a Properties panel can render for a designable
/// component parameter (SPEC2100.US45). Each value maps to one of the
/// existing LowlandTech.Components inputs the canvas designer already
/// renders elsewhere — keeps the visual rhythm of the right-sidebar
/// panel consistent across hand-built and reflectively-rendered rows.
/// </summary>
public enum PropertyEditor
{
    /// <summary>Sentinel — let the registry infer the editor from the
    /// parameter type (bool→Switch, enum→Select, etc.). Default for
    /// <see cref="PropertyGridAttribute.Editor"/> so a bare
    /// <c>[PropertyGrid]</c> works without spelling the editor out.</summary>
    Inferred = -1,

    /// <summary>Single-line text input. Default for <see cref="string"/>
    /// parameters when no editor is specified.</summary>
    Text = 0,

    /// <summary>Numeric input. Default for numeric parameters.</summary>
    Number = 1,

    /// <summary>Two-state Switch — on/off. Default for <see cref="bool"/>
    /// parameters.</summary>
    Switch = 2,

    /// <summary>Dropdown of allowed values from <see cref="PropertyGridAttribute.Options"/>.</summary>
    Select = 3,

    /// <summary>Color picker — accepts CSS hex / rgb / var(--*) tokens.</summary>
    Color = 4,

    /// <summary>Range slider with min/max/step from the attribute.</summary>
    Slider = 5,

    /// <summary>Multi-line textarea for free-form JSON / markup.</summary>
    Json = 6,

    /// <summary>Multi-line textarea for plain text.</summary>
    Textarea = 7,
}
