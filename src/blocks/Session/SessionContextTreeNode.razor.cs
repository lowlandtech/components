namespace LowlandTech.Components;

public partial class SessionContextTreeNode : TinyComponentBase, IDisposable
{
    [Inject] private IPopoverService PopoverService { get; set; } = default!;
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IServiceProvider Services { get; set; } = default!;

    private string L(string key) => Localizer?[key] ?? key;

    [Parameter, EditorRequired] public SessionContextNode Item { get; set; } = new(new("context", "context", "Context"));
    [Parameter] public int Depth { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public int SiblingCount { get; set; }
    [Parameter] public SessionContextProjectionModel? Projection { get; set; }
    [Parameter] public IReadOnlyList<SessionContextPinModel> Pins { get; set; } = [];
    [Parameter] public EventCallback<SessionNodeInput> OnProject { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNavigateToPin { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnPin { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeNew { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeRename { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveUp { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeMoveDown { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnNodeDelete { get; set; }
    [Parameter] public EventCallback<SessionNodeInput> OnExpand { get; set; }
    [Parameter] public string ExpandedNodesPersistKey { get; set; } = "session-context-expanded-nodes";
    [Parameter] public string DeleteNodeSkipConfirmationKey { get; set; } = "session-context-delete-skip-confirmation";

    private readonly Guid _popoverId = Guid.NewGuid();
    private bool _registered;
    private bool _menuOpen;

    private bool _isExpanded;
    private string? _initializedId;
    private string? _hydratedNodeId;
    private bool _showDeleteDialog;
    private bool _skipDeleteConfirmation;
    private bool _isFocused;

    private bool ShowChevron => !Item.IsLoaded || VisibleChildren.Count > 0;
    private bool IsHeader => Item.Node.NodeTypeKey.EndsWith(".header", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Item.Node.NodeTypeKey, "header", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Item.Node.NodeTypeKey, "lists", StringComparison.OrdinalIgnoreCase);
    private string PopoverId => _popoverId.ToString();
    private string NodeIcon => IsHeader
        ? (_isExpanded ? Icons.FolderOpen : Icons.Folder)
        : (Icons.GetByName(Item.Node.NodeTypeKey) ?? string.Empty);
    private IReadOnlyList<SessionContextNode> VisibleChildren => Item.Children ?? [];
    private IEdgeTypeService? EdgeTypeService => Services.GetService<IEdgeTypeService>();
    private IReadOnlyList<EdgeTypeInfo> CreateEdges =>
        EdgeTypeService?.GetOutgoingEdges(Item.Node.NodeTypeKey)
            .Where(edge => string.Equals(edge.Intent, "create", StringComparison.OrdinalIgnoreCase))
            .OrderBy(edge => edge.TargetDescriptor?.Label ?? edge.Label)
            .ToList() ?? [];

    private bool IsNodePinned(string nodeId) => Pins.Any(pin => pin.Node.Id == nodeId);

    protected override void OnInitialized()
    {
        PopoverService.Register(_popoverId);
        _registered = true;
    }

    protected override void OnParametersSet()
    {
        if (_initializedId != Item.Node.Id)
        {
            _initializedId = Item.Node.Id;
            _isExpanded = Depth == 0 && Item.IsLoaded;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_hydratedNodeId != Item.Node.Id)
        {
            _hydratedNodeId = Item.Node.Id;
            await HydrateExpandedAsync();
        }
    }

    private async Task HydrateExpandedAsync()
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", ExpandedNodesPersistKey);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var expanded = JsonSerializer.Deserialize<IReadOnlyList<string>>(json);
            if (expanded?.Contains(Item.Node.Id) != true)
                return;

            if (!_isExpanded)
            {
                _isExpanded = true;
                if (!Item.IsLoaded)
                {
                    await OnExpand.InvokeAsync(Item.Node);
                }
                StateHasChanged();
            }
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best-effort when browser storage is unavailable.
        }
    }

    private async Task ToggleExpand()
    {
        _isExpanded = !_isExpanded;
        if (_isExpanded && !Item.IsLoaded)
        {
            await OnExpand.InvokeAsync(Item.Node);
        }

        await PersistExpandedAsync();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "Enter":
            case "NumpadEnter":
                await ProjectAsync();
                break;
            case " ":
            case "Spacebar":
                await ProjectAsync();
                break;
            case "ArrowDown":
                await JS.InvokeVoidAsync("ltSessionContextTree.focusNext");
                break;
            case "ArrowUp":
                await JS.InvokeVoidAsync("ltSessionContextTree.focusPrevious");
                break;
            case "ArrowRight":
                if (!_isExpanded && ShowChevron)
                {
                    await ToggleExpand();
                }
                else if (_isExpanded && VisibleChildren.Count > 0)
                {
                    await JS.InvokeVoidAsync("ltSessionContextTree.focusFirstChild");
                }
                break;
            case "ArrowLeft":
                if (_isExpanded)
                {
                    await ToggleExpand();
                }
                else if (Depth > 0)
                {
                    await JS.InvokeVoidAsync("ltSessionContextTree.focusParent");
                }
                break;
        }
    }

    private void HandleFocus(FocusEventArgs _)
    {
        _isFocused = true;
    }

    private void HandleBlur(FocusEventArgs _)
    {
        _isFocused = false;
    }

    private async Task PersistExpandedAsync()
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", ExpandedNodesPersistKey);
            var expanded = new HashSet<string>(JsonSerializer.Deserialize<IReadOnlyList<string>>(json ?? "[]") ?? [], StringComparer.Ordinal);

            if (_isExpanded)
                expanded.Add(Item.Node.Id);
            else
                expanded.Remove(Item.Node.Id);

            await JS.InvokeVoidAsync(
                "localStorage.setItem",
                ExpandedNodesPersistKey,
                JsonSerializer.Serialize(expanded));
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best-effort when browser storage is unavailable.
        }
    }

    private string GetChevronClass() => _isExpanded
        ? "text-muted-foreground transition-transform duration-100 rotate-90"
        : "text-muted-foreground transition-transform duration-100";

    private async Task ProjectAsync()
    {
        if (IsNodePinned(Item.Node.Id))
        {
            await OnNavigateToPin.InvokeAsync(Item.Node);
            return;
        }

        await OnProject.InvokeAsync(Item.Node);
    }

    private Task PinAsync() => OnPin.InvokeAsync(Item.Node);

    private async Task HandleCreateAsync()
    {
        await OnProject.InvokeAsync(Item.Node);
        await OnNodeNew.InvokeAsync(Item.Node);
    }

    private async Task HandleDeleteRequestedAsync(SessionNodeInput node)
    {
        if (await IsSkipDeleteConfirmationAsync())
        {
            await OnNodeDelete.InvokeAsync(node);
            return;
        }

        _skipDeleteConfirmation = false;
        _showDeleteDialog = true;
    }

    private async Task ConfirmDeleteAsync(SessionNodeInput node)
    {
        if (_skipDeleteConfirmation)
        {
            await PersistSkipDeleteConfirmationAsync(true);
        }

        await OnNodeDelete.InvokeAsync(node);
    }

    private Task CancelDeleteAsync()
    {
        _showDeleteDialog = false;
        return Task.CompletedTask;
    }

    private async Task<bool> IsSkipDeleteConfirmationAsync()
    {
        try
        {
            var value = await JS.InvokeAsync<string?>("localStorage.getItem", DeleteNodeSkipConfirmationKey);
            return bool.TryParse(value, out var skip) && skip;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (JSException)
        {
            return false;
        }
    }

    private async Task PersistSkipDeleteConfirmationAsync(bool skip)
    {
        try
        {
            await JS.InvokeVoidAsync("localStorage.setItem", DeleteNodeSkipConfirmationKey, skip.ToString().ToLowerInvariant());
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static rendering.
        }
        catch (JSException)
        {
            // Persistence is best-effort when browser storage is unavailable.
        }
    }

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
        builder.AddAttribute(10, nameof(SessionContextTreeNodeMenu.CreateEdges), CreateEdges);
        builder.AddAttribute(6, nameof(SessionContextTreeNodeMenu.RenameRequested), OnNodeRename);
        builder.AddAttribute(7, nameof(SessionContextTreeNodeMenu.PinRequested), OnPin);
        builder.AddAttribute(8, nameof(SessionContextTreeNodeMenu.MoveUpRequested), OnNodeMoveUp);
        builder.AddAttribute(9, nameof(SessionContextTreeNodeMenu.MoveDownRequested), OnNodeMoveDown);
        builder.AddAttribute(10, nameof(SessionContextTreeNodeMenu.DeleteRequested), EventCallback.Factory.Create<SessionNodeInput>(this, HandleDeleteRequestedAsync));
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
