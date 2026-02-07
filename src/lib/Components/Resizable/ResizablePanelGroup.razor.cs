using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Resizable;

/// <summary>
/// Resizable panel orientation.
/// </summary>
public enum ResizableOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// Container for resizable panels.
/// </summary>
public class ResizablePanelGroupBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the orientation of the resizable panel group.
    /// </summary>
    [Parameter]
    public ResizableOrientation Orientation { get; set; } = ResizableOrientation.Horizontal;

    /// <summary>
    /// Gets or sets the child content (ResizablePanel and ResizableHandle components).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the panel group.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Event raised when a panel is resized.
    /// </summary>
    [Parameter]
    public EventCallback<Dictionary<int, double>> OnResize { get; set; }

    private readonly Dictionary<int, double> _panelSizes = new();
    private int _nextPanelId = 0;

    /// <summary>
    /// Registers a panel with the group and returns its ID.
    /// </summary>
    public int RegisterPanel(double defaultSize)
    {
        var id = _nextPanelId++;
        _panelSizes[id] = defaultSize;
        return id;
    }

    /// <summary>
    /// Updates the size of a panel and triggers the resize event.
    /// </summary>
    public async Task UpdatePanelSize(int panelId, double newSize)
    {
        if (_panelSizes.ContainsKey(panelId))
        {
            _panelSizes[panelId] = newSize;
            await OnResize.InvokeAsync(_panelSizes);
        }
    }

    /// <summary>
    /// Gets the size of a specific panel.
    /// </summary>
    public double GetPanelSize(int panelId)
    {
        return _panelSizes.TryGetValue(panelId, out var size) ? size : 0;
    }

    /// <summary>
    /// Gets the combined CSS classes for the panel group.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "resizable-panel-group";
        
        if (Orientation == ResizableOrientation.Horizontal)
        {
            classes += " horizontal";
        }
        else
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
