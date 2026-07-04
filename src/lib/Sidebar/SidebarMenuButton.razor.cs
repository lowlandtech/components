using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components;

public partial class SidebarMenuButton : TinyComponentBase
{
    [CascadingParameter] public SidebarState? SidebarState { get; set; }
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    private async Task HandleClick(MouseEventArgs args)
    {
        if (SidebarState?.IsMobile == true)
        {
            SidebarState.SetOpenMobile(false);
        }

        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }
}
