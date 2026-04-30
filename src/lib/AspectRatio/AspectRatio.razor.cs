using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.AspectRatio;

public partial class AspectRatio : LtComponentBase
{
    [Parameter] public string? ratio { get; set; }
}