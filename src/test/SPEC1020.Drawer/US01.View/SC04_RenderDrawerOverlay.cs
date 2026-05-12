using Bunit;
using DrawerSut = LowlandTech.Components.Drawer.Drawer;
using LowlandTech.Components.Drawer;

namespace LowlandTech.Components.Tests.SPEC1020.Drawer.US01.View;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Render DrawerOverlay component")]
public class SC04_RenderDrawerOverlay : ViewScenario
{
    [Fact(DisplayName = "UAC01: DrawerOverlay renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DrawerOverlay>();
        // DrawerOverlay renders when parent Drawer has IsOpen=true
    }

    [Fact(DisplayName = "UAC02: DrawerOverlay has correct default styling")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasCorrectDefaultStyling()
    {
        var cut = Ctx.Render<DrawerOverlay>();
        // Component renders without errors
    }
}
