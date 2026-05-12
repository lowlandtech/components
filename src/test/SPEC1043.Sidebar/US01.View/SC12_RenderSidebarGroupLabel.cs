using Bunit;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "12")]
[UserStory("01", "Render Sidebar component")]
public class SC12_RenderSidebarGroupLabel : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarGroupLabel renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarGroupLabel>();
        cut.Find("[data-slot='sidebar-group-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarGroupLabel renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarGroupLabel>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarGroupLabel renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarGroupLabel>();
        cut.Find("[data-slot='sidebar-group-label']").ClassList.ShouldContain("sidebar-group-label");
    }
}
