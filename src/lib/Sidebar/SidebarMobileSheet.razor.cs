namespace LowlandTech.Components;

public partial class SidebarMobileSheet : TinyComponentBase
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
