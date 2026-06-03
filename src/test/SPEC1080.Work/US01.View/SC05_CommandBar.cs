namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Command bar dispatches actions for selected jobs")]
public class SC05_CommandBar : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Command bar does not render when no jobs selected")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CommandBarHiddenWhenNoSelection()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        StateService.ClearSelection();
        cut.Render();

        cut.FindAll("[data-slot='work-command-bar']").Count.ShouldBe(0);
        cut.FindAll("[data-testid='work-cmd-requeue']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Command bar renders after selecting a job")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CommandBarRendersAfterSelection()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.ToggleSelection(jobId);
        cut.Render();

        cut.Find("[data-testid='work-cmd-requeue']").ShouldNotBeNull();
        cut.Find("[data-testid='work-cmd-delete']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Clicking requeue adds command to event log")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ClickingRequeueAddsCommandToEventLog()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.ToggleSelection(jobId);
        cut.Render();

        var beforeCount = StateService.State.EventLogEntries.Count;
        cut.Find("[data-testid='work-cmd-requeue']").Click();

        StateService.State.EventLogEntries.Count.ShouldBeGreaterThan(beforeCount);
    }

    [Fact(DisplayName = "UAC04: Clicking delete adds command to event log")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ClickingDeleteAddsCommandToEventLog()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.ToggleSelection(jobId);
        cut.Render();

        var beforeCount = StateService.State.EventLogEntries.Count;
        cut.Find("[data-testid='work-cmd-delete']").Click();

        StateService.State.EventLogEntries.Count.ShouldBeGreaterThan(beforeCount);
    }

    [Fact(DisplayName = "UAC05: Enqueue button renders on scheduled view")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_EnqueueButtonRendersOnScheduledView()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Scheduled);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.ToggleSelection(jobId);
        cut.Render();

        cut.Find("[data-testid='work-cmd-enqueue']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Delete command renders trash icon")]
    [Trait(Spec.UAC, "06")]
    [Given("A job is selected in the enqueued view")]
    [When("The command bar renders the delete command")]
    [Then("The delete button should include the trash icon")]
    public void UAC06_DeleteCommandRendersTrashIcon()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var jobId = StateService.State.Jobs.First().Id;
        StateService.ToggleSelection(jobId);
        cut.Render();

        var delete = cut.Find("[data-testid='work-cmd-delete']");
        delete.QuerySelector("svg path[d='M3 6h18']").ShouldNotBeNull();
        delete.QuerySelector("svg line[x1='10'][y1='11']").ShouldNotBeNull();
    }
}
