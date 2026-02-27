using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Sidebar;

public partial class SidebarMenuSubButton : LtComponentBase
{
    [Parameter] public bool IsActive { get; set; }
}
