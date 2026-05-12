using Bunit;
using CalendarSut = LowlandTech.Components.Calendar.Calendar;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US01.View;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Calendar component")]
public class SC01_RenderCalendar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CalendarSut>();
        cut.Find("[data-slot='calendar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Calendar renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CalendarSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Calendar renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CalendarSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='calendar']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Calendar renders month and year caption")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersMonthAndYearCaption()
    {
        var cut = Ctx.Render<CalendarSut>();
        cut.Find(".cn-calendar-caption").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Calendar renders navigation buttons")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersNavigationButtons()
    {
        var cut = Ctx.Render<CalendarSut>();
        cut.FindAll(".cn-calendar-nav-button").Count.ShouldBe(4);
    }

    [Fact(DisplayName = "UAC06: Calendar renders weekday headers")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWeekdayHeaders()
    {
        var cut = Ctx.Render<CalendarSut>();
        cut.FindAll(".cn-calendar-weekday").Count.ShouldBe(7);
    }

    [Fact(DisplayName = "UAC07: Calendar renders day buttons")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersDayButtons()
    {
        var cut = Ctx.Render<CalendarSut>();
        cut.FindAll(".cn-calendar-day-button").Count.ShouldBe(42); // 6 weeks
    }
}
