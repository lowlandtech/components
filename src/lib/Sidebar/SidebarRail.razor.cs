using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Sidebar;

public partial class SidebarRail : LtComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }

    private void OnClick()
    {
        SidebarState?.Toggle();
    }
}
