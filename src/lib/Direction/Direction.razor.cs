using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Direction;

public partial class Direction : LtComponentBase
{
    [Parameter] public string? dir { get; set; }
}