using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.NavigationMenu;

/// <summary>
/// NavigationMenuTrigger component - clickable button that toggles menu content.
/// Supports keyboard navigation with Enter, Space, and arrow keys.
/// </summary>
public partial class NavigationMenuTrigger : ComponentBase
{
    /// <summary>
    /// The parent NavigationMenuItem component.
    /// </summary>
    [CascadingParameter]
    public NavigationMenuItem? ParentItem { get; set; }

    /// <summary>
    /// The content to render inside the trigger button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the trigger.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the trigger button.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private async Task HandleClickAsync()
    {
        if (ParentItem != null)
        {
            await ParentItem.ToggleAsync();
        }
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (ParentItem == null)
            return;

        switch (e.Key)
        {
            case "Enter":
            case " ": // Space key
                await ParentItem.ToggleAsync();
                break;

            case "ArrowDown":
            case "ArrowRight":
                // For keyboard navigation support (UAC19)
                // In a full implementation, this would move focus to next item
                if (!ParentItem.IsOpen)
                {
                    await ParentItem.ToggleAsync();
                }
                break;

            case "ArrowUp":
            case "ArrowLeft":
                // For keyboard navigation support (UAC19)
                // In a full implementation, this would move focus to previous item
                if (ParentItem.IsOpen)
                {
                    await ParentItem.ToggleAsync();
                }
                break;

            case "Escape":
                // Close the menu on Escape
                if (ParentItem.IsOpen)
                {
                    await ParentItem.ToggleAsync();
                }
                break;
        }
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge("navigation-menu-trigger", Class);
    }
}
