using Bunit;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "27")]
[UserStory("01", "Render SidebarNavPopover component")]
public class SC27_RenderSidebarNavPopover : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarNavPopover renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot='sidebar-nav-popover'")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .AddChildContent("Inner"));

        cut.Find("[data-slot='sidebar-nav-popover']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarNavPopover renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains the child content (e.g. NavLink)")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .AddChildContent("<a href='admin/dashboard'>Dashboard</a>"));

        cut.Markup.ShouldContain("admin/dashboard");
        cut.Markup.ShouldContain("Dashboard");
    }

    [Fact(DisplayName = "UAC03: SidebarNavPopover renders anchor element with popover-anchor id")]
    [Trait(Spec.UAC, "03")]
    [Then("the wrapper element has id starting with popover-anchor-")]
    public void UAC03_RendersPopoverAnchorId()
    {
        var cut = Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .AddChildContent("Inner"));

        var el = cut.Find("[data-slot='sidebar-nav-popover']");
        var id = el.GetAttribute("id");
        id.ShouldNotBeNull();
        id.ShouldStartWith("popover-anchor-");
    }

    [Fact(DisplayName = "UAC04: SidebarNavPopover registers with IPopoverService on init")]
    [Trait(Spec.UAC, "04")]
    [Then("the popover service has one more active popover after render")]
    public void UAC04_RegistersWithPopoverService()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var initialCount = popoverService.ActivePopovers.Count;

        Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .AddChildContent("Inner"));

        popoverService.ActivePopovers.Count.ShouldBe(initialCount + 1);
    }

    [Fact(DisplayName = "UAC05: SidebarNavPopover opens popover synchronously on hover when delay is 0")]
    [Trait(Spec.UAC, "05")]
    [Then("the popover state becomes open immediately after mouse enter")]
    public void UAC05_PopoverOpensOnHover()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .Add(x => x.OpenDelayMs, 0)
            .AddChildContent("Inner"));

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();

        var popover = popoverService.ActivePopovers.Single();
        popover.State.Open.ShouldBeTrue();
        popover.State.Placement.ShouldBe(PopoverPlacement.RightStart);
        popover.State.Fragment.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Popover content includes the route, label, subtitle, kicker, and icon")]
    [Trait(Spec.UAC, "06")]
    [Then("the popover fragment renders a button with the icon tile, kicker, label, subtitle, and route chip")]
    public void UAC06_PopoverContentIncludesRouteAndLabel()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<SidebarNavPopover>(1);
            builder.AddAttribute(2, nameof(SidebarNavPopover.Route), "admin/dashboard");
            builder.AddAttribute(3, nameof(SidebarNavPopover.Label), "Dashboard");
            builder.AddAttribute(4, nameof(SidebarNavPopover.Subtitle), "Home dashboard");
            builder.AddAttribute(5, nameof(SidebarNavPopover.IconName), "home");
            builder.AddAttribute(6, nameof(SidebarNavPopover.OpenDelayMs), 0);
            builder.AddAttribute(7, nameof(SidebarNavPopover.ChildContent),
                (RenderFragment)(b => b.AddContent(0, "Inner")));
            builder.CloseComponent();
        });

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();

        var popover = popoverService.ActivePopovers.Single();
        popover.State.Fragment.ShouldNotBeNull();

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var button = fragmentCut.Find(".lt-sidebar-nav-popover-button");
        button.GetAttribute("data-has-icon").ShouldBe("true");
        button.TextContent.ShouldContain("Page");      // kicker
        button.TextContent.ShouldContain("Dashboard"); // label
        button.TextContent.ShouldContain("Home dashboard"); // subtitle

        // Icon tile rendered with the Icon component inside
        var iconTile = button.QuerySelector(".lt-sidebar-nav-popover-icon");
        iconTile.ShouldNotBeNull();
        iconTile!.QuerySelector("svg").ShouldNotBeNull();

        // Route is rendered as a <code> chip in the footer (outside the button)
        var routeChip = fragmentCut.Find(".lt-sidebar-nav-popover-route");
        routeChip.TagName.ShouldBe("CODE");
        routeChip.TextContent.ShouldContain("admin/dashboard");
    }

    [Fact(DisplayName = "UAC07: Popover hides the icon tile when no icon is provided")]
    [Trait(Spec.UAC, "07")]
    [Then("the button's data-has-icon is 'false' and the icon tile is absent")]
    public void UAC07_PopoverHidesIconWhenAbsent()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<SidebarNavPopover>(1);
            builder.AddAttribute(2, nameof(SidebarNavPopover.Route), "admin/dashboard");
            builder.AddAttribute(3, nameof(SidebarNavPopover.Label), "Dashboard");
            builder.AddAttribute(4, nameof(SidebarNavPopover.OpenDelayMs), 0);
            builder.AddAttribute(5, nameof(SidebarNavPopover.ChildContent),
                (RenderFragment)(b => b.AddContent(0, "Inner")));
            builder.CloseComponent();
        });

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();

        var popover = popoverService.ActivePopovers.Single();
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var button = fragmentCut.Find(".lt-sidebar-nav-popover-button");
        button.GetAttribute("data-has-icon").ShouldBe("false");
        button.QuerySelector(".lt-sidebar-nav-popover-icon").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC08: Clicking the popover button navigates to the route")]
    [Trait(Spec.UAC, "08")]
    [Then("the navigation manager receives a NavigateTo call")]
    public void UAC08_PopoverClickNavigates()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<SidebarNavPopover>(1);
            builder.AddAttribute(2, nameof(SidebarNavPopover.Route), "admin/dashboard");
            builder.AddAttribute(3, nameof(SidebarNavPopover.Label), "Dashboard");
            builder.AddAttribute(4, nameof(SidebarNavPopover.OpenDelayMs), 0);
            builder.AddAttribute(5, nameof(SidebarNavPopover.ChildContent),
                (RenderFragment)(b => b.AddContent(0, "Inner")));
            builder.CloseComponent();
        });

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();

        var popover = popoverService.ActivePopovers.Single();
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".lt-sidebar-nav-popover-button").Click();

        Ctx.Services.GetRequiredService<NavigationManager>()
            .Uri.ShouldEndWith("admin/dashboard");
    }

    [Fact(DisplayName = "UAC09: Popover closes when navigation happens")]
    [Trait(Spec.UAC, "09")]
    [Then("after a LocationChanged event, the popover state becomes closed")]
    public void UAC09_ClosesOnNavigation()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var nav = Ctx.Services.GetRequiredService<NavigationManager>();

        var cut = Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .Add(x => x.OpenDelayMs, 0)
            .AddChildContent("Inner"));

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();
        popoverService.ActivePopovers.Single().State.Open.ShouldBeTrue();

        nav.NavigateTo("admin/other");

        popoverService.ActivePopovers.Single().State.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC10: Popover closes on mouseleave (CloseDelayMs=0 closes synchronously)")]
    [Trait(Spec.UAC, "10")]
    [Then("when CloseDelayMs is 0, the popover state becomes closed immediately on mouse leave")]
    public void UAC10_ClosesOnMouseLeave()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render<SidebarNavPopover>(p => p
            .Add(x => x.Route, "admin/dashboard")
            .Add(x => x.Label, "Dashboard")
            .Add(x => x.OpenDelayMs, 0)
            .Add(x => x.CloseDelayMs, 0)
            .AddChildContent("Inner"));

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();
        popoverService.ActivePopovers.Single().State.Open.ShouldBeTrue();

        anchor.MouseLeave();

        popoverService.ActivePopovers.Single().State.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC11: Kicker label is configurable via the Kicker parameter")]
    [Trait(Spec.UAC, "11")]
    [Then("the popover renders the custom kicker text instead of the default")]
    public void UAC11_CustomKicker()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<SidebarNavPopover>(1);
            builder.AddAttribute(2, nameof(SidebarNavPopover.Route), "admin/dashboard");
            builder.AddAttribute(3, nameof(SidebarNavPopover.Label), "Dashboard");
            builder.AddAttribute(4, nameof(SidebarNavPopover.Kicker), "Dashboard");
            builder.AddAttribute(5, nameof(SidebarNavPopover.OpenDelayMs), 0);
            builder.AddAttribute(6, nameof(SidebarNavPopover.ChildContent),
                (RenderFragment)(b => b.AddContent(0, "Inner")));
            builder.CloseComponent();
        });

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();

        var popover = popoverService.ActivePopovers.Single();
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".lt-sidebar-nav-popover-kicker")
            .TextContent.Trim().ShouldBe("Dashboard");
    }

    [Fact(DisplayName = "UAC12: NavigateToLabel is configurable and rendered in the footer")]
    [Trait(Spec.UAC, "12")]
    [Then("the footer shows the custom NavigateToLabel and the default is 'Navigate to'")]
    public void UAC12_NavigateToLabel()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<SidebarNavPopover>(1);
            builder.AddAttribute(2, nameof(SidebarNavPopover.Route), "admin/dashboard");
            builder.AddAttribute(3, nameof(SidebarNavPopover.Label), "Dashboard");
            builder.AddAttribute(4, nameof(SidebarNavPopover.NavigateToLabel), "Navigeer naar");
            builder.AddAttribute(5, nameof(SidebarNavPopover.OpenDelayMs), 0);
            builder.AddAttribute(6, nameof(SidebarNavPopover.ChildContent),
                (RenderFragment)(b => b.AddContent(0, "Inner")));
            builder.CloseComponent();
        });

        var anchor = cut.Find("[data-slot='sidebar-nav-popover']");
        anchor.MouseEnter();

        var popover = popoverService.ActivePopovers.Single();
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var footerLabel = fragmentCut.Find(".lt-sidebar-nav-popover-footer-label");
        footerLabel.TextContent.Trim().ShouldBe("Navigeer naar");
    }
}
