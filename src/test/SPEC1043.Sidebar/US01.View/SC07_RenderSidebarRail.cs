using Bunit;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Render Sidebar component")]
public class SC07_RenderSidebarRail : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarRail renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarRail>();
        cut.Find("[data-slot='sidebar-rail']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarRail renders default CSS class")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains default class")]
    public void UAC02_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarRail>();
        cut.Find("[data-slot='sidebar-rail']").ClassList.ShouldContain("sidebar-rail");
    }

    [Fact(DisplayName = "UAC03: SidebarRail renders as button element")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered element is a button")]
    public void UAC03_RendersAsButton()
    {
        var cut = Ctx.Render<SidebarRail>();
        cut.Find("button").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: SidebarRail has aria-label")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains aria-label")]
    public void UAC04_HasAriaLabel()
    {
        var cut = Ctx.Render<SidebarRail>();
        cut.Find("[data-slot='sidebar-rail']").GetAttribute("aria-label").ShouldNotBeNull();
    }
}
