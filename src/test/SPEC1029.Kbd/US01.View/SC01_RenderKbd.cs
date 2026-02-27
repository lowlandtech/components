using Bunit;
using KbdSut = LowlandTech.Components.Kbd.Kbd;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1029.Kbd.US01.View;

[Trait(Spec.SPEC, "1029")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Kbd component")]
public class SC01_RenderKbd : ViewScenario
{
    [Fact(DisplayName = "UAC01: Kbd renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<KbdSut>();
        cut.Find("[data-slot='kbd']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Kbd renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<KbdSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Kbd renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<KbdSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='kbd']").ClassList.ShouldContain("test-class");
    }
}
