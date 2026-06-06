using Bunit;
using DrawerSut = LowlandTech.Components.Drawer;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1020.Drawer.US01.View;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Drawer renders with different directions")]
public class SC05_DrawerDirections : ViewScenario
{
    [Fact(DisplayName = "UAC01: Drawer renders from left direction")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersFromLeftDirection()
    {
        var cut = Ctx.Render<DrawerSut>(p =>
        {
            p.Add(x => x.Direction, "left");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        var content = cut.Find("[data-slot='drawer-content']");
        content.GetAttribute("data-direction").ShouldBe("left");
        content.ClassList.ShouldContain("fixed");
        content.ClassList.ShouldContain("left-0");
        content.ClassList.ShouldContain("h-full");
    }

    [Fact(DisplayName = "UAC02: Drawer renders from right direction")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersFromRightDirection()
    {
        var cut = Ctx.Render<DrawerSut>(p =>
        {
            p.Add(x => x.Direction, "right");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        var content = cut.Find("[data-slot='drawer-content']");
        content.GetAttribute("data-direction").ShouldBe("right");
        content.ClassList.ShouldContain("fixed");
        content.ClassList.ShouldContain("right-0");
        content.ClassList.ShouldContain("h-full");
    }

    [Fact(DisplayName = "UAC03: Drawer renders from top direction")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersFromTopDirection()
    {
        var cut = Ctx.Render<DrawerSut>(p =>
        {
            p.Add(x => x.Direction, "top");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        var content = cut.Find("[data-slot='drawer-content']");
        content.GetAttribute("data-direction").ShouldBe("top");
        content.ClassList.ShouldContain("fixed");
        content.ClassList.ShouldContain("top-0");
        content.ClassList.ShouldContain("w-full");
    }

    [Fact(DisplayName = "UAC04: Drawer renders from bottom direction")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersFromBottomDirection()
    {
        var cut = Ctx.Render<DrawerSut>(p =>
        {
            p.Add(x => x.Direction, "bottom");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        var content = cut.Find("[data-slot='drawer-content']");
        content.GetAttribute("data-direction").ShouldBe("bottom");
        content.ClassList.ShouldContain("fixed");
        content.ClassList.ShouldContain("bottom-0");
        content.ClassList.ShouldContain("w-full");
    }

    [Fact(DisplayName = "UAC05: Drawer has data-direction attribute present")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_HasDataDirectionAttribute()
    {
        var cut = Ctx.Render<DrawerSut>(p =>
        {
            p.Add(x => x.Direction, "right");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        var content = cut.Find("[data-slot='drawer-content']");
        content.GetAttribute("data-direction").ShouldNotBeNull();
        content.GetAttribute("data-direction").ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "UAC06: Drawer overlay appears on all directions")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_OverlayAppearsOnAllDirections()
    {
        var directions = new[] { "left", "right", "top", "bottom" };

        foreach (var direction in directions)
        {
            var cut = Ctx.Render<DrawerSut>(p =>
            {
                p.Add(x => x.Direction, direction);
                p.Add(x => x.IsOpen, true);
                p.AddChildContent<DrawerOverlay>();
            });

            var overlay = cut.Find("[data-slot='drawer-overlay']");
            overlay.ShouldNotBeNull();
            overlay.ClassList.ShouldContain("fixed");
            overlay.ClassList.ShouldContain("inset-0");
            overlay.ClassList.ShouldContain("z-50");
        }
    }

    [Fact(DisplayName = "UAC07: Drawer defaults to right direction")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_DefaultsToRightDirection()
    {
        var cut = Ctx.Render<DrawerSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        var content = cut.Find("[data-slot='drawer-content']");
        content.GetAttribute("data-direction").ShouldBe("right");
        content.ClassList.ShouldContain("right-0");
    }
}
