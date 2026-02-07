using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.NavigationMenu;

/// <summary>
/// NavigationMenu component for hierarchical navigation with dropdowns.
/// Based on shadcn/ui NavigationMenu component with dark theme styling.
/// </summary>
public partial class NavigationMenu : ComponentBase
{
    private string? _activeItem;

    /// <summary>
    /// The content to render inside the navigation menu.
    /// Should contain NavigationMenuList and child components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The currently active/open menu item value.
    /// </summary>
    [Parameter]
    public string? Value
    {
        get => _activeItem;
        set
        {
            if (_activeItem != value)
            {
                _activeItem = value;
                ValueChanged.InvokeAsync(value);
            }
        }
    }

    /// <summary>
    /// Event callback when the active menu item changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// The default active menu item if Value is not set.
    /// </summary>
    [Parameter]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// The orientation of the navigation menu.
    /// Options: "horizontal" (default), "vertical".
    /// </summary>
    [Parameter]
    public string Orientation { get; set; } = "horizontal";

    /// <summary>
    /// Optional CSS classes to apply to the navigation menu.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the navigation menu.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void OnInitialized()
    {
        if (_activeItem == null && DefaultValue != null)
        {
            _activeItem = DefaultValue;
        }
    }

    /// <summary>
    /// Sets the active menu item value.
    /// Called by NavigationMenuTrigger components.
    /// </summary>
    public async Task SetValueAsync(string? value)
    {
        if (_activeItem != value)
        {
            _activeItem = value;
            await ValueChanged.InvokeAsync(value);
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Checks if a menu item is currently active/open.
    /// </summary>
    public bool IsActive(string? value)
    {
        return _activeItem == value;
    }

    private string GetClasses()
    {
        var baseClasses = "navigation-menu";
        if (Orientation == "vertical")
        {
            baseClasses += " navigation-menu-vertical";
        }
        else
        {
            baseClasses += " navigation-menu-horizontal";
        }
        return CssClassUtils.Merge(baseClasses, Class);
    }
}
