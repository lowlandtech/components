using Bunit;
using ToggleGroupSut = LowlandTech.Components.ToggleGroup.ToggleGroup;

namespace LowlandTech.Components.Tests.SPEC1053.ToggleGroup.US01.View;

[Trait(Spec.SPEC, "1053")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ToggleGroup component")]
public class SC01_RenderToggleGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: ToggleGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ToggleGroupSut>();
        cut.Find("[data-slot='toggle-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ToggleGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ToggleGroupSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: ToggleGroup renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ToggleGroupSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='toggle-group']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: ToggleGroup defaults to horizontal orientation")]
    [Trait(Spec.UAC, "04")]
    [Then("data-orientation='horizontal' by default")]
    public void UAC04_DefaultOrientation()
    {
        var cut = Ctx.Render<ToggleGroupSut>();
        cut.Find("[data-slot='toggle-group']").GetAttribute("data-orientation").ShouldBe("horizontal");
    }

    [Fact(DisplayName = "UAC05: ToggleGroup passes variant/size to data attributes")]
    [Trait(Spec.UAC, "05")]
    [Then("data-variant and data-size reflect the parameters")]
    public void UAC05_VariantAndSize()
    {
        var cut = Ctx.Render<ToggleGroupSut>(p => p
            .Add(x => x.variant, "outline")
            .Add(x => x.size, "sm"));
        var el = cut.Find("[data-slot='toggle-group']");
        el.GetAttribute("data-variant").ShouldBe("outline");
        el.GetAttribute("data-size").ShouldBe("sm");
    }

    [Fact(DisplayName = "UAC06: ToggleGroup has base cn-toggle-group class")]
    [Trait(Spec.UAC, "06")]
    [Then("the element contains cn-toggle-group base class")]
    public void UAC06_HasBaseClass()
    {
        var cut = Ctx.Render<ToggleGroupSut>();
        cut.Find("[data-slot='toggle-group']").ClassList.ShouldContain("cn-toggle-group");
    }
}
