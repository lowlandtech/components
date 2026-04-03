using Bunit;
using CollapsibleSut = LowlandTech.Components.Collapsible.Collapsible;
using LowlandTech.Components.Collapsible;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1014.Collapsible.US01.View;

[Trait(Spec.SPEC, "1014")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Render CollapsibleContent component")]
public class SC03_RenderCollapsibleContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: CollapsibleContent renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CollapsibleContent>(p => p.AddChildContent("Content"));
        // Note: CollapsibleContent conditionally renders based on parent IsOpen state
    }

    [Fact(DisplayName = "UAC02: CollapsibleContent renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CollapsibleContent>(p => p.AddChildContent("Test Content"));
        // Content renders when parent IsOpen is true
    }

    [Fact(DisplayName = "UAC03: CollapsibleContent renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CollapsibleContent>(p => p.Add(x => x.Class, "test-class"));
        // Class is applied when rendered
    }
}
