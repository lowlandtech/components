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
}
