using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Resizable;

/// <summary>
/// A draggable handle between resizable panels.
/// </summary>
public class ResizableHandleBase : ComponentBase
{
    /// <summary>
    /// Gets or sets custom content for the handle.
    /// If not provided, a default line indicator will be rendered.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the handle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the parent ResizablePanelGroup.
    /// </summary>
    [CascadingParameter]
    public ResizablePanelGroupBase? PanelGroup { get; set; }

    /// <summary>
    /// Gets the combined CSS classes for the handle.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "resizable-handle";
        
        if (PanelGroup?.Orientation == ResizableOrientation.Horizontal)
        {
            classes += " horizontal";
        }
        else if (PanelGroup?.Orientation == ResizableOrientation.Vertical)
        {
            classes += " vertical";
        }
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
