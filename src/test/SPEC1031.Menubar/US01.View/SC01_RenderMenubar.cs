using Bunit;
using MenubarSut = LowlandTech.Components.Menubar.Menubar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1031.Menubar.US01.View;

[Trait(Spec.SPEC, "1031")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Menubar component")]
public class SC01_RenderMenubar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Menubar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<MenubarSut>();
        cut.Find("[data-slot='menubar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Menubar renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<MenubarSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Menubar renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<MenubarSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='menubar']").ClassList.ShouldContain("test-class");
    }
}
