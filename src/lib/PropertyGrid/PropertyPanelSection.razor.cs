namespace LowlandTech.Components;

public partial class PropertyPanelSection : TinyComponentBase
{
    [Parameter, EditorRequired] public string Key { get; set; } = string.Empty;
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;
    [Parameter] public bool IsExpanded { get; set; } = true;
    [Parameter] public bool IsHidden { get; set; }
    [Parameter] public int Order { get; set; }
    [Parameter] public bool IsDefaultOpen { get; set; } = true;
    [Parameter] public RenderFragment? HeaderExtras { get; set; }

    [Parameter] public EventCallback OnToggleExpand { get; set; }
    [Parameter] public EventCallback OnMoveUp { get; set; }
    [Parameter] public EventCallback OnMoveDown { get; set; }
    [Parameter] public EventCallback OnToggleHidden { get; set; }
    [Parameter] public EventCallback OnToggleDefaultOpen { get; set; }

    private bool _isMenuOpen;

    private string OrderStyle => $"order: {Order};";

    private void ToggleMenu() => _isMenuOpen = !_isMenuOpen;

    private async Task OnToggleExpandClick()
    {
        _isMenuOpen = false;
        if (OnToggleExpand.HasDelegate)
            await OnToggleExpand.InvokeAsync();
    }

    private async Task OnMoveUpClick()
    {
        _isMenuOpen = false;
        if (OnMoveUp.HasDelegate)
            await OnMoveUp.InvokeAsync();
    }

    private async Task OnMoveDownClick()
    {
        _isMenuOpen = false;
        if (OnMoveDown.HasDelegate)
            await OnMoveDown.InvokeAsync();
    }

    private async Task OnToggleHiddenClick()
    {
        _isMenuOpen = false;
        if (OnToggleHidden.HasDelegate)
            await OnToggleHidden.InvokeAsync();
    }

    private async Task OnToggleDefaultOpenClick()
    {
        _isMenuOpen = false;
        if (OnToggleDefaultOpen.HasDelegate)
            await OnToggleDefaultOpen.InvokeAsync();
    }
}
