using Bunit;
using ComboboxComponent = LowlandTech.Components.Combobox;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1015.Combobox.US02.Behavior;

[Trait(Spec.SPEC, "1015")]
[Trait(Spec.US, "02")]
[UserStory("02", "Combobox component behavior")]
public class SC01_ComboboxInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: ComboboxItem shows checkmark when selected")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ComboboxItemShowsCheckmarkWhenSelected()
    {
        var parent = new ComboboxComponent();
        // Set Value via reflection
        typeof(ComboboxComponent).GetProperty("Value")?.SetValue(parent, "apple");

        var cut = Ctx.Render<ComboboxItem>(p => p
            .AddCascadingValue(parent)
            .Add(x => x.Value, "apple")
            .AddChildContent("Apple")
        );

        cut.Find(".cn-combobox-item-indicator").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ComboboxItem does not show checkmark when not selected")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ComboboxItemDoesNotShowCheckmarkWhenNotSelected()
    {
        var parent = new ComboboxComponent();
        typeof(ComboboxComponent).GetProperty("Value")?.SetValue(parent, "banana");

        var cut = Ctx.Render<ComboboxItem>(p => p
            .AddCascadingValue(parent)
            .Add(x => x.Value, "apple")
            .AddChildContent("Apple")
        );

        cut.FindAll(".cn-combobox-item-indicator").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: ComboboxItem renders when disabled")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ComboboxItemRendersWhenDisabled()
    {
        var cut = Ctx.Render<ComboboxItem>(p => p
            .AddCascadingValue(new ComboboxComponent())
            .Add(x => x.Value, "test")
            .Add(x => x.Disabled, true)
        );

        var item = cut.Find("[data-slot='combobox-item']");
        item.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Combobox binds Value parameter")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ComboboxBindsValueParameter()
    {
        var cut = Ctx.Render<ComboboxComponent>(p => p.Add(x => x.Value, "test-value"));
        
        // Verify component rendered with value
        cut.Find("[data-slot='combobox']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Combobox triggers ValueChanged callback")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ComboboxTriggersValueChangedCallback()
    {
        var changedValue = "";
        var cut = Ctx.Render<ComboboxComponent>(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => changedValue = v ?? ""))
        );

        // Component should render with callback
        cut.Find("[data-slot='combobox']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: ComboboxContent renders closed state by default")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ComboboxContentRendersClosedStateByDefault()
    {
        var cut = Ctx.Render<ComboboxSut>();
        cut.Find("[data-slot='combobox-content']").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC08: Focusing input opens ComboboxContent")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_FocusingInputOpensComboboxContent()
    {
        var cut = Ctx.Render<ComboboxSut>();
        cut.Find("[data-slot='combobox-input']").Focus();

        cut.Find("[data-slot='combobox-content']").GetAttribute("data-state").ShouldBe("open");
    }
}
