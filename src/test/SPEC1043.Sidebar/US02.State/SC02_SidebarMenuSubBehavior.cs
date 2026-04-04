using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US02.State;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Sidebar sub-menu behavior")]
public class SC02_SidebarMenuSubBehavior : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarMenuSub starts closed by default")]
    [Trait(Spec.UAC, "01")]
    [Then("sub-menu starts closed")]
    public void UAC01_StartsClosed()
    {
        var cut = Ctx.Render<SidebarMenuSub>();
        cut.Find("[data-slot='sidebar-menu-sub']")
           .GetAttribute("data-state")
           .ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC02: SidebarMenuSub opens when IsOpen is true")]
    [Trait(Spec.UAC, "02")]
    [Then("sub-menu is open")]
    public void UAC02_OpensWhenIsOpenTrue()
    {
        var cut = Ctx.Render<SidebarMenuSub>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='sidebar-menu-sub']")
           .GetAttribute("data-state")
           .ShouldBe("open");
    }

    [Fact(DisplayName = "UAC03: SidebarMenuSub ToggleAsync switches state")]
    [Trait(Spec.UAC, "03")]
    [Then("toggle switches state")]
    public async Task UAC03_ToggleSwitchesState()
    {
        var cut = Ctx.Render<SidebarMenuSub>();
        var component = cut.Instance;
        
        await cut.InvokeAsync(() => component.ToggleAsync());
        
        cut.Find("[data-slot='sidebar-menu-sub']")
           .GetAttribute("data-state")
           .ShouldBe("open");
    }

    [Fact(DisplayName = "UAC04: SidebarMenuSub OpenAsync method sets state to open")]
    [Trait(Spec.UAC, "04")]
    [Then("open method sets state")]
    public async Task UAC04_OpenMethodSetsState()
    {
        var cut = Ctx.Render<SidebarMenuSub>();
        var component = cut.Instance;
        
        await cut.InvokeAsync(() => component.OpenAsync());
        
        cut.Find("[data-slot='sidebar-menu-sub']")
           .GetAttribute("data-state")
           .ShouldBe("open");
    }

    [Fact(DisplayName = "UAC05: SidebarMenuSub CloseAsync method sets state to closed")]
    [Trait(Spec.UAC, "05")]
    [Then("close method sets state")]
    public async Task UAC05_CloseMethodSetsState()
    {
        var cut = Ctx.Render<SidebarMenuSub>(p => p.Add(x => x.IsOpen, true));
        var component = cut.Instance;
        
        await cut.InvokeAsync(() => component.CloseAsync());
        
        cut.Find("[data-slot='sidebar-menu-sub']")
           .GetAttribute("data-state")
           .ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC06: SidebarMenuSub adds open class when open")]
    [Trait(Spec.UAC, "06")]
    [Then("open class is added")]
    public void UAC06_AddsOpenClass()
    {
        var cut = Ctx.Render<SidebarMenuSub>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='sidebar-menu-sub']")
           .ClassList.ShouldContain("open");
    }
}
