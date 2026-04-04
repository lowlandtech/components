using Bunit;
using ContextMenuSut = LowlandTech.Components.ContextMenu.ContextMenu;
using LowlandTech.Components.ContextMenu;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.US, "02")]
[UserStory("02", "ContextMenu interaction behaviors")]
public class SC02_ContextMenuModalMode : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenu Modal mode renders overlay")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ModalModeRendersOverlay()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .Add(x => x.Modal, true)
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>());

        // Act
        var trigger = cut.Find("[data-slot='context-menu-trigger']");
        trigger.ContextMenu(new MouseEventArgs { ClientX = 100, ClientY = 200 });

        // Assert
        var overlay = cut.Find("[data-slot='context-menu-overlay']");
        overlay.ShouldNotBeNull();
        overlay.GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC02: ContextMenu Modal mode blocks outside interaction via overlay")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ModalModeBlocksOutsideInteraction()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .Add(x => x.Modal, true)
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>());

        // Open the menu
        var trigger = cut.Find("[data-slot='context-menu-trigger']");
        trigger.ContextMenu(new MouseEventArgs { ClientX = 100, ClientY = 200 });

        // Act - click on overlay closes the menu
        var overlay = cut.Find("[data-slot='context-menu-overlay']");
        overlay.Click();

        // Assert - menu should be closed
        var menu = cut.Find("[data-slot='context-menu']");
        menu.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC03: ContextMenu non-Modal mode does not render overlay")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_NonModalModeDoesNotRenderOverlay()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .Add(x => x.Modal, false)
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>());

        // Act
        var trigger = cut.Find("[data-slot='context-menu-trigger']");
        trigger.ContextMenu(new MouseEventArgs { ClientX = 100, ClientY = 200 });

        // Assert
        var overlays = cut.FindAll("[data-slot='context-menu-overlay']");
        overlays.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC04: ContextMenu Modal defaults to true")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ModalDefaultsToTrue()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSut>();
        var menu = cut.Instance;

        // Assert
        menu.Modal.ShouldBeTrue();
    }
}
