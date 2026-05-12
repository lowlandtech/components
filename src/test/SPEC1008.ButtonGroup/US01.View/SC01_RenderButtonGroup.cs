using Bunit;
using ButtonGroupSut = LowlandTech.Components.ButtonGroup.ButtonGroup;

namespace LowlandTech.Components.Tests.SPEC1008.ButtonGroup.US01.View;

[Trait(Spec.SPEC, "1008")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ButtonGroup component")]
public class SC01_RenderButtonGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: ButtonGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ButtonGroupSut>();
        cut.Find("[data-slot='button-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ButtonGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ButtonGroupSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: ButtonGroup renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ButtonGroupSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='button-group']").ClassList.ShouldContain("test-class");
    }
}
