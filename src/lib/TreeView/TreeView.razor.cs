namespace LowlandTech.Components;

public partial class TreeView<TItem> : TinyComponentBase
{
    [Parameter, EditorRequired] public IEnumerable<TItem> Items { get; set; } = default!;
    [Parameter, EditorRequired] public Func<TItem, IEnumerable<TItem>> GetChildren { get; set; } = default!;
    [Parameter] public Func<TItem, string>? GetKey { get; set; }
    [Parameter] public Func<TItem, bool>? HasChildren { get; set; }

    // Expansion
    /// <summary>Legacy expansion predicate. Use <see cref="ExpandedKeys"/> for controlled state.</summary>
    [Parameter] public Func<TItem, bool>? IsExpanded { get; set; }
    [Parameter] public IReadOnlyList<string>? ExpandedKeys { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<string>> ExpandedKeysChanged { get; set; }
    [Parameter] public EventCallback<TreeViewExpandEventArgs<TItem>> ExpandedChanged { get; set; }
    protected override Type DesignType => typeof(TreeViewDesigner);

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Switch)]
    public bool SingleExpand { get; set; }

    [Parameter, PropertyGrid(Group = "Behavior", Editor = PropertyEditor.Switch)]
    public bool AutoExpand { get; set; }

    // Selection
    [Parameter, PropertyGrid(Group = "Behavior")]
    public TreeViewSelectionMode SelectionMode { get; set; } = TreeViewSelectionMode.None;
    [Parameter] public string? SelectedKey { get; set; }
    [Parameter] public EventCallback<string?> SelectedKeyChanged { get; set; }
    [Parameter] public IReadOnlyList<string>? SelectedKeys { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<string>> SelectedKeysChanged { get; set; }
    [Parameter] public EventCallback<TreeViewSelectEventArgs<TItem>> SelectedChanged { get; set; }

    // Checking
    [Parameter, PropertyGrid(Group = "Behavior")]
    public TreeViewCheckMode CheckMode { get; set; } = TreeViewCheckMode.None;
    [Parameter] public IReadOnlyList<string>? CheckedKeys { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<string>> CheckedKeysChanged { get; set; }

    // Filtering
    [Parameter, PropertyGrid(Group = "Content", Editor = PropertyEditor.Text, Placeholder = "Filter nodes…")]
    public string? FilterQuery { get; set; }
    [Parameter] public Func<TItem, string>? GetText { get; set; }

    // Templates
    [Parameter] public RenderFragment<TreeNodeContext<TItem>>? ItemTemplate { get; set; }
    [Parameter] public RenderFragment<TreeNodeContext<TItem>>? BodyContent { get; set; }
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    // Persistence
    [Parameter] public string? PersistKey { get; set; }
    [Parameter] public bool NoPersist { get; set; }

    private string EffectivePersistKey =>
        PersistKey ?? GuidV5.Create(TreeViewKeyNamespace, string.Join("|", Items?.Select(GetItemKey) ?? [])).ToString("N");

    // Appearance
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool Dense { get; set; }

    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Switch)]
    public bool Hover { get; set; } = true;
    [Parameter, PropertyGrid(Group = "Sizing", Editor = PropertyEditor.Text, Placeholder = "e.g. 100%")]
    public string? Width { get; set; }

    [Parameter, PropertyGrid(Group = "Sizing", Editor = PropertyEditor.Text, Placeholder = "e.g. 400px")]
    public string? MaxHeight { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private HashSet<string> _expandedKeys = new();
    private HashSet<string> _selectedKeys = new();
    private HashSet<string> _checkedKeys = new();
    private string? _focusedKey;
    private string[]? _lastExpandedKeys;
    private string[]? _lastSelectedKeys;
    private string? _lastSelectedKey;
    private string[]? _lastCheckedKeys;
    private bool _expandedKeysBound;
    private bool _selectedKeysBound;
    private bool _checkedKeysBound;

    // Tracks depth of each expanded key for SingleExpand sibling logic
    private readonly Dictionary<string, int> _expandedKeyDepths = new();

    // Namespace UUID for TreeView key derivation (stable, arbitrary v4 constant)
    private static readonly Guid TreeViewKeyNamespace = new("7f3e2a1b-4c5d-5000-8000-9e8f7a6b5c4d");
    // Cache: item → stable key (cleared when Items reference changes)\n    // Uses reference equality so structurally-identical but distinct instances
    // each get their own path-derived key.
    private readonly Dictionary<TItem, string> _keyCache =
        typeof(TItem).IsValueType
            ? new Dictionary<TItem, string>()
#pragma warning disable CS8714
            : new Dictionary<TItem, string>(TreeViewRefComparer<TItem>.Instance);
#pragma warning restore CS8714
    private IEnumerable<TItem>? _lastItems;

    internal string GetItemKey(TItem item)
    {
        if (GetKey != null) return GetKey(item);
        if (_keyCache.TryGetValue(item, out var cached)) return cached;
        var path = BuildItemPath(item);
        var key = GuidV5.Create(TreeViewKeyNamespace, path).ToString("N");
        _keyCache[item] = key;
        return key;
    }

    private string BuildItemPath(TItem item)
    {
        var label = GetText?.Invoke(item) ?? item?.ToString() ?? string.Empty;
        var parent = FindParent(item);
        return parent == null ? label : BuildItemPath(parent) + "/" + label;
    }

    private int GetDepth(TItem item)
    {
        foreach (var root in Items)
        {
            var depth = GetDepthRecursive(root, item, 0);
            if (depth >= 0) return depth;
        }
        return 0;
    }

    private int GetDepthRecursive(TItem current, TItem target, int currentDepth)
    {
        if (EqualityComparer<TItem>.Default.Equals(current, target))
            return currentDepth;

        var children = GetChildren(current);
        if (children == null) return -1;

        foreach (var child in children)
        {
            var found = GetDepthRecursive(child, target, currentDepth + 1);
            if (found >= 0) return found;
        }
        return -1;
    }

    private int GetDepthByKey(string key)
    {
        foreach (var root in Items)
        {
            var depth = GetDepthByKeyRecursive(root, key, 0);
            if (depth >= 0) return depth;
        }
        return 0;
    }

    private int GetDepthByKeyRecursive(TItem current, string key, int currentDepth)
    {
        if (GetItemKey(current) == key)
            return currentDepth;

        var children = GetChildren(current);
        if (children == null) return -1;

        foreach (var child in children)
        {
            var found = GetDepthByKeyRecursive(child, key, currentDepth + 1);
            if (found >= 0) return found;
        }
        return -1;
    }

    internal bool IsItemExpanded(TItem item)
    {
        var key = GetItemKey(item);
        if (_expandedKeys.Contains(key)) return true;
        if (IsExpanded != null) return IsExpanded(item);
        return false;
    }
    internal bool IsItemSelected(TItem item) => _selectedKeys.Contains(GetItemKey(item));
    internal bool IsItemChecked(TItem item) => _checkedKeys.Contains(GetItemKey(item));
    internal bool IsItemFocused(TItem item) => GetItemKey(item) == _focusedKey;

    internal bool NodeHasChildren(TItem item)
    {
        var children = GetChildren(item);
        if (children != null && children.Any()) return true;
        return HasChildren?.Invoke(item) == true;
    }

    protected override void OnParametersSet()
    {
        // Invalidate key cache when the Items collection reference changes
        if (!ReferenceEquals(_lastItems, Items))
        {
            _keyCache.Clear();
            _lastItems = Items;
        }

        // Bind controlled key parameters when their contents change. Hosts may
        // reuse the same list instance when hydrating URL or storage state.
        if (ExpandedKeys != null)
        {
            if (!_expandedKeysBound || !HaveSameKeys(_lastExpandedKeys, ExpandedKeys))
            {
                _expandedKeys = new HashSet<string>(ExpandedKeys);
                _expandedKeyDepths.Clear();
                foreach (var k in _expandedKeys)
                    _expandedKeyDepths[k] = GetDepthByKey(k);
                _lastExpandedKeys = SnapshotKeys(ExpandedKeys);
                _expandedKeysBound = true;
            }
        }
        else if (_expandedKeysBound)
        {
            _expandedKeys.Clear();
            _expandedKeyDepths.Clear();
            _lastExpandedKeys = null;
            _expandedKeysBound = false;
        }

        // Bind controlled SelectedKeys / SelectedKey when their contents change.
        if (SelectedKeys != null)
        {
            if (!_selectedKeysBound || !HaveSameKeys(_lastSelectedKeys, SelectedKeys))
            {
                _selectedKeys = new HashSet<string>(SelectedKeys);
                _lastSelectedKeys = SnapshotKeys(SelectedKeys);
                _lastSelectedKey = null;
                _selectedKeysBound = true;
            }
        }
        else if (SelectedKey != null)
        {
            if (!_selectedKeysBound || _lastSelectedKey != SelectedKey)
            {
                _selectedKeys = new HashSet<string> { SelectedKey };
                _lastSelectedKey = SelectedKey;
                _selectedKeysBound = true;
            }
        }
        else if (_selectedKeysBound)
        {
            _selectedKeys.Clear();
            _lastSelectedKeys = null;
            _lastSelectedKey = null;
            _selectedKeysBound = false;
        }

        // Bind controlled CheckedKeys when its contents change.
        if (CheckedKeys != null)
        {
            if (!_checkedKeysBound || !HaveSameKeys(_lastCheckedKeys, CheckedKeys))
            {
                _checkedKeys = new HashSet<string>(CheckedKeys);
                _lastCheckedKeys = SnapshotKeys(CheckedKeys);
                _checkedKeysBound = true;
            }
        }
        else if (_checkedKeysBound)
        {
            _checkedKeys.Clear();
            _lastCheckedKeys = null;
            _checkedKeysBound = false;
        }
    }

    private static string[] SnapshotKeys(IReadOnlyList<string> keys) => keys.ToArray();

    private static bool HaveSameKeys(string[]? previousKeys, IReadOnlyList<string> nextKeys)
    {
        if (previousKeys is null || previousKeys.Length != nextKeys.Count)
            return false;

        for (var i = 0; i < previousKeys.Length; i++)
        {
            if (!string.Equals(previousKeys[i], nextKeys[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !NoPersist)
        {
            var needsRender = false;

            if (ExpandedKeys == null)
            {
                var expandJson = await JS.InvokeAsync<string?>("localStorage.getItem", $"treeview:{EffectivePersistKey}:expanded");
                if (!string.IsNullOrEmpty(expandJson))
                {
                    var keys = JsonSerializer.Deserialize<List<string>>(expandJson);
                    if (keys != null && keys.Count > 0)
                    {
                        _expandedKeys = new HashSet<string>(keys);
                        _expandedKeyDepths.Clear();
                        foreach (var k in _expandedKeys)
                            _expandedKeyDepths[k] = GetDepthByKey(k);
                        needsRender = true;
                    }
                }
            }

            if (SelectedKeys == null && SelectedKey == null && SelectionMode != TreeViewSelectionMode.None)
            {
                var selectedJson = await JS.InvokeAsync<string?>("localStorage.getItem", $"treeview:{EffectivePersistKey}:selected");
                if (!string.IsNullOrEmpty(selectedJson))
                {
                    var keys = JsonSerializer.Deserialize<List<string>>(selectedJson);
                    if (keys != null && keys.Count > 0)
                    {
                        _selectedKeys = new HashSet<string>(keys);
                        needsRender = true;
                    }
                }
            }

            if (needsRender) StateHasChanged();
        }
    }

    private void PersistExpandedKeys()
    {
        if (NoPersist) return;
        var json = JsonSerializer.Serialize(_expandedKeys.ToList());
        JS.InvokeVoidAsync("localStorage.setItem", $"treeview:{EffectivePersistKey}:expanded", json);
    }

    private void PersistSelectedKeys()
    {
        if (NoPersist) return;
        var json = JsonSerializer.Serialize(_selectedKeys.ToList());
        JS.InvokeVoidAsync("localStorage.setItem", $"treeview:{EffectivePersistKey}:selected", json);
    }

    internal void ToggleExpand(TItem item)
    {
        var key = GetItemKey(item);
        var expanding = !_expandedKeys.Contains(key);
        var depth = GetDepth(item);

        if (SingleExpand && expanding)
        {
            // Collapse siblings at the same depth only
            var keysToRemove = _expandedKeyDepths
                .Where(kv => kv.Value == depth && kv.Key != key)
                .Select(kv => kv.Key)
                .ToList();
            foreach (var k in keysToRemove)
            {
                _expandedKeys.Remove(k);
                _expandedKeyDepths.Remove(k);
            }
        }

        if (expanding)
        {
            _expandedKeys.Add(key);
            _expandedKeyDepths[key] = depth;
        }
        else
        {
            _expandedKeys.Remove(key);
            _expandedKeyDepths.Remove(key);
        }

        PersistExpandedKeys();
        ExpandedKeysChanged.InvokeAsync(_expandedKeys.ToList());
        ExpandedChanged.InvokeAsync(new TreeViewExpandEventArgs<TItem>
        {
            Item = item,
            Key = key,
            Expanded = expanding
        });
        StateHasChanged();
    }

    internal void SelectNode(TItem item)
    {
        var key = GetItemKey(item);
        if (SelectionMode == TreeViewSelectionMode.None) return;

        bool selected;
        bool stateChanged;
        if (SelectionMode == TreeViewSelectionMode.Single)
        {
            selected = true;
            stateChanged = _selectedKeys.Count != 1 || !_selectedKeys.Contains(key);
            if (stateChanged)
            {
                _selectedKeys.Clear();
                _selectedKeys.Add(key);
            }
        }
        else if (SelectionMode == TreeViewSelectionMode.Toggle)
        {
            selected = !_selectedKeys.Remove(key);
            if (selected) _selectedKeys.Add(key);
            stateChanged = true; // toggle always changes state
        }
        else // Multi
        {
            selected = !_selectedKeys.Remove(key);
            if (selected) _selectedKeys.Add(key);
            stateChanged = true; // multi always changes state
        }

        if (stateChanged)
        {
            SelectedKeysChanged.InvokeAsync(_selectedKeys.ToList());
            if (_selectedKeys.Count == 1)
                SelectedKeyChanged.InvokeAsync(_selectedKeys.First());
            else
                SelectedKeyChanged.InvokeAsync(null);

            SelectedChanged.InvokeAsync(new TreeViewSelectEventArgs<TItem>
            {
                Item = item,
                Key = key,
                Selected = selected
            });

            PersistSelectedKeys();
        }

        if (AutoExpand && selected)
        {
            ExpandAncestors(item);
        }

        StateHasChanged();
    }

    internal void ToggleCheck(TItem item)
    {
        var key = GetItemKey(item);
        var checking = !_checkedKeys.Contains(key);

        if (CheckMode == TreeViewCheckMode.TriState)
        {
            ApplyCheckRecursive(item, checking);
            RecalculateIndeterminateStates();
        }
        else
        {
            if (checking)
                _checkedKeys.Add(key);
            else
                _checkedKeys.Remove(key);
        }

        CheckedKeysChanged.InvokeAsync(_checkedKeys.ToList());
        StateHasChanged();
    }

    private void ApplyCheckRecursive(TItem item, bool checking)
    {
        var key = GetItemKey(item);
        if (checking)
            _checkedKeys.Add(key);
        else
            _checkedKeys.Remove(key);

        var children = GetChildren(item);
        if (children != null)
        {
            foreach (var child in children)
            {
                ApplyCheckRecursive(child, checking);
            }
        }
    }

    private void RecalculateIndeterminateStates()
    {
        // Indeterminate is computed on-the-fly in TreeNode via IsIndeterminate
        // We just need to ensure _checkedKeys is correct after cascade.
        // Parents are NOT in _checkedKeys unless explicitly checked or all children checked.
        foreach (var root in Items)
        {
            SyncParentCheckState(root);
        }
    }

    private bool SyncParentCheckState(TItem item)
    {
        var children = GetChildren(item)?.ToList() ?? new List<TItem>();
        if (children.Count == 0) return _checkedKeys.Contains(GetItemKey(item));

        var childStates = children.Select(SyncParentCheckState).ToList();
        var allChecked = childStates.All(s => s);
        var anyChecked = childStates.Any(s => s);
        var key = GetItemKey(item);

        if (allChecked)
        {
            _checkedKeys.Add(key);
        }
        else
        {
            _checkedKeys.Remove(key);
        }

        return allChecked || anyChecked && _checkedKeys.Contains(key);
    }

    internal bool IsIndeterminate(TItem item)
    {
        if (CheckMode != TreeViewCheckMode.TriState) return false;
        var children = GetChildren(item)?.ToList() ?? new List<TItem>();
        if (children.Count == 0) return false;

        var key = GetItemKey(item);
        if (_checkedKeys.Contains(key)) return false;

        var anyChildChecked = children.Any(c => _checkedKeys.Contains(GetItemKey(c)) || IsIndeterminate(c));
        return anyChildChecked;
    }

    private void ExpandAncestors(TItem item)
    {
        // Find item in tree and expand all ancestors
        foreach (var root in Items)
        {
            if (ExpandPath(root, item))
                break;
        }
    }

    private bool ExpandPath(TItem current, TItem target)
    {
        if (EqualityComparer<TItem>.Default.Equals(current, target))
            return true;

        var children = GetChildren(current);
        if (children == null) return false;

        foreach (var child in children)
        {
            if (ExpandPath(child, target))
            {
                _expandedKeys.Add(GetItemKey(current));
                _expandedKeyDepths[GetItemKey(current)] = GetDepth(current);
                return true;
            }
        }
        return false;
    }

    internal void FocusNode(TItem item)
    {
        _focusedKey = GetItemKey(item);
    }

    internal void HandleKeyDown(KeyboardEventArgs e)
    {
        var visible = GetVisibleItems().ToList();
        if (visible.Count == 0) return;

        var focusedIndex = visible.FindIndex(i => GetItemKey(i) == _focusedKey);
        if (focusedIndex < 0) focusedIndex = 0;

        var handled = true;
        switch (e.Key)
        {
            case "ArrowDown":
                if (focusedIndex < visible.Count - 1)
                    _focusedKey = GetItemKey(visible[focusedIndex + 1]);
                break;
            case "ArrowUp":
                if (focusedIndex > 0)
                    _focusedKey = GetItemKey(visible[focusedIndex - 1]);
                break;
            case "ArrowRight":
                {
                    var current = visible[focusedIndex];
                    if (!IsItemExpanded(current) && NodeHasChildren(current))
                        ToggleExpand(current);
                    else if (focusedIndex < visible.Count - 1)
                        _focusedKey = GetItemKey(visible[focusedIndex + 1]);
                }
                break;
            case "ArrowLeft":
                {
                    var current = visible[focusedIndex];
                    if (IsItemExpanded(current))
                        ToggleExpand(current);
                    else
                    {
                        // Move focus to parent
                        var parent = FindParent(current);
                        if (parent != null)
                            _focusedKey = GetItemKey(parent);
                    }
                }
                break;
            case "Home":
                _focusedKey = GetItemKey(visible[0]);
                break;
            case "End":
                _focusedKey = GetItemKey(visible[^1]);
                break;
            case "Enter":
            case " ":
                SelectNode(visible[focusedIndex]);
                break;
            default:
                handled = false;
                break;
        }

        if (handled)
        {
            StateHasChanged();
        }
    }

    private IEnumerable<TItem> GetVisibleItems()
    {
        foreach (var item in Items)
        {
            foreach (var visible in GetVisibleDescendants(item))
                yield return visible;
        }
    }

    private IEnumerable<TItem> GetVisibleDescendants(TItem item)
    {
        yield return item;
        if (IsItemExpanded(item))
        {
            var children = GetChildren(item);
            if (children != null)
            {
                foreach (var child in children)
                {
                    foreach (var descendant in GetVisibleDescendants(child))
                        yield return descendant;
                }
            }
        }
    }

    private TItem? FindParent(TItem target)
    {
        foreach (var root in Items)
        {
            var parent = FindParentRecursive(root, target);
            if (parent != null) return parent;
        }
        return default;
    }

    private TItem? FindParentRecursive(TItem current, TItem target)
    {
        var children = GetChildren(current);
        if (children == null) return default;

        foreach (var child in children)
        {
            // Use reference equality for reference types so structurally-identical
            // but distinct instances (e.g. two Node("lib", []) records) are located
            // correctly without confusing them.
            var match = typeof(TItem).IsValueType
                ? EqualityComparer<TItem>.Default.Equals(child, target)
                : object.ReferenceEquals(child, target);
            if (match)
                return current;
            var found = FindParentRecursive(child, target);
            if (found != null) return found;
        }
        return default;
    }

    public async Task ExpandAll()
    {
        foreach (var item in GetAllItems())
        {
            if (NodeHasChildren(item))
            {
                var key = GetItemKey(item);
                _expandedKeys.Add(key);
                _expandedKeyDepths[key] = GetDepth(item);
            }
        }
        await ExpandedKeysChanged.InvokeAsync(_expandedKeys.ToList());
        await InvokeAsync(StateHasChanged);
    }

    public async Task CollapseAll()
    {
        _expandedKeys.Clear();
        await ExpandedKeysChanged.InvokeAsync(_expandedKeys.ToList());
        await InvokeAsync(StateHasChanged);
    }

    private IEnumerable<TItem> GetAllItems()
    {
        foreach (var root in Items)
        {
            yield return root;
            foreach (var child in GetAllDescendants(root))
                yield return child;
        }
    }

    private IEnumerable<TItem> GetAllDescendants(TItem item)
    {
        var children = GetChildren(item);
        if (children == null) yield break;

        foreach (var child in children)
        {
            yield return child;
            foreach (var descendant in GetAllDescendants(child))
                yield return descendant;
        }
    }

    public bool MatchesFilter(TItem item)
    {
        if (string.IsNullOrWhiteSpace(FilterQuery)) return true;
        var text = GetText?.Invoke(item) ?? item?.ToString() ?? string.Empty;
        return text.Contains(FilterQuery, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsVisibleInFilter(TItem item)
    {
        if (string.IsNullOrWhiteSpace(FilterQuery)) return true;
        if (MatchesFilter(item)) return true;
        // Show if any descendant matches
        var children = GetChildren(item);
        if (children == null) return false;
        return children.Any(child => IsVisibleInFilter(child));
    }
}
