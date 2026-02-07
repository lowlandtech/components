using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Card;

/// <summary>
/// CardDescription component that provides a description text for the Card.
/// Based on shadcn/ui CardDescription component.
/// </summary>
public partial class CardDescription : ComponentBase
{
    /// <summary>
    /// The content to render inside the card description.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the card description.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the card description element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "card-description",
            Class
        );
    }
}
