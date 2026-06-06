using Bunit;
using SpinnerSut = LowlandTech.Components.Spinner;

namespace LowlandTech.Components.Tests.SPEC1047.Spinner.US01.View;

[Trait(Spec.SPEC, "1047")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Spinner component")]
public class SC01_RenderSpinner : ViewScenario
{
    [Fact(DisplayName = "UAC01: Spinner renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SpinnerSut>();
        cut.Find("[data-slot='spinner']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Spinner renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SpinnerSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Spinner renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SpinnerSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='spinner']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Spinner includes loading accessibility semantics")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains status semantics")]
    public void UAC04_IncludesAccessibilitySemantics()
    {
        var cut = Ctx.Render<SpinnerSut>();
        var element = cut.Find("[data-slot='spinner']");

        element.GetAttribute("role").ShouldBe("status");
        element.GetAttribute("aria-label").ShouldBe("Loading");
    }

    [Fact(DisplayName = "UAC05: Spinner includes sample default classes and glyph")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains spinner styling and svg glyph")]
    public void UAC05_IncludesDefaultClassesAndGlyph()
    {
        var cut = Ctx.Render<SpinnerSut>();
        var element = cut.Find("[data-slot='spinner']");

        element.ClassList.ShouldContain("size-4");
        element.ClassList.ShouldContain("animate-spin");
        cut.Find("[data-slot='spinner'] svg").ShouldNotBeNull();
    }
}
