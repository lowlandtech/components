using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1068.Designable.US01.Registry;

// ── Fake components used by the SC* drift guards in this folder. They
// live in the test assembly so the registry's discovery sweep picks
// them up alongside the real components — exercising the "any
// AppDomain assembly is fair game" contract.

public enum FixtureSize { Small, Medium, Large }

[Properties(Group = "Fixtures", Icon = "test-tube", Description = "Test fixture button")]
public partial class FixtureButton : LtComponentBase
{
    [Parameter, PropertyGrid(Group = "Appearance", Editor = PropertyEditor.Select,
        Options = new[] { "default", "destructive", "outline" })]
    public string Variant { get; set; } = "default";

    [Parameter, PropertyGrid(Group = "Appearance")]
    public FixtureSize Size { get; set; } = FixtureSize.Medium;

    [Parameter, PropertyGrid(Group = "State")]
    public bool Disabled { get; set; }

    [Parameter, PropertyGrid(Group = "State", Editor = PropertyEditor.Slider,
        Min = 0, Max = 100, Step = 5)]
    public int Opacity { get; set; } = 100;

    [Parameter, PropertyGrid(Group = "Appearance", Label = "Custom Label",
        Placeholder = "Type here…", Description = "Tooltip text", Order = -1)]
    public string FontSize { get; set; } = "16px";

    // Plain [Parameter] without [PropertyGrid] — must NOT appear in the
    // descriptor's Properties list. Acts as a regression guard against
    // accidental leakage.
    [Parameter] public string? UnregisteredOverride { get; set; }

    // Plain CLR property without [Parameter] — likewise must NOT appear.
    public string InternalCalc { get; set; } = string.Empty;
}

[Properties(Group = "Fixtures", Element = "fixture-card-explicit")]
public partial class FixtureCardWithExplicitElement : LtComponentBase
{
    [Parameter, PropertyGrid] public string Title { get; set; } = "Untitled";
}

// Component without [Properties] — used to assert the registry SKIPS
// undecorated components even if they inherit LtComponentBase.
public partial class UndecoratedFixture : LtComponentBase
{
    [Parameter] public string Name { get; set; } = string.Empty;
}
