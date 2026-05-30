namespace LowlandTech.Components.Session.Agents;

/// <summary>Execute agent — executes commands against the graph.</summary>
public sealed class ExecuteAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "execute";

    public Task ExecuteAsync(SessionContext context)
    {
        // TODO: Implement execute logic using graph service
        return Task.CompletedTask;
    }
}
