using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.ToggleGroup;

public partial class ToggleGroup : LtComponentBase
{
    [Parameter] public string? variant { get; set; }
    [Parameter] public string? size { get; set; }
    [Parameter] public string? orientation { get; set; }
}