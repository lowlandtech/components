using Bunit;
using SeparatorSut = LowlandTech.Components.Separator.Separator;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1041.Separator.US01.View;

[Trait(Spec.SPEC, "1041")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Separator component")]
public class SC01_RenderSeparator : ViewScenario
{
    [Fact(DisplayName = "UAC01: Separator renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SeparatorSut>();
        cut.Find("[data-slot='separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Separator renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SeparatorSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Separator renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SeparatorSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='separator']").ClassList.ShouldContain("test-class");
    }
}
