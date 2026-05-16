namespace LowlandTech.Components.Tooltip;

public partial class TooltipProvider : TinyComponentBase
{
    /// <summary>
    /// The default delay in milliseconds before showing tooltips.
    /// </summary>
    [Parameter] public int Delay { get; set; } = 200;

    /// <summary>
    /// The default delay in milliseconds before hiding tooltips.
    /// </summary>
    [Parameter] public int DelayHide { get; set; } = 0;

    /// <summary>
    /// Whether tooltips are disabled globally.
    /// </summary>
    [Parameter] public bool Disabled { get; set; } = false;

    private string GetClass()
    {
        var baseClass = "cn-tooltip-provider";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
