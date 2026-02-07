using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.InputGroup;

/// <summary>
/// InputGroup component that wraps an input element with optional prefix and suffix content.
/// Based on shadcn/ui InputGroup component.
/// </summary>
public partial class InputGroup
{
    /// <summary>
    /// Child content, typically an input element.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Content to display before the input (prefix).
    /// </summary>
    [Parameter]
    public RenderFragment? Prefix { get; set; }

    /// <summary>
    /// Content to display after the input (suffix).
    /// </summary>
    [Parameter]
    public RenderFragment? Suffix { get; set; }

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
        return CssClassUtils.Merge("input-group", Class);
    }
}
