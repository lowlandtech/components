namespace LowlandTech.Components;

public partial class Tooltip : TinyComponentBase, IDisposable
{
    private CancellationTokenSource? _showCts;

    /// <summary>
    /// Whether the tooltip is open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// The delay in milliseconds before showing the tooltip.
    /// </summary>
    [Parameter] public int Delay { get; set; } = 200;

    /// <summary>
    /// The delay in milliseconds before hiding the tooltip.
    /// </summary>
    [Parameter] public int DelayHide { get; set; } = 0;

    /// <summary>
    /// Opens the tooltip immediately without delay.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Shows the tooltip after the configured delay.
    /// </summary>
    internal async Task ShowAsync()
    {
        _showCts?.Cancel();
        _showCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(Delay, _showCts.Token);
            IsOpen = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
    }

    /// <summary>
    /// Hides the tooltip after the configured delay.
    /// </summary>
    internal async Task HideAsync()
    {
        _showCts?.Cancel();
        _showCts = new CancellationTokenSource();

        try
        {
            if (DelayHide > 0)
            {
                await Task.Delay(DelayHide, _showCts.Token);
            }
            IsOpen = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
    }

    private string GetClass()
    {
        var baseClass = "cn-tooltip relative inline-block";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }

    public void Dispose()
    {
        _showCts?.Cancel();
        _showCts?.Dispose();
    }
}
