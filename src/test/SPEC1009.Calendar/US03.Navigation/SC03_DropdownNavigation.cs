using Bunit;
using CalendarSut = LowlandTech.Components.Calendar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1009.Calendar.US03.Navigation;

[Trait(Spec.SPEC, "1009")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "03")]
[UserStory("03", "Calendar dropdown navigation")]
public class SC03_DropdownNavigation : ViewScenario
{
    [Fact(DisplayName = "UAC01: Calendar renders month and year dropdowns when enabled")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDropdowns()
    {
        var cut = Ctx.Render<CalendarSut>(p => p
            .Add(x => x.ShowMonthDropdown, true)
            .Add(x => x.ShowYearDropdown, true));

        cut.Find("[data-slot='calendar-month-select'] [data-slot='select-trigger']").ShouldNotBeNull();
        cut.Find("[data-slot='calendar-year-select'] [data-slot='select-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Month dropdown changes displayed month")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_MonthDropdownChangesMonth()
    {
        var cut = Ctx.Render<CalendarSut>(p => p
            .Add(x => x.CurrentMonth, new DateTime(2024, 6, 1))
            .Add(x => x.ShowMonthDropdown, true));

        cut.Find("[data-slot='calendar-month-select'] [data-slot='select-trigger']").Click();
        cut.Find("[data-slot='calendar-month-select'] [data-slot='select-item'][data-value='2']").Click();

        cut.Instance.CurrentMonth.Month.ShouldBe(2);
        cut.Instance.CurrentMonth.Year.ShouldBe(2024);
    }

    [Fact(DisplayName = "UAC03: Year dropdown changes displayed year")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_YearDropdownChangesYear()
    {
        var cut = Ctx.Render<CalendarSut>(p => p
            .Add(x => x.CurrentMonth, new DateTime(2024, 6, 1))
            .Add(x => x.ShowYearDropdown, true));

        cut.Find("[data-slot='calendar-year-select'] [data-slot='select-trigger']").Click();
        cut.Find("[data-slot='calendar-year-select'] [data-slot='select-item'][data-value='2028']").Click();

        cut.Instance.CurrentMonth.Year.ShouldBe(2028);
        cut.Instance.CurrentMonth.Month.ShouldBe(6);
    }
}