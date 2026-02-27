using Bunit;
using SheetSut = LowlandTech.Components.Sheet.Sheet;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1042.Sheet.US01.View;

[Trait(Spec.SPEC, "1042")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Sheet component")]
public class SC01_RenderSheet : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sheet renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SheetSut>();
        cut.Find("[data-slot='sheet']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Sheet renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SheetSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Sheet renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SheetSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='sheet']").ClassList.ShouldContain("test-class");
    }
}
