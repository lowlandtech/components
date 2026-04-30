using LowlandTech.Components;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1068.Designable.US01.Registry;

/// <summary>
/// SC03 — Production-component decoration drift guard. Asserts the
/// first wave of LowlandTech.Components types decorated with
/// <see cref="PropertiesAttribute"/> in SPEC2100.US45 step 3 still
/// appear in the descriptor registry. Removing the attribute (or
/// renaming the component class) without updating the canvas Properties
/// panel would silently strip the typed editor for that component —
/// users would see only the raw Attributes editor.
///
/// New components added to the registry should also be added to this
/// drift guard so the wave's contract stays explicit.
/// </summary>
[Trait(Spec.SPEC, "1068")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Production components ship with [Properties] decoration")]
public class SC03_ProductionComponentsAreDecorated
{
    private static readonly IComponentDescriptorRegistry _registry = new ComponentDescriptorRegistry();

    [Theory(DisplayName = "UAC21: each first-wave production component appears in the registry under its expected Group")]
    [Trait(Spec.UAC, "21")]
    [InlineData("Input",  "Forms")]
    [InlineData("Button", "Forms")]
    [InlineData("Select", "Forms")]
    [InlineData("Card",   "Layout")]
    [InlineData("Badge",  "Display")]
    public void UAC21_ComponentDiscovered(string element, string expectedGroup)
    {
        var d = _registry.GetByElement(element);
        d.ShouldNotBeNull(
            customMessage:
                $"Component '{element}' is missing from the descriptor " +
                "registry. Either the [Properties] attribute was removed, " +
                "the Element / class name changed, or the component was " +
                "moved out of LowlandTech.Components. The canvas " +
                "Properties panel would render the raw Attributes editor " +
                "for it instead of typed parameter rows.");
        d.Group.ShouldBe(expectedGroup,
            customMessage:
                $"Component '{element}' moved Group from '{expectedGroup}' " +
                "to something else — the Components palette will list it " +
                "under a new section, breaking user muscle memory. " +
                "Update SC03 if the move was intentional.");
    }

    [Theory(DisplayName = "UAC22: Forms-group components carry an Appearance/Variant Select editor")]
    [Trait(Spec.UAC, "22")]
    [InlineData("Input",  "Variant")]
    [InlineData("Button", "Variant")]
    public void UAC22_VariantIsSelectEditor(string element, string property)
    {
        var d = _registry.GetByElement(element).ShouldNotBeNull();
        var prop = d.Properties.FirstOrDefault(p => p.Name == property);
        prop.ShouldNotBeNull(
            customMessage: $"Property '{property}' missing from {element}.");
        prop.Editor.ShouldBe(PropertyEditor.Select,
            customMessage:
                $"{element}.{property} should render as a Select " +
                "dropdown — without explicit Options the user gets a " +
                "plain text input that accepts any string and silently " +
                "produces broken variant CSS.");
        prop.Options.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC23: Input.Disabled renders as Switch (bool inferred)")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_InputDisabledIsSwitch()
    {
        var d = _registry.GetByElement("Input").ShouldNotBeNull();
        var prop = d.Properties.First(p => p.Name == "Disabled");
        prop.Editor.ShouldBe(PropertyEditor.Switch);
        prop.Group.ShouldBe("State");
    }
}
