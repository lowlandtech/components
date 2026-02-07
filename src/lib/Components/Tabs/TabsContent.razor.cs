using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Tabs;

/// <summary>
/// TabsContent component for tab content panels.
/// Displays content when its Value matches the active tab.
/// </summary>
public partial class TabsContent : ComponentBase
{
    /// <summary>
    /// The parent Tabs component.
    /// Injected via CascadingValue.
    /// </summary>
    [CascadingParameter]
    public Tabs? ParentTabs { get; set; }

    /// <summary>
    /// The content to render inside the tab content panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The value identifying this content panel.
    /// Must match the Value of the corresponding TabsTrigger.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the tab content panel.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tab content panel.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsActive => ParentTabs?.Value == Value;

    private string GetClasses()
    {
        return CssClassUtils.Merge("tabs-content", Class);
    }
}
