namespace LowlandTech.Components;

public partial class SessionContextTreeNodeMenu : TinyComponentBase, IDisposable
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;


    [Parameter, EditorRequired] public SessionNodeInput Node { get; set; } = new("context", "context", "Context");
    [Parameter] public bool CanMoveUp { get; set; }
    [Parameter] public bool CanMoveDown { get; set; }
    [Parameter] public bool CanAddToList { get; set; }
    [Parameter] public IReadOnlyList<EdgeTypeInfo> CreateEdges { get; set; } = [];
    [Parameter] public EventCallback CloseRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> NewRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> RenameRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> PinRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> AddToListRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> MoveUpRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> MoveDownRequested { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> DeleteRequested { get; set; }

    private bool _isNewSubmenuOpen;
    private System.Threading.Timer? _closeTimer;

    public void Dispose()
    {
        _closeTimer?.Dispose();
        _closeTimer = null;
    }

    private Task CloseAsync() => CloseRequested.InvokeAsync();

    private void OpenNewSubmenu()
    {
        _closeTimer?.Dispose();
        _closeTimer = null;
        _isNewSubmenuOpen = true;
    }

    private void ScheduleCloseNewSubmenu()
    {
        _closeTimer?.Dispose();
        _closeTimer = new System.Threading.Timer(_ =>
        {
            _isNewSubmenuOpen = false;
            InvokeAsync(StateHasChanged);
        }, null, 150, System.Threading.Timeout.Infinite);
    }

    private Task ToggleNewSubmenuAsync()
    {
        _closeTimer?.Dispose();
        _closeTimer = null;
        _isNewSubmenuOpen = !_isNewSubmenuOpen;
        return Task.CompletedTask;
    }

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
        if (edge.IsNavigationHeaderChild)
        {
            properties["createHeaderNodeTypeKey"] = edge.HeaderNodeTypeKey ?? "header";
            properties["createHeaderLabel"] = edge.HeaderLabel;
        }

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

    private async Task AddToListAsync()
    {
        await CloseRequested.InvokeAsync();
        await AddToListRequested.InvokeAsync(Node);
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
