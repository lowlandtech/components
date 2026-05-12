using Bunit;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "17")]
[UserStory("01", "Render Sidebar component")]
public class SC17_RenderSidebarMenuButton : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuButton renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuButton>();
        cut.Find("[data-slot='sidebar-menu-button']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuButton renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuButton>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuButton renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuButton>();
        cut.Find("[data-slot='sidebar-menu-button']").ClassList.ShouldContain("sidebar-menu-button");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuButton renders data-active when IsActive")]
    [Trait(Spec.UAC, "04")]
    [Then("data-active is set")]
    public void UAC04_RendersDataActive()
    {
        var cut = Ctx.Render<SidebarMenuButton>(p => p.Add(x => x.IsActive, true));
        cut.Find("[data-slot='sidebar-menu-button']").GetAttribute("data-active").ShouldBe("true");
    }
}
