using Bunit;
using AccordionSut = LowlandTech.Components.Accordion.Accordion;
using LowlandTech.Components.Accordion;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US01.View;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Accordion component")]
public class SC02_RenderAccordionItem : ViewScenario
{
    [Fact(DisplayName = "UAC01: AccordionItem renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<AccordionItem>(0);
            builder.AddAttribute(1, "Value", "item-1");
            builder.CloseComponent();
        }));
        cut.Find("[data-slot='accordion-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: AccordionItem renders default CSS class")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains accordion-item class")]
    public void UAC02_RendersDefaultClass()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<AccordionItem>(0);
            builder.AddAttribute(1, "Value", "item-1");
            builder.CloseComponent();
        }));
        cut.Find("[data-slot='accordion-item']").ClassList.ShouldContain("accordion-item");
    }

    [Fact(DisplayName = "UAC03: AccordionItem renders data-state closed by default")]
    [Trait(Spec.UAC, "03")]
    [Then("data-state is closed")]
    public void UAC03_RendersClosedByDefault()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<AccordionItem>(0);
            builder.AddAttribute(1, "Value", "item-1");
            builder.CloseComponent();
        }));
        cut.Find("[data-slot='accordion-item']").GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC04: AccordionItem renders data-state open when DefaultValue matches")]
    [Trait(Spec.UAC, "04")]
    [Then("data-state is open")]
    public void UAC04_RendersOpenWhenDefaultValueMatches()
    {
        var cut = Ctx.Render<AccordionSut>(p => p
            .Add(x => x.DefaultValue, "item-1")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Value", "item-1");
                builder.CloseComponent();
            }));
        cut.Find("[data-slot='accordion-item']").GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC05: AccordionItem renders data-disabled when disabled")]
    [Trait(Spec.UAC, "05")]
    [Then("data-disabled is present")]
    public void UAC05_RendersDisabledAttribute()
    {
        var cut = Ctx.Render<AccordionSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<AccordionItem>(0);
            builder.AddAttribute(1, "Value", "item-1");
            builder.AddAttribute(2, "Disabled", true);
            builder.CloseComponent();
        }));
        cut.Find("[data-slot='accordion-item']").GetAttribute("data-disabled").ShouldBe("true");
    }
}
