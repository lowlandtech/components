namespace LowlandTech.Components;

/// <summary>
/// A graph edge adapted for display and dispatch from a dynamic node toolbar.
/// </summary>
public sealed record DynamicNodeIntentAction(
    BlockIntent Intent,
    string EdgeKey,
    string Label,
    string Icon,
    string? Surface);

/// <summary>
/// Adapts outgoing graph edges into the actions that belong above a dynamic form.
/// </summary>
public static class DynamicNodeIntentToolbar
{
    public static IReadOnlyList<DynamicNodeIntentAction> Create(
        IEdgeTypeService? edgeTypes,
        string? nodeTypeKey)
    {
        if (edgeTypes is null || string.IsNullOrWhiteSpace(nodeTypeKey))
            return [];

        return edgeTypes.GetOutgoingEdges(nodeTypeKey)
            .Where(IsToolbarEdge)
            .Select(CreateAction)
            .GroupBy(action => action.EdgeKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(action => GetSurfaceOrder(action.Surface))
            .ThenBy(action => action.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsToolbarEdge(EdgeTypeInfo edge) =>
        !edge.IsNavigationHeaderChild
        && !string.Equals(edge.Surface, "form-reference", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(edge.Surface, "settings", StringComparison.OrdinalIgnoreCase);

    private static DynamicNodeIntentAction CreateAction(EdgeTypeInfo edge)
    {
        var encodedIntent = GetEncodedIntent(edge);
        var payload = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["edgeKey"] = edge.EdgeKey,
            ["sourceNodeTypeKey"] = edge.SourceNodeTypeKey,
            ["targetNodeTypeKey"] = edge.TargetNodeTypeKey,
            ["surface"] = edge.Surface
        };

        return new DynamicNodeIntentAction(
            new BlockIntent(encodedIntent, BlockIntentClass.Transition, edge.Label, payload),
            edge.EdgeKey,
            edge.Label,
            GetIcon(edge),
            edge.Surface);
    }

    private static string GetEncodedIntent(EdgeTypeInfo edge)
    {
        var segments = edge.EdgeKey.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var intent = edge.Intent;
        if (string.IsNullOrWhiteSpace(intent))
        {
            var sourcePrefix = segments.FirstOrDefault();
            intent = string.Equals(sourcePrefix, edge.SourceNodeTypeKey, StringComparison.OrdinalIgnoreCase)
                ? segments.Skip(1).FirstOrDefault()
                : sourcePrefix;
        }

        intent ??= edge.EdgeKey;
        return $"{intent}:{edge.EdgeKey}";
    }

    private static string GetIcon(EdgeTypeInfo edge)
    {
        if (!string.IsNullOrWhiteSpace(edge.Icon) && Icons.GetByName(edge.Icon) is not null)
            return edge.Icon;

        if (!string.IsNullOrWhiteSpace(edge.TargetNodeTypeKey)
            && Icons.GetByName(edge.TargetNodeTypeKey) is not null)
        {
            return edge.TargetNodeTypeKey;
        }

        var key = edge.EdgeKey.ToLowerInvariant();
        if (key.Contains("branch", StringComparison.Ordinal))
            return "git-branch";
        if (key.Contains("label", StringComparison.Ordinal))
            return "tag";
        if (key.Contains("pipeline", StringComparison.Ordinal))
            return "pipeline";
        if (key.Contains("opencode", StringComparison.Ordinal))
            return "terminal";

        return "arrow-right";
    }

    private static int GetSurfaceOrder(string? surface) =>
        surface?.ToLowerInvariant() switch
        {
            "command-bar" => 0,
            "collection-tab" => 1,
            "readonly-tab" => 2,
            _ => 3
        };
}
