using Bunit;
using AspectRatioSut = LowlandTech.Components.AspectRatio.AspectRatio;

namespace LowlandTech.Components.Tests.SPEC1003.AspectRatio.US01.View;

[Trait(Spec.SPEC, "1003")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render AspectRatio component")]
public class SC01_RenderAspectRatio : ViewScenario
{
    [Fact(DisplayName = "UAC01: AspectRatio renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AspectRatioSut>();
        cut.Find("[data-slot='aspect-ratio']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: AspectRatio renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<AspectRatioSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: AspectRatio renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<AspectRatioSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='aspect-ratio']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: AspectRatio applies sample default classes and ratio style")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains aspect ratio styling")]
    public void UAC04_AppliesRatioStyling()
    {
        var cut = Ctx.Render<AspectRatioSut>(p => p.Add(x => x.ratio, "4 / 3"));
        var element = cut.Find("[data-slot='aspect-ratio']");

        element.ClassList.ShouldContain("relative");
        element.ClassList.ShouldContain("aspect-(--ratio)");
        element.GetAttribute("style")!.ShouldContain("--ratio: 4 / 3");
    }
}
