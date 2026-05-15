using Bunit;
using DrawerComponent = LowlandTech.Components.Drawer.Drawer;
using LowlandTech.Components.Drawer;

namespace LowlandTech.Components.Tests.SPEC1020.Drawer.US02.Behavior;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "02")]
[UserStory("02", "DrawerContent has direction-aware animation classes")]
public class SC02_DrawerAnimationClasses : ViewScenario
{
    private IRenderedComponent<DrawerComponent> RenderWithDirection(string direction) =>
        Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.Direction, direction);
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

    [Fact(DisplayName = "UAC01: Right drawer content has slide-in-from-right class")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RightDrawerHasSlideInFromRight()
    {
        var cut = RenderWithDirection("right");
        var content = cut.Find("[data-slot='drawer-content']");
        content.ClassList.ShouldContain("slide-in-from-right");
    }

    [Fact(DisplayName = "UAC02: Left drawer content has slide-in-from-left class")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_LeftDrawerHasSlideInFromLeft()
    {
        var cut = RenderWithDirection("left");
        var content = cut.Find("[data-slot='drawer-content']");
        content.ClassList.ShouldContain("slide-in-from-left");
    }

    [Fact(DisplayName = "UAC03: Top drawer content has slide-in-from-top class")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_TopDrawerHasSlideInFromTop()
    {
        var cut = RenderWithDirection("top");
        var content = cut.Find("[data-slot='drawer-content']");
        content.ClassList.ShouldContain("slide-in-from-top");
    }

    [Fact(DisplayName = "UAC04: Bottom drawer content has slide-in-from-bottom class")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_BottomDrawerHasSlideInFromBottom()
    {
        var cut = RenderWithDirection("bottom");
        var content = cut.Find("[data-slot='drawer-content']");
        content.ClassList.ShouldContain("slide-in-from-bottom");
    }

    [Fact(DisplayName = "UAC05: DrawerOverlay has fade-in-0 animation class")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_OverlayHasFadeInClass()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerOverlay>();
        });
        var overlay = cut.Find("[data-slot='drawer-overlay']");
        overlay.ClassList.ShouldContain("fade-in-0");
        overlay.ClassList.ShouldContain("duration-200");
    }

    [Fact(DisplayName = "UAC06: All directions include animate-in and animate-out base classes")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_AllDirectionsHaveBaseAnimateClasses()
    {
        foreach (var direction in new[] { "left", "right", "top", "bottom" })
        {
            var cut = RenderWithDirection(direction);
            var content = cut.Find("[data-slot='drawer-content']");
            content.ClassList.ShouldContain("animate-in");
            content.ClassList.ShouldContain("duration-300");
        }
    }
}
