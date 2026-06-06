using Bunit;
using ContextMenuSut = LowlandTech.Components.ContextMenu;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render ContextMenu subcomponents")]
public class SC02_RenderContextMenuSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenuTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TriggerRendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuTrigger>(p => p.AddChildContent("Open"));
        cut.Find("[data-slot='context-menu-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ContextMenuContent renders data-slot attribute when open")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ContentRendersDataSlot()
    {
        // ContextMenuContent only renders when parent IsOpen is true
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent<ContextMenuContent>());
        cut.Find("[data-slot='context-menu-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: ContextMenuItem renders data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ItemRendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuItem>(p => p.AddChildContent("Item"));
        cut.Find("[data-slot='context-menu-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: ContextMenuGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_GroupRendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuGroup>();
        cut.Find("[data-slot='context-menu-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: ContextMenuSeparator renders data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SeparatorRendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuSeparator>();
        cut.Find("[data-slot='context-menu-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: ContextMenuLabel renders data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_LabelRendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuLabel>(p => p.AddChildContent("Label"));
        cut.Find("[data-slot='context-menu-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: ContextMenuShortcut renders data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ShortcutRendersDataSlot()
    {
        var cut = Ctx.Render<ContextMenuShortcut>(p => p.AddChildContent("Ctrl+K"));
        cut.Find("[data-slot='context-menu-shortcut']").ShouldNotBeNull();
    }
}
