using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Card;

/// <summary>
/// CardHeader component that provides a header section for the Card.
/// Based on shadcn/ui CardHeader component.
/// </summary>
public partial class CardHeader : ComponentBase
{
    /// <summary>
    /// The content to render inside the card header.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the card header.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the card header element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "card-header",
            Class
        );
    }
}
