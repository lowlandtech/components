using Bunit;
using ToggleSut = LowlandTech.Components.Toggle.Toggle;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1052.Toggle.US01.View;

[Trait(Spec.SPEC, "1052")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Toggle component")]
public class SC01_RenderToggle : ViewScenario
{
    [Fact(DisplayName = "UAC01: Toggle renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ToggleSut>();
        cut.Find("[data-slot='toggle']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Toggle renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ToggleSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Toggle renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ToggleSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='toggle']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Toggle has base cn-toggle classes")]
    [Trait(Spec.UAC, "04")]
    [Then("the button contains the cn-toggle base class")]
    public void UAC04_HasBaseClasses()
    {
        var cut = Ctx.Render<ToggleSut>();
        cut.Find("[data-slot='toggle']").ClassList.ShouldContain("cn-toggle");
    }

    [Fact(DisplayName = "UAC05: Toggle renders default data-variant and data-size")]
    [Trait(Spec.UAC, "05")]
    [Then("data-variant='default' and data-size='default' by default")]
    public void UAC05_DefaultDataAttrs()
    {
        var cut = Ctx.Render<ToggleSut>();
        var el = cut.Find("[data-slot='toggle']");
        el.GetAttribute("data-variant").ShouldBe("default");
        el.GetAttribute("data-size").ShouldBe("default");
    }

    [Fact(DisplayName = "UAC06: Toggle aria-pressed reflects Pressed parameter")]
    [Trait(Spec.UAC, "06")]
    [Then("aria-pressed is false by default and true when Pressed=true")]
    public void UAC06_AriaPressedReflectsState()
    {
        var cut = Ctx.Render<ToggleSut>();
        cut.Find("[data-slot='toggle']").GetAttribute("aria-pressed").ShouldBe("false");

        var cut2 = Ctx.Render<ToggleSut>(p => p.Add(x => x.Pressed, true));
        cut2.Find("[data-slot='toggle']").GetAttribute("aria-pressed").ShouldBe("true");
    }
}
