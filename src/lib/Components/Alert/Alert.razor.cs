using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Alert;

/// <summary>
/// Alert component that provides a container for displaying important messages.
/// Based on shadcn/ui Alert component with dark theme styling.
/// </summary>
public partial class Alert : ComponentBase
{
    /// <summary>
    /// The content to render inside the alert.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the alert.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the alert element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "alert",
            Class
        );
    }
}
