namespace LowlandTech.Components.Session;

/// <summary>
/// Descriptor for an agent-type-specific graph execution strategy.
/// Each agent type (ask, plan, edit, run) implements its own behavior.
/// </summary>
public interface IAgentGraphService
{
    /// <summary>The agent type key this service handles (e.g. "ask", "plan").</summary>
    string AgentTypeKey { get; }

    /// <summary>Execute the agent logic against the session context.</summary>
    Task ExecuteAsync(SessionContext context);

    /// <summary>
    /// Stream the agent's response token-by-token. Default implementation throws
    /// <see cref="NotSupportedException"/>; agents that can stream (e.g. remote LLMs
    /// that expose SSE) override this. <see cref="SessionStateService.SubmitAsync"/>
    /// prefers <c>StreamAsync</c> when available and falls back to
    /// <see cref="ExecuteAsync"/> otherwise.
    /// </summary>
    IAsyncEnumerable<string> StreamAsync(SessionContext context)
        => throw new NotSupportedException(
            $"Agent '{AgentTypeKey}' does not support streaming; " +
            $"override {nameof(IAgentGraphService)}.{nameof(StreamAsync)} or use ExecuteAsync.");
}

/// <summary>
/// Registry that owns agent graph services for one feature domain.
/// </summary>
public interface IAgentGraphRegistry
{
    /// <summary>Resolution priority when multiple registries match.</summary>
    int Priority => 0;

    /// <summary>Find a service for the given agent type key.</summary>
    IAgentGraphService? GetService(string agentTypeKey);
}

/// <summary>
/// Discovery service that aggregates all <see cref="IAgentGraphRegistry"/> instances
/// and resolves the appropriate <see cref="IAgentGraphService"/> by agent type key.
/// </summary>
public interface IGraphDiscoveryService
{
    /// <summary>Get the agent graph service for the specified agent type key.</summary>
    IAgentGraphService? Get(string agentTypeKey);
}
