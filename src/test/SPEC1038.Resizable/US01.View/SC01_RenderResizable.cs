using Bunit;
using ResizableSut = LowlandTech.Components.Resizable.Resizable;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US01.View;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Resizable component")]
public class SC01_RenderResizable : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resizable renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ResizableSut>();
        cut.Find("[data-slot='resizable-panel-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Resizable renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ResizableSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Resizable renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ResizableSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='resizable-panel-group']").ClassList.ShouldContain("test-class");
    }
}
