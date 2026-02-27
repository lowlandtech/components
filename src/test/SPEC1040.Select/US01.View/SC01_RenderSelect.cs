using Bunit;
using SelectSut = LowlandTech.Components.Select.Select;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1040.Select.US01.View;

[Trait(Spec.SPEC, "1040")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Select component")]
public class SC01_RenderSelect : ViewScenario
{
    [Fact(DisplayName = "UAC01: Select renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SelectSut>();
        cut.Find("[data-slot='select']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Select renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SelectSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Select renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SelectSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='select']").ClassList.ShouldContain("test-class");
    }
}
