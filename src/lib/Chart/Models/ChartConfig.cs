namespace LowlandTech.Components;

/// <summary>
/// Configuration for a single chart series (color and label).
/// </summary>
public class ChartSeriesConfig
{
    /// <summary>
    /// The display label for the series.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// The color for the series (hex, rgb, or CSS variable).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Optional icon identifier for the series.
    /// </summary>
    public string? Icon { get; set; }
}

/// <summary>
/// Centralized configuration for chart theming.
/// Maps series keys to their display configuration.
/// </summary>
public class ChartConfig : Dictionary<string, ChartSeriesConfig>
{
    /// <summary>
    /// Gets or sets the default colors to use when a key is not found in the config.
    /// </summary>
    public string[] DefaultColors { get; set; } = new[]
    {
        "hsl(var(--chart-1))",
        "hsl(var(--chart-2))",
        "hsl(var(--chart-3))",
        "hsl(var(--chart-4))",
        "hsl(var(--chart-5))"
    };

    /// <summary>
    /// Gets the color for a specific key, or a default color.
    /// </summary>
    public string GetColor(string key, int index = 0)
    {
        if (TryGetValue(key, out var config) && !string.IsNullOrEmpty(config.Color))
            return config.Color;
        return DefaultColors[index % DefaultColors.Length];
    }

    /// <summary>
    /// Gets the label for a specific key.
    /// </summary>
    public string GetLabel(string key)
    {
        if (TryGetValue(key, out var config) && !string.IsNullOrEmpty(config.Label))
            return config.Label;
        return key;
    }
}

/// <summary>
/// CSS variable definitions for chart theming.
/// </summary>
public static class ChartCssVariables
{
    public const string Chart1 = "--chart-1";
    public const string Chart2 = "--chart-2";
    public const string Chart3 = "--chart-3";
    public const string Chart4 = "--chart-4";
    public const string Chart5 = "--chart-5";
}
