using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components;

/// <summary>
/// A list item component with consistent spacing and leading/trailing content slots.
/// </summary>
public class ItemBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the leading content (e.g., icon, avatar).
    /// </summary>
    [Parameter]
    public RenderFragment? Leading { get; set; }

    /// <summary>
    /// Gets or sets the trailing content (e.g., badge, chevron).
    /// </summary>
    [Parameter]
    public RenderFragment? Trailing { get; set; }

    /// <summary>
    /// Gets or sets the main content of the item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the computed CSS class for the component.
    /// </summary>
    protected string CssClass => CssClassUtils.Merge("item", Class);
}
