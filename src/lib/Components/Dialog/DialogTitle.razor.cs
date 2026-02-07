using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Dialog;

/// <summary>
/// Title element for dialog header.
/// </summary>
public partial class DialogTitle : ComponentBase
{
    /// <summary>
    /// The title text to render.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the title.
    /// </summary>
    [Parameter]
    public string? ClassName { get; set; }

    /// <summary>
    /// Additional attributes to apply to the title element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        var baseClasses = "text-lg font-semibold leading-none tracking-tight";
        return string.IsNullOrWhiteSpace(ClassName) ? baseClasses : $"{baseClasses} {ClassName}";
    }
}
