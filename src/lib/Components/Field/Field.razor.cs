using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Field;

/// <summary>
/// Field component that provides a wrapper for label, control, and description.
/// Based on shadcn/ui Field component.
/// </summary>
public partial class Field
{
    /// <summary>
    /// Label text for the field.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Description text for the field.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Error message to display for the field.
    /// </summary>
    [Parameter]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Child content, typically the input control.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to forward to the wrapper element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("field", Class);
    }
}
