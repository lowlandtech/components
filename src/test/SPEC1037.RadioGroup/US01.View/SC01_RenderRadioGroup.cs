using Bunit;
using RadioGroupSut = LowlandTech.Components.RadioGroup.RadioGroup;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1037.RadioGroup.US01.View;

[Trait(Spec.SPEC, "1037")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render RadioGroup component")]
public class SC01_RenderRadioGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: RadioGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<RadioGroupSut>();
        cut.Find("[data-slot='radio-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: RadioGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<RadioGroupSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: RadioGroup renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<RadioGroupSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='radio-group']").ClassList.ShouldContain("test-class");
    }
}
