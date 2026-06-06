using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "14")]
[UserStory("01", "Render Sidebar component")]
public class SC14_RenderSidebarGroupAction : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarGroupAction renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarGroupAction>();
        cut.Find("[data-slot='sidebar-group-action']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarGroupAction renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarGroupAction>(p => p.AddChildContent("Test"));
        cut.Markup.ShouldContain("Test");
    }

    [Fact(DisplayName = "UAC03: SidebarGroupAction renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarGroupAction>();
        cut.Find("[data-slot='sidebar-group-action']").ClassList.ShouldContain("sidebar-group-action");
    }

    [Fact(DisplayName = "UAC04: SidebarGroupAction renders as button element")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered element is a button")]
    public void UAC04_RendersAsButton()
    {
        var cut = Ctx.Render<SidebarGroupAction>();
        cut.Find("button").ShouldNotBeNull();
    }
}
