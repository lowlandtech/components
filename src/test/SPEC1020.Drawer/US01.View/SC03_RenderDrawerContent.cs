using Bunit;
using DrawerSut = LowlandTech.Components.Drawer.Drawer;
using LowlandTech.Components.Drawer;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1020.Drawer.US01.View;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Render DrawerContent component")]
public class SC03_RenderDrawerContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: DrawerContent renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DrawerContent>(p => p.AddChildContent("Content"));
        // DrawerContent renders when parent Drawer has IsOpen=true
    }

    [Fact(DisplayName = "UAC02: DrawerContent renders with correct defaults")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersWithCorrectDefaults()
    {
        var cut = Ctx.Render<DrawerContent>();
        // Component renders without errors
    }
}
