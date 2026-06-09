namespace LowlandTech.Components.Tests.SPEC1078.Session.Pipeline;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "02")]
[UserStory("03", "SessionPipelineHost runtime pipelines")]
public class SC02_PipelineStagesExecuteInOrder : ViewScenario
{
    private sealed class TrackingStage(string name, List<string> log) : IPipelineStage
    {
        public string Name => name;

        public Task ExecuteAsync(PipelineContext context, CancellationToken ct = default)
        {
            log.Add(name);
            context.State[$"stage-{name}"] = $"executed-{name}";
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [Fact(DisplayName = "UAC01: Pipeline stages execute in registration order")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_StagesExecuteInOrder()
    {
        var order = new List<string>();
        var registry = new PipelineRegistry();
        registry.Register(new TrackingStage("alpha", order));
        registry.Register(new TrackingStage("beta", order));
        registry.Register(new TrackingStage("gamma", order));
        Ctx.Services.AddSingleton(registry);

        var cut = Ctx.Render<SessionPipelineHost>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        order.ShouldBe(["alpha", "beta", "gamma"]);
    }

    [Fact(DisplayName = "UAC02: Pipeline stages can write to shared context state")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_StagesWriteToSharedContext()
    {
        var registry = new PipelineRegistry();
        registry.Register(new TrackingStage("one", []));
        registry.Register(new TrackingStage("two", []));
        Ctx.Services.AddSingleton(registry);

        var cut = Ctx.Render<SessionPipelineHost>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.Context.ShouldNotBeNull();
        cut.Instance.Context.State.ShouldContainKey("stage-one");
        cut.Instance.Context.State.ShouldContainKey("stage-two");
    }

    [Fact(DisplayName = "UAC03: Empty registry results in no stage execution")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_EmptyRegistryExecutesNoStages()
    {
        Ctx.Services.AddSingleton(new PipelineRegistry());

        var cut = Ctx.Render<SessionPipelineHost>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.PipelineComplete.ShouldBeTrue();
        cut.Instance.Context.ShouldNotBeNull();
        cut.Instance.Context.State.ShouldBeEmpty();
    }
}
