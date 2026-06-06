using Bunit;
using CalendarSut = LowlandTech.Components.Calendar;
using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US02.Modes;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Calendar selection modes")]
public class SC02_RangeMode : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar Mode range selects start and end")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RangeModeSelectsStartAndEnd()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Range);
        });

        var day15 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");

        day15.Click();
        day20.Click();

        cut.Instance.SelectedRange.ShouldNotBeNull();
        cut.Instance.SelectedRange!.Start.Day.ShouldBe(15);
        cut.Instance.SelectedRange.End.Day.ShouldBe(20);
    }

    [Fact(DisplayName = "UAC02: Range mode handles reverse selection")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RangeModeHandlesReverseSelection()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Range);
        });

        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");
        var day10 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "10");

        day20.Click();
        day10.Click();

        cut.Instance.SelectedRange.ShouldNotBeNull();
        cut.Instance.SelectedRange!.Start.Day.ShouldBe(10);
        cut.Instance.SelectedRange.End.Day.ShouldBe(20);
    }

    [Fact(DisplayName = "UAC03: Range mode marks dates in range as selected")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RangeModeMarksDatesInRange()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Range);
        });

        var day15 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        var day17 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "17");
        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");

        day15.Click();
        day20.Click();

        // Refresh the render to get updated attributes
        cut.Render();

        var day17Cell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "17");
        day17Cell.GetAttribute("data-selected").ShouldBe("True");
    }

    [Fact(DisplayName = "UAC04: Range mode triggers SelectedRangeChanged")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RangeModeTriggersEvent()
    {
        DateRange? range = null;
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Range);
            p.Add(x => x.SelectedRangeChanged, EventCallback.Factory.Create<DateRange?>(this, r => range = r));
        });

        var day15 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");

        day15.Click();
        day20.Click();

        range.ShouldNotBeNull();
        range!.Start.Day.ShouldBe(15);
        range.End.Day.ShouldBe(20);
    }

    [Fact(DisplayName = "UAC05: Range mode sets data-range-start and data-range-end")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RangeModeSetsRangeAttributes()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Range);
        });

        var day15 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");

        day15.Click();
        day20.Click();

        cut.Render();

        var day15Cell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "15");
        var day20Cell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "20");

        day15Cell.GetAttribute("data-range-start").ShouldBe("True");
        day20Cell.GetAttribute("data-range-end").ShouldBe("True");
    }
}
