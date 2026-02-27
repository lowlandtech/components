using Bunit;
using InputGroupSut = LowlandTech.Components.InputGroup.InputGroup;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1026.InputGroup.US01.View;

[Trait(Spec.SPEC, "1026")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render InputGroup component")]
public class SC01_RenderInputGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputGroupSut>();
        cut.Find("[data-slot='input-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: InputGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<InputGroupSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: InputGroup renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<InputGroupSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='input-group']").ClassList.ShouldContain("test-class");
    }
}
