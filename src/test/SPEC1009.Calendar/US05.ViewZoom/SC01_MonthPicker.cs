using Bunit;
using CalendarSut = LowlandTech.Components.Calendar;
using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US05.ViewZoom;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "01")]
[UserStory("05", "Calendar view zoom")]
public class SC01_MonthPicker : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking caption zooms out to month picker")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ClickCaptionZoomsToMonthPicker()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
        });

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.Click();

        cut.Instance.CurrentView.ShouldBe(CalendarView.Months);
        cut.FindAll("[data-slot='calendar-month-button']").Count.ShouldBe(12);
    }

    [Fact(DisplayName = "UAC02: Month picker renders abbreviated month names")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_MonthPickerShowsAbbreviatedNames()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentView, CalendarView.Months);
        });

        var buttons = cut.FindAll("[data-slot='calendar-month-button']");
        buttons.Count.ShouldBe(12);
        buttons[0].TextContent.ShouldContain("Jan");
        buttons[11].TextContent.ShouldContain("Dec");
    }

    [Fact(DisplayName = "UAC03: Selecting a month returns to day view")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectingMonthReturnsToDayView()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 15));
            p.Add(x => x.CurrentView, CalendarView.Months);
        });

        var marchButton = cut.Find("[data-month='3']");
        marchButton.Click();

        cut.Instance.CurrentView.ShouldBe(CalendarView.Days);
        cut.Instance.CurrentMonth.Month.ShouldBe(3);
        cut.Instance.CurrentMonth.Year.ShouldBe(2024);
    }

    [Fact(DisplayName = "UAC04: Current month is highlighted in month picker")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_CurrentMonthIsHighlighted()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 15));
            p.Add(x => x.CurrentView, CalendarView.Months);
        });

        var juneButton = cut.Find("[data-month='6']");
        juneButton.ClassList.ShouldContain("bg-accent");
    }

    [Fact(DisplayName = "UAC05: Month picker prev/next navigate by year")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_MonthPickerNavigatesByYear()
    {
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.AllowViewZoom, true);
            p.Add(x => x.CurrentMonth, new DateTime(2024, 6, 15));
            p.Add(x => x.CurrentView, CalendarView.Months);
        });

        var prevButton = cut.Find("[data-slot='calendar-nav-prev']");
        prevButton.Click();

        cut.Instance.CurrentMonth.Year.ShouldBe(2023);
        cut.Instance.CurrentMonth.Month.ShouldBe(6);

        var nextButton = cut.Find("[data-slot='calendar-nav-next']");
        nextButton.Click();
        nextButton.Click();

        cut.Instance.CurrentMonth.Year.ShouldBe(2025);
    }
}
