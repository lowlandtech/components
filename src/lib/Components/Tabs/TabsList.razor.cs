using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Tabs;

/// <summary>
/// TabsList component that contains TabsTrigger components.
/// Renders a container for tab navigation buttons.
/// </summary>
public partial class TabsList : ComponentBase
{
    /// <summary>
    /// The content to render inside the tabs list.
    /// Should contain TabsTrigger components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the tabs list container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the tabs list container.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge("tabs-list", Class);
    }
}
