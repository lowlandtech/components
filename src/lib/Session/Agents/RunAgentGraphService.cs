namespace LowlandTech.Components.Session.Agents;

/// <summary>Run agent — executes actions against the graph.</summary>
public sealed class RunAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "run";

    public Task ExecuteAsync(SessionContext context)
    {
        // TODO: Implement run logic using graph service
        return Task.CompletedTask;
    }
}
