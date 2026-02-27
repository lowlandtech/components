using Bunit;
using ContextMenuSut = LowlandTech.Components.ContextMenu.ContextMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ContextMenu component")]
public class SC01_RenderContextMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuSut>();
        cut.Find("[data-slot='context-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ContextMenu renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ContextMenuSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: ContextMenu renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ContextMenuSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='context-menu']").ClassList.ShouldContain("test-class");
    }
}
