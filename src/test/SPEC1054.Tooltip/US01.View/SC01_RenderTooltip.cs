using Bunit;
using TooltipSut = LowlandTech.Components.Tooltip.Tooltip;

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
        cut.Find("[data-slot='tooltip']").ShouldNotBeNull();
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
        cut.Find("[data-slot='tooltip']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Tooltip has Delay parameter with default value 200")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HasDelayParameter()
    {
        var cut = Ctx.Render<TooltipSut>();
        cut.Instance.Delay.ShouldBe(200);
    }

    [Fact(DisplayName = "UAC05: Tooltip has DelayHide parameter with default value 0")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_HasDelayHideParameter()
    {
        var cut = Ctx.Render<TooltipSut>();
        cut.Instance.DelayHide.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: Tooltip Delay parameter can be customized")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DelayCanBeCustomized()
    {
        var cut = Ctx.Render<TooltipSut>(p => p.Add(x => x.Delay, 500));
        cut.Instance.Delay.ShouldBe(500);
    }

    [Fact(DisplayName = "UAC07: Tooltip IsOpen starts as false")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_IsOpenStartsFalse()
    {
        var cut = Ctx.Render<TooltipSut>();
        cut.Instance.IsOpen.ShouldBeFalse();
    }
}
