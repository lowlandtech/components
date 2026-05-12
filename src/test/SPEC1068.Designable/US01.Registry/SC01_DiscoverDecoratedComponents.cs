using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1068.Designable.US01.Registry;

/// <summary>
/// SC01 — The reflection-based ComponentDescriptorRegistry discovers
/// every <see cref="PropertiesAttribute"/>-decorated component in
/// every loaded assembly, captures its class metadata, and skips
/// undecorated components even when they inherit LtComponentBase.
/// SPEC2100.US45 (component descriptor registry).
/// </summary>
[Trait(Spec.SPEC, "1068")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Registry discovers [Properties]-decorated components")]
public class SC01_DiscoverDecoratedComponents
{
    private static IComponentDescriptorRegistry Registry() => new ComponentDescriptorRegistry();

    [Fact(DisplayName = "UAC01: All exposes at least one descriptor (the FixtureButton in this assembly)")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DiscoversFixture()
    {
        var registry = Registry();
        registry.All.Count.ShouldBeGreaterThan(0);
        registry.All.ShouldContain(d => d.ComponentType == typeof(FixtureButton));
    }

    [Fact(DisplayName = "UAC02: undecorated LtComponentBase subclasses do NOT leak into the registry")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_UndecoratedSkipped()
    {
        Registry().All.ShouldNotContain(d => d.ComponentType == typeof(UndecoratedFixture),
            customMessage:
                "UndecoratedFixture inherits LtComponentBase but has no " +
                "[Properties] attribute — it must be excluded from the " +
                "registry. Otherwise the Components palette pollutes with " +
                "every helper component the lib ships.");
    }

    [Fact(DisplayName = "UAC03: class metadata flows from [Properties]")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ClassMetadata()
    {
        var d = Registry().GetByType(typeof(FixtureButton)).ShouldNotBeNull();
        d.Group.ShouldBe("Fixtures");
        d.Icon.ShouldBe("test-tube");
        d.Description.ShouldBe("Test fixture button");
    }

    [Fact(DisplayName = "UAC04: Element defaults to the component class name when not overridden")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ElementDefaultsToClassName()
    {
        Registry().GetByType(typeof(FixtureButton))!.Element.ShouldBe("FixtureButton");
    }

    [Fact(DisplayName = "UAC05: Element honours an explicit override on [Properties]")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ElementHonoursOverride()
    {
        Registry().GetByType(typeof(FixtureCardWithExplicitElement))!.Element.ShouldBe("fixture-card-explicit");
    }

    [Fact(DisplayName = "UAC06: GetByElement is case-insensitive and resolves the descriptor")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_GetByElementCaseInsensitive()
    {
        var registry = Registry();
        registry.GetByElement("fixturebutton").ShouldNotBeNull();
        registry.GetByElement("FixtureButton").ShouldNotBeNull();
        registry.GetByElement("FIXTUREBUTTON").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: GetByElement returns null for null/empty/unknown elements")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_GetByElementMisses()
    {
        var registry = Registry();
        registry.GetByElement(null).ShouldBeNull();
        registry.GetByElement("").ShouldBeNull();
        registry.GetByElement("not-a-real-component").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC08: GetByGroup returns every descriptor whose Group matches")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_GetByGroup()
    {
        var fixtures = Registry().GetByGroup("Fixtures");
        fixtures.ShouldContain(d => d.ComponentType == typeof(FixtureButton));
        fixtures.ShouldContain(d => d.ComponentType == typeof(FixtureCardWithExplicitElement));
    }

    [Fact(DisplayName = "UAC09: All is sorted by Group then Element (stable presentation order)")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_StableOrder()
    {
        var fixtures = Registry().All
            .Where(d => d.Group == "Fixtures")
            .Select(d => d.Element)
            .ToList();
        // Within Fixtures group, alpha-sorted by element name.
        fixtures.ShouldBe(fixtures.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList());
    }
}
