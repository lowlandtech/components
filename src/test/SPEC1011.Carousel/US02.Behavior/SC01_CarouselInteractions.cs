using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1011.Carousel.US02.Behavior;

[Trait(Spec.SPEC, "1011")]
[Trait(Spec.US, "02")]
[UserStory("02", "Carousel component behavior")]
public class SC01_CarouselInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: CarouselPrevious triggers OnClick when clicked")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CarouselPreviousTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<CarouselPrevious>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var btn = cut.Find("[data-slot='carousel-previous']");
        await btn.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: CarouselNext triggers OnClick when clicked")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CarouselNextTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<CarouselNext>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var btn = cut.Find("[data-slot='carousel-next']");
        await btn.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Disabled CarouselPrevious does not trigger OnClick")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DisabledCarouselPreviousDoesNotTriggerOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<CarouselPrevious>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var btn = cut.Find("[data-slot='carousel-previous']");
        await btn.ClickAsync();

        clicked.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: Disabled CarouselNext does not trigger OnClick")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_DisabledCarouselNextDoesNotTriggerOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<CarouselNext>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var btn = cut.Find("[data-slot='carousel-next']");
        await btn.ClickAsync();

        clicked.ShouldBeFalse();
    }
}
