using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;
using System;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "05")]
public class SC05_SidebarSupportsMobileResponsive : Scenario
{
    [Fact(DisplayName = "Sidebar renders as Sheet overlay on mobile")]
    public async Task SidebarRendersAsSheetOnMobile()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for mobile
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(true);
        
        // Given I have a Sidebar component specification
        // When I render the Sidebar on mobile viewport
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>());

        // Wait for component to detect mobile state
        cut.WaitForState(() => cut.Instance.IsMobile, timeout: TimeSpan.FromSeconds(2));
        
        // Verify mobile state is true
        var provider = cut.Instance;
        Assert.True(provider.IsMobile);
        
        // Open the mobile sidebar to render the Sheet
        await cut.InvokeAsync(async () => await provider.ToggleSidebarAsync());
        
        // Then it should render as a Sheet overlay
        var sidebar = cut.FindAll("[data-sidebar='sidebar']");
        Assert.NotEmpty(sidebar);
    }

    [Fact(DisplayName = "Mobile sidebar is hidden by default")]
    public async Task MobileSidebarIsHiddenByDefault()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for mobile
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(true);
        
        // Given I have a Sidebar component specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>());

        // Then it should be hidden by default
        var provider = cut.Instance;
        Assert.False(provider.OpenMobile);
    }

    [Fact(DisplayName = "Mobile sidebar opens when triggered")]
    public async Task MobileSidebarOpensWhenTriggered()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for mobile
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(true);
        
        // Given I have a Sidebar component specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>());

        var provider = cut.Instance;
        Assert.False(provider.OpenMobile);
        
        // When triggered
        await cut.InvokeAsync(async () => await provider.ToggleSidebarAsync());

        // Then it should open
        Assert.True(provider.OpenMobile);
    }
}
