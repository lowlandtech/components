using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "24")]
[UserStory("01", "Render Sidebar component")]
public class SC24_RenderSidebarMenuSubTrigger : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSubTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>();
        cut.Find("[data-slot='sidebar-menu-sub-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSubTrigger renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>(p => p.AddChildContent("Trigger"));
        cut.Markup.ShouldContain("Trigger");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSubTrigger renders as button element")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered element is a button")]
    public void UAC03_RendersAsButton()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>();
        cut.Find("button").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSubTrigger renders aria-expanded false by default")]
    [Trait(Spec.UAC, "04")]
    [Then("the button has aria-expanded=false")]
    public void UAC04_RendersAriaExpandedFalse()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>();
        cut.Find("[data-slot='sidebar-menu-sub-trigger']")
           .GetAttribute("aria-expanded")
           .ShouldBe("false");
    }

    [Fact(DisplayName = "UAC05: SidebarMenuSubTrigger renders aria-expanded true when IsExpanded")]
    [Trait(Spec.UAC, "05")]
    [Then("the button has aria-expanded=true")]
    public void UAC05_RendersAriaExpandedTrue()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>(p => p.Add(x => x.IsExpanded, true));
        cut.Find("[data-slot='sidebar-menu-sub-trigger']")
           .GetAttribute("aria-expanded")
           .ShouldBe("true");
    }

    [Fact(DisplayName = "UAC06: SidebarMenuSubTrigger renders chevron icon")]
    [Trait(Spec.UAC, "06")]
    [Then("the button contains a chevron icon")]
    public void UAC06_RendersChevronIcon()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>();
        cut.Find(".sidebar-menu-sub-trigger-chevron").ShouldNotBeNull();
    }
}
