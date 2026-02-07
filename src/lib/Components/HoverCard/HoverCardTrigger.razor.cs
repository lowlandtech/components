using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.HoverCard;

/// <summary>
/// Trigger element that opens the hover card on mouse enter.
/// Must be used inside a HoverCard component.
/// </summary>
public partial class HoverCardTrigger : ComponentBase
{
    /// <summary>
    /// The content to render inside the trigger.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional attributes to apply to the trigger element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Cascaded HoverCard context.
    /// </summary>
    [CascadingParameter]
    private HoverCard? HoverCardContext { get; set; }

    private void OnMouseEnter(MouseEventArgs e)
    {
        HoverCardContext?.OpenHoverCard();
    }

    private void OnMouseLeave(MouseEventArgs e)
    {
        HoverCardContext?.CloseHoverCard();
    }

    protected override void OnInitialized()
    {
        if (HoverCardContext == null)
        {
            throw new InvalidOperationException("HoverCardTrigger must be used inside a HoverCard component.");
        }
    }
}
