using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "07")]
public class SC07_SidebarSupportsCollapsibleModes : Scenario
{
    [Fact(DisplayName = "Sidebar collapses to icon width")]
    public async Task SidebarCollapsesToIconWidth()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        // When I set collapsible mode to "icon"
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, false)
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Collapsible, SidebarCollapsible.Icon)));

        // Then the Sidebar should collapse to icon width
        var sidebarElement = cut.Find("[data-slot='sidebar']");
        Assert.Equal("icon", sidebarElement.GetAttribute("data-collapsible"));
    }

    [Fact(DisplayName = "Text labels hide in collapsed icon state")]
    public async Task TextLabelsHideInCollapsedIconState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, false)
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Collapsible, SidebarCollapsible.Icon)
                .AddChildContent<SidebarGroup>(group => group
                    .AddChildContent<SidebarGroupLabel>(label => label
                        .AddChildContent("Label")))));

        // Then text labels should have opacity-0 class in collapsed state
        var label = cut.Find("[data-sidebar='group-label']");
        var classes = label.GetAttribute("class");
        Assert.Contains("group-data-[collapsible=icon]:opacity-0", classes);
    }
}
