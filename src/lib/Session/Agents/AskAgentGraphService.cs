namespace LowlandTech.Components.Session.Agents;

/// <summary>Ask agent — queries the graph and returns information.</summary>
public sealed class AskAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "ask";

    public Task ExecuteAsync(SessionContext context)
    {
        // TODO: Implement ask logic using graph service
        return Task.CompletedTask;
    }
}
