using Bunit;
using RadioGroupSut = LowlandTech.Components.RadioGroup.RadioGroup;
using LowlandTech.Components.RadioGroup;

namespace LowlandTech.Components.Tests.SPEC1037.RadioGroup.US01.View;

[Trait(Spec.SPEC, "1037")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render RadioGroup items")]
public class SC02_RenderRadioGroupItems : ViewScenario
{
    [Fact(DisplayName = "UAC01: RadioGroupItem renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RadioGroupItemRendersDataSlot()
    {
        var cut = Ctx.Render<RadioGroupItem>(p => p.Add(x => x.Value, "option1"));
        cut.Find("[data-slot='radio-group-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: RadioGroupItem renders input type=radio")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RadioGroupItemRendersInput()
    {
        var cut = Ctx.Render<RadioGroupItem>(p => p.Add(x => x.Value, "option1"));
        cut.Find("input[type='radio']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: RadioGroupItem renders with Value parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RadioGroupItemRendersWithValue()
    {
        var cut = Ctx.Render<RadioGroupItem>(p => p.Add(x => x.Value, "my-value"));
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("value").ShouldBe("my-value");
    }

    [Fact(DisplayName = "UAC04: RadioGroupItem shows checked state when selected")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RadioGroupItemShowsCheckedState()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var item = cut.Find("[data-slot='radio-group-item']");
        item.GetAttribute("data-state").ShouldBe("checked");
    }

    [Fact(DisplayName = "UAC05: RadioGroupItem shows unchecked state when not selected")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RadioGroupItemShowsUncheckedState()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option2"));
        });
        var item = cut.Find("[data-slot='radio-group-item']");
        item.GetAttribute("data-state").ShouldBe("unchecked");
    }

    [Fact(DisplayName = "UAC06: RadioGroupItem renders indicator when checked")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RadioGroupItemRendersIndicatorWhenChecked()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        cut.Find("[data-slot='radio-group-item-indicator']").ShouldNotBeNull();
        cut.Find(".cn-radio-group-item-indicator-icon").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: RadioGroupItem does not render indicator icon when unchecked")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RadioGroupItemDoesNotRenderIndicatorWhenUnchecked()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option2"));
        });
        var indicator = cut.Find("[data-slot='radio-group-item-indicator']");
        indicator.InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC08: RadioGroupItem inherits name from parent")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RadioGroupItemInheritsName()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Name, "test-group");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("name").ShouldBe("test-group");
    }

    [Fact(DisplayName = "UAC09: RadioGroupItem renders with Disabled state")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RadioGroupItemRendersWithDisabledState()
    {
        var cut = Ctx.Render<RadioGroupItem>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.Add(x => x.Disabled, true);
        });
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("disabled").ShouldNotBeNull();
        var item = cut.Find("[data-slot='radio-group-item']");
        item.GetAttribute("data-disabled").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC10: RadioGroupItem inherits disabled from parent")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_RadioGroupItemInheritsDisabledFromParent()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("disabled").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: RadioGroupItem renders ChildContent")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_RadioGroupItemRendersChildContent()
    {
        var cut = Ctx.Render<RadioGroupItem>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.AddChildContent("Option Label");
        });
        cut.Markup.ShouldContain("Option Label");
    }

    [Fact(DisplayName = "UAC12: Multiple RadioGroupItems render with same name")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_MultipleRadioGroupItemsRenderWithSameName()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Name, "my-group");
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option2"));
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option3"));
        });
        var radios = cut.FindAll("input[type='radio']");
        radios.Count.ShouldBe(3);
        foreach (var radio in radios)
        {
            radio.GetAttribute("name").ShouldBe("my-group");
        }
    }

    [Fact(DisplayName = "UAC13: RadioGroupItem Class parameter is applied")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_RadioGroupItemClassParameterApplied()
    {
        var cut = Ctx.Render<RadioGroupItem>(p =>
        {
            p.Add(x => x.Value, "option1");
            p.Add(x => x.Class, "custom-item");
        });
        cut.Find("[data-slot='radio-group-item']").ClassList.ShouldContain("custom-item");
    }

    [Fact(DisplayName = "UAC14: RadioGroupItem renders with required when parent is required")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_RadioGroupItemRendersWithRequired()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Required, true);
            p.AddChildContent<RadioGroupItem>(i => i.Add(x => x.Value, "option1"));
        });
        var radio = cut.Find("input[type='radio']");
        radio.GetAttribute("required").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: Complete RadioGroup renders with items")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_CompleteRadioGroupRendersWithItems()
    {
        var cut = Ctx.Render<RadioGroupSut>(p =>
        {
            p.Add(x => x.Name, "test-group");
            p.Add(x => x.Value, "option2");
            p.AddChildContent<RadioGroupItem>(i =>
            {
                i.Add(x => x.Value, "option1");
                i.AddChildContent("Option 1");
            });
            p.AddChildContent<RadioGroupItem>(i =>
            {
                i.Add(x => x.Value, "option2");
                i.AddChildContent("Option 2");
            });
            p.AddChildContent<RadioGroupItem>(i =>
            {
                i.Add(x => x.Value, "option3");
                i.AddChildContent("Option 3");
            });
        });

        cut.Find("[data-slot='radio-group']").ShouldNotBeNull();
        var items = cut.FindAll("[data-slot='radio-group-item']");
        items.Count.ShouldBe(3);
        cut.Markup.ShouldContain("Option 1");
        cut.Markup.ShouldContain("Option 2");
        cut.Markup.ShouldContain("Option 3");
    }
}
