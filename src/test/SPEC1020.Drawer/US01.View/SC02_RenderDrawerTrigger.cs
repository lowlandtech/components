using Bunit;
using LowlandTech.Components.Drawer;

namespace LowlandTech.Components.Tests.SPEC1020.Drawer.US01.View;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render DrawerTrigger component")]
public class SC02_RenderDrawerTrigger : ViewScenario
{
    [Fact(DisplayName = "UAC01: DrawerTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DrawerTrigger>();
        cut.Find("[data-slot='drawer-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: DrawerTrigger renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<DrawerTrigger>(p => p.AddChildContent("Open"));
        cut.Markup.ShouldContain("Open");
    }
}
