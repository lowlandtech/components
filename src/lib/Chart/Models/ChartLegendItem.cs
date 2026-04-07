namespace LowlandTech.Components.Chart.Models;

/// <summary>
/// Represents a single item in the chart legend.
/// </summary>
public class ChartLegendItem
{
    /// <summary>
    /// The unique key for this legend item.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The display label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The color for the legend indicator.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Optional icon for the legend indicator.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional value to display.
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Whether the item is visible (for toggling).
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Whether the item is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Toggles the visibility of this item.
    /// </summary>
    public void ToggleVisibility()
    {
        IsVisible = !IsVisible;
    }
}
