using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components;

/// <summary>
/// An empty state placeholder component for displaying when no content is available.
/// </summary>
public class EmptyBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the title content to display.
    /// </summary>
    [Parameter]
    public RenderFragment? Title { get; set; }

    /// <summary>
    /// Gets or sets the description content to display.
    /// </summary>
    [Parameter]
    public RenderFragment? Description { get; set; }

    /// <summary>
    /// Gets or sets the action content (e.g., a button) to display.
    /// </summary>
    [Parameter]
    public RenderFragment? Action { get; set; }

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
    protected string CssClass => CssClassUtils.Merge("empty", Class);
}
