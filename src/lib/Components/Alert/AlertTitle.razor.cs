using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Alert;

/// <summary>
/// AlertTitle component that provides a title for the Alert.
/// Based on shadcn/ui AlertTitle component.
/// </summary>
public partial class AlertTitle : ComponentBase
{
    /// <summary>
    /// The content to render inside the alert title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the alert title.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the alert title element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "alert-title",
            Class
        );
    }
}
