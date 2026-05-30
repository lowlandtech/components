namespace LowlandTech.Components.Session;

public partial class ShipmentBlock : TinyComponentBase
{
    [Parameter] public SessionContextBlockModel? Block { get; set; }
    [Parameter] public SessionNodeInput? Context { get; set; }
    [Parameter] public object? Data { get; set; }
}