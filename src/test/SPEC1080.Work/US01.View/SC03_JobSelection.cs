namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Selecting a job opens the detail view")]
public class SC03_JobSelection : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Clicking a job row selects the job")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ClickingJobRowSelectsJob()
    {
        var cut = RenderWorkApp();
        cut.Find("[data-testid='work-sidebar-enqueued']").Click();

        var row = cut.Find("[data-testid='work-list-row']");
        row.Click();

        StateService.State.SelectedJob.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Detail panel appears when a job is selected")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DetailPanelAppearsWhenJobSelected()
    {
        var cut = RenderWorkApp();
        cut.Find("[data-testid='work-sidebar-enqueued']").Click();

        var row = cut.Find("[data-testid='work-list-row']");
        row.Click();

        cut.FindAll("[data-slot='work-detail']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC03: Detail view shows job display name")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DetailViewShowsJobDisplayName()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.SelectJob(jobId);
        cut.Render();

        cut.Find("[data-slot='work-detail']").TextContent.ShouldContain(StateService.State.SelectedJob!.JobDisplayName);
    }

    [Fact(DisplayName = "UAC04: Detail view shows state badge")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DetailViewShowsStateBadge()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.SelectJob(jobId);
        cut.Render();

        cut.Find("[data-slot='work-detail'] [data-slot='work-state-badge']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Detail view shows requeue and delete buttons")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DetailViewShowsActionButtons()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.SelectJob(jobId);
        cut.Render();

        cut.Find("[data-testid='work-detail-requeue']").ShouldNotBeNull();
        cut.Find("[data-testid='work-detail-delete']").ShouldNotBeNull();
    }
}
