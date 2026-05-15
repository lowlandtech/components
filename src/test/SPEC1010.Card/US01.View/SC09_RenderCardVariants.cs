using Bunit;
using CardSut = LowlandTech.Components.Card.Card;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "09")]
[UserStory("01", "Render Card component variants")]
public class SC09_RenderCardVariants : ViewScenario
{
    [Fact(DisplayName = "UAC01: Card renders data-variant attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-variant attribute")]
    public void UAC01_RendersDataVariantAttribute()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Variant, "outlined"));
        cut.Find("[data-slot='card']").GetAttribute("data-variant").ShouldBe("outlined");
    }

    [Fact(DisplayName = "UAC02: Card renders data-elevation attribute")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains data-elevation attribute")]
    public void UAC02_RendersDataElevationAttribute()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Elevation, 5));
        cut.Find("[data-slot='card']").GetAttribute("data-elevation").ShouldBe("5");
    }

    [Fact(DisplayName = "UAC03: Card renders data-square attribute when Square is true")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains data-square attribute")]
    public void UAC03_RendersDataSquareAttribute()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Square, true));
        cut.Find("[data-slot='card']").GetAttribute("data-square").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC04: Card does not render data-square attribute when Square is false")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup does not contain data-square attribute")]
    public void UAC04_DoesNotRenderDataSquareWhenFalse()
    {
        var cut = Ctx.Render<CardSut>();
        cut.Find("[data-slot='card']").GetAttribute("data-square").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC05: Default variant applies shadow-sm class")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains shadow-sm class")]
    public void UAC05_DefaultVariantAppliesShadow()
    {
        var cut = Ctx.Render<CardSut>();
        cut.Find("[data-slot='card']").ClassList.ShouldContain("shadow-sm");
    }

    [Fact(DisplayName = "UAC06: Outlined variant suppresses shadow")]
    [Trait(Spec.UAC, "06")]
    [Then("the rendered markup does not contain shadow classes")]
    public void UAC06_OutlinedVariantSuppressesShadow()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Variant, "outlined"));
        var classList = cut.Find("[data-slot='card']").ClassList;
        classList.ShouldNotContain("shadow-sm");
        classList.ShouldNotContain("shadow");
        classList.ShouldNotContain("shadow-md");
    }

    [Fact(DisplayName = "UAC07: Elevation 0 applies shadow-none class")]
    [Trait(Spec.UAC, "07")]
    [Then("the rendered markup contains shadow-none class")]
    public void UAC07_ElevationZeroAppliesShadowNone()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Elevation, 0));
        cut.Find("[data-slot='card']").ClassList.ShouldContain("shadow-none");
    }

    [Fact(DisplayName = "UAC08: High elevation applies shadow-2xl class")]
    [Trait(Spec.UAC, "08")]
    [Then("the rendered markup contains shadow-2xl class")]
    public void UAC08_HighElevationAppliesShadow2Xl()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Elevation, 25));
        cut.Find("[data-slot='card']").ClassList.ShouldContain("shadow-2xl");
    }
}
