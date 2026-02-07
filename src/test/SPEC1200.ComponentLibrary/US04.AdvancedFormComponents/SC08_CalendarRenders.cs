using Bunit;
using LowlandTech.Components.Components.Calendar;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Calendar component rendering and functionality.
/// SC08 - Implement Calendar component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "08")]
public class SC08_CalendarRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Calendar>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Calendar>(parameters => parameters
            .Add(p => p.Value, new DateTime(2024, 2, 15)));
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a monthly calendar grid")]
    [Trait(Spec.UAC, "25")]
    public async Task UAC25_ShouldRenderCalendarGrid()
    {
        await ArrangeAndActAsync();

        // Calendar root should exist
        var calendar = _component!.Find(".calendar");
        calendar.ShouldNotBeNull();

        // Header with navigation should exist
        var header = _component.Find(".calendar-header");
        header.ShouldNotBeNull();

        // Month display should exist
        var month = _component.Find(".calendar-month");
        month.ShouldNotBeNull();
        month.TextContent.ShouldContain("February");
        month.TextContent.ShouldContain("2024");

        // Navigation buttons should exist
        var navButtons = _component.FindAll(".calendar-nav");
        navButtons.Count.ShouldBe(2); // Previous and Next

        // Grid should exist
        var grid = _component.Find(".calendar-grid");
        grid.ShouldNotBeNull();

        // Should have 7 weekday headers
        var weekdays = _component.FindAll(".calendar-weekday");
        weekdays.Count.ShouldBe(7);
        weekdays[0].TextContent.ShouldBe("Su");
        weekdays[1].TextContent.ShouldBe("Mo");
        weekdays[6].TextContent.ShouldBe("Sa");

        // Should have day cells (at least 28 for the shortest month)
        var days = _component.FindAll(".calendar-day");
        days.Count.ShouldBeGreaterThan(27);
    }

    [Fact(DisplayName = "And it should support single date selection")]
    [Trait(Spec.UAC, "26")]
    public async Task UAC26_ShouldSupportDateSelection()
    {
        _ctx = new BunitContext();
        DateTime? selectedDate = new DateTime(2024, 2, 15);
        var dateChanged = false;

        _component = _ctx.Render<Calendar>(parameters => parameters
            .Add(p => p.Value, selectedDate)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<DateTime?>(this, date =>
            {
                selectedDate = date;
                dateChanged = true;
            })));

        // Initial selected date should be highlighted
        var selectedDay = _component.FindAll(".calendar-day-selected");
        selectedDay.Count.ShouldBeGreaterThan(0);

        // Find a day button that's not disabled and not outside month
        var dayButtons = _component.FindAll(".calendar-day")
            .Where(d => !d.ClassList.Contains("calendar-day-outside") && 
                       !d.HasAttribute("disabled"))
            .ToList();

        dayButtons.Count.ShouldBeGreaterThan(0);

        // Click on a different day (first available day)
        var firstDay = dayButtons.First();
        firstDay.Click();

        // ValueChanged should have been invoked
        dateChanged.ShouldBeTrue();
        selectedDate.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support disabled dates")]
    [Trait(Spec.UAC, "27")]
    public async Task UAC27_ShouldSupportDisabledDates()
    {
        _ctx = new BunitContext();

        // Disable all Sundays (day of week = 0)
        Func<DateTime, bool> disableSundays = date => date.DayOfWeek == DayOfWeek.Sunday;

        _component = _ctx.Render<Calendar>(parameters => parameters
            .Add(p => p.Value, new DateTime(2024, 2, 15))
            .Add(p => p.DisabledDates, disableSundays));

        // Find all day buttons
        var allDays = _component.FindAll(".calendar-day");
        allDays.Count.ShouldBeGreaterThan(0);

        // Find disabled days
        var disabledDays = allDays.Where(d => d.HasAttribute("disabled")).ToList();
        disabledDays.Count.ShouldBeGreaterThan(0);

        // Check that disabled days have the disabled class
        var disabledDaysWithClass = _component.FindAll(".calendar-day-disabled");
        disabledDaysWithClass.Count.ShouldBeGreaterThan(0);

        // Verify at least some Sundays are disabled (there are 4-5 Sundays in a month view)
        disabledDays.Count.ShouldBeGreaterThanOrEqualTo(4);
    }
}
