using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Card;

/// <summary>
/// CardTitle component that provides a title for the Card.
/// Based on shadcn/ui CardTitle component.
/// </summary>
public partial class CardTitle : ComponentBase
{
    /// <summary>
    /// The content to render inside the card title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the card title.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the card title element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "card-title",
            Class
        );
    }
}
