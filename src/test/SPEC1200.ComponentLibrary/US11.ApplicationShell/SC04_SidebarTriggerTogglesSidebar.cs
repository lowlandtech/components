using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "04")]
public class SC04_SidebarTriggerTogglesSidebar : Scenario
{
    [Fact(DisplayName = "SidebarTrigger toggles sidebar state")]
    public async Task SidebarTriggerTogglesSidebarState()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        module.SetupVoid("saveState", _ => true).SetVoidResult();
        
        // Given I have a SidebarTrigger specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Sidebar>(0);
                builder.CloseComponent();
                builder.OpenComponent<SidebarTrigger>(1);
                builder.CloseComponent();
            }));

        // When I click the SidebarTrigger button
        var trigger = cut.Find("[data-sidebar='trigger']");
        var provider = cut.Instance;
        
        var initialState = provider.State;
        Assert.Equal("expanded", initialState);
        
        await trigger.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        // Then the Sidebar should toggle state
        Assert.Equal("collapsed", provider.State);
    }

    [Fact(DisplayName = "State change is reflected in context")]
    public async Task StateChangeIsReflectedInContext()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        module.SetupVoid("saveState", _ => true).SetVoidResult();
        
        // Given I have a SidebarTrigger specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent<SidebarTrigger>());

        var provider = cut.Instance;
        var initialState = provider.IsOpen;
        
        // When I toggle the sidebar
        await cut.InvokeAsync(async () => await provider.ToggleSidebarAsync());

        // Then the state change should be reflected
        Assert.NotEqual(initialState, provider.IsOpen);
    }
}
