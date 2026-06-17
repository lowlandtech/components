namespace LowlandTech.Components;

public sealed record DynamicBlockFieldDescriptor(
    string Key,
    string Label,
    Type ValueType,
    bool IsReadOnly = false);

public sealed record DynamicBlockCollectionDescriptor(
    string Key,
    string Label,
    string HeaderNodeTypeKey,
    string ChildNodeTypeKey,
    Func<object, IEnumerable<object?>> GetItems);

public interface IDynamicBlockDescriptor
{
    Type ModelType { get; }

    string NodeTypeKey { get; }

    string Label { get; }

    IReadOnlyList<DynamicBlockFieldDescriptor> Fields { get; }

    IReadOnlyList<DynamicBlockCollectionDescriptor> Collections { get; }

    string GetId(object model);

    string GetLabel(object model);

    object? GetFieldValue(object model, string fieldKey);

    void ApplyParent(object model, string? parentEntityId);
}

public sealed record DynamicBlockDescriptor(
    Type ModelType,
    string NodeTypeKey,
    string Label,
    Func<object, string> IdAccessor,
    Func<object, string> LabelAccessor,
    Func<object, string, object?> FieldAccessor,
    IReadOnlyList<DynamicBlockFieldDescriptor> Fields,
    IReadOnlyList<DynamicBlockCollectionDescriptor> Collections,
    Action<object, string?>? ParentApplicator = null) : IDynamicBlockDescriptor
{
    public string GetId(object model) => IdAccessor(model);

    public string GetLabel(object model) => LabelAccessor(model);

    public object? GetFieldValue(object model, string fieldKey) => FieldAccessor(model, fieldKey);

    public void ApplyParent(object model, string? parentEntityId) => ParentApplicator?.Invoke(model, parentEntityId);
}

public interface IDynamicBlockRegistry
{
    int Priority => 0;

    IReadOnlyList<IDynamicBlockDescriptor> GetDescriptors();
}

public interface IDynamicBlockDiscovery
{
    IDynamicBlockDescriptor? GetDescriptor(object? model);

    IDynamicBlockDescriptor? GetDescriptor(Type? modelType);

    IDynamicBlockDescriptor? GetDescriptor(string? nodeTypeKey);

    IReadOnlyList<IDynamicBlockDescriptor> GetDescriptors();
}

public sealed class DynamicBlockDiscovery(IEnumerable<IDynamicBlockRegistry> registries) : IDynamicBlockDiscovery
{
    private readonly IReadOnlyList<IDynamicBlockDescriptor> _descriptors = registries
        .OrderByDescending(registry => registry.Priority)
        .SelectMany(registry => registry.GetDescriptors())
        .ToList();

    public IDynamicBlockDescriptor? GetDescriptor(object? model)
        => model is null ? null : GetDescriptor(model.GetType());

    public IDynamicBlockDescriptor? GetDescriptor(Type? modelType)
    {
        if (modelType is null)
            return null;

        return _descriptors.FirstOrDefault(descriptor => descriptor.ModelType.IsAssignableFrom(modelType))
            ?? ReflectionDynamicBlockDescriptor.Create(modelType);
    }

