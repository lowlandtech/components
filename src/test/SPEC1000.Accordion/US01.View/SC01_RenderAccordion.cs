using Bunit;
using AccordionSut = LowlandTech.Components.Accordion;

namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Accordion component")]
public class SC01_RenderAccordion : ViewScenario
{
    [Fact(DisplayName = "UAC01: Accordion renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AccordionSut>();
        cut.Find("[data-slot='accordion']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Accordion renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Accordion renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='accordion']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Accordion renders default accordion CSS class")]
    [Trait(Spec.UAC, "04")]
    [Then("the rendered markup contains accordion class")]
    public void UAC04_RendersDefaultClass()
    {
        var cut = Ctx.Render<AccordionSut>();
        cut.Find("[data-slot='accordion']").ClassList.ShouldContain("accordion");
    }

    [Fact(DisplayName = "UAC05: Accordion renders data-type attribute")]
    [Trait(Spec.UAC, "05")]
    [Then("the rendered markup contains data-type")]
    public void UAC05_RendersDataType()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.Add(x => x.Type, "multiple"));
        cut.Find("[data-slot='accordion']").GetAttribute("data-type").ShouldBe("multiple");
    }

    [Fact(DisplayName = "UAC06: Accordion defaults to single type")]
    [Trait(Spec.UAC, "06")]
    [Then("data-type is single by default")]
    public void UAC06_DefaultsToSingleType()
    {
        var cut = Ctx.Render<AccordionSut>();
        cut.Find("[data-slot='accordion']").GetAttribute("data-type").ShouldBe("single");
    }
}
