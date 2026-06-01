namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Batch selection toggles and selects all jobs")]
public class SC07_BatchSelection : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Batch selector renders on listing views")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_BatchSelectorRendersOnListingViews()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        cut.Find("[data-slot='work-batch-selector']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Select All checkbox selects all visible jobs")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SelectAllCheckboxSelectsAllVisibleJobs()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        StateService.ClearSelection();
        cut.Render();

        var visibleCount = StateService.GetPagedJobs().Count;
        visibleCount.ShouldBeGreaterThan(0);

        cut.Find("[data-testid='work-batch-select-all']").Click();

        StateService.State.Ui.SelectedJobIds.Count.ShouldBe(visibleCount);
    }

    [Fact(DisplayName = "UAC03: Selecting a single job via checkbox updates state")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectingSingleJobViaCheckboxUpdatesState()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        StateService.ClearSelection();
        cut.Render();

        var jobId = StateService.State.Jobs.First().Id;
        StateService.ToggleSelection(jobId);
        cut.Render();

        StateService.State.Ui.SelectedJobIds.ShouldContain(jobId);
        cut.Find("[data-testid='work-batch-count']").TextContent.ShouldContain("1 of");
    }

    [Fact(DisplayName = "UAC04: Selecting all then deselecting all clears selection")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SelectingAllThenDeselectingAllClearsSelection()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        StateService.SelectAll();
        cut.Render();

        StateService.State.Ui.SelectedJobIds.Count.ShouldBeGreaterThan(0);

        StateService.ClearSelection();
        cut.Render();

        StateService.State.Ui.SelectedJobIds.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: Event log captures batch selection action")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_EventLogCapturesBatchSelectionAction()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var beforeCount = StateService.State.EventLogEntries.Count;

        StateService.SelectAll();
        cut.Render();

        StateService.State.EventLogEntries.Count.ShouldBeGreaterThan(beforeCount);
    }
}
