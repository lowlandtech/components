using Bunit;
using CarouselComponent = LowlandTech.Components.Carousel;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1011.Carousel.US01.View;

[Trait(Spec.SPEC, "1011")]
[Trait(Spec.US, "01")]
[UserStory("01", "Carousel component rendering")]
public class SC01_RenderCarousel : ViewScenario
{
    [Fact(DisplayName = "UAC01: Carousel renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CarouselRendersWithDataSlot()
    {
        var cut = Ctx.Render<CarouselComponent>();
        cut.Find("[data-slot='carousel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Carousel renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CarouselRendersChildContent()
    {
        var cut = Ctx.Render<CarouselComponent>(p => p.AddChildContent("<span class='test'>Content</span>"));
        cut.Find(".test").TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "UAC03: CarouselContent renders with data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CarouselContentRendersWithDataSlot()
    {
        var cut = Ctx.Render<CarouselContent>();
        cut.Find("[data-slot='carousel-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: CarouselItem renders with data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_CarouselItemRendersWithDataSlot()
    {
        var cut = Ctx.Render<CarouselItem>();
        cut.Find("[data-slot='carousel-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: CarouselItem renders ChildContent")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_CarouselItemRendersChildContent()
    {
        var cut = Ctx.Render<CarouselItem>(p => p.AddChildContent("Slide"));
        cut.Find("[data-slot='carousel-item']").TextContent.ShouldBe("Slide");
    }

    [Fact(DisplayName = "UAC06: CarouselPrevious renders with data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_CarouselPreviousRendersWithDataSlot()
    {
        var cut = Ctx.Render<CarouselPrevious>();
        var btn = cut.Find("[data-slot='carousel-previous']");
        btn.ShouldNotBeNull();
        btn.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC07: CarouselPrevious has sr-only text")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CarouselPreviousHasSrOnlyText()
    {
        var cut = Ctx.Render<CarouselPrevious>();
        cut.Find(".sr-only").TextContent.ShouldBe("Previous slide");
    }

    [Fact(DisplayName = "UAC08: CarouselNext renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_CarouselNextRendersWithDataSlot()
    {
        var cut = Ctx.Render<CarouselNext>();
        var btn = cut.Find("[data-slot='carousel-next']");
        btn.ShouldNotBeNull();
        btn.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC09: CarouselNext has sr-only text")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_CarouselNextHasSrOnlyText()
    {
        var cut = Ctx.Render<CarouselNext>();
        cut.Find(".sr-only").TextContent.ShouldBe("Next slide");
    }

    [Fact(DisplayName = "UAC10: CarouselPrevious respects Disabled parameter")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_CarouselPreviousRespectsDisabled()
    {
        var cut = Ctx.Render<CarouselPrevious>(p => p.Add(x => x.Disabled, true));
        cut.Find("[data-slot='carousel-previous']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC11: CarouselNext respects Disabled parameter")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_CarouselNextRespectsDisabled()
    {
        var cut = Ctx.Render<CarouselNext>(p => p.Add(x => x.Disabled, true));
        cut.Find("[data-slot='carousel-next']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC12: Full Carousel SUT renders correctly")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_FullCarouselSutRenders()
    {
        var cut = Ctx.Render<CarouselSut>();
        cut.Find("[data-slot='carousel']").ShouldNotBeNull();
        cut.Find("[data-slot='carousel-content']").ShouldNotBeNull();
        cut.FindAll("[data-slot='carousel-item']").Count.ShouldBe(3);
        cut.Find("[data-slot='carousel-previous']").ShouldNotBeNull();
        cut.Find("[data-slot='carousel-next']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: Carousel renders horizontal orientation")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_CarouselRendersHorizontalOrientation()
    {
        var cut = Ctx.Render<CarouselComponent>(p => p.Add(x => x.Orientation, "horizontal"));
        cut.Find("[data-slot='carousel']").GetAttribute("data-orientation").ShouldBe("horizontal");
    }

    [Fact(DisplayName = "UAC14: Carousel renders vertical orientation")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_CarouselRendersVerticalOrientation()
    {
        var cut = Ctx.Render<CarouselComponent>(p => p.Add(x => x.Orientation, "vertical"));
        cut.Find("[data-slot='carousel']").GetAttribute("data-orientation").ShouldBe("vertical");
    }

    [Fact(DisplayName = "UAC15: CarouselItem renders data-active when active")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_CarouselItemRendersDataActiveWhenActive()
    {
        var cut = Ctx.Render<CarouselItem>(p => p.Add(x => x.IsActive, true));
        cut.Find("[data-slot='carousel-item']").GetAttribute("data-active").ShouldBe("true");
    }
}
