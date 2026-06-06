namespace LowlandTech.Components;

public partial class ToggleGroup : TinyComponentBase
{
    [Parameter] public string? variant { get; set; }
    [Parameter] public string? size { get; set; }
    [Parameter] public string? orientation { get; set; }
}