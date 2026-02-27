using Bunit;
using ToggleGroupSut = LowlandTech.Components.ToggleGroup.ToggleGroup;
using LowlandTech.Components.Tests.Infrastructure;

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
}
