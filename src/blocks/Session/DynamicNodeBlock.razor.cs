namespace LowlandTech.Components;

public partial class DynamicNodeBlock : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    [Parameter] public SessionContextBlockModel? Block { get; set; }
    [Parameter] public SessionNodeInput? Context { get; set; }
    [Parameter] public object? Data { get; set; }
    [Parameter] public string Mode { get; set; } = SessionContextBlock.ModeNormal;

    private object? _sourceModel;
    private object? _draftModel;
    private object? _savedModel;
    private readonly List<object> _createdItems = [];
    private readonly List<SessionNodeInput> _createdNodes = [];
    private string _filterText = string.Empty;
    private int _pageIndex;
    private int _pageSize = 10;
    private bool _isEditing;

    private string L(string key) => Localizer?[key] ?? key;

    private IReadOnlyDictionary<string, object?> Properties =>
        Context?.Properties
        ?? Block?.Data as IReadOnlyDictionary<string, object?>
        ?? Data as IReadOnlyDictionary<string, object?>
        ?? new Dictionary<string, object?>(StringComparer.Ordinal);

    private IReadOnlyList<DynamicBlockFieldDescriptor> Fields =>
        Properties.TryGetValue("dynamic.fields", out var fields) && fields is IReadOnlyList<DynamicBlockFieldDescriptor> typedFields
            ? typedFields
            : Properties
                .Where(pair => !pair.Key.StartsWith("dynamic.", StringComparison.Ordinal))
                .Select(pair => new DynamicBlockFieldDescriptor(pair.Key, ReflectionDynamicBlockDescriptor.ToNodeTypeKey(pair.Key), pair.Value?.GetType() ?? typeof(string)))
                .ToList();

    private object? Model =>
        Properties.TryGetValue("dynamic.model", out var model) ? model : null;

    private object? DisplayModel => _isEditing ? _draftModel : _savedModel ?? Model;

    private string Presentation =>
        Properties.TryGetValue("dynamic.presentation", out var presentation)
            ? Convert.ToString(presentation, CultureInfo.InvariantCulture) ?? "form"
            : "form";

    private string ViewMode =>
        Properties.TryGetValue("dynamic.view-mode", out var viewMode)
            ? Convert.ToString(viewMode, CultureInfo.InvariantCulture) ?? "form"
            : "form";

    private bool IsGridView => string.Equals(ViewMode, "grid", StringComparison.OrdinalIgnoreCase);

    private string FormMode => _isEditing ? "edit" : "view";

    private bool IsFullHeightMode =>
        Mode?.ToLowerInvariant() is
            SessionContextBlock.ModeMaximized or
            SessionContextBlock.ModeOverlayPage or
            SessionContextBlock.ModeOverlayApp;

    private string RootClass => IsFullHeightMode ? "h-full min-h-0 space-y-4" : "space-y-4";

    private string PresentationCardClass => IsFullHeightMode ? "h-full min-h-0" : string.Empty;

    private IReadOnlyList<string> FieldKeys =>
        Fields.Select(descriptor => descriptor.Key).ToList();

    private IReadOnlyList<object> CollectionItems =>
        Properties.TryGetValue("dynamic.collection.items", out var items) && items is IReadOnlyList<object> typedItems
            ? [.. typedItems, .. _createdItems]
            : _createdItems;

    private IReadOnlyList<SessionNodeInput> CollectionNodes =>
        Properties.TryGetValue("dynamic.collection.nodes", out var nodes) && nodes is IReadOnlyList<SessionNodeInput> typedNodes
            ? [.. typedNodes, .. _createdNodes]
            : _createdNodes;

    private IReadOnlyList<DynamicBlockFieldDescriptor> CollectionFields =>
        Properties.TryGetValue("dynamic.collection.fields", out var fields) && fields is IReadOnlyList<DynamicBlockFieldDescriptor> typedFields
            ? typedFields
            : [];

    private string CollectionNodeType =>
        Properties.TryGetValue("dynamic.collection.node-type", out var nodeType)
            ? Convert.ToString(nodeType, CultureInfo.InvariantCulture) ?? string.Empty
            : string.Empty;

    private string? CollectionModelTypeName =>
        Properties.TryGetValue("dynamic.collection.model-type", out var modelTypeName)
            ? Convert.ToString(modelTypeName, CultureInfo.InvariantCulture)
            : null;

    private string? CollectionParentId =>
        Properties.TryGetValue("dynamic.collection.parent-id", out var parentId)
            ? Convert.ToString(parentId, CultureInfo.InvariantCulture)
            : null;

    private IReadOnlyList<object> FilteredItems =>
        string.IsNullOrWhiteSpace(_filterText)
            ? CollectionItems
            : CollectionItems.Where(ItemMatchesFilter).ToList();

    private RenderFragment FormContent => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "data-slot", "dynamic-node-form");
        builder.OpenComponent<DataForm>(2);
        builder.AddAttribute(3, nameof(DataForm.Model), DisplayModel);
        builder.AddAttribute(4, nameof(DataForm.IncludeProperties), FieldKeys);
        builder.AddAttribute(5, nameof(DataForm.Layout), "grid");
        builder.AddAttribute(6, nameof(DataForm.ShowDefaultActions), false);
        builder.AddAttribute(7, nameof(DataForm.ReadOnly), !_isEditing);
        builder.CloseComponent();
        builder.CloseElement();
    };

    private RenderFragment FormActions => builder =>
    {
        if (!_isEditing)
        {
            builder.OpenComponent<Button>(0);
            builder.AddAttribute(1, nameof(Button.Size), ButtonSize.Sm);
            builder.AddAttribute(2, nameof(Button.Icon), "pencil");
            builder.AddAttribute(3, nameof(Button.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, _ => BeginEdit()));
            builder.AddAttribute(4, "data-action", "edit");
            builder.AddAttribute(5, nameof(Button.ChildContent), (RenderFragment)(childBuilder => childBuilder.AddContent(0, L("AgentEdit"))));
            builder.CloseComponent();
            return;
        }

        builder.OpenComponent<Button>(6);
        builder.AddAttribute(7, nameof(Button.Variant), ButtonVariant.Outline);
        builder.AddAttribute(8, nameof(Button.Size), ButtonSize.Sm);
        builder.AddAttribute(9, nameof(Button.Icon), "x");
        builder.AddAttribute(10, nameof(Button.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, _ => CancelEdit()));
        builder.AddAttribute(11, "data-action", "cancel");
        builder.AddAttribute(12, nameof(Button.ChildContent), (RenderFragment)(childBuilder => childBuilder.AddContent(0, L("DeleteNodeCancel"))));
        builder.CloseComponent();
        builder.OpenComponent<Button>(13);
        builder.AddAttribute(14, nameof(Button.Size), ButtonSize.Sm);
        builder.AddAttribute(15, nameof(Button.Icon), "save");
        builder.AddAttribute(16, nameof(Button.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, _ => SaveAsync()));
        builder.AddAttribute(17, "data-action", "save");
        builder.AddAttribute(18, nameof(Button.ChildContent), (RenderFragment)(childBuilder => childBuilder.AddContent(0, L("AddToListConfirm"))));
        builder.CloseComponent();
    };

    private RenderFragment GridColumns => builder =>
    {
        var sequence = 0;
        foreach (var columnField in CollectionFields)
        {
            var currentField = columnField;
            builder.OpenComponent<DataGridColumn<object>>(sequence++);
            builder.AddAttribute(sequence++, nameof(DataGridColumn<object>.Title), currentField.Label);
            builder.AddAttribute(
                sequence++,
                nameof(DataGridColumn<object>.Template),
                (RenderFragment<object>)(item => cellBuilder =>
                {
                    cellBuilder.OpenElement(0, "span");
                    cellBuilder.AddAttribute(1, "class", currentField.Key is "Id" or "Key" ? "font-mono text-xs text-muted-foreground" : null);
                    cellBuilder.AddContent(2, GetItemDisplayValue(item, currentField.Key));
                    cellBuilder.CloseElement();
                }));
            builder.CloseComponent();
        }
    };

    private IDynamicBlockDispatcher? Dispatcher =>
        Properties.TryGetValue("dynamic.dispatcher", out var dispatcher) ? dispatcher as IDynamicBlockDispatcher : null;

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_sourceModel, Model))
            return;

        _sourceModel = Model;
        _savedModel = Model;
        _draftModel = null;
        _isEditing = false;
    }

    private bool IsFlowEnabled =>
        DisplayModel?.GetType().GetProperty("IsEnabled")?.GetValue(DisplayModel) is true;

    private string FlowStatusVariant => IsFlowEnabled ? "default" : "secondary";

    private double FlowProgress => IsFlowEnabled ? 100 : 35;

    private string PriorityVariant =>
        GetDisplayValue("Priority").ToLowerInvariant() switch
        {
            "high" or "urgent" => "destructive",
            "medium" => "secondary",
            _ => "outline"
        };

    private double WorkitemProgress =>
        GetDisplayValue("Priority").ToLowerInvariant() switch
        {
            "high" or "urgent" => 75,
            "medium" => 50,
            _ => 25
        };

    private string Initials
    {
        get
        {
            var label = Context?.Label ?? string.Empty;
            return string.Concat(label
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => char.ToUpperInvariant(part[0])));
        }
    }

    private string GetDisplayValue(string propertyName)
    {
        var value = DisplayModel?.GetType().GetProperty(propertyName)?.GetValue(DisplayModel);
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private void BeginEdit()
    {
        _draftModel = CloneModel(DisplayModel);
        _isEditing = _draftModel is not null;
    }

    private void CancelEdit()
    {
        _draftModel = null;
        _isEditing = false;
    }

    private async Task SaveAsync()
    {
        if (Context is null || Dispatcher is null || _draftModel is null)
            return;

        var values = Fields.ToDictionary(
            field => field.Key,
            field => _draftModel.GetType().GetProperty(field.Key)?.GetValue(_draftModel),
            StringComparer.Ordinal);
        await Dispatcher.DispatchAsync(new DynamicEntityEditRequested(
            Context.Id,
            Context.NodeTypeKey,
            values));
        _savedModel = _draftModel;
        _draftModel = null;
        _isEditing = false;
    }

    private void HandleSearchChanged(string? value)
    {
        _filterText = value ?? string.Empty;
        _pageIndex = 0;
    }

    private async Task HandleSearchKeyUp(KeyboardEventArgs args)
    {
        if (args.Key != "Enter"
            || string.IsNullOrWhiteSpace(_filterText)
            || FilteredItems.Count > 0)
        {
            return;
        }

        await CreateAsync(_filterText.Trim());
    }

    private void HandlePageChanged(int pageIndex) => _pageIndex = pageIndex;

    private void HandlePageSizeChanged(int pageSize)
    {
        _pageSize = pageSize;
        _pageIndex = 0;
    }

    private Task HandleAddAsync(MouseEventArgs _)
    {
        var label = string.IsNullOrWhiteSpace(_filterText)
            ? $"New {Singularize(Context?.Label ?? CollectionNodeType)}"
            : _filterText.Trim();
        return CreateAsync(label);
    }

    private async Task CreateAsync(string label)
    {
        if (Dispatcher is null || string.IsNullOrWhiteSpace(CollectionNodeType))
            return;

        var labelField = CollectionFields.FirstOrDefault(field =>
            field.Key is "Name" or "Label" or "Title")?.Key ?? "Name";
        var created = await Dispatcher.CreateAsync(
            CollectionNodeType,
            CollectionParentId,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                [labelField] = label
            },
            CollectionModelTypeName);
        if (created is null)
            return;

        _createdItems.Add(created.Instance);
        _createdNodes.Add(created.Node);
        _filterText = string.Empty;
        _pageIndex = Math.Max(0, (CollectionItems.Count - 1) / Math.Max(1, _pageSize));
    }

    private Task HandleGridSelectionAsync(object? item)
    {
        if (item is null || Dispatcher is null)
            return Task.CompletedTask;

        var key = GetItemKey(item);
        var node = CollectionNodes.FirstOrDefault(candidate =>
            string.Equals(candidate.Id, key, StringComparison.Ordinal));
        return node is null ? Task.CompletedTask : Dispatcher.ProjectAsync(node);
    }

    private string GetItemKey(object item)
    {
        var value = item.GetType().GetProperty("Id")?.GetValue(item)
            ?? item.GetType().GetProperty("Key")?.GetValue(item);
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? item.GetHashCode().ToString(CultureInfo.InvariantCulture);
    }

    private bool ItemMatchesFilter(object item) =>
        CollectionFields.Any(field =>
            GetItemDisplayValue(item, field.Key).Contains(_filterText, StringComparison.OrdinalIgnoreCase));

    private static string GetItemDisplayValue(object item, string propertyName)
    {
        var value = item.GetType().GetProperty(propertyName)?.GetValue(item);
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static object? CloneModel(object? model)
    {
        if (model is null)
            return null;

        var clone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
        return clone?.Invoke(model, null);
    }

    private static string Singularize(string value) =>
        value.EndsWith("ies", StringComparison.OrdinalIgnoreCase)
            ? $"{value[..^3]}y"
            : value.EndsWith("s", StringComparison.OrdinalIgnoreCase) && value.Length > 1
                ? value[..^1]
                : value;
}
