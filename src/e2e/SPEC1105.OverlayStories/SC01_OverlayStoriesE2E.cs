using LowlandTech.Components.E2E.Infrastructure;

namespace LowlandTech.Components.E2E.SPEC1105.OverlayStories;

/// <summary>
/// SPEC1105 US01 — E2E tests for every Overlay group story.
/// Each test opens and (where applicable) closes the overlay component.
/// </summary>
[Trait(Spec.SPEC, "1105")]
[Trait(Spec.US, "01")]
public sealed class SC01_OverlayStoriesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: Dialog story — clicking Open Dialog shows the dialog")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DialogStory()
    {
        await Showcode.GoToStoryAsync("Dialog");

        // Click the trigger button.
        var trigger = Showcode.Page.Locator("main [data-slot='dialog-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.ClickAsync();

        // Dialog content should appear in the DOM.
        await Showcode.Page.WaitForSelectorAsync("[data-slot='dialog-content']",
            new PageWaitForSelectorOptions { Timeout = 5_000 });
        var content = Showcode.DataSlot("dialog-content").First;
        await Assertions.Expect(content).ToBeVisibleAsync();

        // Close via Escape.
        await Showcode.Page.Keyboard.PressAsync("Escape");
        await Showcode.Page.WaitForTimeoutAsync(400);
    }

    [Fact(DisplayName = "UAC02: Alert Dialog story — clicking trigger shows the alert dialog")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_AlertDialogStory()
    {
        await Showcode.GoToStoryAsync("Alert Dialog");

        var trigger = Showcode.Page.Locator("main [data-slot='alert-dialog-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.ClickAsync();

        await Showcode.Page.WaitForSelectorAsync("[data-slot='alert-dialog-content']",
            new PageWaitForSelectorOptions { Timeout = 5_000 });
        var content = Showcode.DataSlot("alert-dialog-content").First;
        await Assertions.Expect(content).ToBeVisibleAsync();

        // Click the Cancel button to dismiss.
        var cancel = Showcode.DataSlot("alert-dialog-cancel").First;
        await cancel.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(400);
    }

    [Fact(DisplayName = "UAC03: Tooltip story — hovering the trigger reveals tooltip content")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_TooltipStory()
    {
        await Showcode.GoToStoryAsync("Tooltip");

        // Find the tooltip trigger.
        var trigger = Showcode.Page.Locator("main [data-slot='tooltip-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();

        // Hover to activate the tooltip.
        await trigger.HoverAsync();
        await Showcode.Page.WaitForTimeoutAsync(600);

        // Tooltip content should be visible.
        var tooltip = Showcode.Page.Locator("[data-slot='tooltip-content']").First;
        await Assertions.Expect(tooltip).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC04: Drawer story — clicking Open Drawer shows the drawer panel")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_DrawerStory()
    {
        await Showcode.GoToStoryAsync("Drawer");

        var trigger = Showcode.Page.Locator("main [data-slot='drawer-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.ClickAsync();

        await Showcode.Page.WaitForSelectorAsync("[data-slot='drawer-content']",
            new PageWaitForSelectorOptions { Timeout = 5_000 });
        var content = Showcode.DataSlot("drawer-content").First;
        await Assertions.Expect(content).ToBeVisibleAsync();

        // Close via Escape.
        await Showcode.Page.Keyboard.PressAsync("Escape");
        await Showcode.Page.WaitForTimeoutAsync(400);
    }

    [Fact(DisplayName = "UAC05: Popover story — clicking trigger shows the popover content")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_PopoverStory()
    {
        await Showcode.GoToStoryAsync("Popover");

        var trigger = Showcode.Page.Locator("main [data-slot='popover-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();
        await trigger.ClickAsync();

        await Showcode.Page.WaitForSelectorAsync("[data-slot='popover-content']",
            new PageWaitForSelectorOptions { Timeout = 5_000 });
        var content = Showcode.DataSlot("popover-content").First;
        await Assertions.Expect(content).ToBeVisibleAsync();
    }
}
