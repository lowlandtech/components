using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US01.View;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.US, "03")]
[UserStory("03", "ContextMenu sub-menu functionality")]
public class SC02_ContextMenuSubContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenuSubContent renders when sub-menu is open")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersWhenSubMenuIsOpen()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent<ContextMenuSubContent>(p => p
                .AddChildContent("Sub Content")));

        // Assert
        var content = cut.Find("[data-slot='context-menu-sub-content']");
        content.ShouldNotBeNull();
        content.TextContent.ShouldContain("Sub Content");
    }

    [Fact(DisplayName = "UAC02: ContextMenuSubContent does not render when sub-menu is closed")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DoesNotRenderWhenSubMenuIsClosed()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, false)
            .AddChildContent<ContextMenuSubContent>(p => p
                .AddChildContent("Sub Content")));

        // Assert
        var contents = cut.FindAll("[data-slot='context-menu-sub-content']");
        contents.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: ContextMenuSubContent has data-state attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_HasDataStateAttribute()
    {
        // Arrange & Act
        var cut = Ctx.Render<ContextMenuSub>(p => p
            .Add(x => x.IsOpen, true)
            .AddChildContent<ContextMenuSubContent>());

        // Assert
        var content = cut.Find("[data-slot='context-menu-sub-content']");
        content.GetAttribute("data-state").ShouldBe("open");
    }
}
