using Bunit;
using LowlandTech.Components.Card;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Render Card component")]
public class SC06_RenderCardFooter : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardFooter renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardFooter>();
        cut.Find("[data-slot='card-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CardFooter renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardFooter>(p => p.AddChildContent("<span>Footer</span>"));
        cut.Markup.ShouldContain("Footer");
    }

    [Fact(DisplayName = "UAC03: CardFooter renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains card-footer class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<CardFooter>();
        cut.Find("[data-slot='card-footer']").ClassList.ShouldContain("card-footer");
    }

    [Fact(DisplayName = "UAC04: CardFooter merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<CardFooter>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='card-footer']");
        el.ClassList.ShouldContain("card-footer");
        el.ClassList.ShouldContain("custom");
    }
}
