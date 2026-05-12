using Bunit;
using AccordionSut = LowlandTech.Components.Accordion.Accordion;
using LowlandTech.Components.Accordion;

namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Render Accordion component")]
public class SC05_AccordionBehavior : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking trigger opens accordion content")]
    [Trait(Spec.UAC, "01")]
    [Then("content becomes visible")]
    public void UAC01_ClickTriggerOpensContent()
    {
        var cut = RenderFullAccordion("single", collapsible: true);

        cut.Markup.ShouldNotContain("Content 1");

        cut.Find("[data-slot='accordion-trigger']").Click();

        cut.Find("[data-slot='accordion-content']").ShouldNotBeNull();
        cut.Markup.ShouldContain("Content 1");
    }

    [Fact(DisplayName = "UAC02: Clicking trigger again closes content in collapsible mode")]
    [Trait(Spec.UAC, "02")]
    [Then("content is hidden again")]
    public void UAC02_ClickAgainClosesInCollapsibleMode()
    {
        var cut = RenderFullAccordion("single", collapsible: true);

        var trigger = cut.Find("[data-slot='accordion-trigger']");
        trigger.Click(); // open
        cut.Markup.ShouldContain("Content 1");

        trigger = cut.Find("[data-slot='accordion-trigger']");
        trigger.Click(); // close
        cut.Markup.ShouldNotContain("Content 1");
    }

    [Fact(DisplayName = "UAC03: Single mode — opening second item closes first")]
    [Trait(Spec.UAC, "03")]
    [Then("only the second item is open")]
    public void UAC03_SingleModeClosesOther()
    {
        var cut = RenderTwoItemAccordion("single", collapsible: true);

        var triggers = cut.FindAll("[data-slot='accordion-trigger']");
        triggers[0].Click(); // open item-1
        cut.Markup.ShouldContain("Content 1");

        triggers = cut.FindAll("[data-slot='accordion-trigger']");
        triggers[1].Click(); // open item-2
        cut.Markup.ShouldNotContain("Content 1");
        cut.Markup.ShouldContain("Content 2");
    }

    [Fact(DisplayName = "UAC04: Multiple mode — both items can be open")]
    [Trait(Spec.UAC, "04")]
    [Then("both items have visible content")]
    public void UAC04_MultipleModeAllowsBothOpen()
    {
        var cut = RenderTwoItemAccordion("multiple");

        var triggers = cut.FindAll("[data-slot='accordion-trigger']");
        triggers[0].Click();
        triggers = cut.FindAll("[data-slot='accordion-trigger']");
        triggers[1].Click();

        cut.Markup.ShouldContain("Content 1");
        cut.Markup.ShouldContain("Content 2");
    }

    [Fact(DisplayName = "UAC05: Disabled item trigger does not open content")]
    [Trait(Spec.UAC, "05")]
    [Then("content stays hidden")]
    public void UAC05_DisabledItemDoesNotOpen()
    {
        var cut = Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Collapsible, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.AddAttribute(2, "Disabled", true);
                builder.AddAttribute(3, "ChildContent",
                    (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    {
                        b.OpenComponent<AccordionTrigger>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Trigger 1")));
                        b.CloseComponent();
                        b.OpenComponent<AccordionContent>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Content 1")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));

        cut.Find("[data-slot='accordion-trigger']").Click();
        cut.Markup.ShouldNotContain("Content 1");
    }

    [Fact(DisplayName = "UAC06: DefaultValue opens item on initial render")]
    [Trait(Spec.UAC, "06")]
    [Then("item is open on first render")]
    public void UAC06_DefaultValueOpensOnRender()
    {
        var cut = Ctx.Render<AccordionSut>(p => p
            .Add(x => x.DefaultValue, "item-1")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.AddAttribute(2, "ChildContent",
                    (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    {
                        b.OpenComponent<AccordionTrigger>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Trigger")));
                        b.CloseComponent();
                        b.OpenComponent<AccordionContent>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Pre-opened Content")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));

        cut.Markup.ShouldContain("Pre-opened Content");
        cut.Find("[data-slot='accordion-item']").GetAttribute("data-state").ShouldBe("open");
        cut.Find("[data-slot='accordion-trigger']").GetAttribute("data-state").ShouldBe("open");
    }

    private IRenderedComponent<AccordionSut> RenderFullAccordion(string type, bool collapsible = false)
    {
        return Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Type, type)
            .Add(x => x.Collapsible, collapsible)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.AddAttribute(2, "ChildContent",
                    (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    {
                        b.OpenComponent<AccordionTrigger>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Trigger 1")));
                        b.CloseComponent();
                        b.OpenComponent<AccordionContent>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Content 1")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));
    }

    private IRenderedComponent<AccordionSut> RenderTwoItemAccordion(string type, bool collapsible = false)
    {
        return Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Type, type)
            .Add(x => x.Collapsible, collapsible)
            .AddChildContent(builder =>
            {
                for (var i = 1; i <= 2; i++)
                {
                    var idx = i;
                    builder.OpenComponent<AccordionItem>(i * 10);
                    builder.AddAttribute(i * 10 + 1, "Value", $"item-{idx}");
                    builder.AddAttribute(i * 10 + 2, "ChildContent",
                        (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                        {
                            b.OpenComponent<AccordionTrigger>(0);
                            b.AddAttribute(1, "ChildContent",
                                (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                    b2.AddContent(0, $"Trigger {idx}")));
                            b.CloseComponent();
                            b.OpenComponent<AccordionContent>(0);
                            b.AddAttribute(1, "ChildContent",
                                (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                    b2.AddContent(0, $"Content {idx}")));
                            b.CloseComponent();
                        }));
                    builder.CloseComponent();
                }
            }));
    }
}
