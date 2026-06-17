namespace LowlandTech.Components;

public partial class DynamicNodeBlock : TinyComponentBase
{
    [Parameter] public SessionContextBlockModel? Block { get; set; }
    [Parameter] public SessionNodeInput? Context { get; set; }
    [Parameter] public object? Data { get; set; }

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

    private IDynamicBlockDispatcher? Dispatcher =>
        Properties.TryGetValue("dynamic.dispatcher", out var dispatcher) ? dispatcher as IDynamicBlockDispatcher : null;

    private string GetValue(string fieldKey)
    {
        if (Context is not null
            && Dispatcher?.DynamicState.EntityPropertiesById.TryGetValue(Context.Id, out var latest) == true
            && latest.TryGetValue(fieldKey, out var latestValue))
        {
            return Convert.ToString(latestValue, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        return Properties.TryGetValue(fieldKey, out var value)
            ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            : string.Empty;
    }

    private Task SetFieldAsync(DynamicBlockFieldDescriptor field, object? value)
    {
        if (field.IsReadOnly || Context is null || Dispatcher is null)
            return Task.CompletedTask;

        return Dispatcher.DispatchAsync(new DynamicFieldChanged(Context.Id, Context.NodeTypeKey, field.Key, value));
    }
}
