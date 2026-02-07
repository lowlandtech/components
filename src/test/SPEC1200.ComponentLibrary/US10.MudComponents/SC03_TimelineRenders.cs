using Bunit;
using LowlandTech.Components.Components.Timeline;
using Shouldly;

namespace LowlandTech.Tests.SPEC1227.ComponentLibrary.US10.MudComponents;

[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "03")]
public class SC03_TimelineRenders : Scenario
{
    private TestContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new TestContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "UAC09: Then I should have Timeline and TimelineItem components")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_ShouldHaveTimelineComponents()
    {
        await ArrangeAndActAsync();
        var timeline = _ctx.Render<Timeline>();
        var timelineItem = _ctx.Render<TimelineItem>();

        timeline.ShouldNotBeNull();
        timelineItem.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: And Timeline should render a wrapper with class timeline")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldRenderTimelineWrapper()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<Timeline>();

        var element = component.Find(".timeline");
        element.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: And TimelineItem should render a wrapper with class timeline-item")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_ShouldRenderTimelineItemWrapper()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<TimelineItem>();

        var element = component.Find(".timeline-item");
        element.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: And Timeline should support orientation classes")]
    [Trait(Spec.UAC, "12")]
    public async Task UAC12_ShouldSupportOrientationClasses()
    {
        await ArrangeAndActAsync();
        
        using var ctx1 = new TestContext();
        var vertical = ctx1.Render<Timeline>(parameters => parameters.Add(p => p.Orientation, "vertical"));
        vertical.Find(".timeline").ClassList.ShouldContain("timeline-vertical");

        using var ctx2 = new TestContext();
        var horizontal = ctx2.Render<Timeline>(parameters => parameters.Add(p => p.Orientation, "horizontal"));
        horizontal.Find(".timeline").ClassList.ShouldContain("timeline-horizontal");
    }

    [Fact(DisplayName = "UAC13: And TimelineItem should support alignment classes")]
    [Trait(Spec.UAC, "13")]
    public async Task UAC13_ShouldSupportAlignmentClasses()
    {
        await ArrangeAndActAsync();
        
        using var ctx1 = new TestContext();
        var start = ctx1.Render<TimelineItem>(parameters => parameters.Add(p => p.Align, "start"));
        start.Find(".timeline-item").ClassList.ShouldContain("timeline-start");

        using var ctx2 = new TestContext();
        var center = ctx2.Render<TimelineItem>(parameters => parameters.Add(p => p.Align, "center"));
        center.Find(".timeline-item").ClassList.ShouldContain("timeline-center");

        using var ctx3 = new TestContext();
        var end = ctx3.Render<TimelineItem>(parameters => parameters.Add(p => p.Align, "end"));
        end.Find(".timeline-item").ClassList.ShouldContain("timeline-end");
    }
}
