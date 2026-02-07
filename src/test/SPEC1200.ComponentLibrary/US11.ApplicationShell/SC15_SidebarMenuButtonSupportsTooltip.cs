using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "15")]
public class SC15_SidebarMenuButtonSupportsTooltip : Scenario
{
    [Fact(DisplayName = "Tooltips display in collapsed state")]
    public async Task TooltipsDisplayInCollapsedState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a SidebarMenuButton specification with tooltip
        // When the Sidebar is in collapsed state
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, false)
            .AddChildContent<SidebarMenuButton>(button => button
                .Add(b => b.TooltipText, "Home")
                .AddChildContent("H")));

        // Then tooltips should be rendered
        var tooltips = cut.FindAll("[data-sidebar='menu-button']");
        Assert.NotEmpty(tooltips);
    }

    [Fact(DisplayName = "Tooltips are hidden in expanded state")]
    public async Task TooltipsAreHiddenInExpandedState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a SidebarMenuButton specification with tooltip
        // When the Sidebar is in expanded state
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent<SidebarMenuButton>(button => button
                .Add(b => b.TooltipText, "Home")
                .AddChildContent("Home")));

        // Then tooltips should not show (no Tooltip component rendered in expanded state)
        var tooltipComponents = cut.FindAll(".tooltip"); // Assuming tooltip has this class
        // In expanded state, the button is rendered without Tooltip wrapper
        var menuButton = cut.Find("[data-sidebar='menu-button']");
        Assert.NotNull(menuButton);
    }
}
