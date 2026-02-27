using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "19")]
[UserStory("01", "Render Sidebar component")]
public class SC19_RenderSidebarMenuBadge : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuBadge renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuBadge>();
        cut.Find("[data-slot='sidebar-menu-badge']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuBadge renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuBadge>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuBadge renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuBadge>();
        cut.Find("[data-slot='sidebar-menu-badge']").ClassList.ShouldContain("sidebar-menu-badge");
    }
}
