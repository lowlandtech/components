using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "23")]
[UserStory("01", "Render Sidebar component")]
public class SC23_RenderSidebarMenuSubButton : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSubButton renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuSubButton>();
        cut.Find("[data-slot='sidebar-menu-sub-button']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSubButton renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuSubButton>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSubButton renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuSubButton>();
        cut.Find("[data-slot='sidebar-menu-sub-button']").ClassList.ShouldContain("sidebar-menu-sub-button");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSubButton renders data-active when IsActive")]
    [Trait(Spec.UAC, "04")]
    [Then("data-active is set")]
    public void UAC04_RendersDataActive()
    {
        var cut = Ctx.Render<SidebarMenuSubButton>(p => p.Add(x => x.IsActive, true));
        cut.Find("[data-slot='sidebar-menu-sub-button']").GetAttribute("data-active").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC05: SidebarMenuSubButton renders as a element")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered element is an anchor")]
    public void UAC05_RendersAsAnchor()
    {
        var cut = Ctx.Render<SidebarMenuSubButton>();
        cut.Find("a").ShouldNotBeNull();
    }
}
