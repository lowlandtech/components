using Bunit;
using CalendarSut = LowlandTech.Components.Calendar.Calendar;
using LowlandTech.Components.Calendar;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US02.Modes;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "03")]
[UserStory("02", "Calendar selection modes")]
public class SC03_MultipleMode : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar Mode multiple selects multiple dates")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MultipleModeSelectsMultipleDates()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Multiple);
        });

        var day10 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "10");
        var day15 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");

        day10.Click();
        day15.Click();
        day20.Click();

        cut.Instance.SelectedDates.Count.ShouldBe(3);
        cut.Instance.SelectedDates.ShouldContain(d => d.Day == 10);
        cut.Instance.SelectedDates.ShouldContain(d => d.Day == 15);
        cut.Instance.SelectedDates.ShouldContain(d => d.Day == 20);
    }

    [Fact(DisplayName = "UAC02: Multiple mode toggles selection on second click")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_MultipleModeTogglesSelection()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Multiple);
        });

        var day10 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "10");

        day10.Click();
        cut.Instance.SelectedDates.Count.ShouldBe(1);

        day10.Click();
        cut.Instance.SelectedDates.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Multiple mode triggers SelectedDatesChanged")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_MultipleModeTriggersEvent()
    {
        List<DateTime>? dates = null;
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Multiple);
            p.Add(x => x.SelectedDatesChanged, EventCallback.Factory.Create<List<DateTime>>(this, d => dates = d));
        });

        var day15 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");

        day15.Click();

        dates.ShouldNotBeNull();
        dates!.Count.ShouldBe(1);
        dates[0].Day.ShouldBe(15);
    }

    [Fact(DisplayName = "UAC04: Multiple mode marks all selected dates")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_MultipleModeMarksAllSelected()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Multiple);
        });

        var day10 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "10");
        var day20 = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");

        day10.Click();
        day20.Click();

        cut.Render();

        var selectedCells = cut.FindAll("[data-slot='calendar-day'][data-selected='True']");
        selectedCells.Count.ShouldBe(2);
    }
}
