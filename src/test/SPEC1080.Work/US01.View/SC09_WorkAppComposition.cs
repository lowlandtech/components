namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "09")]
[UserStory("01", "WorkApp composes the Work surface from explicit child components")]
public class SC09_WorkAppComposition : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Default layout exposes compound WorkApp slots")]
    [Trait(Spec.UAC, "01")]
    [Given("a WorkApp rendered without custom child content")]
    [When("the default layout is produced")]
    [Then("the sidebar, main, header, and content slots are visible as composition points")]
    public void UAC01_DefaultLayoutExposesCompoundSlots()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-slot='work-app-sidebar']").ShouldNotBeNull();
        cut.Find("[data-slot='work-app-main']").ShouldNotBeNull();
        cut.Find("[data-slot='work-app-header']").ShouldNotBeNull();
        cut.Find("[data-slot='work-app-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Consumer child content replaces default layout")]
    [Trait(Spec.UAC, "02")]
    [Given("a WorkApp rendered with explicit child content")]
    [When("the consumer supplies a WorkAppMain fragment")]
    [Then("the supplied layout is rendered inside the WorkApp state provider")]
    public void UAC02_ConsumerChildContentReplacesDefaultLayout()
    {
        var cut = Ctx.Render<WorkBlock>(parameters => parameters
            .Add(x => x.StateService, StateService)
            .Add(x => x.ChildContent, builder =>
            {
                builder.OpenComponent<WorkAppMain>(0);
                builder.AddAttribute(1, "data-testid", "custom-work-main");
                builder.CloseComponent();
            }));

        cut.Find("[data-testid='custom-work-main']").ShouldNotBeNull();
        cut.FindAll("[data-slot='work-app-sidebar']").Count.ShouldBe(0);
    }
}
