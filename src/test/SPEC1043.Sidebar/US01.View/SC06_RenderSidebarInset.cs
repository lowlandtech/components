using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Render Sidebar component")]
public class SC06_RenderSidebarInset : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarInset renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarInset>();
        cut.Find("[data-slot='sidebar-inset']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarInset renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarInset>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarInset renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarInset>();
        cut.Find("[data-slot='sidebar-inset']").ClassList.ShouldContain("sidebar-inset");
    }

    [Fact(DisplayName = "UAC04: SidebarInset renders as main element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is a main")]
    public void UAC04_RendersAsMain()
    {
        var cut = Ctx.Render<SidebarInset>();
        cut.Find("main").ShouldNotBeNull();
    }
}
