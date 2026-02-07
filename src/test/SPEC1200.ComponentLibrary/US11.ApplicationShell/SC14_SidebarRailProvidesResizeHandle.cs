using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "14")]
public class SC14_SidebarRailProvidesResizeHandle : Scenario
{
    [Fact(DisplayName = "SidebarRail shows visual feedback on hover")]
    public async Task SidebarRailShowsVisualFeedback()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarRail specification
        var cut = ctx.Render<SidebarRail>();

        // When I hover over the SidebarRail
        var rail = cut.Find("[data-sidebar='rail']");
        
        // Then it should show visual feedback
        var classes = rail.GetAttribute("class");
        Assert.Contains("hover:", classes);
    }

    [Fact(DisplayName = "Clicking rail toggles sidebar state")]
    public async Task ClickingRailTogglesSidebarState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        module.SetupVoid("saveState", _ => true).SetVoidResult();
        
        // Given I have a SidebarRail specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent<SidebarRail>());

        var provider = cut.Instance;
        var initialState = provider.IsOpen;
        
        // When clicking the rail
        var rail = cut.Find("[data-sidebar='rail']");
        await rail.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        // Then it should toggle sidebar state
        Assert.NotEqual(initialState, provider.IsOpen);
    }
}
