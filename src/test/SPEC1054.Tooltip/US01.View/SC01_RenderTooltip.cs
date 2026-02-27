using Bunit;
using TooltipSut = LowlandTech.Components.Tooltip.Tooltip;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1054.Tooltip.US01.View;

[Trait(Spec.SPEC, "1054")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Tooltip component")]
public class SC01_RenderTooltip : ViewScenario
{
    [Fact(DisplayName = "UAC01: Tooltip renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TooltipSut>();
        cut.Find("[data-slot='tooltip-provider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Tooltip renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<TooltipSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Tooltip renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<TooltipSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='tooltip-provider']").ClassList.ShouldContain("test-class");
    }
}
