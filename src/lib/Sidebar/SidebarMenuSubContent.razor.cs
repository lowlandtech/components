namespace LowlandTech.Components.Sidebar;

public partial class SidebarMenuSubContent : TinyComponentBase
{
    private bool _isVisible;

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

    protected override void OnParametersSet()
    {
        _isVisible = IsVisible;
    }
}
