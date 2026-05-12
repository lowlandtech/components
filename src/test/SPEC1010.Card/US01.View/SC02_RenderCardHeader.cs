using Bunit;
using LowlandTech.Components.Card;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Card component")]
public class SC02_RenderCardHeader : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardHeader>();
        cut.Find("[data-slot='card-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CardHeader renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardHeader>(p => p.AddChildContent("<span>Header</span>"));
        cut.Markup.ShouldContain("Header");
    }

    [Fact(DisplayName = "UAC03: CardHeader renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains card-header class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<CardHeader>();
        cut.Find("[data-slot='card-header']").ClassList.ShouldContain("card-header");
    }

    [Fact(DisplayName = "UAC04: CardHeader merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<CardHeader>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='card-header']");
        el.ClassList.ShouldContain("card-header");
        el.ClassList.ShouldContain("custom");
    }
}
