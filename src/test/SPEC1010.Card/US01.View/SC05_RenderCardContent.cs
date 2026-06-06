using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Render Card component")]
public class SC05_RenderCardContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardContent renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardContent>();
        cut.Find("[data-slot='card-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CardContent renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardContent>(p => p.AddChildContent("<span>Content</span>"));
        cut.Markup.ShouldContain("Content");
    }

    [Fact(DisplayName = "UAC03: CardContent renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains card-content class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = Ctx.Render<CardContent>();
        cut.Find("[data-slot='card-content']").ClassList.ShouldContain("card-content");
    }

    [Fact(DisplayName = "UAC04: CardContent merges custom Class parameter")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains both default and custom classes")]
    public void UAC04_MergesCustomClass()
    {
        var cut = Ctx.Render<CardContent>(p => p.Add(x => x.Class, "custom"));
        var el = cut.Find("[data-slot='card-content']");
        el.ClassList.ShouldContain("card-content");
        el.ClassList.ShouldContain("custom");
    }
}
