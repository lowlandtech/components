namespace LowlandTech.Components;

public sealed class HttpDynamicNodeRuntime(HttpClient client) : IDynamicNodeRuntime
{
    public async Task<DynamicEntity?> LoadAsync(
        SessionNodeInput node,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(GetEntityUrl(descriptor.NodeTypeKey, node.Id), cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var instance = await response.Content.ReadFromJsonAsync(descriptor.ModelType, cancellationToken: cancellationToken);
        return instance is null ? null : ToDynamicEntity(node, descriptor, instance);
    }

    public async Task<DynamicEntity?> ApplyAsync(
        DynamicMutationEffect mutation,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        return mutation.Mutation switch
        {
            "create" => await CreateAsync(mutation, descriptor, cancellationToken),
            "update" => await UpdateAsync(mutation, descriptor, cancellationToken),
            "delete" => await DeleteAsync(mutation, descriptor, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported dynamic mutation '{mutation.Mutation}'."),
        };
    }

    private async Task<DynamicEntity?> CreateAsync(
        DynamicMutationEffect mutation,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        var instance = ToInstance(descriptor.ModelType, mutation.Values);
        descriptor.ApplyParent(instance, mutation.ParentEntityId);
        using var content = JsonContent.Create(instance, descriptor.ModelType);
        var response = await client.PostAsync(
            $"/api/graph/entities/{descriptor.NodeTypeKey}",
            content,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync(descriptor.ModelType, cancellationToken: cancellationToken);
        if (created is null)
            return null;

        var node = new SessionNodeInput(
            descriptor.GetId(created),
            descriptor.NodeTypeKey,
            descriptor.GetLabel(created),
            "DynamicNodeBlock");
        return ToDynamicEntity(node, descriptor, created);
    }

    private async Task<DynamicEntity?> UpdateAsync(
        DynamicMutationEffect mutation,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mutation.EntityId))
            throw new InvalidOperationException("Dynamic update mutations require an entity id.");

        var instance = ToInstance(descriptor.ModelType, mutation.Values);
        SetId(instance, mutation.EntityId);
        using var content = JsonContent.Create(instance, descriptor.ModelType);
        var response = await client.PutAsync(
            GetEntityUrl(descriptor.NodeTypeKey, mutation.EntityId),
            content,
            cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync(descriptor.ModelType, cancellationToken: cancellationToken);
        if (updated is null)
            return null;

        var node = new SessionNodeInput(
            descriptor.GetId(updated),
            descriptor.NodeTypeKey,
            descriptor.GetLabel(updated),
            "DynamicNodeBlock");
        return ToDynamicEntity(node, descriptor, updated);
    }

    private async Task<DynamicEntity?> DeleteAsync(
        DynamicMutationEffect mutation,
        IDynamicBlockDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mutation.EntityId))
            throw new InvalidOperationException("Dynamic delete mutations require an entity id.");

        var response = await client.DeleteAsync(
            GetEntityUrl(descriptor.NodeTypeKey, mutation.EntityId),
            cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return null;
    }

    private static DynamicEntity ToDynamicEntity(
        SessionNodeInput node,
        IDynamicBlockDescriptor descriptor,
        object instance)
        => new(
            node,
            descriptor.ModelType,
            instance,
            descriptor.Fields.ToDictionary(
                field => field.Key,
                field => descriptor.GetFieldValue(instance, field.Key),
                StringComparer.Ordinal));

    private static object ToInstance(Type modelType, IReadOnlyDictionary<string, object?> values)
    {
        var json = JsonSerializer.Serialize(values);
        return JsonSerializer.Deserialize(json, modelType)
            ?? throw new InvalidOperationException($"Could not materialize dynamic entity '{modelType.Name}'.");
    }

    private static void SetId(object instance, string entityId)
    {
        var property = instance.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        if (property is null || !property.CanWrite)
            return;

        object? value = property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?)
            ? Guid.Parse(entityId)
            : Convert.ChangeType(entityId, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType, CultureInfo.InvariantCulture);
        property.SetValue(instance, value);
    }

    private static string GetEntityUrl(string nodeTypeKey, string entityId)
        => $"/api/graph/entities/{Uri.EscapeDataString(nodeTypeKey)}/{Uri.EscapeDataString(entityId)}";
}
