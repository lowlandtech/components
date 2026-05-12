using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1068.Designable.US01.Registry;

/// <summary>
/// SC02 — Per-parameter descriptor extraction. Asserts the registry
/// reads each <see cref="PropertyGridAttribute"/> correctly, infers
/// the editor from the parameter type when none is specified, captures
/// default values, and groups properties for the Properties panel
/// renderer. SPEC2100.US45.
/// </summary>
[Trait(Spec.SPEC, "1068")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Per-property descriptors carry editor, group, options, defaults")]
public class SC02_PropertyDescriptorContract
{
    private static readonly IComponentDescriptorRegistry _registry = new ComponentDescriptorRegistry();
    private static readonly ComponentDescriptor _btn = _registry.GetByType(typeof(FixtureButton))!;

    private static PropertyDescriptor Prop(string name) =>
        _btn.Properties.First(p => p.Name == name);

    [Fact(DisplayName = "UAC10: only [Parameter, PropertyGrid] properties enter the descriptor set")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_OnlyDecoratedParameters()
    {
        var names = _btn.Properties.Select(p => p.Name).ToHashSet();
        names.ShouldContain("Variant");
        names.ShouldContain("Disabled");
        names.ShouldNotContain("UnregisteredOverride",
            customMessage: "Plain [Parameter] without [PropertyGrid] must not leak into the panel.");
        names.ShouldNotContain("InternalCalc",
            customMessage: "CLR-only properties (no [Parameter]) must not leak into the panel.");
    }

    [Fact(DisplayName = "UAC11: explicit Editor= on [PropertyGrid] is honoured")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ExplicitEditor()
    {

        Prop("Variant").Editor.ShouldBe(PropertyEditor.Select);
        Prop("Opacity").Editor.ShouldBe(PropertyEditor.Slider);
    }

    [Fact(DisplayName = "UAC12: bool parameters infer Switch editor")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_BoolInfersSwitch()
    {

        Prop("Disabled").Editor.ShouldBe(PropertyEditor.Switch);
    }

    [Fact(DisplayName = "UAC13: enum parameters infer Select editor with enum-name Options")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_EnumInfersSelect()
    {

        var size = Prop("Size");
        size.Editor.ShouldBe(PropertyEditor.Select);
        size.Options.ShouldContain("Small");
        size.Options.ShouldContain("Medium");
        size.Options.ShouldContain("Large");
    }

    [Fact(DisplayName = "UAC14: explicit Options= override the type-inferred set")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ExplicitOptions()
    {

        var variant = Prop("Variant");
        variant.Options.ShouldBe(new[] { "default", "destructive", "outline" });
    }

    [Fact(DisplayName = "UAC15: string parameters with no explicit Editor default to Text")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_StringDefaultsToText()
    {

        var card = _registry.GetByType(typeof(FixtureCardWithExplicitElement)).ShouldNotBeNull();
        var title = card.Properties.First(p => p.Name == "Title");
        title.Editor.ShouldBe(PropertyEditor.Text);
    }

    [Fact(DisplayName = "UAC16: Slider editor carries Min / Max / Step from the attribute")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_SliderRange()
    {

        var op = Prop("Opacity");
        op.Min.ShouldBe(0);
        op.Max.ShouldBe(100);
        op.Step.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC17: default values are captured from the parameter's initializer")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_DefaultsCaptured()
    {

        Prop("Variant").DefaultValue.ShouldBe("default");
        Prop("Opacity").DefaultValue.ShouldBe(100);
        Prop("Disabled").DefaultValue.ShouldBe(false);
    }

    [Fact(DisplayName = "UAC18: Label defaults to humanised property name (\"FontSize\" → \"Font size\")")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_LabelHumanised()
    {

        Prop("Disabled").Label.ShouldBe("Disabled");
        Prop("Size").Label.ShouldBe("Size");
        // FontSize has an explicit Label override on the fixture, so
        // assert the override wins over humanisation.
        Prop("FontSize").Label.ShouldBe("Custom Label");
    }

    [Fact(DisplayName = "UAC19: Group / Placeholder / Description / Order flow through verbatim")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_AttributeFieldsFlowThrough()
    {

        var fs = Prop("FontSize");
        fs.Group.ShouldBe("Appearance");
        fs.Placeholder.ShouldBe("Type here…");
        fs.Description.ShouldBe("Tooltip text");
        fs.Order.ShouldBe(-1);
    }

    [Fact(DisplayName = "UAC20: GroupedProperties returns one bucket per Group, sorted by Order then declaration")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_GroupedProperties()
    {

        var groups = _btn.GroupedProperties;
        var keys = groups.Select(g => g.Key).ToList();
        keys.ShouldContain("Appearance");
        keys.ShouldContain("State");

        // Within Appearance: FontSize has Order=-1 so it sorts before
        // Variant + Size (Order=0, declaration-index tiebreak).
        var appearance = groups.First(g => g.Key == "Appearance").Select(p => p.Name).ToList();
        appearance[0].ShouldBe("FontSize",
            customMessage:
                "Properties with explicit Order should render before Order=0 " +
                "siblings — without that contract a designer can't promote " +
                "the most-edited row to the top of its group.");
    }
}
