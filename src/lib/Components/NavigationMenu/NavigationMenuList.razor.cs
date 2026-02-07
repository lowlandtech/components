using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.NavigationMenu;

/// <summary>
/// NavigationMenuList component - container for navigation menu items.
/// Renders as an unordered list with proper ARIA attributes.
/// </summary>
public partial class NavigationMenuList : ComponentBase
{
    /// <summary>
    /// The parent NavigationMenu component.
    /// </summary>
    [CascadingParameter]
    public NavigationMenu? ParentMenu { get; set; }

    /// <summary>
    /// The content to render inside the list.
    /// Should contain NavigationMenuItem components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the list.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the list element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("navigation-menu-list", Class);
    }
}
