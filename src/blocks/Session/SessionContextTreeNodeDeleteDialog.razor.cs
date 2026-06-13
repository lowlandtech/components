namespace LowlandTech.Components;

public partial class SessionContextTreeNodeDeleteDialog : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public SessionNodeInput Node { get; set; } = new("context", "context", "Context");
    [Parameter] public bool SkipConfirmation { get; set; }
    [Parameter] public EventCallback<bool> SkipConfirmationChanged { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private bool _skipConfirmation;

    protected override void OnParametersSet()
    {
        _skipConfirmation = SkipConfirmation;
    }

    private async Task ConfirmAsync()
    {
        await SkipConfirmationChanged.InvokeAsync(_skipConfirmation);
        await OnConfirm.InvokeAsync(Node);
        await SetOpenAsync(false);
    }

    private async Task CancelAsync()
    {
        await OnCancel.InvokeAsync();
        await SetOpenAsync(false);
    }

    private async Task SetOpenAsync(bool isOpen)
    {
        IsOpen = isOpen;
        await IsOpenChanged.InvokeAsync(IsOpen);
    }
}
