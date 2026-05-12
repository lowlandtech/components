using Bunit;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "25")]
[UserStory("01", "Render Sidebar component")]
public class SC25_RenderSidebarMenuSubContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSubContent renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuSubContent>();
        cut.Find("[data-slot='sidebar-menu-sub-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSubContent renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuSubContent>(p => p.AddChildContent("Sub Content"));
        cut.Markup.ShouldContain("Sub Content");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSubContent renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the root element has the default class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarMenuSubContent>();
        cut.Find("[data-slot='sidebar-menu-sub-content']")
           .ClassList.ShouldContain("sidebar-menu-sub-content");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSubContent adds visible class when IsVisible")]
    [Trait(Spec.UAC, "04")]
    [Then("the root element has visible class")]
    public void UAC04_AddsVisibleClass()
    {
        var cut = Ctx.Render<SidebarMenuSubContent>(p => p.Add(x => x.IsVisible, true));
        cut.Find("[data-slot='sidebar-menu-sub-content']")
           .ClassList.ShouldContain("visible");
    }
}
