using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Render Sidebar component")]
public class SC04_RenderSidebarFooter : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarFooter renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarFooter>();
        cut.Find("[data-slot='sidebar-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarFooter renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarFooter>(p => p.AddChildContent("<span>Footer</span>"));
        cut.Markup.ShouldContain("Footer");
    }

    [Fact(DisplayName = "UAC03: SidebarFooter renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains sidebar-footer class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarFooter>();
        cut.Find("[data-slot='sidebar-footer']").ClassList.ShouldContain("sidebar-footer");
    }

    [Fact(DisplayName = "UAC04: SidebarFooter merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<SidebarFooter>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='sidebar-footer']");
        el.ClassList.ShouldContain("sidebar-footer");
        el.ClassList.ShouldContain("custom");
    }
}
