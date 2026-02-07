using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Resizable;

/// <summary>
/// A panel within a ResizablePanelGroup.
/// </summary>
public class ResizablePanelBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the default size of the panel (as a percentage, 0-100).
    /// </summary>
    [Parameter]
    public double DefaultSize { get; set; } = 50;

    /// <summary>
    /// Gets or sets the child content to be rendered inside the panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the panel.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the parent ResizablePanelGroup.
    /// </summary>
    [CascadingParameter]
    public ResizablePanelGroupBase? PanelGroup { get; set; }

    private int _panelId;
    private double _currentSize;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        if (PanelGroup != null)
        {
            _panelId = PanelGroup.RegisterPanel(DefaultSize);
            _currentSize = DefaultSize;
        }
    }

    /// <summary>
    /// Gets the inline style for the panel based on orientation.
    /// </summary>
    protected string GetStyle()
    {
        var size = PanelGroup?.GetPanelSize(_panelId) ?? _currentSize;
        
        if (PanelGroup?.Orientation == ResizableOrientation.Horizontal)
        {
            return $"width: {size}%;";
        }
        else
        {
            return $"height: {size}%;";
        }
    }

    /// <summary>
    /// Gets the combined CSS classes for the panel.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "resizable-panel";
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
