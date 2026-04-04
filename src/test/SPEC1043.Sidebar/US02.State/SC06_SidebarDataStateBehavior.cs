using Bunit;
using SidebarSut = LowlandTech.Components.Sidebar.Sidebar;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US02.State;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "06")]
[UserStory("02", "Sidebar data-state behavior")]
public class SC06_SidebarDataStateBehavior : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sidebar data-state is expanded when SidebarState.Open is true")]
    [Trait(Spec.UAC, "01")]
    [Then("data-state is expanded")]
    public void UAC01_DataStateExpanded()
    {
        var state = new SidebarState { Open = true };
        var cut = Ctx.Render<SidebarSut>(p => 
            p.Add(x => x.SidebarState, state));
        
        cut.Find("[data-slot='sidebar']")
           .GetAttribute("data-state")
           .ShouldBe("expanded");
    }

    [Fact(DisplayName = "UAC02: Sidebar data-state is collapsed when SidebarState.Open is false")]
    [Trait(Spec.UAC, "02")]
    [Then("data-state is collapsed")]
    public void UAC02_DataStateCollapsed()
    {
        var state = new SidebarState { Open = false };
        var cut = Ctx.Render<SidebarSut>(p => 
            p.Add(x => x.SidebarState, state));
        
        cut.Find("[data-slot='sidebar']")
           .GetAttribute("data-state")
           .ShouldBe("collapsed");
    }
}
