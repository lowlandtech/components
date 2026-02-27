using Bunit;
using ProgressSut = LowlandTech.Components.Progress.Progress;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1036.Progress.US01.View;

[Trait(Spec.SPEC, "1036")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Progress component")]
public class SC01_RenderProgress : ViewScenario
{
    [Fact(DisplayName = "UAC01: Progress renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-slot='progress']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Progress renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Progress renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='progress']").ClassList.ShouldContain("test-class");
    }
}
