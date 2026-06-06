using Bunit;
using NavigationMenuComponent = LowlandTech.Components.NavigationMenu;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1033.NavigationMenu.US01.View;

[Trait(Spec.SPEC, "1033")]
[Trait(Spec.US, "01")]
[UserStory("01", "NavigationMenu component rendering")]
public class SC01_RenderNavigationMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: NavigationMenu renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NavigationMenuRendersWithDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuComponent>();
        cut.Find("[data-slot='navigation-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: NavigationMenu renders as nav element")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NavigationMenuRendersAsNav()
    {
        var cut = Ctx.Render<NavigationMenuComponent>();
        cut.Find("nav[data-slot='navigation-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: NavigationMenu renders ChildContent")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_NavigationMenuRendersChildContent()
    {
        var cut = Ctx.Render<NavigationMenuComponent>(p => p.AddChildContent("<span class='test'>Content</span>"));
        cut.Find(".test").TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "UAC04: NavigationMenuList renders with data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NavigationMenuListRendersWithDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuList>();
        var list = cut.Find("[data-slot='navigation-menu-list']");
        list.ShouldNotBeNull();
        list.TagName.ShouldBe("UL");
    }

    [Fact(DisplayName = "UAC05: NavigationMenuItem renders with data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NavigationMenuItemRendersWithDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuItem>();
        var item = cut.Find("[data-slot='navigation-menu-item']");
        item.ShouldNotBeNull();
        item.TagName.ShouldBe("LI");
    }

    [Fact(DisplayName = "UAC06: NavigationMenuTrigger renders with data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NavigationMenuTriggerRendersWithDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuTrigger>();
        var trigger = cut.Find("[data-slot='navigation-menu-trigger']");
        trigger.ShouldNotBeNull();
        trigger.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC07: NavigationMenuTrigger renders ChildContent")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NavigationMenuTriggerRendersChildContent()
    {
        var cut = Ctx.Render<NavigationMenuTrigger>(p => p.AddChildContent("Menu"));
        cut.Find("[data-slot='navigation-menu-trigger']").TextContent.ShouldBe("Menu");
    }

    [Fact(DisplayName = "UAC08: NavigationMenuContent renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_NavigationMenuContentRendersWithDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuContent>();
        cut.Find("[data-slot='navigation-menu-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: NavigationMenuContent renders closed state when IsOpen is false")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_NavigationMenuContentRendersClosedState()
    {
        var cut = Ctx.Render<NavigationMenuContent>(p => p.Add(x => x.IsOpen, false));
        var content = cut.Find("[data-slot='navigation-menu-content']");
        content.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC10: NavigationMenuLink renders with data-slot attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_NavigationMenuLinkRendersWithDataSlot()
    {
        var cut = Ctx.Render<NavigationMenuLink>();
        var link = cut.Find("[data-slot='navigation-menu-link']");
        link.ShouldNotBeNull();
        link.TagName.ShouldBe("A");
    }

    [Fact(DisplayName = "UAC11: NavigationMenuLink renders with href")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_NavigationMenuLinkRendersWithHref()
    {
        var cut = Ctx.Render<NavigationMenuLink>(p => p.Add(x => x.Href, "/test"));
        cut.Find("[data-slot='navigation-menu-link']").GetAttribute("href").ShouldBe("/test");
    }

    [Fact(DisplayName = "UAC12: NavigationMenuLink renders ChildContent")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_NavigationMenuLinkRendersChildContent()
    {
        var cut = Ctx.Render<NavigationMenuLink>(p => p.AddChildContent("Link"));
        cut.Find("[data-slot='navigation-menu-link']").TextContent.ShouldBe("Link");
    }

    [Fact(DisplayName = "UAC13: Full NavigationMenu SUT renders correctly")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_FullNavigationMenuSutRenders()
    {
        var cut = Ctx.Render<NavigationMenuSut>();
        cut.Find("[data-slot='navigation-menu']").ShouldNotBeNull();
        cut.Find("[data-slot='navigation-menu-list']").ShouldNotBeNull();
        cut.FindAll("[data-slot='navigation-menu-item']").Count.ShouldBe(3);
        cut.FindAll("[data-slot='navigation-menu-trigger']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='navigation-menu-content']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='navigation-menu-link']").Count.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC14: NavigationMenuTrigger renders data-state")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_NavigationMenuTriggerRendersDataState()
    {
        var cut = Ctx.Render<NavigationMenuTrigger>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='navigation-menu-trigger']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC15: NavigationMenuViewport renders with data-slot")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_NavigationMenuViewportRenders()
    {
        var cut = Ctx.Render<NavigationMenuViewport>();
        cut.Find("[data-slot='navigation-menu-viewport']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC16: NavigationMenuIndicator renders with data-slot")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_NavigationMenuIndicatorRenders()
    {
        var cut = Ctx.Render<NavigationMenuIndicator>();
        cut.Find("[data-slot='navigation-menu-indicator']").ShouldNotBeNull();
    }
}
