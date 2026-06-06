using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.US, "03")]
[UserStory("03", "ContextMenu sub-menu functionality")]
public class SC03_ContextMenuSubInteraction : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenuSubTrigger opens on hover")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_OpensOnHover()
    {
        // Arrange - start with closed sub-menu
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, false)
            .Add(x => x.OpenDelay, 0) // No delay for testing
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Sub Menu"))
            .AddChildContent<ContextMenuSubContent>(p => p
                .AddChildContent("Sub Content")));

        // Verify initial state - content should not be visible
        var contentsBefore = cut.FindAll("[data-slot='context-menu-sub-content']");
        contentsBefore.Count.ShouldBe(0);

        // Act - hover on trigger (using MouseEnter as that's what the component uses)
        var trigger = cut.Find("[data-slot='context-menu-sub-trigger']");
        trigger.MouseEnter();

        // Wait for the delay to complete
        cut.WaitForState(() => cut.FindAll("[data-slot='context-menu-sub-content']").Count > 0, TimeSpan.FromSeconds(1));

        // Assert - content should now be visible
        var contentsAfter = cut.FindAll("[data-slot='context-menu-sub-content']");
        contentsAfter.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC02: ContextMenuSub has configurable OpenDelay")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasConfigurableOpenDelay()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.OpenDelay, 250));

        var subMenu = cut.Instance;

        // Assert
        subMenu.OpenDelay.ShouldBe(250);
    }

    [Fact(DisplayName = "UAC03: ContextMenuSub default OpenDelay is 150ms")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DefaultOpenDelayIs150ms()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>();
        var subMenu = cut.Instance;

        // Assert
        subMenu.OpenDelay.ShouldBe(150);
    }

    [Fact(DisplayName = "UAC04: ContextMenuSubTrigger and Content have data-slot attributes")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DataSlotAttributes()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent<ContextMenuSubTrigger>(p => p
                .AddChildContent("Trigger"))
            .AddChildContent<ContextMenuSubContent>(p => p
                .AddChildContent("Content")));

        // Assert
        cut.Find("[data-slot='context-menu-sub-trigger']").ShouldNotBeNull();
        cut.Find("[data-slot='context-menu-sub-content']").ShouldNotBeNull();
    }
}
