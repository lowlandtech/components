using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Alert;

/// <summary>
/// AlertDescription component that provides description text for the Alert.
/// Based on shadcn/ui AlertDescription component.
/// </summary>
public partial class AlertDescription : ComponentBase
{
    /// <summary>
    /// The content to render inside the alert description.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the alert description.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the alert description element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "alert-description",
            Class
        );
    }
}
