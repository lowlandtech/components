using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.ScrollArea;

public partial class ScrollArea : LtComponentBase
{
    [Parameter] public string? Orientation { get; set; }

    private string ViewportOverflowClass => Orientation switch
    {
        "horizontal" => "overflow-x-auto overflow-y-hidden",
        "both" => "overflow-auto",
        _ => "overflow-y-auto overflow-x-hidden"
    };
}
