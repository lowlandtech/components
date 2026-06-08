namespace LowlandTech.Components;

public sealed class DefaultSessionSurfaceRegistry : ISessionSurfaceRegistry
{
    private static readonly IReadOnlyDictionary<string, SessionSurfaceDescriptor> Surfaces =
        new Dictionary<string, SessionSurfaceDescriptor>(StringComparer.Ordinal)
        {
            ["orders-session"] = new(
                "orders-session",
                new SessionNodeInput("request-orders", "session-request", "Order summary"),
                new SessionNodeInput("response-orders", "session-response", "Orders context")),
            ["support-session"] = new(
                "support-session",
                new SessionNodeInput("request-support", "session-request", "Support ticket summary"),
                new SessionNodeInput("response-support", "session-response", "Support context")),
            ["release-session"] = new(
                "release-session",
                new SessionNodeInput("request-release", "session-request", "Release draft"),
                new SessionNodeInput("response-release", "session-response", "Release context"))
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