namespace LowlandTech.Components.Tests.SPEC1078.SessionBlock;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "01")]
[UserStory("03", "SessionBlock runtime pipelines")]
public class SC01_SessionBlockRenders : ViewScenario
{
    [Fact(DisplayName = "UAC01: SessionBlock renders its child content")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SessionBlockRendersChildContent()
    {
        var cut = Ctx.Render<SessionBlockSut>(parameters => parameters
            .Add(p => p.ChildContent, builder =>
                builder.AddMarkupContent(0, "<span data-testid='child'>Hello Session</span>")));

        cut.Find("[data-testid='session-app']").ShouldNotBeNull();
        cut.Find("[data-testid='child']").TextContent.ShouldBe("Hello Session");
    }

    [Fact(DisplayName = "UAC02: SessionBlock renders without child content")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SessionBlockRendersWithoutChildContent()
    {
        var cut = Ctx.Render<SessionBlockSut>();

        cut.Find("[data-testid='session-app']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SessionBlock exposes PipelineComplete after init")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PipelineCompleteAfterInit()
    {
        var cut = Ctx.Render<SessionBlockSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.PipelineComplete.ShouldBeTrue();
    }
}
