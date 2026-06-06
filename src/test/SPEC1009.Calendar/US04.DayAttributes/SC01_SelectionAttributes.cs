using Bunit;
using CalendarSut = LowlandTech.Components.Calendar;
using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US04.DayAttributes;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "Calendar day attributes")]
public class SC01_SelectionAttributes : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar day data-selected reflects selection")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DataSelectedReflectsSelection()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
        });

        var dayButton = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        dayButton.Click();

        cut.Render();

        var dayCell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "15");
        dayCell.GetAttribute("data-selected").ShouldBe("True");
    }

    [Fact(DisplayName = "UAC02: Calendar day data-selected is false when not selected")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DataSelectedFalseWhenNotSelected()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
            p.Add(x => x.SelectedDate, new DateTime(2024, 1, 15));
        });

        var dayCell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "20");
        dayCell.GetAttribute("data-selected").ShouldBe("False");
    }

    [Fact(DisplayName = "UAC03: Calendar day data-today marks today")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DataTodayMarksToday()
    {
        var today = DateTime.Today;
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, new DateTime(today.Year, today.Month, 1));
        });

        var todayCell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == today.Day.ToString());
        todayCell.GetAttribute("data-today").ShouldBe("True");
    }

    [Fact(DisplayName = "UAC04: Calendar day data-today is false for other days")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DataTodayFalseForOtherDays()
    {
        var today = DateTime.Today;
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, new DateTime(today.Year, today.Month, 1));
        });

        var otherDay = today.Day == 15 ? 16 : 15;
        var otherCell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == otherDay.ToString());
        otherCell.GetAttribute("data-today").ShouldBe("False");
    }

    [Fact(DisplayName = "UAC05: Calendar day data-outside marks outside month days")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DataOutsideMarksOutsideDays()
    {
        // Render for a month where we know there are outside days
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 1)); // June 2024
        });

        var outsideCells = cut.FindAll("[data-slot='calendar-day'][data-outside='True']");
        outsideCells.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC06: Calendar day data-outside is false for current month")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DataOutsideFalseForCurrentMonth()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 1));
        });

        var day15Cell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "15");
        day15Cell.GetAttribute("data-outside").ShouldBe("False");
    }
}
