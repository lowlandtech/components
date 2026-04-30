using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.NativeSelect;

public partial class NativeSelect : LtComponentBase
{
    [Parameter] public string? size { get; set; }
}