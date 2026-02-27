using Bunit;
using ScrollAreaSut = LowlandTech.Components.ScrollArea.ScrollArea;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1039.ScrollArea.US01.View;

[Trait(Spec.SPEC, "1039")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ScrollArea component")]
public class SC01_RenderScrollArea : ViewScenario
{
    [Fact(DisplayName = "UAC01: ScrollArea renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ScrollAreaSut>();
        cut.Find("[data-slot='scroll-area']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ScrollArea renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ScrollAreaSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: ScrollArea renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ScrollAreaSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='scroll-area']").ClassList.ShouldContain("test-class");
    }
}
