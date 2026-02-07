using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Label;

/// <summary>
/// Label component for form field labels.
/// Based on shadcn/ui Label component.
/// </summary>
public partial class Label
{
    /// <summary>
    /// Child content, typically the label text.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to forward to the label element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "cn-label",
            "flex",
            "items-center",
            "select-none",
            Class
        );
    }
}
