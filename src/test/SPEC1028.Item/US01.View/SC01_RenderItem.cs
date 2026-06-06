using Bunit;
using ItemSut = LowlandTech.Components.Item;

namespace LowlandTech.Components.Tests.SPEC1028.Item.US01.View;

[Trait(Spec.SPEC, "1028")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Item component")]
public class SC01_RenderItem : ViewScenario
{
    [Fact(DisplayName = "UAC01: Item renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ItemSut>();
        cut.Find("[data-slot='item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Item renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ItemSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Item renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ItemSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='item']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Item renders variant parameter")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersVariant()
    {
        var cut = Ctx.Render<ItemSut>(p => p.Add(x => x.Variant, "primary"));
        cut.Find("[data-slot='item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Item renders size parameter")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersSize()
    {
        var cut = Ctx.Render<ItemSut>(p => p.Add(x => x.Size, "lg"));
        cut.Find("[data-slot='item']").ShouldNotBeNull();
    }
}
