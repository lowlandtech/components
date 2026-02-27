using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "11")]
[UserStory("01", "Render Sidebar component")]
public class SC11_RenderSidebarGroup : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarGroup>();
        cut.Find("[data-slot='sidebar-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarGroup renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarGroup>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarGroup renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarGroup>();
        cut.Find("[data-slot='sidebar-group']").ClassList.ShouldContain("sidebar-group");
    }
}
