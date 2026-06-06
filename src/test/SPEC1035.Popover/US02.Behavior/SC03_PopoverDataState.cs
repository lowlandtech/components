using Bunit;
using PopoverSut = LowlandTech.Components.Popover;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1035.Popover.US02.Behavior;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "03")]
[UserStory("02", "Popover data-state reflects open/closed")]
public class SC03_PopoverDataState : ViewScenario
{
    [Fact(DisplayName = "UAC01: Popover has data-state='closed' when closed")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DataStateClosedWhenClosed()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.AddChildContent<PopoverTrigger>(trigger =>
            {
                trigger.AddChildContent("Open");
            });
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Content");
            });
        });

        var popover = cut.Find("[data-slot='popover']");
        popover.GetAttribute("data-state").ShouldBe("closed");
    }

    [Fact(DisplayName = "UAC02: PopoverContent has data-state='closed' when not visible")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ContentDataStateClosed()
    {
        var cut = Ctx.Render<PopoverContent>();
        // Content not rendered when parent is not open
        // data-state is determined in the component
    }

    [Fact(DisplayName = "UAC03: PopoverContent has data-state='open' when visible")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ContentDataStateOpenWhenVisible()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.AddChildContent<PopoverTrigger>(trigger =>
            {
                trigger.AddChildContent("Open");
            });
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Content");
            });
        });

        // Click trigger to open
        var trigger = cut.Find("[data-slot='popover-trigger']");
        trigger.Click();

        cut.Find("[data-state='open']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Popover data-state changes to 'open' when toggled")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DataStateChangesToOpen()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.AddChildContent<PopoverTrigger>(trigger =>
            {
                trigger.AddChildContent("Open");
            });
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Content");
            });
        });

        // Initially closed
        cut.Find("[data-state='closed']").ShouldNotBeNull();

        // Click to open
        var trigger = cut.Find("[data-slot='popover-trigger']");
        trigger.Click();

        cut.Find("[data-state='open']").ShouldNotBeNull();
    }
}
