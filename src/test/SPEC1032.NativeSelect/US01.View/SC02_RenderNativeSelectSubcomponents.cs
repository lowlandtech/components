using Bunit;
using LowlandTech.Components.NativeSelect;

namespace LowlandTech.Components.Tests.SPEC1032.NativeSelect.US01.View;

[Trait(Spec.SPEC, "1032")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render NativeSelect subcomponents")]
public class SC02_RenderNativeSelectSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: NativeSelectOption renders data-slot")]
    [Trait(Spec.UAC, "01")]
    [Then("option element has data-slot='native-select-option'")]
    public void UAC01_NativeSelectOption()
    {
        var cut = Ctx.Render<NativeSelectOption>();
        cut.Find("[data-slot='native-select-option']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: NativeSelectOption renders ChildContent as option text")]
    [Trait(Spec.UAC, "02")]
    [Then("option contains the child text")]
    public void UAC02_NativeSelectOptionChildContent()
    {
        var cut = Ctx.Render<NativeSelectOption>(p => p.AddChildContent("Choice A"));
        cut.Markup.ShouldContain("Choice A");
    }

    [Fact(DisplayName = "UAC03: NativeSelectOptGroup renders data-slot")]
    [Trait(Spec.UAC, "03")]
    [Then("optgroup element has data-slot='native-select-optgroup'")]
    public void UAC03_NativeSelectOptGroup()
    {
        var cut = Ctx.Render<NativeSelectOptGroup>();
        cut.Find("[data-slot='native-select-optgroup']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: NativeSelectOptGroup renders Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("optgroup has the supplied CSS class")]
    public void UAC04_NativeSelectOptGroupClass()
    {
        var cut = Ctx.Render<NativeSelectOptGroup>(p => p.Add(x => x.Class, "my-group"));
        cut.Find("[data-slot='native-select-optgroup']").ClassList.ShouldContain("my-group");
    }
}
