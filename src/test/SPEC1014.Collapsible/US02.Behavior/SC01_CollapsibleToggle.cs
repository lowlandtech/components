using Bunit;
using CollapsibleSut = LowlandTech.Components.Collapsible;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1014.Collapsible.US02.Behavior;

[Trait(Spec.SPEC, "1014")]
[Trait(Spec.SC, "04")]
[UserStory("02", "Collapsible toggle behavior")]
public class SC01_CollapsibleToggle : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking trigger toggles content visibility")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ClickTriggerTogglesContent()
    {
        var cut = Ctx.Render<CollapsibleSut>(p => 
        {
            p.AddChildContent<CollapsibleTrigger>(tp => tp.AddChildContent("Toggle"));
            p.AddChildContent<CollapsibleContent>(cp => cp.AddChildContent("Content"));
        });
        
        // Initially closed
        var content = cut.Find("[data-slot='collapsible-content']");
        content.ClassList.ShouldContain("grid-rows-[0fr]");
        
        // Click trigger
        cut.Find("[data-slot='collapsible-trigger']").Click();
        
        // Content should be expanded
        content = cut.Find("[data-slot='collapsible-content']");
        content.ClassList.ShouldContain("grid-rows-[1fr]");
    }

    [Fact(DisplayName = "UAC02: IsOpenChanged event is fired on toggle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_IsOpenChangedFiredOnToggle()
    {
        bool eventFired = false;
        bool newValue = false;
        
        var cut = Ctx.Render<CollapsibleSut>(p => 
        {
            p.Add(x => x.IsOpenChanged, (bool val) => { eventFired = true; newValue = val; });
            p.AddChildContent<CollapsibleTrigger>();
        });
        
        cut.Find("[data-slot='collapsible-trigger']").Click();
        
        eventFired.ShouldBeTrue();
        newValue.ShouldBeTrue();
    }
}
