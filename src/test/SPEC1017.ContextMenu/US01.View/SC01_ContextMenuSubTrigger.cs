using Bunit;
using LowlandTech.Components.ContextMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.US, "03")]
[UserStory("03", "ContextMenu sub-menu functionality")]
public class SC01_ContextMenuSubTrigger : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenuSubTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Sub Menu")));

        // Assert
        var trigger = cut.Find("[data-slot='context-menu-sub-trigger']");
        trigger.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ContextMenuSubTrigger has arrow indicator")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasArrowIndicator()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Sub Menu")));

        // Assert
        var icon = cut.Find("[data-slot='icon']");
        icon.ShouldNotBeNull();
        // The icon should be a chevron-right based on the chevron-right icon name
    }

    [Fact(DisplayName = "UAC03: ContextMenuSubTrigger aria-expanded reflects closed state")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_AriaExpandedReflectsClosedState()
    {
        // Arrange & Act - sub-menu starts closed
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, false)
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Sub Menu")));

        // Assert
        var trigger = cut.Find("[data-slot='context-menu-sub-trigger']");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC04: ContextMenuSubTrigger aria-expanded reflects open state")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_AriaExpandedReflectsOpenState()
    {
        // Arrange & Act - sub-menu starts open
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Sub Menu")));

        // Assert
        var trigger = cut.Find("[data-slot='context-menu-sub-trigger']");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        trigger.GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC05: ContextMenuSubTrigger renders ChildContent")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersChildContent()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Sub Menu Label")));

        // Assert
        cut.Markup.ShouldContain("Sub Menu Label");
    }
}
