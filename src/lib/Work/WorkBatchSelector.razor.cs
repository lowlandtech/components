namespace LowlandTech.Components.Work;

public partial class WorkBatchSelector : TinyComponentBase
{
    [Parameter] public bool IsAllSelected { get; set; }
    [Parameter] public int SelectedCount { get; set; }
    [Parameter] public int TotalCount { get; set; }
    [Parameter] public EventCallback<bool> OnSelectAllChanged { get; set; }

    private async Task OnSelectAllChangedAsync(bool value)
    {
        await OnSelectAllChanged.InvokeAsync(value);
    }
}
