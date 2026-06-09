namespace LowlandTech.Components.Tests.SPEC1078.SessionPipelineHost;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "01")]
[UserStory("03", "SessionPipelineHost runtime pipelines")]
public class SC01_SessionPipelineHostRenders : ViewScenario
{
    [Fact(DisplayName = "UAC01: SessionPipelineHost renders its child content")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SessionPipelineHostRendersChildContent()
    {
        var cut = Ctx.Render<SessionPipelineHostSut>(parameters => parameters
            .Add(p => p.ChildContent, builder =>
                builder.AddMarkupContent(0, "<span data-testid='child'>Hello Session</span>")));

        cut.Find("[data-testid='session-pipeline-host']").ShouldNotBeNull();
        cut.Find("[data-testid='child']").TextContent.ShouldBe("Hello Session");
    }

    [Fact(DisplayName = "UAC02: SessionPipelineHost renders without child content")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SessionPipelineHostRendersWithoutChildContent()
    {
        var cut = Ctx.Render<SessionPipelineHostSut>();

        cut.Find("[data-testid='session-pipeline-host']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SessionPipelineHost exposes PipelineComplete after init")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PipelineCompleteAfterInit()
    {
        var cut = Ctx.Render<SessionPipelineHostSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.PipelineComplete.ShouldBeTrue();
    }
}
