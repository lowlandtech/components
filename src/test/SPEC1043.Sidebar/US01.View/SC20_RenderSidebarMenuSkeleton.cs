using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "20")]
[UserStory("01", "Render Sidebar component")]
public class SC20_RenderSidebarMenuSkeleton : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSkeleton renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuSkeleton>();
        cut.Find("[data-slot='sidebar-menu-skeleton']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSkeleton renders skeleton icon and text elements")]
    [Trait(Spec.UAC, "02")]
    [Then("skeleton children are rendered")]
    public void UAC02_RendersSkeletonChildren()
    {
        var cut = Ctx.Render<SidebarMenuSkeleton>();
        cut.Find(".sidebar-menu-skeleton-icon").ShouldNotBeNull();
        cut.Find(".sidebar-menu-skeleton-text").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSkeleton renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuSkeleton>();
        cut.Find("[data-slot='sidebar-menu-skeleton']").ClassList.ShouldContain("sidebar-menu-skeleton");
    }
}
