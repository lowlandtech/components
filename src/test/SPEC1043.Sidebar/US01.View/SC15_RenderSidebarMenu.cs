using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "15")]
[UserStory("01", "Render Sidebar component")]
public class SC15_RenderSidebarMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenu renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenu>();
        cut.Find("[data-slot='sidebar-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenu renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenu>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarMenu renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenu>();
        cut.Find("[data-slot='sidebar-menu']").ClassList.ShouldContain("sidebar-menu");
    }

    [Fact(DisplayName = "UAC04: SidebarMenu renders as ul element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is a ul")]
    public void UAC04_RendersAsUl()
    {
        var cut = Ctx.Render<SidebarMenu>();
        cut.Find("ul").ShouldNotBeNull();
    }
}
