using Bunit;
using LowlandTech.Components.Components.Sidebar;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US11.ApplicationShell;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "06")]
public class SC06_SidebarSupportsVariants : Scenario
{
    [Fact(DisplayName = "Sidebar applies floating variant styles")]
    public async Task SidebarAppliesFloatingVariantStyles()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        // When I render the Sidebar with variant "floating"
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Variant, SidebarVariant.Floating)));

        // Then it should apply floating variant styles
        var sidebarElement = cut.Find("[data-slot='sidebar']");
        Assert.Equal("floating", sidebarElement.GetAttribute("data-variant"));
    }

    [Fact(DisplayName = "Floating variant has rounded corners and shadow")]
    public async Task FloatingVariantHasRoundedCornersAndShadow()
    {
        await using var ctx = new BunitContext();
        
        // Setup JSInterop
        var module = ctx.JSInterop.SetupModule("./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
        module.SetupVoid("initialize", _ => true).SetVoidResult();
        module.Setup<bool>("isMobile").SetResult(false);
        
        // Given I have a Sidebar component specification
        var cut = ctx.Render<SidebarProvider>(parameters => parameters
            .AddChildContent<Sidebar>(sidebar => sidebar
                .Add(s => s.Variant, SidebarVariant.Floating)));

        // Then it should have rounded corners and shadow
        var inner = cut.Find("[data-slot='sidebar-inner']");
        var classes = inner.GetAttribute("class");
        Assert.Contains("rounded-lg", classes);
        Assert.Contains("shadow", classes);
    }
}
