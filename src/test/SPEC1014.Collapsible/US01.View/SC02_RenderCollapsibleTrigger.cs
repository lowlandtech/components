using Bunit;
using LowlandTech.Components.Collapsible;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1014.Collapsible.US01.View;

[Trait(Spec.SPEC, "1014")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render CollapsibleTrigger component")]
public class SC02_RenderCollapsibleTrigger : ViewScenario
{
    [Fact(DisplayName = "UAC01: CollapsibleTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CollapsibleTrigger>();
        cut.Find("[data-slot='collapsible-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CollapsibleTrigger renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CollapsibleTrigger>(p => p.AddChildContent("Toggle"));
        cut.Markup.ShouldContain("Toggle");
    }

    [Fact(DisplayName = "UAC03: CollapsibleTrigger renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CollapsibleTrigger>(p => p.Add(x => x.Class, "trigger-class"));
        cut.Find("[data-slot='collapsible-trigger']").ClassList.ShouldContain("trigger-class");
    }
}