    public IDynamicBlockDescriptor? GetDescriptor(string? nodeTypeKey)
        => string.IsNullOrWhiteSpace(nodeTypeKey)
            ? null
            : _descriptors.FirstOrDefault(descriptor =>
                string.Equals(descriptor.NodeTypeKey, nodeTypeKey, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<IDynamicBlockDescriptor> GetDescriptors() => _descriptors;
}

public sealed class DefaultDynamicBlockRegistry : IDynamicBlockRegistry
{
    public IReadOnlyList<IDynamicBlockDescriptor> GetDescriptors() => [];
}

public sealed record DynamicBlockState(
    string? RootNodeId = null,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> EntityPropertiesById = null!,
    IReadOnlyList<SessionContextNode> ContextNodes = null!,
    IReadOnlySet<string> DeletedEntityIds = null!)
{
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> EntityPropertiesById { get; init; } =
        EntityPropertiesById ?? new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.Ordinal);

    public IReadOnlyList<SessionContextNode> ContextNodes { get; init; } = ContextNodes ?? [];

    public IReadOnlySet<string> DeletedEntityIds { get; init; } =
        DeletedEntityIds ?? new HashSet<string>(StringComparer.Ordinal);
}

public abstract record DynamicBlockAction(string ActionKey);

public sealed record DynamicBlockInitialized(
    string RootNodeId,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> EntityPropertiesById,
    IReadOnlyList<SessionContextNode> ContextNodes) : DynamicBlockAction("dynamic.initialized");

public sealed record DynamicFieldChanged(
    string EntityId,
    string NodeTypeKey,
    string FieldKey,
    object? Value) : DynamicBlockAction("dynamic.field.changed");

public sealed record DynamicEntityCreateRequested(
    string NodeTypeKey,
    string? ParentEntityId,
    IReadOnlyDictionary<string, object?> Values) : DynamicBlockAction("dynamic.entity.create");

public sealed record DynamicEntityEditRequested(
    string EntityId,
    string NodeTypeKey,
    IReadOnlyDictionary<string, object?> Values) : DynamicBlockAction("dynamic.entity.edit");

public sealed record DynamicEntityDeleteRequested(
    string EntityId,
    string NodeTypeKey) : DynamicBlockAction("dynamic.entity.delete");

public sealed record DynamicReducerResult(
    DynamicBlockState State,
    IReadOnlyList<object> Effects);

public interface IDynamicReducerArm
{
    string Key { get; }

    bool CanReduce(DynamicBlockState state, DynamicBlockAction action);

    DynamicReducerResult Reduce(DynamicBlockState state, DynamicBlockAction action);
}

public sealed class DynamicInitializerReducerArm : IDynamicReducerArm
{
    public string Key => "dynamic.initialized";

    public bool CanReduce(DynamicBlockState state, DynamicBlockAction action) => action is DynamicBlockInitialized;

    public DynamicReducerResult Reduce(DynamicBlockState state, DynamicBlockAction action)
    {
        var init = (DynamicBlockInitialized)action;
        return new DynamicReducerResult(
            state with
            {
                RootNodeId = init.RootNodeId,
                EntityPropertiesById = init.EntityPropertiesById,
                ContextNodes = init.ContextNodes,
            },
            []);
    }
}

public sealed class DynamicFieldReducerArm : IDynamicReducerArm
{
    public string Key => "dynamic.field.changed";

    public bool CanReduce(DynamicBlockState state, DynamicBlockAction action) => action is DynamicFieldChanged;

    public DynamicReducerResult Reduce(DynamicBlockState state, DynamicBlockAction action)
    {
        var changed = (DynamicFieldChanged)action;
        var entities = state.EntityPropertiesById.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        var values = entities.TryGetValue(changed.EntityId, out var current)
            ? current.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
            : new Dictionary<string, object?>(StringComparer.Ordinal);

        values[changed.FieldKey] = changed.Value;
        entities[changed.EntityId] = values;

        return new DynamicReducerResult(
            state with { EntityPropertiesById = entities },
            [DynamicMutationEffect.Update(changed.NodeTypeKey, changed.EntityId, values)]);
    }
}

public sealed class DynamicEntityMutationReducerArm : IDynamicReducerArm
{
    public string Key => "dynamic.entity.mutation";

    public bool CanReduce(DynamicBlockState state, DynamicBlockAction action)
        => action is DynamicEntityCreateRequested
            or DynamicEntityEditRequested
            or DynamicEntityDeleteRequested;

    public DynamicReducerResult Reduce(DynamicBlockState state, DynamicBlockAction action)
        => action switch
        {
            DynamicEntityCreateRequested create => new DynamicReducerResult(
                state,
                [DynamicMutationEffect.Create(create.NodeTypeKey, create.ParentEntityId, create.Values)]),
            DynamicEntityEditRequested edit => ReduceEdit(state, edit),
            DynamicEntityDeleteRequested delete => ReduceDelete(state, delete),
            _ => new DynamicReducerResult(state, []),
        };

