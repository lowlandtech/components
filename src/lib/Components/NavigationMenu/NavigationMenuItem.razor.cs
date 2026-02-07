using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.NavigationMenu;

/// <summary>
/// NavigationMenuItem component - individual menu item wrapper.
/// Manages the open/closed state for its trigger and content.
/// </summary>
public partial class NavigationMenuItem : ComponentBase
{
    private bool _isOpen;

    /// <summary>
    /// The parent NavigationMenu component.
    /// </summary>
    [CascadingParameter]
    public NavigationMenu? ParentMenu { get; set; }

    /// <summary>
    /// The value that identifies this menu item.
    /// Used to track which item is active/open.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// The content to render inside the menu item.
    /// Should contain NavigationMenuTrigger and NavigationMenuContent.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the menu item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the menu item element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets whether this menu item is currently open.
    /// </summary>
    public bool IsOpen => ParentMenu?.IsActive(Value) ?? false;

    /// <summary>
    /// Toggles the open/closed state of this menu item.
    /// </summary>
    public async Task ToggleAsync()
    {
        if (ParentMenu != null)
        {
            // If currently active, close it (set to null), otherwise open it
            var newValue = IsOpen ? null : Value;
            await ParentMenu.SetValueAsync(newValue);
        }
    }

    private string GetClasses()
    {
        return CssClassUtils.Merge("navigation-menu-item", Class);
    }
}
