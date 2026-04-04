using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US02.State;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "04")]
[UserStory("02", "Sidebar mobile sheet behavior")]
public class SC04_SidebarMobileSheetBehavior : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMobileSheet trigger opens sidebar on click")]
    [Trait(Spec.UAC, "01")]
    [Then("clicking trigger opens sidebar")]
    public void UAC01_TriggerOpensSidebar()
    {
        var state = new SidebarState { IsMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find("[data-slot='sidebar-mobile-trigger']").Click();
        
        state.OpenMobile.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: SidebarMobileSheet closes when overlay is clicked")]
    [Trait(Spec.UAC, "02")]
    [Then("clicking overlay closes sidebar")]
    public void UAC02_OverlayClickClosesSidebar()
    {
        var state = new SidebarState { IsMobile = true, OpenMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find(".sidebar-mobile-overlay").Click();
        
        state.OpenMobile.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC03: SidebarMobileSheet close button closes sidebar")]
    [Trait(Spec.UAC, "03")]
    [Then("clicking close button closes sidebar")]
    public void UAC03_CloseButtonClosesSidebar()
    {
        var state = new SidebarState { IsMobile = true, OpenMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find(".sidebar-mobile-close").Click();
        
        state.OpenMobile.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: SidebarMobileSheet is not rendered on desktop")]
    [Trait(Spec.UAC, "04")]
    [Then("mobile components not rendered on desktop")]
    public void UAC04_NotRenderedOnDesktop()
    {
        var state = new SidebarState { IsMobile = false };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state)
             .AddChildContent("Desktop"));
        
        cut.FindAll("[data-slot='sidebar-mobile-trigger']").Count.ShouldBe(0);
        cut.FindAll(".sidebar-mobile-overlay").Count.ShouldBe(0);
        cut.Markup.ShouldContain("Desktop");
    }
}
