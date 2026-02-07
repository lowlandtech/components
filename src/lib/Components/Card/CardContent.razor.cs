using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Card;

/// <summary>
/// CardContent component that provides the main content area for the Card.
/// Based on shadcn/ui CardContent component.
/// </summary>
public partial class CardContent : ComponentBase
{
    /// <summary>
    /// The content to render inside the card content area.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the card content.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the card content element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "card-content",
            Class
        );
    }
}
