using Bunit;
using ContextMenuSut = LowlandTech.Components.ContextMenu.ContextMenu;
using LowlandTech.Components.ContextMenu;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.US, "02")]
[UserStory("02", "ContextMenu interaction behaviors")]
public class SC03_ContextMenuPortal : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenu Portal parameter exists")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PortalParameterExists()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSut>();
        var menu = cut.Instance;

        // Assert
        // Portal property should exist and have a default value
        menu.Portal.ShouldBeFalse(); // Default is false until full portal implementation
    }

    [Fact(DisplayName = "UAC02: ContextMenu renders content when Portal is enabled")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersContentWithPortalEnabled()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .Add(x => x.Portal, true)
            .AddChildContent<ContextMenuTrigger>(p => p
                .AddChildContent("Right click me"))
            .AddChildContent<ContextMenuContent>(p => p
                .AddChildContent("Menu Content")));

        // Act
        var trigger = cut.Find("[data-slot='context-menu-trigger']");
        trigger.ContextMenu(new MouseEventArgs { ClientX = 100, ClientY = 200 });

        // Assert - content should still be rendered
        var content = cut.Find("[data-slot='context-menu-content']");
        content.ShouldNotBeNull();
        content.TextContent.ShouldContain("Menu Content");
    }

    [Fact(DisplayName = "UAC03: ContextMenu content has fixed positioning for portal-like behavior")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ContentHasFixedPositioning()
    {
        // Arrange
        var cut = Ctx.Render<ContextMenuSut>(p => p
            .AddChildContent<ContextMenuTrigger>()
            .AddChildContent<ContextMenuContent>());

        // Act
        var trigger = cut.Find("[data-slot='context-menu-trigger']");
        trigger.ContextMenu(new MouseEventArgs { ClientX = 150, ClientY = 250 });

        // Assert
        var content = cut.Find("[data-slot='context-menu-content']");
        var style = content.GetAttribute("style");
        style.ShouldNotBeNull();
        style!.ShouldContain("position: fixed");
        style.ShouldContain("left: 150px");
        style.ShouldContain("top: 250px");
    }
}
