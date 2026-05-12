using Bunit;
using LowlandTech.Components.Menubar;

namespace LowlandTech.Components.Tests.SPEC1031.Menubar.US02.Behavior;

[Trait(Spec.SPEC, "1031")]
[Trait(Spec.US, "02")]
[UserStory("02", "Menubar component behavior")]
public class SC01_MenubarInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: MenubarTrigger triggers OnClick when clicked")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_MenubarTriggerTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<MenubarTrigger>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var trigger = cut.Find("[data-slot='menubar-trigger']");
        await trigger.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: MenubarItem triggers OnSelect when clicked")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_MenubarItemTriggersOnSelect()
    {
        var selected = false;

        var cut = Ctx.Render<MenubarItem>(p => p
            .Add(x => x.OnSelect, EventCallback.Factory.Create(this, () => selected = true))
        );

        var item = cut.Find("[data-slot='menubar-item']");
        await item.ClickAsync();

        selected.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Disabled MenubarItem does not trigger OnSelect")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DisabledMenubarItemDoesNotTriggerOnSelect()
    {
        var selected = false;

        var cut = Ctx.Render<MenubarItem>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.OnSelect, EventCallback.Factory.Create(this, () => selected = true))
        );

        var item = cut.Find("[data-slot='menubar-item']");
        await item.ClickAsync();

        selected.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: MenubarContent renders when IsOpen is true")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_MenubarContentRendersWhenIsOpenTrue()
    {
        var cut = Ctx.Render<MenubarContent>(p => p.Add(x => x.IsOpen, true));
        cut.Find("[data-slot='menubar-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: MenubarContent does not render when IsOpen is false")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_MenubarContentDoesNotRenderWhenIsOpenFalse()
    {
        var cut = Ctx.Render<MenubarContent>(p => p.Add(x => x.IsOpen, false));
        cut.FindAll("[data-slot='menubar-content']").Count.ShouldBe(0);
    }
}
