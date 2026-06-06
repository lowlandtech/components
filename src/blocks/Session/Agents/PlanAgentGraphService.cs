namespace LowlandTech.Components;

/// <summary>Plan agent — generates an implementation plan from the session context.</summary>
public sealed class PlanAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "plan";

    public Task ExecuteAsync(SessionContext context)
    {
        // TODO: Implement plan logic using graph service
        return Task.CompletedTask;
    }
}
