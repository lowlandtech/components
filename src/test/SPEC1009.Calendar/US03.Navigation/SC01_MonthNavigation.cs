using Bunit;
using CalendarSut = LowlandTech.Components.Calendar.Calendar;
using LowlandTech.Components.Calendar;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US03.Navigation;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "Calendar navigation")]
public class SC01_MonthNavigation : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar navigation previous month works")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PreviousMonthWorks()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var prevButton = cut.Find("[data-slot='calendar-nav-prev']");
        prevButton.Click();

        cut.Instance.CurrentMonth.Month.ShouldBe(5);
        cut.Instance.CurrentMonth.Year.ShouldBe(2024);
    }

    [Fact(DisplayName = "UAC02: Calendar navigation next month works")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NextMonthWorks()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var nextButton = cut.Find("[data-slot='calendar-nav-next']");
        nextButton.Click();

        cut.Instance.CurrentMonth.Month.ShouldBe(7);
        cut.Instance.CurrentMonth.Year.ShouldBe(2024);
    }

    [Fact(DisplayName = "UAC03: Previous month updates caption")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PreviousMonthUpdatesCaption()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var prevButton = cut.Find("[data-slot='calendar-nav-prev']");
        prevButton.Click();

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.TextContent.ShouldContain("May");
        caption.TextContent.ShouldContain("2024");
    }

    [Fact(DisplayName = "UAC04: Next month updates caption")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NextMonthUpdatesCaption()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var nextButton = cut.Find("[data-slot='calendar-nav-next']");
        nextButton.Click();

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.TextContent.ShouldContain("July");
        caption.TextContent.ShouldContain("2024");
    }

    [Fact(DisplayName = "UAC05: Previous month handles year boundary")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_PreviousMonthHandlesYearBoundary()
    {
        var initialMonth = new DateTime(2024, 1, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var prevButton = cut.Find("[data-slot='calendar-nav-prev']");
        prevButton.Click();

        cut.Instance.CurrentMonth.Month.ShouldBe(12);
        cut.Instance.CurrentMonth.Year.ShouldBe(2023);
    }

    [Fact(DisplayName = "UAC06: Next month handles year boundary")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NextMonthHandlesYearBoundary()
    {
        var initialMonth = new DateTime(2024, 12, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var nextButton = cut.Find("[data-slot='calendar-nav-next']");
        nextButton.Click();

        cut.Instance.CurrentMonth.Month.ShouldBe(1);
        cut.Instance.CurrentMonth.Year.ShouldBe(2025);
    }
}
