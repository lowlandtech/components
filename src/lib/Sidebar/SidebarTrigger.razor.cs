using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Sidebar;

public partial class SidebarTrigger : LtComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }

    private void OnClick()
    {
        SidebarState?.Toggle();
    }
}
