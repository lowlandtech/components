namespace LowlandTech.Components;

public partial class Progress : TinyComponentBase
{
    /// <summary>
    /// The current value of the progress bar (0-Max).
    /// </summary>
    [Parameter] public double? Value { get; set; }

    /// <summary>
    /// The maximum value of the progress bar.
    /// </summary>
    [Parameter] public double Max { get; set; } = 100;

    /// <summary>
    /// Whether the progress is in an indeterminate state.
    /// </summary>
    [Parameter] public bool IsIndeterminate { get; set; } = false;

    private double GetValuePercent()
    {
        if (IsIndeterminate || !Value.HasValue)
        {
            return 0;
        }
        return Math.Min(100, Math.Max(0, (Value.Value / Max) * 100));
    }

    private string? GetAriaValueNow()
    {
        if (IsIndeterminate || !Value.HasValue)
        {
            return null;
        }
        return Value.Value.ToString();
    }

    private string? GetAriaValueText()
    {
        if (IsIndeterminate)
        {
            return "Loading...";
        }
        if (!Value.HasValue)
        {
            return null;
        }
        return $"{GetValuePercent():F0} percent";
    }

    private string GetState()
    {
        if (IsIndeterminate)
        {
            return "indeterminate";
        }
        if (!Value.HasValue)
        {
            return "loading";
        }
        return Value.Value >= Max ? "complete" : "loading";
    }

    private string GetIndicatorStyle()
    {
        if (IsIndeterminate)
        {
            return "width: 100%; animation: cn-progress-indeterminate 2s ease-in-out infinite;";
        }
        var percent = GetValuePercent();
        return $"transform: translateX(-{100 - percent}%);";
    }

    private string GetClass()
    {
        var baseClass = "cn-progress relative h-2 w-full overflow-hidden rounded-full bg-muted";
        
        if (IsIndeterminate)
        {
            baseClass += " cn-progress-indeterminate";
        }
        
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
