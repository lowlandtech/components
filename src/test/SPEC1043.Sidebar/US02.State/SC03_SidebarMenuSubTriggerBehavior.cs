using Bunit;
using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US02.State;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "03")]
[UserStory("02", "Sidebar sub-menu trigger behavior")]
public class SC03_SidebarMenuSubTriggerBehavior : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSubTrigger calls OnToggle when clicked")]
    [Trait(Spec.UAC, "01")]
    [Then("OnToggle is called on click")]
    public void UAC01_CallsOnToggleOnClick()
    {
        var toggled = false;
        var cut = Ctx.Render<SidebarMenuSubTrigger>(p => 
            p.Add(x => x.OnToggle, () => toggled = true));
        
        cut.Find("button").Click();
        
        toggled.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSubTrigger aria-expanded reflects IsExpanded")]
    [Trait(Spec.UAC, "02")]
    [Then("aria-expanded reflects state")]
    public void UAC02_AriaExpandedReflectsIsExpanded()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>(p => p.Add(x => x.IsExpanded, true));
        cut.Find("button").GetAttribute("aria-expanded").ShouldBe("True");
        
        cut = Ctx.Render<SidebarMenuSubTrigger>(p => p.Add(x => x.IsExpanded, false));
        cut.Find("button").GetAttribute("aria-expanded").ShouldBe("False");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSubTrigger chevron rotates when expanded")]
    [Trait(Spec.UAC, "03")]
    [Then("chevron has rotated class when expanded")]
    public void UAC03_ChevronRotatesWhenExpanded()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>(p => p.Add(x => x.IsExpanded, true));
        cut.Find(".sidebar-menu-sub-trigger-chevron")
           .ClassList.ShouldContain("rotated");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSubTrigger chevron is not rotated when collapsed")]
    [Trait(Spec.UAC, "04")]
    [Then("chevron does not have rotated class when collapsed")]
    public void UAC04_ChevronNotRotatedWhenCollapsed()
    {
        var cut = Ctx.Render<SidebarMenuSubTrigger>(p => p.Add(x => x.IsExpanded, false));
        cut.Find(".sidebar-menu-sub-trigger-chevron")
           .ClassList.ShouldNotContain("rotated");
    }
}
