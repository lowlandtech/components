namespace LowlandTech.Components.Work;

public partial class WorkErrorBanner : TinyComponentBase
{
    [Parameter] public string? Error { get; set; }
    [Parameter] public EventCallback OnDismiss { get; set; }

    private async Task DismissAsync()
    {
        await OnDismiss.InvokeAsync();
    }
}
