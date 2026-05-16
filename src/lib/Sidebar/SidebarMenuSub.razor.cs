namespace LowlandTech.Components.Sidebar;

public partial class SidebarMenuSub : TinyComponentBase
{
    private bool _isOpen;

    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    protected override void OnParametersSet()
    {
        _isOpen = IsOpen;
    }

    public async Task ToggleAsync()
    {
        _isOpen = !_isOpen;
        await IsOpenChanged.InvokeAsync(_isOpen);
        await InvokeAsync(StateHasChanged);
    }

    public async Task OpenAsync()
    {
        _isOpen = true;
        await IsOpenChanged.InvokeAsync(_isOpen);
        await InvokeAsync(StateHasChanged);
    }

    public async Task CloseAsync()
    {
        _isOpen = false;
        await IsOpenChanged.InvokeAsync(_isOpen);
        await InvokeAsync(StateHasChanged);
    }
}
