using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "09")]
[UserStory("01", "Render Sidebar component")]
public class SC09_RenderSidebarSeparator : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarSeparator renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarSeparator>();
        cut.Find("[data-slot='sidebar-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SidebarSeparator renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarSeparator>();
        cut.Find("[data-slot='sidebar-separator']").ClassList.ShouldContain("sidebar-separator");
    }

    [Fact(DisplayName = "UAC04: SidebarSeparator renders as hr element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is an hr")]
    public void UAC04_RendersAsHr()
    {
        var cut = Ctx.Render<SidebarSeparator>();
        cut.Find("hr").ShouldNotBeNull();
    }
}
