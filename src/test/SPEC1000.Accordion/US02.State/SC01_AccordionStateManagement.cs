using LowlandTech.Components.Accordion;

namespace LowlandTech.Components.Tests.SPEC1000.Accordion.US02.State;

[Trait(Spec.SPEC, "1000")]
[Trait(Spec.SC, "05")]
[UserStory("02", "Accordion state management")]
public class SC01_AccordionStateManagement
{
    [Fact(DisplayName = "UAC01: Initial state has no open items")]
    [Trait(Spec.UAC, "01")]
    [Then("no items are open")]
    public void UAC01_InitialStateHasNoOpenItems()
    {
        var state = new AccordionState();
        state.OpenItems.Count.ShouldBe(0);
        state.IsOpen("item-1").ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: Toggle opens an item")]
    [Trait(Spec.UAC, "02")]
    [Then("the item is open")]
    public void UAC02_ToggleOpensItem()
    {
        var state = new AccordionState();
        state.Toggle("item-1");
        state.IsOpen("item-1").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: Single mode — toggle closes current and opens new")]
    [Trait(Spec.UAC, "03")]
    [Then("only the new item is open")]
    public void UAC03_SingleModeOnlyOneOpen()
    {
        var state = new AccordionState { Type = "single", Collapsible = true };
        state.Toggle("item-1");
        state.Toggle("item-2");
        state.IsOpen("item-1").ShouldBeFalse();
        state.IsOpen("item-2").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: Single mode non-collapsible — cannot close the open item")]
    [Trait(Spec.UAC, "04")]
    [Then("the item stays open")]
    public void UAC04_SingleNonCollapsibleCannotClose()
    {
        var state = new AccordionState { Type = "single", Collapsible = false };
        state.Toggle("item-1");
        state.Toggle("item-1"); // try to close
        state.IsOpen("item-1").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC05: Single mode collapsible — can close the open item")]
    [Trait(Spec.UAC, "05")]
    [Then("the item closes")]
    public void UAC05_SingleCollapsibleCanClose()
    {
        var state = new AccordionState { Type = "single", Collapsible = true };
        state.Toggle("item-1");
        state.Toggle("item-1");
        state.IsOpen("item-1").ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC06: Multiple mode — multiple items can be open")]
    [Trait(Spec.UAC, "06")]
    [Then("both items are open")]
    public void UAC06_MultipleModeMultipleOpen()
    {
        var state = new AccordionState { Type = "multiple" };
        state.Toggle("item-1");
        state.Toggle("item-2");
        state.IsOpen("item-1").ShouldBeTrue();
        state.IsOpen("item-2").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC07: Multiple mode — toggle closes individual items")]
    [Trait(Spec.UAC, "07")]
    [Then("only the toggled item closes")]
    public void UAC07_MultipleModeToggleClosesIndividual()
    {
        var state = new AccordionState { Type = "multiple" };
        state.Toggle("item-1");
        state.Toggle("item-2");
        state.Toggle("item-1");
        state.IsOpen("item-1").ShouldBeFalse();
        state.IsOpen("item-2").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC08: OnChange fires on Toggle")]
    [Trait(Spec.UAC, "08")]
    [Then("change event is raised")]
    public void UAC08_OnChangeFiresOnToggle()
    {
        var state = new AccordionState();
        var fired = false;
        state.OnChange += () => fired = true;
        state.Toggle("item-1");
        fired.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: SetOpen opens item in single mode")]
    [Trait(Spec.UAC, "09")]
    [Then("the item is open")]
    public void UAC09_SetOpenOpensItem()
    {
        var state = new AccordionState { Type = "single" };
        state.SetOpen("item-1");
        state.IsOpen("item-1").ShouldBeTrue();
    }
}
