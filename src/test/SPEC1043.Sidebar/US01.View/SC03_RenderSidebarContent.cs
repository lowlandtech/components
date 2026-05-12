using Bunit;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Render Sidebar component")]
public class SC03_RenderSidebarContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarContent renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarContent>();
        cut.Find("[data-slot='sidebar-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarContent renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarContent>(p => p.AddChildContent("<span>Content</span>"));
        cut.Markup.ShouldContain("Content");
    }

    [Fact(DisplayName = "UAC03: SidebarContent renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains app-sidebar-nav class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarContent>();
        cut.Find("[data-slot='sidebar-content']").ClassList.ShouldContain("app-sidebar-nav");
    }

    [Fact(DisplayName = "UAC04: SidebarContent merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<SidebarContent>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='sidebar-content']");
        el.ClassList.ShouldContain("app-sidebar-nav");
        el.ClassList.ShouldContain("custom");
    }
}
