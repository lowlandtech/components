using Bunit;
using LowlandTech.Components.Card;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Render Card component")]
public class SC07_RenderCardAction : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardAction renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardAction>();
        cut.Find("[data-slot='card-action']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CardAction renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardAction>(p => p.AddChildContent("<span>Action</span>"));
        cut.Markup.ShouldContain("Action");
    }

    [Fact(DisplayName = "UAC03: CardAction renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains card-action class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<CardAction>();
        cut.Find("[data-slot='card-action']").ClassList.ShouldContain("card-action");
    }

    [Fact(DisplayName = "UAC04: CardAction merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<CardAction>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='card-action']");
        el.ClassList.ShouldContain("card-action");
        el.ClassList.ShouldContain("custom");
    }
}
