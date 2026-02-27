using Bunit;
using CarouselSut = LowlandTech.Components.Carousel.Carousel;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1011.Carousel.US01.View;

[Trait(Spec.SPEC, "1011")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Carousel component")]
public class SC01_RenderCarousel : ViewScenario
{
    [Fact(DisplayName = "UAC01: Carousel renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CarouselSut>();
        cut.Find("[data-slot='carousel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Carousel renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CarouselSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Carousel renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CarouselSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='carousel']").ClassList.ShouldContain("test-class");
    }
}
