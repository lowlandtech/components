using Bunit;
using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Sheet;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1042.Sheet.US02.Behavior;

[Trait(Spec.SPEC, "1042")]
[Trait(Spec.US, "02")]
[UserStory("02", "Sheet component behavior")]
public class SC01_SheetInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: SheetTrigger triggers OnClick when clicked")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_SheetTriggerTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<SheetTrigger>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var trigger = cut.Find("[data-slot='sheet-trigger']");
        await trigger.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: SheetClose triggers OnClick when clicked")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_SheetCloseTriggersOnClick()
    {
        var clicked = false;

        var cut = Ctx.Render<SheetClose>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => clicked = true))
        );

        var close = cut.Find("[data-slot='sheet-close']");
        await close.ClickAsync();

        clicked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: SheetContent closes when overlay is clicked")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_SheetContentClosesWhenOverlayClicked()
    {
        var changed = false;
        bool newValue = true;

        var cut = Ctx.Render<SheetContent>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, v =>
            {
                changed = true;
                newValue = v;
            }))
        );

        var overlay = cut.Find("[data-slot='sheet-overlay']");
        await overlay.ClickAsync();

        changed.ShouldBeTrue();
        newValue.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: SheetContent triggers IsOpenChanged when closed via overlay")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_SheetContentTriggersIsOpenChanged()
    {
        var changed = false;

        var cut = Ctx.Render<SheetContent>(p => p
            .Add(x => x.IsOpen, true)
            .Add(x => x.IsOpenChanged, EventCallback.Factory.Create<bool>(this, _ => changed = true))
        );

        var overlay = cut.Find("[data-slot='sheet-overlay']");
        await overlay.ClickAsync();

        changed.ShouldBeTrue();
    }
}
