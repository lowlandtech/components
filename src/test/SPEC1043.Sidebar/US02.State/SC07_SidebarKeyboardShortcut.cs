using Bunit;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components.Web;
using SidebarSut = LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US02.State;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "07")]
[UserStory("02", "Sidebar keyboard shortcut")]
public class SC07_SidebarKeyboardShortcut : ViewScenario
{
    [Fact(DisplayName = "UAC01: Ctrl+B toggles desktop sidebar state")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CtrlBTogglesSidebar()
    {
        var cut = Ctx.Render<SidebarProvider>(p => p.AddChildContent<SidebarSut>());

        cut.Find("[data-slot='sidebar']").GetAttribute("data-state").ShouldBe("expanded");

        cut.Find("[data-slot='sidebar-provider']").KeyDown(new KeyboardEventArgs { Key = "b", CtrlKey = true });

        cut.Find("[data-slot='sidebar']").GetAttribute("data-state").ShouldBe("collapsed");
    }

    [Fact(DisplayName = "UAC02: Cmd+B toggles desktop sidebar state")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CmdBTogglesSidebar()
    {
        var cut = Ctx.Render<SidebarProvider>(p => p.AddChildContent<SidebarSut>());

        cut.Find("[data-slot='sidebar-provider']").KeyDown(new KeyboardEventArgs { Key = "b", MetaKey = true });

        cut.Find("[data-slot='sidebar']").GetAttribute("data-state").ShouldBe("collapsed");
    }
}