using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1017.ContextMenu.US02.Behavior;

[Trait(Spec.SPEC, "1017")]
[Trait(Spec.SC, "04")]
[UserStory("02", "ContextMenu checkbox and radio items")]
public class SC04_ContextMenuSelectionItems : ViewScenario
{
    [Fact(DisplayName = "UAC01: ContextMenuCheckboxItem renders menuitemcheckbox semantics")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CheckboxItemRendersSemantics()
    {
        var cut = Ctx.Render<ContextMenuCheckboxItem>(p => p
            .Add(x => x.Checked, true)
            .AddChildContent("Show Labels"));

        var item = cut.Find("[data-slot='context-menu-checkbox-item']");
        item.GetAttribute("role").ShouldBe("menuitemcheckbox");
        item.GetAttribute("aria-checked").ShouldBe("true");
        item.GetAttribute("data-state").ShouldBe("checked");
    }

    [Fact(DisplayName = "UAC02: ContextMenuCheckboxItem toggles checked state on click")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CheckboxItemTogglesOnClick()
    {
        var changed = false;
        var newValue = false;

        var cut = Ctx.Render<ContextMenuCheckboxItem>(p => p
            .Add(x => x.CheckedChanged, value => { changed = true; newValue = value; })
            .AddChildContent("Show Labels"));

        cut.Find("[data-slot='context-menu-checkbox-item']").Click();

        changed.ShouldBeTrue();
        newValue.ShouldBeTrue();
        cut.Find("[data-slot='context-menu-checkbox-item']").GetAttribute("aria-checked").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC03: ContextMenuRadioGroup renders mutually exclusive radio items")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RadioGroupRendersSelection()
    {
        var cut = Ctx.Render<ContextMenuRadioGroup>(p => p
            .Add(x => x.Value, "medium")
            .AddChildContent<ContextMenuRadioItem>(item => item.Add(x => x.Value, "small").AddChildContent("Small"))
            .AddChildContent<ContextMenuRadioItem>(item => item.Add(x => x.Value, "medium").AddChildContent("Medium"))
            .AddChildContent<ContextMenuRadioItem>(item => item.Add(x => x.Value, "large").AddChildContent("Large")));

        var items = cut.FindAll("[data-slot='context-menu-radio-item']");
        items.Count.ShouldBe(3);
        items[1].GetAttribute("aria-checked").ShouldBe("true");
        items[0].GetAttribute("aria-checked").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC04: Clicking a radio item updates the group value")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RadioItemUpdatesGroupValue()
    {
        string? changedValue = null;

        var cut = Ctx.Render<ContextMenuRadioGroup>(p => p
            .Add(x => x.Value, "medium")
            .Add(x => x.ValueChanged, value => changedValue = value)
            .AddChildContent<ContextMenuRadioItem>(item => item.Add(x => x.Value, "small").AddChildContent("Small"))
            .AddChildContent<ContextMenuRadioItem>(item => item.Add(x => x.Value, "medium").AddChildContent("Medium"))
            .AddChildContent<ContextMenuRadioItem>(item => item.Add(x => x.Value, "large").AddChildContent("Large")));

        cut.FindAll("[data-slot='context-menu-radio-item']")[2].Click();

        changedValue.ShouldBe("large");
        cut.Instance.Value.ShouldBe("large");
        cut.FindAll("[data-slot='context-menu-radio-item']")[2].GetAttribute("aria-checked").ShouldBe("true");
    }
}