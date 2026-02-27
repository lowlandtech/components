using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Sidebar;

public partial class SidebarMenuButton : LtComponentBase
{
    [Parameter] public bool IsActive { get; set; }
}
