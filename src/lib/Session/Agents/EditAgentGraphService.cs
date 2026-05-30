namespace LowlandTech.Components.Session.Agents;

/// <summary>Edit agent — modifies graph nodes based on the session context.</summary>
public sealed class EditAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "edit";

    public Task ExecuteAsync(SessionContext context)
    {
        // TODO: Implement edit logic using graph service
        return Task.CompletedTask;
    }
}
