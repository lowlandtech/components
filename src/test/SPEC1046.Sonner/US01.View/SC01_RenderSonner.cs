using Bunit;
using SonnerSut = LowlandTech.Components.Sonner.Sonner;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1046.Sonner.US01.View;

[Trait(Spec.SPEC, "1046")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Sonner component")]
public class SC01_RenderSonner : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sonner renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SonnerSut>();
        cut.Find("[data-slot='sonner-toaster']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Sonner renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SonnerSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Sonner renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SonnerSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='sonner-toaster']").ClassList.ShouldContain("test-class");
    }
}
