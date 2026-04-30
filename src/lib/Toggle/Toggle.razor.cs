using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Toggle;

public partial class Toggle : LtComponentBase
{
    [Parameter] public string? variant { get; set; }
    [Parameter] public string? size { get; set; }
}