using Bunit;
using CardSut = LowlandTech.Components.Card.Card;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Card component")]
public class SC01_RenderCard : ViewScenario
{
    [Fact(DisplayName = "UAC01: Card renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardSut>();
        cut.Find("[data-slot='card']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Card renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CardSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Card renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='card']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Card renders default card CSS class")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains card class")]
    public void UAC04_RendersDefaultCardClass()
    {
        var cut = Ctx.Render<CardSut>();
        cut.Find("[data-slot='card']").ClassList.ShouldContain("card");
    }

    [Fact(DisplayName = "UAC05: Card renders data-size attribute when Size is set")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains data-size attribute")]
    public void UAC05_RendersDataSizeAttribute()
    {
        var cut = Ctx.Render<CardSut>(p => p.Add(x => x.Size, "sm"));
        cut.Find("[data-slot='card']").GetAttribute("data-size").ShouldBe("sm");
    }
}
