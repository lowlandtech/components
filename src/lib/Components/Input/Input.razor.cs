using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Input;

/// <summary>
/// Input component that renders a styled text input field.
/// Supports various input types (text, email, password, number, search) and forwards all unmatched attributes.
/// </summary>
public partial class Input : ComponentBase
{
    /// <summary>
    /// The type of input to render (text, email, password, number, search, etc.).
    /// Defaults to "text".
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "text";

    /// <summary>
    /// Placeholder text to display when the input is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the input element.
    /// This is merged with the base "input" class.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the input element.
    /// Supports all standard HTML input attributes (e.g., disabled, readonly, value, name, etc.).
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("input", Class);
    }
}
