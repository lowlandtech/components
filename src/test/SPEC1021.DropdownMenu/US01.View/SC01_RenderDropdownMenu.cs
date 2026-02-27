using Bunit;
using DropdownMenuSut = LowlandTech.Components.DropdownMenu.DropdownMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1021.DropdownMenu.US01.View;

[Trait(Spec.SPEC, "1021")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render DropdownMenu component")]
public class SC01_RenderDropdownMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: DropdownMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuSut>();
        cut.Find("[data-slot='dropdown-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: DropdownMenu renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<DropdownMenuSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: DropdownMenu renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<DropdownMenuSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='dropdown-menu']").ClassList.ShouldContain("test-class");
    }
}
