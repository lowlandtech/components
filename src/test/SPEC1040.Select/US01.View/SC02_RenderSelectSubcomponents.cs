using Bunit;
using SelectSut = LowlandTech.Components.Select.Select;
using LowlandTech.Components.Select;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1040.Select.US01.View;

[Trait(Spec.SPEC, "1040")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Select subcomponents")]
public class SC02_RenderSelectSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: SelectTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TriggerRendersDataSlot()
    {
        var cut = Ctx.Render<SelectTrigger>(p => p.AddChildContent("Select"));
        cut.Find("[data-slot='select-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SelectTrigger renders as button")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TriggerRendersAsButton()
    {
        var cut = Ctx.Render<SelectTrigger>(p => p.AddChildContent("Select"));
        cut.Find("button").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SelectTrigger has type button")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_TriggerHasTypeButton()
    {
        var cut = Ctx.Render<SelectTrigger>(p => p.AddChildContent("Select"));
        cut.Find("button").GetAttribute("type").ShouldBe("button");
    }

    [Fact(DisplayName = "UAC04: SelectTrigger renders chevron icon")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_TriggerRendersChevronIcon()
    {
        var cut = Ctx.Render<SelectTrigger>(p => p.AddChildContent("Select"));
        cut.Find("svg").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: SelectValue renders data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ValueRendersDataSlot()
    {
        var cut = Ctx.Render<SelectValue>();
        cut.Find("[data-slot='select-value']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: SelectValue renders placeholder when no selection")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ValueRendersPlaceholder()
    {
        var cut = Ctx.Render<SelectValue>(p => p.Add(x => x.Placeholder, "Choose an option"));
        cut.Markup.ShouldContain("Choose an option");
    }

    [Fact(DisplayName = "UAC07: SelectContent renders when IsOpen is true")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ContentRendersWhenOpen()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp => cp.AddChildContent("Content"));
        });
        cut.Markup.ShouldContain("Content");
        cut.Find("[data-slot='select-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: SelectContent does not render when IsOpen is false")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ContentDoesNotRenderWhenClosed()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.AddChildContent<SelectContent>(cp => cp.AddChildContent("Content"));
        });
        cut.Markup.ShouldNotContain("Content");
    }

    [Fact(DisplayName = "UAC09: SelectItem renders data-slot attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ItemRendersDataSlot()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "1").AddChildContent("Item 1")));
        });
        cut.Find("[data-slot='select-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: SelectItem renders check icon when selected")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ItemRendersCheckIconWhenSelected()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.Value, "1");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "1").AddChildContent("Item 1")));
        });
        cut.Find(".cn-select-item-indicator").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: SelectItem does not render check icon when not selected")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ItemDoesNotRenderCheckIconWhenNotSelected()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.Value, "2");
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "1").AddChildContent("Item 1")));
        });
        cut.FindAll(".cn-select-item-indicator").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC12: SelectGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_GroupRendersDataSlot()
    {
        var cut = Ctx.Render<SelectGroup>(p => p.AddChildContent("Group"));
        cut.Find("[data-slot='select-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: SelectLabel renders data-slot attribute")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_LabelRendersDataSlot()
    {
        var cut = Ctx.Render<SelectLabel>(p => p.AddChildContent("Label"));
        cut.Find("[data-slot='select-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC14: SelectSeparator renders data-slot attribute")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_SeparatorRendersDataSlot()
    {
        var cut = Ctx.Render<SelectSeparator>();
        cut.Find("[data-slot='select-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: SelectTrigger supports Size parameter")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_TriggerSupportsSizeParameter()
    {
        var cut = Ctx.Render<SelectTrigger>(p =>
        {
            p.Add(x => x.Size, "sm");
            p.AddChildContent("Select");
        });
        cut.Find("button").GetAttribute("data-size").ShouldBe("sm");
    }

    [Fact(DisplayName = "UAC16: SelectItem supports Disabled parameter")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_ItemSupportsDisabledParameter()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "1")
                      .Add(x => x.Disabled, true)
                      .AddChildContent("Item 1")));
        });
        var item = cut.Find("[data-slot='select-item']");
        item.HasAttribute("data-disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC17: SelectTrigger toggles IsOpen when clicked")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_TriggerTogglesIsOpen()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.AddChildContent<SelectTrigger>(tp => tp.AddChildContent("Open"));
        });
        cut.Find("button").Click();
        cut.Instance.IsOpen.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC18: SelectContent renders scroll buttons when open")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_ContentRendersScrollButtons()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>();
        });
        cut.FindAll("[data-slot='select-content'] svg").Count.ShouldBe(2); // 2 scroll buttons in content
    }

    [Fact(DisplayName = "UAC19: SelectItem sets Value when clicked")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_ItemSetsValueWhenClicked()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "test-value")
                      .Add(x => x.Text, "Test Text")
                      .AddChildContent("Test Item")));
        });
        cut.Find("[data-slot='select-item']").Click();
        cut.Instance.Value.ShouldBe("test-value");
    }

    [Fact(DisplayName = "UAC20: SelectItem closes dropdown when clicked")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_ItemClosesDropdownWhenClicked()
    {
        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectContent>(cp =>
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "1").AddChildContent("Item 1")));
        });
        cut.Find("[data-slot='select-item']").Click();
        cut.Instance.IsOpen.ShouldBeFalse();
    }
}
