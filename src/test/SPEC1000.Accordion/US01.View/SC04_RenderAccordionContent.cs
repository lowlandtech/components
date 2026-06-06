using Bunit;
using AccordionSut = LowlandTech.Components.Accordion;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Render Accordion component")]
public class SC04_RenderAccordionContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: AccordionContent is hidden when item is closed")]
    [Trait(Spec.UAC, "01")]
    [Then("content is not in the DOM")]
    public void UAC01_HiddenWhenClosed()
    {
        var cut = RenderAccordionWithContent("item-1", "Secret Content");
        cut.Markup.ShouldNotContain("Secret Content");
    }

    [Fact(DisplayName = "UAC02: AccordionContent is visible when item is open via DefaultValue")]
    [Trait(Spec.UAC, "02")]
    [Then("content is visible")]
    public void UAC02_VisibleWhenOpen()
    {
        var cut = RenderAccordionWithContent("item-1", "Visible Content", defaultValue: "item-1");
        cut.Find("[data-slot='accordion-content']").ShouldNotBeNull();
        cut.Markup.ShouldContain("Visible Content");
    }

    [Fact(DisplayName = "UAC03: AccordionContent renders default CSS class when open")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains accordion-content class")]
    public void UAC03_RendersDefaultClassWhenOpen()
    {
        var cut = RenderAccordionWithContent("item-1", "Content", defaultValue: "item-1");
        cut.Find("[data-slot='accordion-content']").ClassList.ShouldContain("accordion-content");
    }

    [Fact(DisplayName = "UAC04: AccordionContent renders data-state open")]
    [Trait(Spec.UAC, "04")]
    [Then("data-state is open")]
    public void UAC04_RendersDataStateOpen()
    {
        var cut = RenderAccordionWithContent("item-1", "Content", defaultValue: "item-1");
        cut.Find("[data-slot='accordion-content']").GetAttribute("data-state").ShouldBe("open");
    }

    private IRenderedComponent<AccordionSut> RenderAccordionWithContent(string value, string content, string? defaultValue = null)
    {
        return Ctx.Render<AccordionSut>(p =>
        {
            if (defaultValue != null)
                p.Add(x => x.DefaultValue, defaultValue);

            p.AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", value);
                builder.AddAttribute(2, "ChildContent",
                    (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    {
                        b.OpenComponent<AccordionContent>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, content)));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            });
        });
    }
}
