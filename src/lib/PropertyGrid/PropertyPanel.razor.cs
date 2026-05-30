namespace LowlandTech.Components;

public partial class PropertyPanel : TinyComponentBase
{
    [Parameter] public bool ShowToolbar { get; set; }
    [Parameter] public IReadOnlyList<string> HiddenPanels { get; set; } = [];
    [Parameter] public EventCallback<string> OnRestorePanel { get; set; }
    [Parameter] public EventCallback OnResetPanels { get; set; }

    private async Task OnRestorePanelClick(string key)
    {
        if (OnRestorePanel.HasDelegate)
            await OnRestorePanel.InvokeAsync(key);
    }

    private async Task OnResetPanelsClick()
    {
        if (OnResetPanels.HasDelegate)
            await OnResetPanels.InvokeAsync();
    }
}
