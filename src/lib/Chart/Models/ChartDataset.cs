namespace LowlandTech.Components;

/// <summary>
/// Represents a dataset for chart rendering.
/// </summary>
public class ChartDataset
{
    /// <summary>
    /// The label for the dataset shown in the legend and tooltips.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// The data values for the dataset.
    /// </summary>
    public double[] Data { get; set; } = Array.Empty<double>();

    /// <summary>
    /// The background color(s) for the dataset.
    /// </summary>
    public string[]? BackgroundColor { get; set; }

    /// <summary>
    /// The border color(s) for the dataset.
    /// </summary>
    public string[]? BorderColor { get; set; }

    /// <summary>
    /// The border width for the dataset.
    /// </summary>
    public int BorderWidth { get; set; } = 1;

    /// <summary>
    /// Whether to fill the area under the line (for line/area charts).
    /// </summary>
    public bool Fill { get; set; }

    /// <summary>
    /// The tension for line curves (0 = straight lines, 0.4 = smooth curves).
    /// </summary>
    public double Tension { get; set; } = 0;

    /// <summary>
    /// The key for looking up colors in ChartConfig.
    /// </summary>
    public string? ConfigKey { get; set; }
}

/// <summary>
/// Represents chart data with labels and datasets.
/// </summary>
public class ChartData
{
    /// <summary>
    /// The labels for the chart axes.
    /// </summary>
    public string[] Labels { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The datasets to display.
    /// </summary>
    public List<ChartDataset> Datasets { get; set; } = new();
}
