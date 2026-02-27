using Bunit;
using NativeSelectSut = LowlandTech.Components.NativeSelect.NativeSelect;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1032.NativeSelect.US01.View;

[Trait(Spec.SPEC, "1032")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render NativeSelect component")]
public class SC01_RenderNativeSelect : ViewScenario
{
    [Fact(DisplayName = "UAC01: NativeSelect renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<NativeSelectSut>();
        cut.Find("[data-slot='native-select']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: NativeSelect renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: NativeSelect renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='native-select']").ClassList.ShouldContain("test-class");
    }
}
