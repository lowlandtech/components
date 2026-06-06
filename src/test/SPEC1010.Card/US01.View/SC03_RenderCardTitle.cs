using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Render Card component")]
public class SC03_RenderCardTitle : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardTitle renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardTitle>();
        cut.Find("[data-slot='card-title']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CardTitle renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardTitle>(p => p.AddChildContent("<span>Title</span>"));
        cut.Markup.ShouldContain("Title");
    }

    [Fact(DisplayName = "UAC03: CardTitle renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains card-title class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<CardTitle>();
        cut.Find("[data-slot='card-title']").ClassList.ShouldContain("card-title");
    }

    [Fact(DisplayName = "UAC04: CardTitle merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<CardTitle>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='card-title']");
        el.ClassList.ShouldContain("card-title");
        el.ClassList.ShouldContain("custom");
    }
}
