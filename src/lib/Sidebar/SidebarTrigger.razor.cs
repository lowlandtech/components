namespace LowlandTech.Components;

public partial class SidebarTrigger : TinyComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }

    private void OnClick()
    {
        SidebarState?.Toggle();
    }
}
