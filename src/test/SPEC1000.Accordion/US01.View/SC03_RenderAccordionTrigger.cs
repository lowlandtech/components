using Bunit;
using AccordionSut = LowlandTech.Components.Accordion.Accordion;
using LowlandTech.Components.Accordion;

namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Render Accordion component")]
public class SC03_RenderAccordionTrigger : ViewScenario
{
    [Fact(DisplayName = "UAC01: AccordionTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = RenderAccordionWithItem("item-1", "Trigger Text");
        cut.Find("[data-slot='accordion-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: AccordionTrigger renders as button element")]
    [Trait(Spec.UAC, "02")]
    [Then("the element is a button")]
    public void UAC02_RendersAsButton()
    {
        var cut = RenderAccordionWithItem("item-1", "Trigger Text");
        cut.Find("[data-slot='accordion-trigger']").TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC03: AccordionTrigger renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains accordion-trigger class")]
    public void UAC03_RendersDefaultClass()
    {
        var cut = RenderAccordionWithItem("item-1", "Trigger Text");
        cut.Find("[data-slot='accordion-trigger']").ClassList.ShouldContain("accordion-trigger");
    }

    [Fact(DisplayName = "UAC04: AccordionTrigger renders ChildContent")]
    [Trait(Spec.UAC, "04")]
    [Then("the trigger text is rendered")]
    public void UAC04_RendersChildContent()
    {
        var cut = RenderAccordionWithItem("item-1", "My Trigger");
        cut.Find("[data-slot='accordion-trigger']").TextContent.ShouldContain("My Trigger");
    }

    [Fact(DisplayName = "UAC05: AccordionTrigger data-state is closed by default")]
    [Trait(Spec.UAC, "05")]
    [Then("data-state is closed")]
    public void UAC05_DataStateClosedByDefault()
    {
        var cut = RenderAccordionWithItem("item-1", "Trigger");
        cut.Find("[data-slot='accordion-trigger']").GetAttribute("data-state").ShouldBe("closed");
    }

    private IRenderedComponent<AccordionSut> RenderAccordionWithItem(string value, string triggerText)
    {
        return Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Collapsible, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", value);
                builder.AddAttribute(2, "ChildContent",
                    (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    {
                        b.OpenComponent<AccordionTrigger>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, triggerText)));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));
    }
}
