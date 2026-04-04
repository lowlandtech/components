using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Sidebar;

public partial class SidebarMobileSheet : LtComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    private void Open()
    {
        SidebarState?.SetOpenMobile(true);
    }

    private void Close()
    {
        SidebarState?.SetOpenMobile(false);
    }
}
