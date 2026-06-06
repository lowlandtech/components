using Bunit;
using Microsoft.AspNetCore.Components.Web;
using SwitchSut = LowlandTech.Components.Switch;

namespace LowlandTech.Components.Tests.SPEC1048.Switch.US02.Interaction;

[Trait(Spec.SPEC, "1048")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Switch keyboard interaction")]
public class SC01_SwitchKeyboard : ViewScenario
{
    [Fact(DisplayName = "UAC01: Space key toggles switch on")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SpaceKeyTogglesSwitchOn()
    {
        bool isChecked = false;
        var cut = Ctx.Render<SwitchSut>(p =>
        {
            p.Add(x => x.IsChecked, false);
            p.Add(x => x.IsCheckedChanged, (bool val) => isChecked = val);
        });

        var button = cut.Find("[data-slot='switch']");
        button.KeyDown(new KeyboardEventArgs { Key = " " });

        isChecked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: Enter key toggles switch on")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_EnterKeyTogglesSwitchOn()
    {
        bool isChecked = false;
        var cut = Ctx.Render<SwitchSut>(p =>
        {
            p.Add(x => x.IsChecked, false);
            p.Add(x => x.IsCheckedChanged, (bool val) => isChecked = val);
        });

        var button = cut.Find("[data-slot='switch']");
        button.KeyDown(new KeyboardEventArgs { Key = "Enter" });

        isChecked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Space key toggles switch off")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SpaceKeyTogglesSwitchOff()
    {
        bool isChecked = true;
        var cut = Ctx.Render<SwitchSut>(p =>
        {
            p.Add(x => x.IsChecked, true);
            p.Add(x => x.IsCheckedChanged, (bool val) => isChecked = val);
        });

        var button = cut.Find("[data-slot='switch']");
        button.KeyDown(new KeyboardEventArgs { Key = " " });

        isChecked.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: Click toggles switch")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ClickTogglesSwitch()
    {
        bool isChecked = false;
        var cut = Ctx.Render<SwitchSut>(p =>
        {
            p.Add(x => x.IsChecked, false);
            p.Add(x => x.IsCheckedChanged, (bool val) => isChecked = val);
        });

        var button = cut.Find("[data-slot='switch']");
        button.Click();

        isChecked.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC05: aria-checked reflects unchecked state")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_AriaCheckedReflectsUncheckedState()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.IsChecked, false));
        var button = cut.Find("[data-slot='switch']");
        button.GetAttribute("aria-checked").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC06: data-state shows unchecked")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DataStateShowsUnchecked()
    {
        var cut = Ctx.Render<SwitchSut>(p => p.Add(x => x.IsChecked, false));
        var button = cut.Find("[data-slot='switch']");
        button.GetAttribute("data-state").ShouldBe("unchecked");
    }

    [Fact(DisplayName = "UAC07: Disabled switch does not toggle on click")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_DisabledSwitchDoesNotToggleOnClick()
    {
        bool changed = false;
        var cut = Ctx.Render<SwitchSut>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.IsChecked, false);
            p.Add(x => x.IsCheckedChanged, (bool _) => changed = true);
        });

        cut.Find("[data-slot='switch']").Click();

        changed.ShouldBeFalse();
        cut.Find("[data-slot='switch']").GetAttribute("data-state").ShouldBe("unchecked");
    }

    [Fact(DisplayName = "UAC08: Disabled switch does not toggle on Enter")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_DisabledSwitchDoesNotToggleOnEnter()
    {
        bool changed = false;
        var cut = Ctx.Render<SwitchSut>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.Add(x => x.IsChecked, false);
            p.Add(x => x.IsCheckedChanged, (bool _) => changed = true);
        });

        cut.Find("[data-slot='switch']").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        changed.ShouldBeFalse();
        cut.Find("[data-slot='switch']").GetAttribute("data-state").ShouldBe("unchecked");
    }
}
