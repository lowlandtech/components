using Bunit;
using LowlandTech.Components.Drawer;
using LowlandTech.Components.Tests.Infrastructure;
using DrawerComponent = LowlandTech.Components.Drawer.Drawer;

namespace LowlandTech.Components.Tests.SPEC1020.Drawer.US02.Behavior;

[Trait(Spec.SPEC, "1020")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Drawer data-state reflects open/closed state")]
public class SC01_DrawerDataState : ViewScenario
{
    [Fact(DisplayName = "UAC01: Drawer root has data-state=open when IsOpen is true")]
    [Trait(Spec.UAC, "01")]
    [Then("the root element has data-state='open'")]
    public void UAC01_DrawerRootHasDataStateOpen()
    {
        var cut = Ctx.Render<DrawerComponent>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='drawer']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC02: Drawer root has data-state=closed when IsOpen is false")]
    [Trait(Spec.UAC, "02")]
    [Then("the root element has data-state='closed'")]
    public void UAC02_DrawerRootHasDataStateClosed()
    {
        var cut = Ctx.Render<DrawerComponent>(p => p.Add(x => x.IsOpen, false));
        cut.Find("[data-slot='drawer']").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC03: DrawerContent has data-state=open when parent IsOpen is true")]
    [Trait(Spec.UAC, "03")]
    [Then("DrawerContent has data-state='open'")]
    public void UAC03_DrawerContentHasDataStateOpen()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });
        cut.Find("[data-slot='drawer-content']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC04: DrawerContent has data-state=closed when parent IsOpen is false")]
    [Trait(Spec.UAC, "04")]
    [Then("DrawerContent has data-state='closed'")]
    public void UAC04_DrawerContentHasDataStateClosed()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.AddChildContent<DrawerContent>();
        });
        cut.Find("[data-slot='drawer-content']").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC05: DrawerOverlay has data-state=open when parent IsOpen is true")]
    [Trait(Spec.UAC, "05")]
    [Then("DrawerOverlay has data-state='open'")]
    public void UAC05_DrawerOverlayHasDataStateOpen()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerOverlay>();
        });
        cut.Find("[data-slot='drawer-overlay']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC06: DrawerOverlay has data-state=closed when parent IsOpen is false")]
    [Trait(Spec.UAC, "06")]
    [Then("DrawerOverlay has data-state='closed'")]
    public void UAC06_DrawerOverlayHasDataStateClosed()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.AddChildContent<DrawerOverlay>();
        });
        cut.Find("[data-slot='drawer-overlay']").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC07: data-state updates to open after Drawer.Open() is called")]
    [Trait(Spec.UAC, "07")]
    [Then("data-state changes to 'open'")]
    public async Task UAC07_DataStateUpdatesToOpenAfterOpen()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.AddChildContent<DrawerContent>();
        });

        cut.Find("[data-slot='drawer-content']").GetAttribute("data-state").ShouldBe("closed");

        await cut.InvokeAsync(() => cut.Instance.Open());

        cut.Find("[data-slot='drawer-content']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC08: data-state updates to closed after Drawer.Close() is called")]
    [Trait(Spec.UAC, "08")]
    [Then("data-state changes to 'closed'")]
    public async Task UAC08_DataStateUpdatesToClosedAfterClose()
    {
        var cut = Ctx.Render<DrawerComponent>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DrawerContent>();
        });

        cut.Find("[data-slot='drawer-content']").GetAttribute("data-state").ShouldBe("open");

        await cut.InvokeAsync(() => cut.Instance.Close());

        cut.Find("[data-slot='drawer-content']").GetAttribute("data-state").ShouldBe("closed");
    }
}
