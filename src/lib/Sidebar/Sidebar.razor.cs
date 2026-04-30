using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Sidebar;

public partial class Sidebar : LtComponentBase
{
    [Parameter] public bool defaultOpen { get; set; }
}