namespace LowlandTech.Components.Sidebar;

public partial class SidebarTrigger : TinyComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }

    private void OnClick()
    {
        SidebarState?.Toggle();
    }
}
