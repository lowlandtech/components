using Bunit;
using LowlandTech.Components.NavigationMenu;

namespace LowlandTech.Components.Tests.SPEC1033.NavigationMenu.US02.Behavior;

[Trait(Spec.SPEC, "1033")]
[Trait(Spec.US, "02")]
[UserStory("02", "NavigationMenu component behavior")]
public class SC02_NavigationMenuManagedState : ViewScenario
{
    [Fact(DisplayName = "UAC04: Clicking trigger opens its dropdown in managed mode")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ClickingTriggerOpensDropdown()
    {
        var cut = Ctx.Render<NavigationMenuManagedSut>();

        var triggers = cut.FindAll("[data-slot='navigation-menu-trigger']");

        cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("closed");
        cut.FindAll("[data-slot='navigation-menu-content']")[1].GetAttribute("data-state").ShouldBe("closed");

        await triggers[0].ClickAsync();

        cut.WaitForAssertion(() =>
        {
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("open");
            cut.FindAll("[data-slot='navigation-menu-content']")[1].GetAttribute("data-state").ShouldBe("closed");
        });
    }

    [Fact(DisplayName = "UAC05: Opening another dropdown closes the previous one")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_OpeningAnotherDropdownClosesPrevious()
    {
        var cut = Ctx.Render<NavigationMenuManagedSut>();

        var triggers = cut.FindAll("[data-slot='navigation-menu-trigger']");

        await triggers[0].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("open"));

        await triggers[1].ClickAsync();
        cut.WaitForAssertion(() =>
        {
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("closed");
            cut.FindAll("[data-slot='navigation-menu-content']")[1].GetAttribute("data-state").ShouldBe("open");
        });
    }

    [Fact(DisplayName = "UAC06: Clicking the same trigger closes its dropdown")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ClickingSameTriggerClosesDropdown()
    {
        var cut = Ctx.Render<NavigationMenuManagedSut>();

        var triggers = cut.FindAll("[data-slot='navigation-menu-trigger']");

        await triggers[0].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("open"));

        await triggers[0].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("closed"));
    }

    [Fact(DisplayName = "UAC07: Clicking NavigationMenuLink inside dropdown closes it")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ClickingLinkInsideDropdownClosesIt()
    {
        var cut = Ctx.Render<NavigationMenuManagedSut>();

        var triggers = cut.FindAll("[data-slot='navigation-menu-trigger']");
        var links = cut.FindAll("[data-slot='navigation-menu-link']");

        await triggers[0].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("open"));

        await links[0].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("closed"));
    }

    [Fact(DisplayName = "UAC08: Clicking top-level NavigationMenuLink closes open dropdown")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ClickingTopLevelLinkClosesDropdown()
    {
        var cut = Ctx.Render<NavigationMenuManagedSut>();

        var triggers = cut.FindAll("[data-slot='navigation-menu-trigger']");
        var links = cut.FindAll("[data-slot='navigation-menu-link']");

        await triggers[0].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("open"));

        await links[2].ClickAsync();
        cut.WaitForAssertion(() =>
            cut.FindAll("[data-slot='navigation-menu-content']")[0].GetAttribute("data-state").ShouldBe("closed"));
    }

    [Fact(DisplayName = "UAC09: NavigationMenuLink accepts PreventDefault parameter")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_NavigationMenuLinkAcceptsPreventDefault()
    {
        var cut = Ctx.Render<NavigationMenuLink>(p => p
            .Add(x => x.Href, "/test")
            .Add(x => x.PreventDefault, true)
        );

        var link = cut.Find("[data-slot='navigation-menu-link']");
        link.GetAttribute("href").ShouldBe("/test");
    }
}