    private static DynamicReducerResult ReduceEdit(DynamicBlockState state, DynamicEntityEditRequested edit)
    {
        var entities = state.EntityPropertiesById.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        var values = entities.TryGetValue(edit.EntityId, out var current)
            ? current.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
            : new Dictionary<string, object?>(StringComparer.Ordinal);

        foreach (var pair in edit.Values)
            values[pair.Key] = pair.Value;

        entities[edit.EntityId] = values;
        return new DynamicReducerResult(
            state with { EntityPropertiesById = entities },
            [DynamicMutationEffect.Update(edit.NodeTypeKey, edit.EntityId, values)]);
    }

    private static DynamicReducerResult ReduceDelete(DynamicBlockState state, DynamicEntityDeleteRequested delete)
    {
        var entities = state.EntityPropertiesById
            .Where(pair => !string.Equals(pair.Key, delete.EntityId, StringComparison.Ordinal))
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        var deleted = state.DeletedEntityIds.ToHashSet(StringComparer.Ordinal);
        deleted.Add(delete.EntityId);

        return new DynamicReducerResult(
            state with { EntityPropertiesById = entities, DeletedEntityIds = deleted },
            [DynamicMutationEffect.Delete(delete.NodeTypeKey, delete.EntityId)]);
    }
}

public sealed record DynamicMutationEffect(
    string Mutation,
    string NodeTypeKey,
    string? EntityId,
    string? ParentEntityId,
    IReadOnlyDictionary<string, object?> Values)
{
    public static DynamicMutationEffect Create(
        string nodeTypeKey,
        string? parentEntityId,
        IReadOnlyDictionary<string, object?> values)
        => new("create", nodeTypeKey, null, parentEntityId, values);

    public static DynamicMutationEffect Update(
        string nodeTypeKey,
        string entityId,
        IReadOnlyDictionary<string, object?> values)
        => new("update", nodeTypeKey, entityId, null, values);

    public static DynamicMutationEffect Delete(string nodeTypeKey, string entityId)
        => new("delete", nodeTypeKey, entityId, null, new Dictionary<string, object?>(StringComparer.Ordinal));
}

public sealed record DynamicEntity(
    SessionNodeInput Node,
    Type ClrType,
    object Instance,
    IReadOnlyDictionary<string, object?> Properties);

public interface IDynamicNodeRuntime
{
    Task<DynamicEntity?> LoadAsync(
        SessionNodeInput node,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken = default);

    Task<DynamicEntity?> ApplyAsync(
        DynamicMutationEffect mutation,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken = default);
}

public interface IDynamicBlockDispatcher
{
    DynamicBlockState DynamicState { get; }

    Task DispatchAsync(DynamicBlockAction action);
}

public sealed class DynamicBlockStateMachine(IEnumerable<IDynamicReducerArm> reducerArms, ISessionOutbox? outbox = null)
{
    private readonly IReadOnlyList<IDynamicReducerArm> _reducerArms = reducerArms.ToList();

    public DynamicBlockState State { get; private set; } = new();

    public ISessionOutbox? Outbox { get; } = outbox;

    public void Dispatch(DynamicBlockAction action)
    {
        var arm = _reducerArms.FirstOrDefault(candidate => candidate.CanReduce(State, action));
        if (arm is null)
            return;

        var result = arm.Reduce(State, action);
        State = result.State;

        foreach (var effect in result.Effects)
        {
            Outbox?.Append(new SessionOutboxEntry(effect.GetType().Name, effect, DateTimeOffset.UtcNow));
        }
    }
}

public static class ReflectionDynamicBlockDescriptor
{
    public static IDynamicBlockDescriptor Create(Type modelType)
    {
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead)
            .ToArray();
        var idProperty = FindProperty(properties, "Id") ?? FindProperty(properties, "Key");
        var labelProperty = FindProperty(properties, "Label") ?? FindProperty(properties, "Name") ?? FindProperty(properties, "Title") ?? idProperty;
        var parentProperty = FindProperty(properties, "ParentId");
        var fields = properties
            .Where(property => IsScalar(property.PropertyType))
            .Select(property => new DynamicBlockFieldDescriptor(property.Name, Humanize(property.Name), property.PropertyType, !property.CanWrite))
            .ToList();
        var collections = properties
            .Where(property => TryGetCollectionItemType(property.PropertyType, out _))
            .Select(property => CreateCollectionDescriptor(property))
            .ToList();

