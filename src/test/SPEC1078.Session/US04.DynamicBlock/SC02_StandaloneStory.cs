namespace LowlandTech.Components.Tests.SPEC1078.Session.US04.DynamicBlock;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "02")]
[UserStory("04", "The standalone SessionBlock story hosts the complete DynamicBlock showcase")]
public sealed class SC02_StandaloneStory : ViewScenario
{
    [Fact(DisplayName = "UAC01: Standalone story renders the complete DynamicBlock showcase")]
    [Trait(Spec.UAC, "01")]
    [Given("the standalone SessionBlock story used for full-surface defect discovery")]
    [When("the story preview is rendered")]
    [Then("it hosts DynamicBlock and exposes model-derived workspace, flow, and workitem collections")]
    public void UAC01_StandaloneStoryRendersCompleteDynamicBlockShowcase()
    {
        var storyType = typeof(StorySourceRegistry).Assembly.GetType(
            "LowlandTech.Components.Stories.Showcode.Examples.Session.SessionBlockStandaloneExample");
        storyType.ShouldNotBeNull();

        var cut = Ctx.Render<DynamicComponent>(parameters => parameters
            .Add(component => component.Type, storyType));

        cut.Find("[data-slot='dynamic-block']").ShouldNotBeNull();
        cut.Find("[data-node-id='foundry-usecase:Workspaces']").ShouldNotBeNull();
        cut.Find("[data-node-id='foundry-usecase:Flows']").ShouldNotBeNull();
        cut.Find("[data-node-id='foundry-usecase:Workitems']").ShouldNotBeNull();
    }
}
