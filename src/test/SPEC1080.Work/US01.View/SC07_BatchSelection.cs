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

    [Fact(DisplayName = "UAC06: Work list selection uses Checkbox component")]
    [Trait(Spec.UAC, "06")]
    [Given("The enqueued work list is rendered")]
    [When("Selection controls are inspected")]
    [Then("Rows use the shared Checkbox component instead of native checkbox inputs")]
    public void UAC06_WorkListSelectionUsesCheckboxComponent()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var checkbox = cut.Find("[data-testid='work-list-checkbox']");
        checkbox.GetAttribute("data-slot").ShouldBe("checkbox");
        checkbox.GetAttribute("role").ShouldBe("checkbox");
        cut.FindAll("input[type='checkbox']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC07: Recurring list selection uses Checkbox component")]
    [Trait(Spec.UAC, "07")]
    [Given("The recurring work list is rendered")]
    [When("Selection controls are inspected")]
    [Then("Rows use the shared Checkbox component instead of native checkbox inputs")]
    public void UAC07_RecurringListSelectionUsesCheckboxComponent()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Recurring);
        cut.Render();

        var checkbox = cut.Find("[data-testid='work-recurring-checkbox']");
        checkbox.GetAttribute("data-slot").ShouldBe("checkbox");
        checkbox.GetAttribute("role").ShouldBe("checkbox");
        cut.FindAll("input[type='checkbox']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: Batch selector renders as standalone panel")]
    [Trait(Spec.UAC, "08")]
    [Given("The enqueued work list is rendered")]
    [When("The batch selector is inspected")]
    [Then("It should have rounded corners, full border, and bottom spacing")]
    public void UAC08_BatchSelectorRendersAsStandalonePanel()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var selector = cut.Find("[data-slot='work-batch-selector']");
        selector.ClassList.ShouldContain("rounded-md");
        selector.ClassList.ShouldContain("border");
        selector.ClassList.ShouldContain("mb-3");
        selector.ClassList.ShouldNotContain("border-b");
    }

    [Fact(DisplayName = "UAC09: Batch selected count aligns to the end")]
    [Trait(Spec.UAC, "09")]
    [Given("A partial batch selection is rendered")]
    [When("The batch selector layout is inspected")]
    [Then("The selected count is separated from the select-all control with leading spacing")]
    public void UAC09_BatchSelectedCountAlignsToEnd()
    {
        var cut = Ctx.Render<WorkBatchSelector>(parameters => parameters
            .Add(x => x.SelectedCount, 3)
            .Add(x => x.TotalCount, 12));

        var selector = cut.Find("[data-slot='work-batch-selector']");
        var status = cut.Find("[data-slot='work-batch-status']");

        selector.ClassList.ShouldContain("justify-between");
        status.ClassList.ShouldContain("ml-3");
        status.TextContent.ShouldContain("3 of 12 selected");
    }
}