        return new DynamicBlockDescriptor(
            modelType,
            ToNodeTypeKey(modelType.Name),
            Humanize(modelType.Name),
            model => Convert.ToString(idProperty?.GetValue(model), CultureInfo.InvariantCulture)
                ?? CreateStableId(model, labelProperty),
            model => Convert.ToString(labelProperty?.GetValue(model), CultureInfo.InvariantCulture)
                ?? Humanize(model.GetType().Name),
            (model, fieldKey) => FindProperty(properties, fieldKey)?.GetValue(model),
            fields,
            collections,
            (model, parentEntityId) => SetStringValue(parentProperty, model, parentEntityId));
    }

    private static DynamicBlockCollectionDescriptor CreateCollectionDescriptor(PropertyInfo property)
    {
        TryGetCollectionItemType(property.PropertyType, out var itemType);
        var childKey = ToNodeTypeKey(itemType?.Name ?? Singularize(property.Name));
        var headerKey = $"{ToNodeTypeKey(property.Name)}.header";

        return new DynamicBlockCollectionDescriptor(
            property.Name,
            Humanize(property.Name),
            headerKey,
            childKey,
            model => ((property.GetValue(model) as IEnumerable) ?? Array.Empty<object>())
                .Cast<object?>());
    }

    private static PropertyInfo? FindProperty(IEnumerable<PropertyInfo> properties, string name)
        => properties.FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase));

    private static bool IsScalar(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying.IsPrimitive
            || underlying.IsEnum
            || underlying == typeof(string)
            || underlying == typeof(Guid)
            || underlying == typeof(DateTime)
            || underlying == typeof(DateOnly)
            || underlying == typeof(decimal);
    }

    private static bool TryGetCollectionItemType(Type type, out Type? itemType)
    {
        itemType = null;
        if (type == typeof(string))
            return false;

        if (!typeof(IEnumerable).IsAssignableFrom(type))
            return false;

        itemType = type.IsArray
            ? type.GetElementType()
            : type.GetInterfaces()
                .Concat([type])
                .Where(candidate => candidate.IsGenericType)
                .FirstOrDefault(candidate => candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments()[0];

        return itemType is not null && itemType != typeof(char);
    }

    public static string ToNodeTypeKey(string value)
    {
        var text = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2").Replace('_', '-');
        return text.Trim('-').ToLowerInvariant();
    }

    private static string Singularize(string value)
    {
        if (value.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            return $"{value[..^3]}y";
        if (value.EndsWith("s", StringComparison.OrdinalIgnoreCase) && value.Length > 1)
            return value[..^1];
        return value;
    }

    private static string Humanize(string value)
        => Regex.Replace(value, "([a-z0-9])([A-Z])", "$1 $2");

    private static void SetStringValue(PropertyInfo? property, object model, string? value)
    {
        if (property is null || !property.CanWrite || string.IsNullOrWhiteSpace(value))
            return;

        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        var converted = targetType == typeof(Guid)
            ? Guid.Parse(value)
            : Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        property.SetValue(model, converted);
    }

    private static string CreateStableId(object model, PropertyInfo? labelProperty)
    {
        var label = Convert.ToString(labelProperty?.GetValue(model), CultureInfo.InvariantCulture)
            ?? model.GetType().Name;

        return $"{ToNodeTypeKey(model.GetType().Name)}:{ToNodeTypeKey(label)}";
    }
}
