using Bunit;
using DialogSut = LowlandTech.Components.Dialog.Dialog;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1018.Dialog.US01.View;

[Trait(Spec.SPEC, "1018")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Dialog component")]
public class SC01_RenderDialog : ViewScenario
{
    [Fact(DisplayName = "UAC01: Dialog renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DialogSut>();
        cut.Find("[data-slot='dialog']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Dialog renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<DialogSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Dialog renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<DialogSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='dialog']").ClassList.ShouldContain("test-class");
    }
}
