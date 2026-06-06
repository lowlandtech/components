namespace LowlandTech.Components;

public partial class Toggle : TinyComponentBase
{
    [Parameter] public string? variant { get; set; }
    [Parameter] public string? size { get; set; }
}