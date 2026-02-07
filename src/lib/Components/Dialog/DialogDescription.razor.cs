using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Dialog;

/// <summary>
/// Description element for dialog header.
/// </summary>
public partial class DialogDescription : ComponentBase
{
    /// <summary>
    /// The description text to render.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the description.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// Additional attributes to apply to the description element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "text-sm text-gray-500 dark:text-gray-400";
        return string.IsNullOrWhiteSpace(ClassName) ? baseClasses : $"{baseClasses} {ClassName}";
    }
}
