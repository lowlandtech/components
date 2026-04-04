using Bunit;
using CalendarSut = LowlandTech.Components.Calendar.Calendar;
using LowlandTech.Components.Calendar;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US03.Navigation;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "02")]
[UserStory("03", "Calendar navigation")]
public class SC02_YearNavigation : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar navigation previous year works")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PreviousYearWorks()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var prevYearButton = cut.Find("[data-slot='calendar-nav-prev-year']");
        prevYearButton.Click();

        cut.Instance.CurrentMonth.Year.ShouldBe(2023);
        cut.Instance.CurrentMonth.Month.ShouldBe(6);
    }

    [Fact(DisplayName = "UAC02: Calendar navigation next year works")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NextYearWorks()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var nextYearButton = cut.Find("[data-slot='calendar-nav-next-year']");
        nextYearButton.Click();

        cut.Instance.CurrentMonth.Year.ShouldBe(2025);
        cut.Instance.CurrentMonth.Month.ShouldBe(6);
    }

    [Fact(DisplayName = "UAC03: Previous year updates caption")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PreviousYearUpdatesCaption()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var prevYearButton = cut.Find("[data-slot='calendar-nav-prev-year']");
        prevYearButton.Click();

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.TextContent.ShouldContain("June");
        caption.TextContent.ShouldContain("2023");
    }

    [Fact(DisplayName = "UAC04: Next year updates caption")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NextYearUpdatesCaption()
    {
        var initialMonth = new DateTime(2024, 6, 15);
        var cut = Ctx.Render<CalendarSut>(p =>
        {
            p.Add(x => x.CurrentMonth, initialMonth);
        });

        var nextYearButton = cut.Find("[data-slot='calendar-nav-next-year']");
        nextYearButton.Click();

        var caption = cut.Find("[data-slot='calendar-caption']");
        caption.TextContent.ShouldContain("June");
        caption.TextContent.ShouldContain("2025");
    }
}
