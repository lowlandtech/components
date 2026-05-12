using Bunit;
using PopoverSut = LowlandTech.Components.Popover.Popover;
using LowlandTech.Components.Popover;

namespace LowlandTech.Components.Tests.SPEC1035.Popover.US02.Behavior;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Popover Modal mode traps focus")]
public class SC01_PopoverModal : ViewScenario
{
    [Fact(DisplayName = "UAC01: Popover Modal mode adds overlay when open")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ModalAddsOverlay()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.Modal, true);
            p.AddChildContent<PopoverTrigger>(trigger =>
            {
                trigger.AddChildContent("Open");
            });
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Modal Content");
            });
        });

        // Click trigger to open
        var trigger = cut.Find("[data-slot='popover-trigger']");
        trigger.Click();

        cut.Find("[data-slot='popover-overlay']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Popover overlay closes popover on click")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_OverlayClosesOnClick()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.Modal, true);
            p.AddChildContent<PopoverTrigger>(trigger =>
            {
                trigger.AddChildContent("Open");
            });
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Modal Content");
            });
        });

        // Click trigger to open
        var trigger = cut.Find("[data-slot='popover-trigger']");
        trigger.Click();

        // Click overlay to close
        var overlay = cut.Find("[data-slot='popover-overlay']");
        overlay.Click();

        cut.Instance.IsOpen.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC03: Popover non-modal does not render overlay")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_NonModalNoOverlay()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.Modal, false);
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

        // Should not throw - overlay should not exist
        cut.FindAll("[data-slot='popover-overlay']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC04: Popover Modal parameter is inherited by content")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ModalParameterInheritedByContent()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.Modal, true);
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

        // When rendered with Modal=true, content should have fixed and z-50 classes
        var content = cut.Find("[data-slot='popover-content']");
        content.ShouldNotBeNull();
        content.ClassName.ShouldContain("fixed");
    }
}
