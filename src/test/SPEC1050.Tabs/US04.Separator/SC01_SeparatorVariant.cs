using Bunit;
using LowlandTech.Components;
using TabsSut = LowlandTech.Components.Tabs;
using TabsListSut = LowlandTech.Components.TabsList;

namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US04.Separator;

/// <summary>
/// US04 — The separator variant.
///
/// A short strip of a few names reads like a segmented switch when the list draws its tabs as
/// small buttons with a vertical rule between them. The variant is opt-in; every other
/// variant keeps the classes it had.
/// </summary>
[Trait(Spec.SPEC, "1050")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "Separator variant")]
public class SC01_SeparatorVariant : ViewScenario
{
    private const string SeparatorClass = "cn-tabs-list-seperator";

    private IRenderedComponent<TabsSut> RenderListWithVariant(string variant)
        => Ctx.Render<TabsSut>(p => p.AddChildContent<TabsListSut>(list => list
            .Add(x => x.Variant, variant)));

    [Fact(DisplayName = "UAC01: Variant='separator' adds the separator class")]
    [Trait(Spec.UAC, "01")]
    [Then("the list carries cn-tabs-list-seperator")]
    public void UAC01_SeparatorVariantAddsClass()
    {
        var cut = RenderListWithVariant("separator");

        cut.Find("[data-slot='tabs-list']").ClassList.ShouldContain(SeparatorClass);
    }

    [Fact(DisplayName = "UAC02: the application's 'seperator' spelling adds the same class")]
    [Trait(Spec.UAC, "02")]
    [Then("the list carries cn-tabs-list-seperator")]
    public void UAC02_MisspelledVariantAddsClass()
    {
        var cut = RenderListWithVariant("seperator");

        cut.Find("[data-slot='tabs-list']").ClassList.ShouldContain(SeparatorClass);
    }

    [Fact(DisplayName = "UAC03: another variant does not add the separator class")]
    [Trait(Spec.UAC, "03")]
    [Then("the list does not carry cn-tabs-list-seperator")]
    public void UAC03_OtherVariantKeepsItsClasses()
    {
        var cut = RenderListWithVariant("line");

        cut.Find("[data-slot='tabs-list']").ClassList.ShouldNotContain(SeparatorClass);
    }
}
