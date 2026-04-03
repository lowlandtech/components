using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tooltip;

public partial class TooltipContent : LtComponentBase
{
    [CascadingParameter] private Tooltip? Parent { get; set; }

    /// <summary>
    /// The side of the trigger where the tooltip content appears.
    /// </summary>
    [Parameter] public string Side { get; set; } = "top";

    /// <summary>
    /// The alignment of the tooltip content relative to the trigger.
    /// </summary>
    [Parameter] public string Align { get; set; } = "center";

    private string GetClass()
    {
        var baseClass = "cn-tooltip-content cn-tooltip-content-logical z-50 max-w-sm animate-in fade-in-0 zoom-in-95 overflow-hidden rounded-md bg-primary px-3 py-1.5 text-xs text-primary-foreground shadow-md transition-all duration-200";
        
        // Add side-specific transform origin
        baseClass += Side switch
        {
            "top" => " origin-bottom",
            "bottom" => " origin-top",
            "left" => " origin-right",
            "right" => " origin-left",
            _ => " origin-bottom"
        };

        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
