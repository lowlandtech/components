using Bunit;
using SliderSut = LowlandTech.Components.Slider.Slider;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1045.Slider.US01.View;

[Trait(Spec.SPEC, "1045")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Slider component")]
public class SC01_RenderSlider : ViewScenario
{
    [Fact(DisplayName = "UAC01: Slider renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SliderSut>();
        cut.Find("[data-slot='slider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Slider renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SliderSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Slider renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SliderSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='slider']").ClassList.ShouldContain("test-class");
    }
}
