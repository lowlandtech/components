using Bunit;
using CalendarSut = LowlandTech.Components.Calendar.Calendar;
using LowlandTech.Components.Calendar;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US04.DayAttributes;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "02")]
[UserStory("04", "Calendar day attributes")]
public class SC02_DisabledState : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar day data-disabled reflects disabled")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DataDisabledReflectsDisabled()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.IsDateDisabled, date => date.Day == 15);
        });

        var day15Cell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "15");
        day15Cell.GetAttribute("data-disabled").ShouldBe("True");
    }

    [Fact(DisplayName = "UAC02: Calendar day data-disabled is false for enabled dates")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DataDisabledFalseForEnabled()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.IsDateDisabled, date => date.Day == 15);
        });

        var day20Cell = cut.FindAll("[data-slot='calendar-day']")
            .First(c => c.QuerySelector("button")?.TextContent?.Trim() == "20");
        day20Cell.GetAttribute("data-disabled").ShouldBe("False");
    }

    [Fact(DisplayName = "UAC03: Disabled day button is disabled")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DisabledDayButtonIsDisabled()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.IsDateDisabled, date => date.Day == 15);
        });

        var day15Button = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        day15Button.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: Clicking disabled date does not select it")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ClickingDisabledDoesNotSelect()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.Mode, CalendarMode.Single);
            p.Add(x => x.IsDateDisabled, date => date.Day == 15);
        });

        var day15Button = cut.FindAll("[data-slot='calendar-day'] button")
            .First(b => b.TextContent.Trim() == "15");
        day15Button.Click();

        cut.Instance.SelectedDate.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC05: Weekend dates can be disabled")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_WeekendDatesCanBeDisabled()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 1)); // June 2024
            p.Add(x => x.IsDateDisabled, date => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        });

        var disabledCells = cut.FindAll("[data-slot='calendar-day'][data-disabled='True']");
        disabledCells.Count.ShouldBeGreaterThan(0);
    }
}
