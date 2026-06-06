using Bunit;
using AccordionSut = LowlandTech.Components.Accordion;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Accessibility attributes")]
public class SC07_AccessibilityAttributes : ViewScenario
{
    [Fact(DisplayName = "UAC01: AccordionTrigger has aria-expanded false when closed")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TriggerHasAriaExpandedFalseWhenClosed()
    {
        var cut = RenderAccordionWithItem();

        var trigger = cut.Find("[data-slot='accordion-trigger']");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC02: AccordionTrigger has aria-expanded true when open")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TriggerHasAriaExpandedTrueWhenOpen()
    {
        var cut = RenderAccordionWithItem(defaultValue: "item-1");

        var trigger = cut.Find("[data-slot='accordion-trigger']");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC03: AccordionTrigger has id attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_TriggerHasIdAttribute()
    {
        var cut = RenderAccordionWithItem();

        var trigger = cut.Find("[data-slot='accordion-trigger']");
        var id = trigger.GetAttribute("id");
        id.ShouldNotBeNull();
        id.ShouldStartWith("accordion-trigger-");
    }

    [Fact(DisplayName = "UAC04: AccordionTrigger has aria-controls pointing to content id when open")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_TriggerHasAriaControlsWhenOpen()
    {
        var cut = RenderAccordionWithItem(defaultValue: "item-1");

        var trigger = cut.Find("[data-slot='accordion-trigger']");
        var ariaControls = trigger.GetAttribute("aria-controls");
        ariaControls.ShouldNotBeNull();
        ariaControls.ShouldStartWith("accordion-content-");

        // Verify content has matching id
        var content = cut.Find("[data-slot='accordion-content']");
        content.GetAttribute("id").ShouldBe(ariaControls);
    }

    [Fact(DisplayName = "UAC05: AccordionContent has aria-labelledby pointing to trigger id")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ContentHasAriaLabelledBy()
    {
        var cut = RenderAccordionWithItem(defaultValue: "item-1");

        var trigger = cut.Find("[data-slot='accordion-trigger']");
        var triggerId = trigger.GetAttribute("id");

        var content = cut.Find("[data-slot='accordion-content']");
        content.GetAttribute("aria-labelledby").ShouldBe(triggerId);
    }

    [Fact(DisplayName = "UAC06: AccordionContent has role region")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ContentHasRoleRegion()
    {
        var cut = RenderAccordionWithItem(defaultValue: "item-1");

        var content = cut.Find("[data-slot='accordion-content']");
        content.GetAttribute("role").ShouldBe("region");
    }

    [Fact(DisplayName = "UAC07: AccordionContent has data-state closed when collapsed")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ContentHasDataStateClosedWhenCollapsed()
    {
        var cut = RenderAccordionWithItem();

        var content = cut.Find("[data-slot='accordion-content']");
        content.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC08: AccordionHeader renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_AccordionHeaderRendersDataSlot()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<AccordionItem>(0);
            builder.AddAttribute(1, "Value", "item-1");
            builder.AddAttribute(2, "ChildContent",
                (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<AccordionHeader>(0);
                    b.AddAttribute(1, "ChildContent",
                        (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                        {
                            b2.OpenComponent<AccordionTrigger>(0);
                            b2.AddAttribute(1, "ChildContent",
                                (Microsoft.AspNetCore.Components.RenderFragment)(b3 =>
                                    b3.AddContent(0, "Trigger")));
                            b2.CloseComponent();
                        }));
                    b.CloseComponent();
                }));
            builder.CloseComponent();
        }));

        cut.Find("[data-slot='accordion-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: AccordionHeader renders as h3 element")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_AccordionHeaderRendersAsH3()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<AccordionItem>(0);
            builder.AddAttribute(1, "Value", "item-1");
            builder.AddAttribute(2, "ChildContent",
                (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                {
                    b.OpenComponent<AccordionHeader>(0);
                    b.AddAttribute(1, "ChildContent",
                        (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                        {
                            b2.OpenComponent<AccordionTrigger>(0);
                            b2.AddAttribute(1, "ChildContent",
                                (Microsoft.AspNetCore.Components.RenderFragment)(b3 =>
                                    b3.AddContent(0, "Trigger")));
                            b2.CloseComponent();
                        }));
                    b.CloseComponent();
                }));
            builder.CloseComponent();
        }));

        cut.Find("[data-slot='accordion-header']").TagName.ShouldBe("H3");
    }

    private IRenderedComponent<AccordionSut> RenderAccordionWithItem(string? defaultValue = null)
    {
        return Ctx.Render<AccordionSut>(p =>
        {
            if (defaultValue != null)
                p.Add(x => x.DefaultValue, defaultValue);

            p.AddChildContent(builder =>
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
                                b2.AddContent(0, "Content")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            });
        });
    }
}
