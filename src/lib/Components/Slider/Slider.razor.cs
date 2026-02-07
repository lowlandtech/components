using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Slider;

public partial class Slider : ComponentBase
{
    [Parameter]
    public double Value { get; set; }

    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }

    [Parameter]
    public double Min { get; set; } = 0;

    [Parameter]
    public double Max { get; set; } = 100;

    [Parameter]
    public double Step { get; set; } = 1;

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string RootClass => CssClassUtils.Merge("slider", Class);

    private string ThumbClass => CssClassUtils.Merge(
        "slider-thumb",
        Disabled ? "slider-thumb-disabled" : null
    );

    private string RangeStyle
    {
        get
        {
            var percent = GetPercent();
            return $"width: {percent}%";
        }
    }

    private string ThumbStyle
    {
        get
        {
            var percent = GetPercent();
            return $"left: {percent}%";
        }
    }

    private double GetPercent()
    {
        if (Max <= Min)
            return 0;

        var clamped = Math.Clamp(Value, Min, Max);
        return (clamped - Min) / (Max - Min) * 100.0;
    }

    private async Task HandleInputAsync(ChangeEventArgs args)
    {
        if (Disabled)
            return;

        if (args.Value == null)
            return;

        if (!double.TryParse(args.Value.ToString(), out var parsed))
            return;

        var clamped = Math.Clamp(parsed, Min, Max);
        Value = clamped;
        await ValueChanged.InvokeAsync(clamped);
    }
}
