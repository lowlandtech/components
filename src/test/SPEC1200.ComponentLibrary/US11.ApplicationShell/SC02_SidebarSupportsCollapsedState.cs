using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "02")]
public class SC02_SidebarSupportsCollapsedState : Scenario
{
    [Fact(DisplayName = "Sidebar applies collapsed state styles")]
    public async Task SidebarAppliesCollapsedStateStyles()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        // When I render the Sidebar in collapsed state
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, false)
            .AddChildContent<Sidebar>());

        // Then the Sidebar should apply collapsed state styles
        var sidebar = cut.Find("[data-slot='sidebar']");
        Assert.Equal("collapsed", sidebar.GetAttribute("data-state"));
    }

    [Fact(DisplayName = "Sidebar reduces width in collapsed state")]
    public async Task SidebarReducesWidthInCollapsedState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        // When I render the Sidebar in collapsed state with icon mode
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, false)
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Collapsible, SidebarCollapsible.Icon)));

        // Then it should have collapsed state
        var sidebarElement = cut.Find("[data-slot='sidebar']");
        Assert.Equal("collapsed", sidebarElement.GetAttribute("data-state"));
        Assert.Equal("icon", sidebarElement.GetAttribute("data-collapsible"));
    }
}
