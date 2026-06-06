using Bunit;
using LowlandTech.Components;
using ToggleGroupSut = LowlandTech.Components.ToggleGroup;

namespace LowlandTech.Components.Tests.SPEC1053.ToggleGroup.US01.View;

[Trait(Spec.SPEC, "1053")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render ToggleGroupItem")]
public class SC02_RenderToggleGroupItem : ViewScenario
{
    [Fact(DisplayName = "UAC01: ToggleGroupItem renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("data-slot='toggle-group-item' is present")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ToggleGroupItem>();
        cut.Find("[data-slot='toggle-group-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ToggleGroupItem aria-pressed is false by default")]
    [Trait(Spec.UAC, "02")]
    [Then("aria-pressed defaults to false")]
    public void UAC02_DefaultAriaPressedFalse()
    {
        var cut = Ctx.Render<ToggleGroupItem>();
        cut.Find("[data-slot='toggle-group-item']").GetAttribute("aria-pressed").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC03: ToggleGroupItem inherits context variant from ToggleGroup")]
    [Trait(Spec.UAC, "03")]
    [Then("data-variant on item reflects the group's variant")]
    public void UAC03_InheritsGroupVariant()
    {
        var cut = Ctx.Render<ToggleGroupSut>(p => p
            .Add(x => x.variant, "outline")
            .AddChildContent<ToggleGroupItem>());
        cut.Find("[data-slot='toggle-group-item']").GetAttribute("data-variant").ShouldBe("outline");
    }

    [Fact(DisplayName = "UAC04: ToggleGroupItem has cn-toggle-group-item base class")]
    [Trait(Spec.UAC, "04")]
    [Then("the item button contains the cn-toggle-group-item class")]
    public void UAC04_HasBaseClass()
    {
        var cut = Ctx.Render<ToggleGroupItem>();
        cut.Find("[data-slot='toggle-group-item']").ClassList.ShouldContain("cn-toggle-group-item");
    }
}
