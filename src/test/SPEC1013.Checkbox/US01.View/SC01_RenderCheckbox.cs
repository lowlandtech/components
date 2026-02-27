using Bunit;
using CheckboxSut = LowlandTech.Components.Checkbox.Checkbox;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1013.Checkbox.US01.View;

[Trait(Spec.SPEC, "1013")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Checkbox component")]
public class SC01_RenderCheckbox : ViewScenario
{
    [Fact(DisplayName = "UAC01: Checkbox renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CheckboxSut>();
        cut.Find("[data-slot='checkbox']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Checkbox renders without error")]
    [Trait(Spec.UAC, "02")]
    [Then("the component renders without throwing")]
    public void UAC02_RendersWithoutError()
    {
        var cut = Ctx.Render<CheckboxSut>();
        cut.Markup.ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "UAC03: Checkbox renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CheckboxSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='checkbox']").ClassList.ShouldContain("test-class");
    }
}
