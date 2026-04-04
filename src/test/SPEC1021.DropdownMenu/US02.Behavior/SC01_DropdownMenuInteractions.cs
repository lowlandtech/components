using Bunit;
using Microsoft.AspNetCore.Components;
using LowlandTech.Components.DropdownMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1021.DropdownMenu.US02.Behavior;

[Trait(Spec.SPEC, "1021")]
[Trait(Spec.US, "02")]
[UserStory("02", "DropdownMenu component behavior")]
public class SC01_DropdownMenuInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: DropdownMenuTrigger triggers OnClick when clicked")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DropdownMenuTriggerTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<DropdownMenuTrigger>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var trigger = cut.Find("[data-slot='dropdown-menu-trigger']");
        await trigger.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: DropdownMenuItem triggers OnSelect when clicked")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_DropdownMenuItemTriggersOnSelect()
    {
        var selected = false;

        var cut = Ctx.Render<DropdownMenuItem>(p => p
            .Add(x => x.OnSelect, EventCallback.Factory.Create(this, () => selected = true))
        );

        var item = cut.Find("[data-slot='dropdown-menu-item']");
        await item.ClickAsync();

        selected.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Disabled DropdownMenuItem does not trigger OnSelect")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DisabledDropdownMenuItemDoesNotTriggerOnSelect()
    {
        var selected = false;

        var cut = Ctx.Render<DropdownMenuItem>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnSelect, EventCallback.Factory.Create(this, () => selected = true))
        );

        var item = cut.Find("[data-slot='dropdown-menu-item']");
        await item.ClickAsync();

        selected.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: DropdownMenuContent renders when IsOpen is true")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DropdownMenuContentRendersWhenIsOpenTrue()
    {
        var cut = Ctx.Render<DropdownMenuContent>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='dropdown-menu-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: DropdownMenuContent does not render when IsOpen is false")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DropdownMenuContentDoesNotRenderWhenIsOpenFalse()
    {
        var cut = Ctx.Render<DropdownMenuContent>(p => p.Add(x => x.IsOpen, false));
        cut.FindAll("[data-slot='dropdown-menu-content']").Count.ShouldBe(0);
    }
}
