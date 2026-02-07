using Bunit;
using LowlandTech.Components.Components.Sidebar;
using LowlandTech.Components.Tests;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "01")]
public class SC01_SidebarRenders : Scenario
{
    [Fact(DisplayName = "Sidebar renders with default configuration")]
    public async Task SidebarRendersWithDefaultConfiguration()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for SidebarProvider
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>());

        // When I render the Sidebar with default settings
        // Then the Sidebar should be visible
        var wrapper = cut.Find("[data-slot='sidebar-wrapper']");
        Assert.NotNull(wrapper);
        
        var sidebar = cut.Find("[data-slot='sidebar']");
        Assert.NotNull(sidebar);
    }

    [Fact(DisplayName = "Sidebar provider manages state context")]
    public async Task SidebarProviderManagesStateContext()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for SidebarProvider
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a SidebarProvider specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent("<div>Test Content</div>"));

        // When I render the provider
        // Then the provider wrapper should exist
        var wrapper = cut.Find("[data-slot='sidebar-wrapper']");
        Assert.NotNull(wrapper);
    }

    [Fact(DisplayName = "SidebarTrigger renders button")]
    public async Task SidebarTriggerRendersButton()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for SidebarProvider
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a SidebarTrigger specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<SidebarTrigger>());

        // When I render the SidebarTrigger
        // Then it should render as a button
        var trigger = cut.Find("[data-sidebar='trigger']");
        Assert.NotNull(trigger);
    }

    [Fact(DisplayName = "SidebarContent renders scrollable area")]
    public async Task SidebarContentRendersScrollableArea()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarContent specification
        var cut = ctx.Render<SidebarContent>(parameters => parameters
            .AddChildContent("<div>Content</div>"));

        // When I render the content
        // Then it should have the scrollable classes
        var content = cut.Find("[data-sidebar='content']");
        Assert.Contains("overflow-auto", content.GetAttribute("class"));
    }

    [Fact(DisplayName = "SidebarGroup renders with proper structure")]
    public async Task SidebarGroupRendersWithProperStructure()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarGroup specification
        var cut = ctx.Render<SidebarGroup>(parameters => parameters
            .AddChildContent("<div>Group Content</div>"));

        // When I render the group
        // Then it should have the group data attribute
        var group = cut.Find("[data-sidebar='group']");
        Assert.NotNull(group);
    }

    [Fact(DisplayName = "SidebarMenu renders as list")]
    public async Task SidebarMenuRendersAsList()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarMenu specification
        var cut = ctx.Render<SidebarMenu>(parameters => parameters
            .AddChildContent<SidebarMenuItem>(item => item
                .AddChildContent("Menu Item")));

        // When I render the menu
        // Then it should render as a ul element
        var menu = cut.Find("[data-sidebar='menu']");
        Assert.Equal("ul", menu.TagName.ToLower());
    }

    [Fact(DisplayName = "SidebarMenuButton renders with variants")]
    public async Task SidebarMenuButtonRendersWithVariants()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop for SidebarProvider
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a SidebarMenuButton specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<SidebarMenuButton>(button => button
                .Add(b => b.IsActive, true)
                .AddChildContent("Button Text")));

        // When I render the button with active state
        // Then it should have the active data attribute
        var button = cut.Find("[data-sidebar='menu-button']");
        Assert.Equal("true", button.GetAttribute("data-active"));
    }

    [Fact(DisplayName = "SidebarHeader renders in correct position")]
    public async Task SidebarHeaderRendersInCorrectPosition()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarHeader specification
        var cut = ctx.Render<SidebarHeader>(parameters => parameters
            .AddChildContent("<div>Header Content</div>"));

        // When I render the header
        // Then it should have the header data attribute
        var header = cut.Find("[data-sidebar='header']");
        Assert.NotNull(header);
    }

    [Fact(DisplayName = "SidebarFooter renders in correct position")]
    public async Task SidebarFooterRendersInCorrectPosition()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarFooter specification
        var cut = ctx.Render<SidebarFooter>(parameters => parameters
            .AddChildContent("<div>Footer Content</div>"));

        // When I render the footer
        // Then it should have the footer data attribute
        var footer = cut.Find("[data-sidebar='footer']");
        Assert.NotNull(footer);
    }

    [Fact(DisplayName = "SidebarInset provides main content area")]
    public async Task SidebarInsetProvidesMainContentArea()
    {
        await using var ctx = new BunitContext();
        
        // Given I have a SidebarInset specification
        var cut = ctx.Render<SidebarInset>(parameters => parameters
            .AddChildContent("<div>Main Content</div>"));

        // When I render the inset
        // Then it should render as a main element
        var inset = cut.Find("[data-slot='sidebar-inset']");
        Assert.Equal("main", inset.TagName.ToLower());
    }
}
