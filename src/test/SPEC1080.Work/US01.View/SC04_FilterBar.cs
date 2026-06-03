namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Filter bar filters the job list")]
public class SC04_FilterBar : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Filter input renders on listing views")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_FilterInputRendersOnListingViews()
    {
        var cut = RenderWorkApp();
        cut.Find("[data-testid='work-sidebar-enqueued']").Click();

        cut.Find("[data-testid='work-filter-input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Filtering reduces visible job rows")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_FilteringReducesVisibleJobRows()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var before = cut.FindAll("[data-slot='work-list'] [data-slot='data-grid-row']").Count;
        before.ShouldBeGreaterThan(0);

        StateService.SetFilter("nonexistent-job-xyz");
        cut.Render();

        cut.FindAll("[data-slot='work-list'] [data-slot='data-grid-row']").Count.ShouldBe(0);
        cut.Find("[data-slot='work-list'] [data-slot='data-grid-empty']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Filtering by job display name shows matching rows")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_FilteringByDisplayNameShowsMatchingRows()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var targetName = StateService.State.Jobs.First().JobDisplayName;
        StateService.SetFilter(targetName);
        cut.Render();

        var rows = cut.FindAll("[data-slot='work-list'] [data-slot='data-grid-row']");
        rows.Count.ShouldBeGreaterThan(0);
        rows.All(r => r.TextContent.Contains(targetName, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: Clearing filter restores all rows")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ClearingFilterRestoresAllRows()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        var before = StateService.State.Jobs.Count;

        StateService.SetFilter("narrow");
        cut.Render();
        cut.FindAll("[data-slot='work-list'] [data-slot='data-grid-row']").Count.ShouldBeLessThan(before);

        StateService.SetFilter(string.Empty);
        cut.Render();
        cut.FindAll("[data-slot='work-list'] [data-slot='data-grid-row']").Count.ShouldBe(StateService.GetPagedJobs().Count);
    }

    [Fact(DisplayName = "UAC05: Filter bar does not render page size controls")]
    [Trait(Spec.UAC, "05")]
    [Given("A listing view is rendered")]
    [When("The filter bar is inspected")]
    [Then("Page-size controls should not share the filter bar row")]
    public void UAC05_FilterBarDoesNotRenderPageSizeControls()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var filterBar = cut.Find("[data-slot='work-filter-bar']");
        filterBar.QuerySelector("[data-testid='work-page-size-20']").ShouldBeNull();
    }
}
