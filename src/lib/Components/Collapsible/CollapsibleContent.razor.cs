using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Collapsible;

/// <summary>
/// A component that renders content conditionally based on the collapsible's open state.
/// </summary>
public class CollapsibleContentBase : ComponentBase
{
    /// <summary>
    /// Gets the parent Collapsible component from cascading value.
    /// </summary>
    [CascadingParameter]
    public CollapsibleBase? Collapsible { get; set; }

    /// <summary>
    /// Gets or sets the child content to be rendered when the collapsible is open.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the content container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the content element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the combined CSS classes for the content container.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "collapsible-content";
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
