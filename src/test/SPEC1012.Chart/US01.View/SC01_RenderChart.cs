using Bunit;
using ChartSut = LowlandTech.Components.Chart.Chart;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1012.Chart.US01.View;

[Trait(Spec.SPEC, "1012")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Chart component")]
public class SC01_RenderChart : ViewScenario
{
    [Fact(DisplayName = "UAC01: Chart renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ChartSut>();
        cut.Find("[data-slot='chart']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Chart renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ChartSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Chart renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ChartSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='chart']").ClassList.ShouldContain("test-class");
    }
}
