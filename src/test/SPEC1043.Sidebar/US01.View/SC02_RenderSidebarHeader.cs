using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Sidebar component")]
public class SC02_RenderSidebarHeader : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarHeader>();
        cut.Find("[data-slot='sidebar-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarHeader renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarHeader>(p => p.AddChildContent("<span>Header</span>"));
        cut.Markup.ShouldContain("Header");
    }

    [Fact(DisplayName = "UAC03: SidebarHeader renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains sidebar-header class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarHeader>();
        cut.Find("[data-slot='sidebar-header']").ClassList.ShouldContain("sidebar-header");
    }

    [Fact(DisplayName = "UAC04: SidebarHeader merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<SidebarHeader>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='sidebar-header']");
        el.ClassList.ShouldContain("sidebar-header");
        el.ClassList.ShouldContain("custom");
    }
}
