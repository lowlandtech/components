using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "18")]
[UserStory("01", "Render Sidebar component")]
public class SC18_RenderSidebarMenuAction : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuAction renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuAction>();
        cut.Find("[data-slot='sidebar-menu-action']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuAction renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuAction>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuAction renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuAction>();
        cut.Find("[data-slot='sidebar-menu-action']").ClassList.ShouldContain("sidebar-menu-action");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuAction renders as button element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is a button")]
    public void UAC04_RendersAsButton()
    {
        var cut = Ctx.Render<SidebarMenuAction>();
        cut.Find("button").ShouldNotBeNull();
    }
}
