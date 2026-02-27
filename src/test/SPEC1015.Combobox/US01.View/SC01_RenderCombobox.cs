using Bunit;
using ComboboxSut = LowlandTech.Components.Combobox.Combobox;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1015.Combobox.US01.View;

[Trait(Spec.SPEC, "1015")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Combobox component")]
public class SC01_RenderCombobox : ViewScenario
{
    [Fact(DisplayName = "UAC01: Combobox renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ComboboxSut>();
        cut.Find("[data-slot='combobox']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Combobox renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ComboboxSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Combobox renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ComboboxSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='combobox']").ClassList.ShouldContain("test-class");
    }
}
