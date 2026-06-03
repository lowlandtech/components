namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "08")]
[UserStory("01", "Timeline displays work state history")]
public class SC08_Timeline : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Timeline markers align to the center of status cards")]
    [Trait(Spec.UAC, "01")]
    [Given("A work timeline with state history")]
    [When("The timeline is rendered")]
    [Then("Timeline markers are centered beside each status card")]
    public void UAC01_MarkersAlignToCenterOfStatusCards()
    {
        var history = WorkUseCaseData.BuildHistory(StateService.State.Jobs.First());
        var cut = Ctx.Render<WorkTimeline>(p => p.Add(x => x.History, history));

        var item = cut.Find("[data-slot='work-timeline-item']");
        var markerColumn = item.FirstElementChild;
        var dot = item.QuerySelector("[data-slot='work-timeline-dot']");

        item.ClassList.ShouldContain("grid-cols-[0.75rem_minmax(0,1fr)]");
        markerColumn!.ClassList.ShouldContain("items-center");
        markerColumn.ClassList.ShouldContain("justify-center");
        dot.ShouldNotBeNull();
        cut.Find("[data-slot='work-timeline-line']").ClassList.ShouldContain("left-1.5");
    }

    [Fact(DisplayName = "UAC02: Empty timeline explains that events will appear later")]
    [Trait(Spec.UAC, "02")]
    [Given("A work timeline without state history")]
    [When("The timeline is rendered")]
    [Then("The empty state communicates that future job events will appear there")]
    public void UAC02_EmptyTimelineExplainsFutureEvents()
    {
        var cut = Ctx.Render<WorkTimeline>(p => p.Add(x => x.History, []));

        var emptyState = cut.Find("[data-slot='work-timeline-empty']");

        emptyState.TextContent.ShouldContain("No timeline events yet");
        emptyState.TextContent.ShouldContain("State changes will appear here");
        emptyState.TextContent.ShouldNotContain("No state history");
        emptyState.QuerySelector("[data-slot='icon']").ShouldNotBeNull();
    }
}
