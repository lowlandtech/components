using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "12")]
public class SC12_SidebarSupportsLeftAndRightPositioning : Scenario
{
    [Fact(DisplayName = "Sidebar positions on the right")]
    public async Task SidebarPositionsOnTheRight()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        // When I set side to "right"
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Side, SidebarSide.Right)));

        // Then the Sidebar should position on the right
        var sidebarElement = cut.Find("[data-slot='sidebar']");
        Assert.Equal("right", sidebarElement.GetAttribute("data-side"));
    }

    [Fact(DisplayName = "Positioning styles flip appropriately")]
    public async Task PositioningStylesFlipAppropriately()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Side, SidebarSide.Right)));

        // Then positioning styles should flip
        var container = cut.Find("[data-slot='sidebar-container']");
        Assert.Equal("right", container.GetAttribute("data-side"));
        
        var classes = container.GetAttribute("class");
        Assert.Contains("data-[side=right]:", classes);
    }
}
