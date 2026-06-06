namespace LowlandTech.Components;

public partial class SidebarMenuSubTrigger : TinyComponentBase
{
    [Parameter] public bool IsExpanded { get; set; }
    [Parameter] public EventCallback<bool> IsExpandedChanged { get; set; }
    [Parameter] public EventCallback OnToggle { get; set; }

    private async Task OnClick()
    {
        await OnToggle.InvokeAsync();
    }
}
