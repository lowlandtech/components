using Bunit;
using SidebarSut = LowlandTech.Components.Sidebar.Sidebar;
using LowlandTech.Components.Sidebar;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Sidebar component")]
public class SC01_RenderSidebar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sidebar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarSut>();
        cut.Find("[data-slot='sidebar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Sidebar renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Sidebar renders as aside element")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered element is an aside")]
    public void UAC03_RendersAsAside()
    {
        var cut = Ctx.Render<SidebarSut>();
        cut.Find("aside").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Sidebar renders default app-sidebar class")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains app-sidebar class")]
    public void UAC04_RendersDefaultClass()
    {
        var cut = Ctx.Render<SidebarSut>();
        cut.Find("[data-slot='sidebar']").ClassList.ShouldContain("app-sidebar");
    }

    [Fact(DisplayName = "UAC05: Sidebar renders data-side attribute")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains data-side")]
    public void UAC05_RendersDataSide()
    {
        var cut = Ctx.Render<SidebarSut>(p => p.Add(x => x.Side, "right"));
        cut.Find("[data-slot='sidebar']").GetAttribute("data-side").ShouldBe("right");
    }

    [Fact(DisplayName = "UAC06: Sidebar renders data-variant attribute")]
    [Trait(Spec.UAC, "06")]
    [Then("the rendered markup contains data-variant")]
    public void UAC06_RendersDataVariant()
    {
        var cut = Ctx.Render<SidebarSut>(p => p.Add(x => x.Variant, "floating"));
        cut.Find("[data-slot='sidebar']").GetAttribute("data-variant").ShouldBe("floating");
    }

    [Fact(DisplayName = "UAC07: Sidebar renders data-collapsible attribute")]
    [Trait(Spec.UAC, "07")]
    [Then("the rendered markup contains data-collapsible")]
    public void UAC07_RendersDataCollapsible()
    {
        var cut = Ctx.Render<SidebarSut>(p => p.Add(x => x.Collapsible, "icon"));
        cut.Find("[data-slot='sidebar']").GetAttribute("data-collapsible").ShouldBe("icon");
    }

    [Fact(DisplayName = "UAC08: Sidebar renders data-state from SidebarState")]
    [Trait(Spec.UAC, "08")]
    [Then("the rendered markup reflects sidebar state")]
    public void UAC08_RendersDataStateFromSidebarState()
    {
        var state = new SidebarState { Open = false };
        var cut = Ctx.Render<SidebarSut>(p => p
            .Add(x => x.SidebarState, state));
        cut.Find("[data-slot='sidebar']").GetAttribute("data-state").ShouldBe("collapsed");
    }
}
