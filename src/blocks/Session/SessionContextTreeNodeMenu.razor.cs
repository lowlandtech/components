namespace LowlandTech.Components;

public partial class SessionContextTreeNodeMenu : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;


    [Parameter, EditorRequired] public SessionNodeInput Node { get; set; } = new("context", "context", "Context");
    [Parameter] public bool CanMoveUp { get; set; }
    [Parameter] public bool CanMoveDown { get; set; }
    [Parameter] public IReadOnlyList<EdgeTypeInfo> CreateEdges { get; set; } = [];
    [Parameter] public EventCallback CloseRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> NewRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> RenameRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> PinRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> MoveUpRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> MoveDownRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> DeleteRequested { get; set; }

    private Task CloseAsync() => CloseRequested.InvokeAsync();

    private async Task NewAsync() => await NewAsync(null);

    private async Task NewAsync(EdgeTypeInfo? edge)
    {
        await CloseRequested.InvokeAsync();

        if (edge is null)
        {
            await NewRequested.InvokeAsync(Node);
            return;
        }

        var properties = new Dictionary<string, object?>(Node.Properties ?? new Dictionary<string, object?>());
        properties["createEdgeKey"] = edge.EdgeKey;
        await NewRequested.InvokeAsync(Node with { Properties = properties });
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