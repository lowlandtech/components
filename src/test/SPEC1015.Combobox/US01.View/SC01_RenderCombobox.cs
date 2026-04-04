using Bunit;
using ComboboxComponent = LowlandTech.Components.Combobox.Combobox;
using LowlandTech.Components.Combobox;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1015.Combobox.US01.View;

[Trait(Spec.SPEC, "1015")]
[Trait(Spec.US, "01")]
[UserStory("01", "Combobox component rendering")]
public class SC01_RenderCombobox : ViewScenario
{
    [Fact(DisplayName = "UAC01: Combobox renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ComboboxRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxComponent>();
        cut.Find("[data-slot='combobox']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Combobox renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ComboboxRendersChildContent()
    {
        var cut = Ctx.Render<ComboboxComponent>(p => p.AddChildContent("<span class='test'>Content</span>"));
        cut.Find(".test").TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "UAC03: ComboboxList renders with data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ComboboxListRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxList>();
        cut.Find("[data-slot='combobox-list']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: ComboboxEmpty renders with data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ComboboxEmptyRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxEmpty>();
        cut.Find("[data-slot='combobox-empty']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: ComboboxEmpty renders ChildContent")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ComboboxEmptyRendersChildContent()
    {
        var cut = Ctx.Render<ComboboxEmpty>(p => p.AddChildContent("No items"));
        cut.Find("[data-slot='combobox-empty']").TextContent.ShouldBe("No items");
    }

    [Fact(DisplayName = "UAC06: ComboboxGroup renders with data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ComboboxGroupRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxGroup>();
        cut.Find("[data-slot='combobox-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: ComboboxLabel renders with data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ComboboxLabelRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxLabel>();
        cut.Find("[data-slot='combobox-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: ComboboxSeparator renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ComboboxSeparatorRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxSeparator>();
        cut.Find("[data-slot='combobox-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: ComboboxItem renders with data-slot attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ComboboxItemRendersWithDataSlot()
    {
        var cut = Ctx.Render<ComboboxItem>(p => p
            .AddCascadingValue(new ComboboxComponent())
            .Add(x => x.Value, "test")
        );
        cut.Find("[data-slot='combobox-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: ComboboxItem renders ChildContent")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ComboboxItemRendersChildContent()
    {
        var cut = Ctx.Render<ComboboxItem>(p => p
            .AddCascadingValue(new ComboboxComponent())
            .Add(x => x.Value, "test")
            .AddChildContent("Item")
        );
        cut.Find("[data-slot='combobox-item']").TextContent.ShouldContain("Item");
    }

    [Fact(DisplayName = "UAC11: Full Combobox SUT renders correctly")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_FullComboboxSutRenders()
    {
        var cut = Ctx.Render<ComboboxSut>();
        cut.Find("[data-slot='combobox']").ShouldNotBeNull();
        cut.Find("[data-slot='combobox-input']").ShouldNotBeNull();
    }
}
