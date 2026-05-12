using Bunit;
using BadgeSut = LowlandTech.Components.Badge.Badge;

namespace LowlandTech.Components.Tests.SPEC1005.Badge.US01.View;

[Trait(Spec.SPEC, "1005")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Badge component")]
public class SC01_RenderBadge : ViewScenario
{
    [Fact(DisplayName = "UAC01: Badge renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<BadgeSut>();
        cut.Find("[data-slot='badge']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Badge renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<BadgeSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Badge renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<BadgeSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='badge']").ClassList.ShouldContain("test-class");
    }
}
