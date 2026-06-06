namespace LowlandTech.Components;

/// <summary>Create agent — creates new graph nodes from the session context.</summary>
public sealed class CreateAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "create";

    public Task ExecuteAsync(SessionContext context)
    {
        // TODO: Implement create logic using graph service
        return Task.CompletedTask;
    }
}
