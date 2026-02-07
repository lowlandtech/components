using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.AlertDialog;

/// <summary>
/// Header section for alert dialog content, typically containing title and description.
/// </summary>
public partial class AlertDialogHeader : ComponentBase
{
    /// <summary>
    /// The content to render inside the alert dialog header.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the header.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// Additional attributes to apply to the header element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "flex flex-col space-y-1.5 text-center sm:text-left";
        return string.IsNullOrWhiteSpace(ClassName) ? baseClasses : $"{baseClasses} {ClassName}";
    }
}
