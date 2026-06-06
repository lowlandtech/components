using Bunit;
using AccordionSut = LowlandTech.Components.Accordion;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Accordion controlled state")]
public class SC06_AccordionControlledState : ViewScenario
{
    [Fact(DisplayName = "UAC01: Value parameter sets controlled state")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ValueParameterSetsControlledState()
    {
        var cut = Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Value, "item-1")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.AddAttribute(2, "ChildContent",
                    (Microsoft.AspNetCore.Components.RenderFragment)(b =>
                    {
                        b.OpenComponent<AccordionContent>(0);
                        b.AddAttribute(1, "ChildContent",
                            (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                b2.AddContent(0, "Content 1")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));

        cut.Markup.ShouldContain("Content 1");
        cut.Find("[data-slot='accordion-item']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC02: ValueChanged fires when item toggled")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ValueChangedFiresWhenItemToggled()
    {
        string? changedValue = null;
        var cut = Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Value, (string?)null)
            .Add(x => x.ValueChanged, (string? val) => changedValue = val)
            .Add(x => x.Collapsible, true)
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
                                b2.AddContent(0, "Content")));
                        b.CloseComponent();
                    }));
                builder.CloseComponent();
            }));

        cut.Find("[data-slot='accordion-trigger']").Click();

        changedValue.ShouldBe("item-1");
    }

    [Fact(DisplayName = "UAC03: Value takes precedence over DefaultValue")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ValueTakesPrecedenceOverDefaultValue()
    {
        var cut = Ctx.Render<AccordionSut>(p => p
            .Add(x => x.Value, "item-2")
            .Add(x => x.DefaultValue, "item-1")
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
                            b.OpenComponent<AccordionContent>(0);
                            b.AddAttribute(1, "ChildContent",
                                (Microsoft.AspNetCore.Components.RenderFragment)(b2 =>
                                    b2.AddContent(0, $"Content {idx}")));
                            b.CloseComponent();
                        }));
                    builder.CloseComponent();
                }
            }));

        cut.Markup.ShouldNotContain("Content 1");
        cut.Markup.ShouldContain("Content 2");
    }
}
