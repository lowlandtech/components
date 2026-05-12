using Bunit;
using LowlandTech.Components.Card;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Render Card component")]
public class SC04_RenderCardDescription : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardDescription renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardDescription>();
        cut.Find("[data-slot='card-description']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CardDescription renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardDescription>(p => p.AddChildContent("<span>Description</span>"));
        cut.Markup.ShouldContain("Description");
    }

    [Fact(DisplayName = "UAC03: CardDescription renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains card-description class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<CardDescription>();
        cut.Find("[data-slot='card-description']").ClassList.ShouldContain("card-description");
    }

    [Fact(DisplayName = "UAC04: CardDescription merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<CardDescription>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='card-description']");
        el.ClassList.ShouldContain("card-description");
        el.ClassList.ShouldContain("custom");
    }
}
