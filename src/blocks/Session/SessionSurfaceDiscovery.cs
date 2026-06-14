namespace LowlandTech.Components;

public sealed class DefaultSessionSurfaceRegistry : ISessionSurfaceRegistry
{
    private static readonly IReadOnlyDictionary<string, SessionSurfaceDescriptor> Surfaces =
        new Dictionary<string, SessionSurfaceDescriptor>(StringComparer.Ordinal)
        {
            ["workspaces-session"] = new(
                "workspaces-session",
                new SessionNodeInput("request-workspaces", "session-request", "Workspace summary"),
                new SessionNodeInput("response-workspaces", "session-response", "Workspace context")),
            ["flows-session"] = new(
                "flows-session",
                new SessionNodeInput("request-flows", "session-request", "Flow summary"),
                new SessionNodeInput("response-flows", "session-response", "Flows context")),
            ["workitems-session"] = new(
                "workitems-session",
                new SessionNodeInput("request-workitems", "session-request", "Workitem summary"),
                new SessionNodeInput("response-workitems", "session-response", "Workitems context"))
        };

    public SessionSurfaceDescriptor? GetSurface(string sessionId)
        => Surfaces.TryGetValue(sessionId, out var descriptor)
            ? descriptor
            : null;
}

public sealed class SessionSurfaceDiscovery(IEnumerable<ISessionSurfaceRegistry> registries) : ISessionSurfaceDiscovery
{
    private static readonly ISessionSurfaceRegistry FallbackRegistry = new DefaultSessionSurfaceRegistry();

    private readonly IReadOnlyList<ISessionSurfaceRegistry> _ordered = registries
        .Append(FallbackRegistry)
        .OrderByDescending(registry => registry.Priority)
        .ToList();

    public SessionSurfaceDescriptor Find(string sessionId)
    {
        foreach (var registry in _ordered)
        {
            var descriptor = registry.GetSurface(sessionId);
            if (descriptor is not null)
                return descriptor;
        }

        return new SessionSurfaceDescriptor(
            sessionId,
            new SessionNodeInput($"{sessionId}-request", "session-request", "Session"),
            new SessionNodeInput($"{sessionId}-response", "session-response", "Context"));
    }
}