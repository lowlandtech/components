using Bunit;
using CalendarSut = LowlandTech.Components.Calendar;
using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US02.Modes;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Calendar selection modes")]
public class SC01_SingleMode : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar Mode single selects one date")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SingleModeSelectsOneDate()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
        });

        var dayButton = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        dayButton.Click();

        cut.Instance.SelectedDate.ShouldNotBeNull();
        cut.Instance.SelectedDate.Value.Day.ShouldBe(15);
    }

    [Fact(DisplayName = "UAC02: Single mode replaces previous selection")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SingleModeReplacesSelection()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
            p.Add(x => x.SelectedDate, new DateTime(2024, 1, 10));
        });

        var dayButton = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "20");
        dayButton.Click();

        cut.Instance.SelectedDate.ShouldNotBeNull();
        cut.Instance.SelectedDate.Value.Day.ShouldBe(20);
    }

    [Fact(DisplayName = "UAC03: Single mode triggers DateChanged event")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SingleModeTriggersEvent()
    {
        DateChangedEventArgs? eventArgs = null;
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
            p.Add(x => x.DateChanged, EventCallback.Factory.Create<DateChangedEventArgs>(this, args => eventArgs = args));
        });

        var dayButton = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        dayButton.Click();

        eventArgs.ShouldNotBeNull();
        eventArgs!.Mode.ShouldBe(CalendarMode.Single);
        eventArgs.Date.Day.ShouldBe(15);
    }

    [Fact(DisplayName = "UAC04: Single mode SelectedDateChanged is called")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SingleModeSelectedDateChanged()
    {
        DateTime? selectedDate = null;
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
            p.Add(x => x.SelectedDateChanged, EventCallback.Factory.Create<DateTime?>(this, date => selectedDate = date));
        });

        var dayButton = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        dayButton.Click();

        selectedDate.ShouldNotBeNull();
        selectedDate.Value.Day.ShouldBe(15);
    }
}
