using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Dialog;

/// <summary>
/// Footer section for dialog content, typically containing action buttons.
/// </summary>
public partial class DialogFooter : ComponentBase
{
    /// <summary>
    /// The content to render inside the dialog footer.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the footer.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// Additional attributes to apply to the footer element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "flex flex-col-reverse sm:flex-row sm:justify-end sm:space-x-2";
        return string.IsNullOrWhiteSpace(ClassName) ? baseClasses : $"{baseClasses} {ClassName}";
    }
}
