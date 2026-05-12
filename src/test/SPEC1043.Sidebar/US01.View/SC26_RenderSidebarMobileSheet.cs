using Bunit;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "26")]
[UserStory("01", "Render Sidebar component")]
public class SC26_RenderSidebarMobileSheet : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMobileSheet renders trigger button on mobile")]
    [Trait(Spec.UAC, "01")]
    [Then("the trigger button is rendered when IsMobile")]
    public void UAC01_RendersTriggerOnMobile()
    {
        var state = new SidebarState { IsMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find("[data-slot='sidebar-mobile-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMobileSheet renders hamburger icon")]
    [Trait(Spec.UAC, "02")]
    [Then("the trigger contains hamburger icon")]
    public void UAC02_RendersHamburgerIcon()
    {
        var state = new SidebarState { IsMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        var trigger = cut.Find("[data-slot='sidebar-mobile-trigger']");
        trigger.InnerHtml.ShouldContain("svg");
    }

    [Fact(DisplayName = "UAC03: SidebarMobileSheet renders overlay when OpenMobile")]
    [Trait(Spec.UAC, "03")]
    [Then("the overlay is rendered when OpenMobile is true")]
    public void UAC03_RendersOverlayWhenOpen()
    {
        var state = new SidebarState { IsMobile = true, OpenMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find(".sidebar-mobile-overlay").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: SidebarMobileSheet renders content when OpenMobile")]
    [Trait(Spec.UAC, "04")]
    [Then("the content is rendered when OpenMobile is true")]
    public void UAC04_RendersContentWhenOpen()
    {
        var state = new SidebarState { IsMobile = true, OpenMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state)
             .AddChildContent("Mobile Content"));
        
        cut.Find("[data-slot='sidebar-mobile-content']").ShouldNotBeNull();
        cut.Markup.ShouldContain("Mobile Content");
    }

    [Fact(DisplayName = "UAC05: SidebarMobileSheet content has role=dialog")]
    [Trait(Spec.UAC, "05")]
    [Then("the content has dialog role")]
    public void UAC05_ContentHasDialogRole()
    {
        var state = new SidebarState { IsMobile = true, OpenMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find("[data-slot='sidebar-mobile-content']")
           .GetAttribute("role")
           .ShouldBe("dialog");
    }

    [Fact(DisplayName = "UAC06: SidebarMobileSheet close button has aria-label")]
    [Trait(Spec.UAC, "06")]
    [Then("the close button has aria-label")]
    public void UAC06_CloseButtonHasAriaLabel()
    {
        var state = new SidebarState { IsMobile = true, OpenMobile = true };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state));
        
        cut.Find(".sidebar-mobile-close")
           .GetAttribute("aria-label")
           .ShouldBe("Close sidebar");
    }

    [Fact(DisplayName = "UAC07: SidebarMobileSheet renders desktop content directly when not mobile")]
    [Trait(Spec.UAC, "07")]
    [Then("desktop content is rendered directly")]
    public void UAC07_RendersDesktopContentDirectly()
    {
        var state = new SidebarState { IsMobile = false };
        var cut = Ctx.Render<SidebarMobileSheet>(p => 
            p.AddCascadingValue(state)
             .AddChildContent("Desktop Content"));
        
        cut.Markup.ShouldContain("Desktop Content");
        cut.FindAll("[data-slot='sidebar-mobile-trigger']").Count.ShouldBe(0);
    }
}
