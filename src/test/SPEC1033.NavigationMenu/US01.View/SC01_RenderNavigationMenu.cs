using Bunit;
using NavigationMenuSut = LowlandTech.Components.NavigationMenu.NavigationMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1033.NavigationMenu.US01.View;

[Trait(Spec.SPEC, "1033")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render NavigationMenu component")]
public class SC01_RenderNavigationMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: NavigationMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuSut>();
        cut.Find("[data-slot='navigation-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: NavigationMenu renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<NavigationMenuSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: NavigationMenu renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<NavigationMenuSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='navigation-menu']").ClassList.ShouldContain("test-class");
    }
}
