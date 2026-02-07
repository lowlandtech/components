using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Tabs;

/// <summary>
/// TabsTrigger component for individual tab buttons.
/// Renders a button that switches between tab content panels.
/// </summary>
public partial class TabsTrigger : ComponentBase
{
    /// <summary>
    /// The parent Tabs component.
    /// Injected via CascadingValue.
    /// </summary>
    [CascadingParameter]
    public Tabs? ParentTabs { get; set; }

    /// <summary>
    /// The content to render inside the tab trigger button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The value identifying this tab.
    /// Must match the Value of the corresponding TabsContent.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Whether this tab trigger is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the tab trigger button.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tab trigger button.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsActive => ParentTabs?.Value == Value;

    private async Task HandleClickAsync()
    {
        if (ParentTabs != null && !Disabled && Value != null)
        {
            await ParentTabs.SetValueAsync(Value);
        }
    }

    private string GetClasses()
    {
        var baseClasses = "tabs-trigger";
        if (IsActive)
        {
            baseClasses += " tabs-trigger-active";
        }
        if (Disabled)
        {
            baseClasses += " tabs-trigger-disabled";
        }
        return CssClassUtils.Merge(baseClasses, Class);
    }
}
