using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "03")]
public class SC03_SidebarProviderManagesState : Scenario
{
    [Fact(DisplayName = "SidebarProvider provides state context to children")]
    public async Task SidebarProviderProvidesStateContext()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a SidebarProvider specification
        // When I wrap components with SidebarProvider
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent<Sidebar>());

        // Then child components should access sidebar state via context
        var wrapper = cut.Find("[data-slot='sidebar-wrapper']");
        Assert.NotNull(wrapper);
        
        var sidebar = cut.Find("[data-slot='sidebar']");
        Assert.Equal("expanded", sidebar.GetAttribute("data-state"));
    }

    [Fact(DisplayName = "SidebarProvider manages open/closed state")]
    public async Task SidebarProviderManagesOpenClosedState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        module.SetupVoid("saveState", _ => true).SetVoidResult();
        
        // Given I have a SidebarProvider specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent<Sidebar>());

        // When the state changes
        var provider = cut.Instance;
        Assert.True(provider.IsOpen); // Initially open
        
        await cut.InvokeAsync(async () => await provider.SetOpenAsync(false));

        // Then the provider should manage the state
        Assert.False(provider.IsOpen);
    }

    [Fact(DisplayName = "SidebarProvider tracks mobile vs desktop state")]
    public async Task SidebarProviderTracksMobileVsDesktop()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for mobile
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(true);
        
        // Given I have a SidebarProvider specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>());

        // Wait for component to initialize (OnAfterRenderAsync)
        await cut.InvokeAsync(() => Task.CompletedTask);
        
        // Then the provider should track mobile state
        var provider = cut.Instance;
        Assert.True(provider.IsMobile);
    }
}
