using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.Collapsible;

/// <summary>
/// A button component that triggers the collapsible open/close state.
/// </summary>
public class CollapsibleTriggerBase : ComponentBase
{
    /// <summary>
    /// Gets the parent Collapsible component from cascading value.
    /// </summary>
    [CascadingParameter]
    public CollapsibleBase? Collapsible { get; set; }

    /// <summary>
    /// Gets or sets the child content to be rendered inside the trigger button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the trigger button.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the button element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Handles click event to toggle the collapsible.
    /// </summary>
    protected async Task HandleClick()
    {
        if (Collapsible != null)
        {
            await Collapsible.Toggle();
        }
    }

    /// <summary>
    /// Handles keyboard events for accessibility (Enter and Space).
    /// </summary>
    protected async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Key == " ")
        {
            await HandleClick();
        }
    }

    /// <summary>
    /// Gets the combined CSS classes for the trigger button.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "collapsible-trigger";
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
