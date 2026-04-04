using Bunit;
using Microsoft.AspNetCore.Components;
using LowlandTech.Components.NavigationMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1033.NavigationMenu.US02.Behavior;

[Trait(Spec.SPEC, "1033")]
[Trait(Spec.US, "02")]
[UserStory("02", "NavigationMenu component behavior")]
public class SC01_NavigationMenuInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: NavigationMenuTrigger triggers OnClick when clicked")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_NavigationMenuTriggerTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<NavigationMenuTrigger>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var trigger = cut.Find("[data-slot='navigation-menu-trigger']");
        await trigger.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: NavigationMenuContent renders when IsOpen is true")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NavigationMenuContentRendersWhenIsOpenTrue()
    {
        var cut = Ctx.Render<NavigationMenuContent>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='navigation-menu-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: NavigationMenuContent does not render when IsOpen is false")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_NavigationMenuContentDoesNotRenderWhenIsOpenFalse()
    {
        var cut = Ctx.Render<NavigationMenuContent>(p => p.Add(x => x.IsOpen, false));
        cut.FindAll("[data-slot='navigation-menu-content']").Count.ShouldBe(0);
    }
}
