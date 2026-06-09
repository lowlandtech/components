namespace LowlandTech.Components.Tests.SPEC1078.SessionPipelineHost;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "04")]
[UserStory("03", "SessionPipelineHost runtime pipelines")]
public class SC04_TeardownDisposesAllStagesOnCircuitDrop : ViewScenario
{
    private sealed class DisposableStage(string name, List<string> log) : IPipelineStage
    {
        public string Name => name;
        public bool IsDisposed { get; private set; }

        public Task ExecuteAsync(PipelineContext context, CancellationToken ct = default)
        {
            log.Add($"execute-{name}");
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                log.Add($"dispose-{name}");
            }
            return ValueTask.CompletedTask;
        }
    }

    [Fact(DisplayName = "UAC01: DisposeAsync disposes all registered stages")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DisposeAsyncDisposesStages()
    {
        var log = new List<string>();
        var registry = new PipelineRegistry();
        var stage1 = new DisposableStage("alpha", log);
        var stage2 = new DisposableStage("beta", log);
        registry.Register(stage1);
        registry.Register(stage2);
        Ctx.Services.AddSingleton(registry);

        var cut = Ctx.Render<SessionPipelineHostSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        log.ShouldContain("execute-alpha");
        log.ShouldContain("execute-beta");
        stage1.IsDisposed.ShouldBeFalse();
        stage2.IsDisposed.ShouldBeFalse();

        await cut.Instance.DisposeAsync();

        stage1.IsDisposed.ShouldBeTrue();
        stage2.IsDisposed.ShouldBeTrue();
        log.ShouldContain("dispose-alpha");
        log.ShouldContain("dispose-beta");
    }

    [Fact(DisplayName = "UAC02: Stages are disposed in registration order")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_DisposeOrderMatchesRegistration()
    {
        var log = new List<string>();
        var registry = new PipelineRegistry();
        registry.Register(new DisposableStage("first", log));
        registry.Register(new DisposableStage("second", log));
        Ctx.Services.AddSingleton(registry);

        var cut = Ctx.Render<SessionPipelineHostSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);
        await cut.Instance.DisposeAsync();

        var disposeIndex1 = log.IndexOf("dispose-first");
        var disposeIndex2 = log.IndexOf("dispose-second");
        disposeIndex1.ShouldBeLessThan(disposeIndex2);
    }

    [Fact(DisplayName = "UAC03: Dispose is idempotent — double dispose does not re-log")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DoubleDisposeIsIdempotent()
    {
        var log = new List<string>();
        var registry = new PipelineRegistry();
        registry.Register(new DisposableStage("solo", log));
        Ctx.Services.AddSingleton(registry);

        var cut = Ctx.Render<SessionPipelineHostSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);
        await cut.Instance.DisposeAsync();
        await cut.Instance.DisposeAsync();

        log.Count(x => x == "dispose-solo").ShouldBe(1);
    }

    [Fact(DisplayName = "UAC04: Component context disposal disposes stages")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ContextDisposalCleansUpStages()
    {
        var log = new List<string>();
        var registry = new PipelineRegistry();
        var stage = new DisposableStage("ctx-stage", log);
        registry.Register(stage);
        Ctx.Services.AddSingleton(registry);

        var cut = Ctx.Render<SessionPipelineHostSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        Ctx.Dispose();

        // After BunitContext disposal, the component tree is disposed
        stage.IsDisposed.ShouldBeTrue();
        log.ShouldContain("dispose-ctx-stage");
    }
}
