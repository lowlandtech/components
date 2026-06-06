using Bunit;
using CalendarSut = LowlandTech.Components.Calendar;
using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US05.ViewZoom;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "02")]
[UserStory("05", "Calendar view zoom")]
public class SC02_YearPicker : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking caption in month view zooms to year picker")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ClickCaptionZoomsToYearPicker()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentView, CalendarView.Months);
        });

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.Click();

        cut.Instance.CurrentView.ShouldBe(CalendarView.Years);
        cut.FindAll("[data-slot='calendar-year-button']").Count.ShouldBe(12);
    }

    [Fact(DisplayName = "UAC02: Year picker shows correct year range")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_YearPickerShowsCorrectRange()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentMonth, new DateTime(2026, 3, 15));
            p.Add(x => x.CurrentView, CalendarView.Years);
        });

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.TextContent.ShouldContain("2021");
        caption.TextContent.ShouldContain("2032");

        var firstYear = cut.Find("[data-year='2021']");
        firstYear.ShouldNotBeNull();

        var lastYear = cut.Find("[data-year='2032']");
        lastYear.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Selecting a year returns to month view")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectingYearReturnsToMonthView()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 15));
            p.Add(x => x.CurrentView, CalendarView.Years);
        });

        var yearButton = cut.Find("[data-year='2030']");
        yearButton.Click();

        cut.Instance.CurrentView.ShouldBe(CalendarView.Months);
        cut.Instance.CurrentMonth.Year.ShouldBe(2030);
    }

    [Fact(DisplayName = "UAC04: Year picker prev/next navigate by range")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_YearPickerNavigatesByRange()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentMonth, new DateTime(2026, 3, 15));
            p.Add(x => x.CurrentView, CalendarView.Years);
        });

        var prevButton = cut.Find("[data-slot='calendar-nav-prev']");
        prevButton.Click();

        // Was 2021-2032, now 2009-2020
        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.TextContent.ShouldContain("2009");
        caption.TextContent.ShouldContain("2020");

        var nextButton = cut.Find("[data-slot='calendar-nav-next']");
        nextButton.Click();
        nextButton.Click();

        caption.TextContent.ShouldContain("2033");
        caption.TextContent.ShouldContain("2044");
    }

    [Fact(DisplayName = "UAC05: Year picker hides year jump buttons")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_YearPickerHidesYearNavButtons()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentView, CalendarView.Years);
        });

        cut.FindAll("[data-slot='calendar-nav-prev-year']").Count.ShouldBe(0);
        cut.FindAll("[data-slot='calendar-nav-next-year']").Count.ShouldBe(0);
        cut.FindAll("[data-slot='calendar-nav-prev']").Count.ShouldBe(1);
        cut.FindAll("[data-slot='calendar-nav-next']").Count.ShouldBe(1);
    }
}
