namespace LowlandTech.Components;

/// <summary>
/// Chart options for configuring chart behavior and appearance.
/// </summary>
public class ChartOptions
{
    /// <summary>
    /// Whether the chart should be responsive.
    /// </summary>
    public bool Responsive { get; set; } = true;

    /// <summary>
    /// Whether to maintain the aspect ratio.
    /// </summary>
    public bool MaintainAspectRatio { get; set; } = false;

    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public ChartPlugins Plugins { get; set; } = new();

    /// <summary>
    /// Scales configuration for cartesian charts.
    /// </summary>
    public ChartScales? Scales { get; set; }

    /// <summary>
    /// The axis to use for horizontal bar charts.
    /// </summary>
    public string? IndexAxis { get; set; }

    /// <summary>
    /// Interaction options.
    /// </summary>
    public ChartInteraction? Interaction { get; set; }
}

/// <summary>
/// Chart plugins configuration.
/// </summary>
public class ChartPlugins
{
    /// <summary>
    /// Legend plugin configuration.
    /// </summary>
    public ChartLegendOptions? Legend { get; set; }

    /// <summary>
    /// Tooltip plugin configuration.
    /// </summary>
    public ChartTooltipOptions? Tooltip { get; set; }
}

/// <summary>
/// Legend plugin options.
/// </summary>
public class ChartLegendOptions
{
    /// <summary>
    /// Whether to display the legend.
    /// </summary>
    public bool Display { get; set; } = true;

    /// <summary>
    /// The position of the legend.
    /// </summary>
    public string Position { get; set; } = "bottom";

    /// <summary>
    /// Alignment of the legend.
    /// </summary>
    public string? Align { get; set; }
}

/// <summary>
/// Tooltip plugin options.
/// </summary>
public class ChartTooltipOptions
{
    /// <summary>
    /// Whether to display tooltips.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Background color for tooltips.
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Title color for tooltips.
    /// </summary>
    public string? TitleColor { get; set; }

    /// <summary>
    /// Body color for tooltips.
    /// </summary>
    public string? BodyColor { get; set; }

    /// <summary>
    /// Border color for tooltips.
    /// </summary>
    public string? BorderColor { get; set; }

    /// <summary>
    /// Border width for tooltips.
    /// </summary>
    public int BorderWidth { get; set; }

    /// <summary>
    /// Padding for tooltips.
    /// </summary>
    public int Padding { get; set; } = 8;

    /// <summary>
    /// Display colors in tooltip.
    /// </summary>
    public bool DisplayColors { get; set; } = true;

    /// <summary>
    /// Use point style for tooltip colors.
    /// </summary>
    public bool UsePointStyle { get; set; } = true;
}

/// <summary>
/// Chart scales configuration.
/// </summary>
public class ChartScales
{
    public ChartAxis? X { get; set; }
    public ChartAxis? Y { get; set; }
}

/// <summary>
/// Chart axis configuration.
/// </summary>
public class ChartAxis
{
    /// <summary>
    /// Whether to display the axis grid.
    /// </summary>
    public bool? Display { get; set; }

    /// <summary>
    /// Grid configuration.
    /// </summary>
    public ChartGrid? Grid { get; set; }

    /// <summary>
    /// Ticks configuration.
    /// </summary>
    public ChartTicks? Ticks { get; set; }

    /// <summary>
    /// Border configuration.
    /// </summary>
    public ChartBorder? Border { get; set; }
}

/// <summary>
/// Grid line configuration.
/// </summary>
public class ChartGrid
{
    public bool? Display { get; set; }
    public string? Color { get; set; }
    public int? TickLength { get; set; }
}

/// <summary>
/// Tick configuration.
/// </summary>
public class ChartTicks
{
    public string? Color { get; set; }
    public string? Font { get; set; }
}

/// <summary>
/// Border configuration.
/// </summary>
public class ChartBorder
{
    public bool? Display { get; set; }
    public string? Color { get; set; }
}

/// <summary>
/// Chart interaction options.
/// </summary>
public class ChartInteraction
{
    public string Mode { get; set; } = "nearest";
    public bool Axis { get; set; }
    public bool Intersect { get; set; } = true;
}
