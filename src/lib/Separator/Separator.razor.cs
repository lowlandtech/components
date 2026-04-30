using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Separator;

public partial class Separator : LtComponentBase
{
    [Parameter] public string? orientation { get; set; }
}