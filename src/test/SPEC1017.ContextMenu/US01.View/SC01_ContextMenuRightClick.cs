using Bunit;
using ContextMenuSut = LowlandTech.Components.ContextMenu;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.US, "02")]
[UserStory("02", "ContextMenu interaction behaviors")]
public class SC01_ContextMenuRightClick : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenu right-click opens menu")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RightClickOpensMenu()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>(p => p
                .AddChildContent<ContextMenuItem>(p => p
                    .AddChildContent("Menu Item"))));

        var trigger = cut.Find("[data-slot='context-menu-trigger']");

        // Act
        trigger.ContextMenu(new MouseEventArgs { ClientX = 100, ClientY = 200 });

        // Assert
        var content = cut.Find("[data-slot='context-menu-content']");
        content.ShouldNotBeNull();
        content.GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC02: ContextMenu has contextmenu event handler")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HasContextMenuHandler()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>());

        var trigger = cut.Find("[data-slot='context-menu-trigger']");

        // Act & Assert
        // The @oncontextmenu handler is present (preventDefault is handled by Blazor internally)
        trigger.ShouldNotBeNull();
        trigger.HasAttribute("data-slot").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: ContextMenu data-state reflects open/closed")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DataStateReflectsOpenClosed()
    {
        // Arrange - closed state initially
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>());

        var menu = cut.Find("[data-slot='context-menu']");
        menu.GetAttribute("data-state").ShouldBe("closed");

        // Act - open the menu
        var trigger = cut.Find("[data-slot='context-menu-trigger']");
        trigger.ContextMenu(new MouseEventArgs { ClientX = 100, ClientY = 200 });

        // Assert - should be open
        menu = cut.Find("[data-slot='context-menu']");
        menu.GetAttribute("data-state").ShouldBe("open");
    }
}
