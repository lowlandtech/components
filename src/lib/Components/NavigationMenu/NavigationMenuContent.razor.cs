using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.NavigationMenu;

/// <summary>
/// NavigationMenuContent component - dropdown content container.
/// Only visible when the parent NavigationMenuItem is open.
/// Supports animation states via data-state attribute.
/// </summary>
public partial class NavigationMenuContent : ComponentBase
{
    /// <summary>
    /// The parent NavigationMenuItem component.
    /// </summary>
    [CascadingParameter]
    public NavigationMenuItem? ParentItem { get; set; }

    /// <summary>
    /// The content to render inside the menu content area.
    /// Can include nested navigation items or any other content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the content container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the content element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("navigation-menu-content", Class);
    }
}
