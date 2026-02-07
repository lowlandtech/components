using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Card;

/// <summary>
/// CardFooter component that provides a footer section for the Card.
/// Based on shadcn/ui CardFooter component.
/// </summary>
public partial class CardFooter : ComponentBase
{
    /// <summary>
    /// The content to render inside the card footer.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the card footer.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the card footer element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "card-footer",
            Class
        );
    }
}
