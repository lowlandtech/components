using Bunit;
using SeparatorSut = LowlandTech.Components.Separator.Separator;

namespace LowlandTech.Components.Tests.SPEC1041.Separator.US01.View;

[Trait(Spec.SPEC, "1041")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Separator component")]
public class SC01_RenderSeparator : ViewScenario
{
    [Fact(DisplayName = "UAC01: Separator renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SeparatorSut>();
        cut.Find("[data-slot='separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Separator renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SeparatorSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Separator renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SeparatorSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='separator']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Separator defaults to horizontal orientation")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains horizontal separator semantics")]
    public void UAC04_DefaultsToHorizontal()
    {
        var cut = Ctx.Render<SeparatorSut>();
        var element = cut.Find("[data-slot='separator']");

        element.GetAttribute("role").ShouldBe("separator");
        element.GetAttribute("data-orientation").ShouldBe("horizontal");
        element.ClassList.ShouldContain("h-px");
        element.ClassList.ShouldContain("w-full");
    }

    [Fact(DisplayName = "UAC05: Separator renders vertical orientation")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains vertical separator semantics")]
    public void UAC05_RendersVerticalOrientation()
    {
        var cut = Ctx.Render<SeparatorSut>(p => p.Add(x => x.orientation, "vertical"));
        var element = cut.Find("[data-slot='separator']");

        element.GetAttribute("aria-orientation").ShouldBe("vertical");
        element.ClassList.ShouldContain("w-px");
        element.ClassList.ShouldContain("self-stretch");
    }
}
