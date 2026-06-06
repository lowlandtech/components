namespace LowlandTech.Components.Session;

public partial class SessionContextTreeNode : TinyComponentBase, IDisposable
{
    [Inject] private IPopoverService PopoverService { get; set; } = default!;

    [Parameter, EditorRequired] public SessionContextNode Item { get; set; } = new(new("context", "context", "Context"));
    [Parameter] public int Depth { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public int SiblingCount { get; set; }
    [Parameter] public SessionContextProjectionModel? Projection { get; set; }
    [Parameter] public IReadOnlyList<SessionContextPinModel> Pins { get; set; } = [];
    [Parameter] public EventCallback<SessionNodeInput> OnProject { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnPin { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeNew { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeRename { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveUp { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveDown { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeDelete { get; set; }

    private readonly Guid _popoverId = Guid.NewGuid();
    private bool _registered;
    private bool _menuOpen;

    private bool _isExpanded = true;

    private bool HasChildren => VisibleChildren.Count > 0;
    private string PopoverId => _popoverId.ToString();
    private string NodeIcon => Icons.GetByName(Item.Node.NodeTypeKey) ?? string.Empty;
    private IReadOnlyList<SessionContextNode> VisibleChildren =>
        [.. (Item.Children ?? []).Where(child => !IsPinned(child.Node.Id))];

    protected override void OnInitialized()
    {
        PopoverService.Register(_popoverId);
        _registered = true;
    }

    private bool IsPinned(string nodeId) => Pins.Any(pin => pin.Node.Id == nodeId);

    private void ToggleExpand() => _isExpanded = !_isExpanded;

    private string GetChevronClass() => _isExpanded
        ? "text-muted-foreground transition-transform duration-100 rotate-90"
        : "text-muted-foreground transition-transform duration-100";

    private Task ProjectAsync() => OnProject.InvokeAsync(Item.Node);

    private Task PinAsync() => OnPin.InvokeAsync(Item.Node);

    private void ToggleMenuAsync()
    {
        _menuOpen = !_menuOpen;
        PopoverService.Update(_popoverId, new PopoverState
        {
            Open = _menuOpen,
            Placement = PopoverPlacement.BottomEnd,
            CssClass = "min-w-[10rem]",
            Fragment = _menuOpen ? BuildMenuContent() : null
        });
    }

    private void CloseMenu()
    {
        _menuOpen = false;
        PopoverService.Update(_popoverId, new PopoverState { Open = false });
    }

    private RenderFragment BuildMenuContent() => builder =>
    {
        builder.OpenComponent<SessionContextTreeNodeMenu>(0);
        builder.AddAttribute(1, nameof(SessionContextTreeNodeMenu.Node), Item.Node);
        builder.AddAttribute(2, nameof(SessionContextTreeNodeMenu.CanMoveUp), Index > 0);
        builder.AddAttribute(3, nameof(SessionContextTreeNodeMenu.CanMoveDown), Index < SiblingCount - 1);
        builder.AddAttribute(4, nameof(SessionContextTreeNodeMenu.CloseRequested), EventCallback.Factory.Create(this, CloseMenu));
        builder.AddAttribute(5, nameof(SessionContextTreeNodeMenu.NewRequested), OnNodeNew);
        builder.AddAttribute(6, nameof(SessionContextTreeNodeMenu.RenameRequested), OnNodeRename);
        builder.AddAttribute(7, nameof(SessionContextTreeNodeMenu.PinRequested), OnPin);
        builder.AddAttribute(8, nameof(SessionContextTreeNodeMenu.MoveUpRequested), OnNodeMoveUp);
        builder.AddAttribute(9, nameof(SessionContextTreeNodeMenu.MoveDownRequested), OnNodeMoveDown);
        builder.AddAttribute(10, nameof(SessionContextTreeNodeMenu.DeleteRequested), OnNodeDelete);
        builder.CloseComponent();
    };

    public void Dispose()
    {
        if (_registered)
        {
            PopoverService.Unregister(_popoverId);
            _registered = false;
        }
    }
}