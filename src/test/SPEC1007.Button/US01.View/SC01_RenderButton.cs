using Bunit;
using ButtonSut = LowlandTech.Components.Button.Button;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1007.Button.US01.View;

[Trait(Spec.SPEC, "1007")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Button component")]
public class SC01_RenderButton : ViewScenario
{
    [Fact(DisplayName = "UAC01: Button renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ButtonSut>();
        cut.Find("[data-slot='button']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Button renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Button renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ButtonSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='button']").ClassList.ShouldContain("test-class");
    }
}
