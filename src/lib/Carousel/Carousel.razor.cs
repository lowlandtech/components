using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Carousel;

public partial class Carousel : LtComponentBase
{
    [Parameter] public string? Orientation { get; set; }
}