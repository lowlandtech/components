using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "10")]
[UserStory("01", "Render Sidebar component")]
public class SC10_RenderSidebarInput : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarInput renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarInput>();
        cut.Find("[data-slot='sidebar-input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SidebarInput renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarInput>();
        cut.Find("[data-slot='sidebar-input']").ClassList.ShouldContain("sidebar-input");
    }

    [Fact(DisplayName = "UAC04: SidebarInput renders as input element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is an input")]
    public void UAC04_RendersAsInput()
    {
        var cut = Ctx.Render<SidebarInput>();
        cut.Find("input").ShouldNotBeNull();
    }
}
