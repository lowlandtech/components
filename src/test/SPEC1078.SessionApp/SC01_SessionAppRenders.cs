namespace LowlandTech.Components.Tests.SPEC1078.SessionApp;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "01")]
[UserStory("03", "SessionApp runtime pipelines")]
public class SC01_SessionAppRenders : ViewScenario
{
    [Fact(DisplayName = "UAC01: SessionApp renders its child content")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SessionAppRendersChildContent()
    {
        var cut = Ctx.Render<SessionAppSut>(parameters => parameters
            .Add(p => p.ChildContent, builder =>
                builder.AddMarkupContent(0, "<span data-testid='child'>Hello Session</span>")));

        cut.Find("[data-testid='session-app']").ShouldNotBeNull();
        cut.Find("[data-testid='child']").TextContent.ShouldBe("Hello Session");
    }

    [Fact(DisplayName = "UAC02: SessionApp renders without child content")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SessionAppRendersWithoutChildContent()
    {
        var cut = Ctx.Render<SessionAppSut>();

        cut.Find("[data-testid='session-app']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SessionApp exposes PipelineComplete after init")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PipelineCompleteAfterInit()
    {
        var cut = Ctx.Render<SessionAppSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.PipelineComplete.ShouldBeTrue();
    }
}
