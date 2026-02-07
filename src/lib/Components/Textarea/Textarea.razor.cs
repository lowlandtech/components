using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Textarea;

/// <summary>
/// Textarea component that renders a styled multi-line text input field.
/// Supports rows, placeholder, disabled, and readonly states.
/// </summary>
public partial class Textarea : ComponentBase
{
    /// <summary>
    /// The number of visible text rows.
    /// Default is 3.
    /// </summary>
    [Parameter]
    public int Rows { get; set; } = 3;

    /// <summary>
    /// Placeholder text to display when the textarea is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the textarea is disabled.
    /// When disabled, the textarea cannot be edited and pointer events are disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the textarea is read-only.
    /// When read-only, the textarea cannot be edited but can still be focused and selected.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the textarea element.
    /// This is merged with the base "input" class.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the textarea element.
    /// Supports all standard HTML textarea attributes (e.g., name, maxlength, etc.).
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("input", Class);
    }
}
