namespace LowlandTech.Components.Sidebar;

public partial class SidebarRail : TinyComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }

    private void OnClick()
    {
        SidebarState?.Toggle();
    }
}
