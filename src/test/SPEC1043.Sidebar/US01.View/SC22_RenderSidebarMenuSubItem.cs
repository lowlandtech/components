using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "22")]
[UserStory("01", "Render Sidebar component")]
public class SC22_RenderSidebarMenuSubItem : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSubItem renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuSubItem>();
        cut.Find("[data-slot='sidebar-menu-sub-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSubItem renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuSubItem>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSubItem renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuSubItem>();
        cut.Find("[data-slot='sidebar-menu-sub-item']").ClassList.ShouldContain("sidebar-menu-sub-item");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSubItem renders as li element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is a li")]
    public void UAC04_RendersAsLi()
    {
        var cut = Ctx.Render<SidebarMenuSubItem>();
        cut.Find("li").ShouldNotBeNull();
    }
}
