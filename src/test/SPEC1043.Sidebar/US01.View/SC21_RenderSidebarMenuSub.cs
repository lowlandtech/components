using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "21")]
[UserStory("01", "Render Sidebar component")]
public class SC21_RenderSidebarMenuSub : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSub renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuSub>();
        cut.Find("[data-slot='sidebar-menu-sub']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSub renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuSub>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSub renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuSub>();
        cut.Find("[data-slot='sidebar-menu-sub']").ClassList.ShouldContain("sidebar-menu-sub");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSub renders as ul element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is a ul")]
    public void UAC04_RendersAsUl()
    {
        var cut = Ctx.Render<SidebarMenuSub>();
        cut.Find("ul").ShouldNotBeNull();
    }
}
