using Bunit;
using InputSut = LowlandTech.Components.Input.Input;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1025.Input.US01.View;

[Trait(Spec.SPEC, "1025")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Input component")]
public class SC01_RenderInput : ViewScenario
{
    [Fact(DisplayName = "UAC01: Input renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Find("[data-slot='input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Input renders without error")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup is not empty")]
    public void UAC02_RendersWithoutError()
    {
        var cut = Ctx.Render<InputSut>();
        cut.Markup.ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "UAC03: Input renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='input']").ClassList.ShouldContain("test-class");
    }
}
