namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "01")]
[UserStory("01", "WorkApp renders with default dashboard view")]
public class SC01_WorkAppRendersDashboard : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: WorkApp root element has data-testid and data-slot")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RootElementHasDataTestIdAndSlot()
    {
        var cut = RenderWorkApp();

        var root = cut.Find("[data-testid='work-app']");
        root.ShouldNotBeNull();
        root.GetAttribute("data-slot").ShouldBe("work-app");
    }

    [Fact(DisplayName = "UAC02: Default active view is Dashboard")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DefaultActiveViewIsDashboard()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-active-view='dashboard']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Dashboard renders metric cards")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DashboardRendersMetricCards()
    {
        var cut = RenderWorkApp();

        var metrics = cut.FindAll("[data-slot='work-metric']");
        metrics.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC04: Dashboard renders history graph")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DashboardRendersHistoryGraph()
    {
        var cut = RenderWorkApp();

        cut.Find("[data-slot='work-graph']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Dashboard metrics derive from deterministic scenario")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_MetricsDeriveFromScenario()
    {
        var cut = RenderWorkApp();

        var metricValues = StateService.State.Metrics.Select(m => m.Value).ToList();
        metricValues.ShouldContain("15"); // succeeded count from HealthyCluster
        metricValues.ShouldContain("5");  // failed count
        metricValues.ShouldContain("3");  // processing count
        metricValues.ShouldContain("8");  // enqueued count
    }

    [Fact(DisplayName = "UAC06: Queue counts match generated jobs")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_QueueCountsMatchGeneratedJobs()
    {
        var cut = RenderWorkApp();

        var expectedCounts = StateService.State.Jobs
            .Where(j => j.State == WorkItemState.Enqueued)
            .GroupBy(j => j.QueueName)
            .ToDictionary(g => g.Key!, g => g.Count());

        foreach (var queue in StateService.State.Queues)
        {
            var expected = expectedCounts.GetValueOrDefault(queue.Name, 0);
            queue.Length.ShouldBe(expected);
        }
    }
}
