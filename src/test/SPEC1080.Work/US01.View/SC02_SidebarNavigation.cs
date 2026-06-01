namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Sidebar navigation changes active view")]
public class SC02_SidebarNavigation : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Sidebar renders all job state links")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SidebarRendersAllJobStateLinks()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-testid='work-sidebar-enqueued']").ShouldNotBeNull();
        cut.Find("[data-testid='work-sidebar-scheduled']").ShouldNotBeNull();
        cut.Find("[data-testid='work-sidebar-processing']").ShouldNotBeNull();
        cut.Find("[data-testid='work-sidebar-succeeded']").ShouldNotBeNull();
        cut.Find("[data-testid='work-sidebar-failed']").ShouldNotBeNull();
        cut.Find("[data-testid='work-sidebar-deleted']").ShouldNotBeNull();
        cut.Find("[data-testid='work-sidebar-awaiting']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Clicking enqueued switches view to enqueued list")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ClickingEnqueuedSwitchesView()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-testid='work-sidebar-enqueued']").Click();

        cut.Find("[data-active-view='enqueued']").ShouldNotBeNull();
        cut.Find("[data-slot='work-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Clicking succeeded switches view to succeeded list")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ClickingSucceededSwitchesView()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-testid='work-sidebar-succeeded']").Click();

        cut.Find("[data-active-view='succeeded']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Clicking failed switches view to failed list")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ClickingFailedSwitchesView()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-testid='work-sidebar-failed']").Click();

        cut.Find("[data-active-view='failed']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: StateService reflects the navigated view")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_StateServiceReflectsNavigatedView()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-testid='work-sidebar-processing']").Click();

        StateService.State.Ui.ActiveView.ShouldBe(WorkView.Processing);
    }
}
