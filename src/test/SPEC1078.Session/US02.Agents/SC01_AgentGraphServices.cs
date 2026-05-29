namespace LowlandTech.Components.Tests.SPEC1078.Session.US02.Agents;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Agent graph services resolve by type key through DRD discovery")]
public class SC01_AgentGraphServices : ViewScenario
{
    [Fact(DisplayName = "UAC01: Discovery resolves Ask agent by key")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DiscoveryResolvesAskAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("ask");
        agent.ShouldNotBeNull();
        agent.AgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC02: Discovery resolves Plan agent by key")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DiscoveryResolvesPlanAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("plan");
        agent.ShouldNotBeNull();
        agent.AgentTypeKey.ShouldBe("plan");
    }

    [Fact(DisplayName = "UAC03: Discovery resolves Edit agent by key")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DiscoveryResolvesEditAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("edit");
        agent.ShouldNotBeNull();
        agent.AgentTypeKey.ShouldBe("edit");
    }

    [Fact(DisplayName = "UAC04: Discovery resolves Run agent by key")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DiscoveryResolvesRunAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("run");
        agent.ShouldNotBeNull();
        agent.AgentTypeKey.ShouldBe("run");
    }

    [Fact(DisplayName = "UAC05: Discovery resolves Create agent by key")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DiscoveryResolvesCreateAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("create");
        agent.ShouldNotBeNull();
        agent.AgentTypeKey.ShouldBe("create");
    }

    [Fact(DisplayName = "UAC06: Discovery resolves Execute agent by key")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DiscoveryResolvesExecuteAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("execute");
        agent.ShouldNotBeNull();
        agent.AgentTypeKey.ShouldBe("execute");
    }

    [Fact(DisplayName = "UAC07: Discovery returns null for unknown agent key")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_DiscoveryReturnsNullForUnknownAgent()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("unknown");
        agent.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC08: Resolved agent ExecuteAsync does not throw")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ResolvedAgentExecuteAsyncDoesNotThrow()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        var agent = discovery.Get("ask");
        agent.ShouldNotBeNull();

        var context = new SessionContext();
        await agent.ExecuteAsync(context);
    }

    [Fact(DisplayName = "UAC09: Discovery is case-insensitive")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_DiscoveryIsCaseInsensitive()
    {
        var discovery = Ctx.Services.GetRequiredService<IGraphDiscoveryService>();
        discovery.Get("ASK").ShouldNotBeNull();
        discovery.Get("Plan").ShouldNotBeNull();
        discovery.Get("EDIT").ShouldNotBeNull();
    }
}
