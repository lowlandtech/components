using Bunit;
using LowlandTech.Components.Sidebar;

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

    [Fact(DisplayName = "UAC05: SidebarBrand renders logo and title")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains brand logo and title slots")]
    public void UAC05_SidebarBrandRendersLogoAndTitle()
    {
        var cut = Ctx.Render<SidebarBrand>(p => p
            .Add(x => x.LogoSrc, "_content/LowlandTech.Components/img/session-app-logo.svg")
            .Add(x => x.LogoAlt, "SessionApp")
            .Add(x => x.Title, "SessionApp"));

        var logo = cut.Find("[data-slot='sidebar-brand-logo']");
        logo.GetAttribute("src").ShouldBe("_content/LowlandTech.Components/img/session-app-logo.svg");
        logo.GetAttribute("alt").ShouldBe("SessionApp");
        cut.Find("[data-slot='sidebar-brand-title']").TextContent.Trim().ShouldBe("SessionApp");
        logo.ClassList.ShouldContain("rounded-md");
    }
}
