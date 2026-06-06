namespace LowlandTech.Components;

public partial class SessionContextTreeNodeMenu : TinyComponentBase
{
    [Parameter, EditorRequired] public SessionNodeInput Node { get; set; } = new("context", "context", "Context");
    [Parameter] public bool CanMoveUp { get; set; }
    [Parameter] public bool CanMoveDown { get; set; }
    [Parameter] public EventCallback CloseRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> NewRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> RenameRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> PinRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> MoveUpRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> MoveDownRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> DeleteRequested { get; set; }

    private Task CloseAsync() => CloseRequested.InvokeAsync();

    private async Task NewAsync()
    {
        await CloseRequested.InvokeAsync();
        await NewRequested.InvokeAsync(Node);
    }

    private async Task RenameAsync()
    {
        await CloseRequested.InvokeAsync();
        await RenameRequested.InvokeAsync(Node);
    }

    private async Task PinAsync()
    {
        await CloseRequested.InvokeAsync();
        await PinRequested.InvokeAsync(Node);
    }

    private async Task MoveUpAsync()
    {
        await CloseRequested.InvokeAsync();
        await MoveUpRequested.InvokeAsync(Node);
    }

    private async Task MoveDownAsync()
    {
        await CloseRequested.InvokeAsync();
        await MoveDownRequested.InvokeAsync(Node);
    }

    private async Task DeleteAsync()
    {
        await CloseRequested.InvokeAsync();
        await DeleteRequested.InvokeAsync(Node);
    }
}