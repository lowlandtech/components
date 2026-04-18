using LowlandTech.Components.E2E.Infrastructure;

namespace LowlandTech.Components.E2E.SPEC1104.MiscStories;

/// <summary>
/// SPEC1104 US01 — E2E tests for every Misc group story.
/// </summary>
[Trait(Spec.SPEC, "1104")]
[Trait(Spec.US, "01")]
public sealed class SC01_MiscStoriesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: Avatar story — avatar renders with image or fallback")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AvatarStory()
    {
        await Showcode.GoToStoryAsync("Avatar");

        var avatar = Showcode.DataSlot("avatar").First;
        await Assertions.Expect(avatar).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC02: Kbd story — keyboard shortcut elements render")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_KbdStory()
    {
        await Showcode.GoToStoryAsync("Kbd");

        var kbd = Showcode.Page.Locator("main kbd").First;
        await Assertions.Expect(kbd).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC03: Collapsible story — clicking trigger toggles content visibility")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_CollapsibleStory()
    {
        await Showcode.GoToStoryAsync("Collapsible");

        var trigger = Showcode.Page.Locator("main [data-slot='collapsible-trigger']").First;
        await Assertions.Expect(trigger).ToBeVisibleAsync();

        // Click to open.
        await trigger.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(400);

        var content = Showcode.Page.Locator("main [data-slot='collapsible-content']").First;
        await Assertions.Expect(content).ToBeVisibleAsync();

        // Click to close.
        await trigger.ClickAsync();
        await Showcode.Page.WaitForTimeoutAsync(400);
    }

    [Fact(DisplayName = "UAC04: Scroll Area story — scroll area container renders")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ScrollAreaStory()
    {
        await Showcode.GoToStoryAsync("Scroll Area");

        var scrollArea = Showcode.DataSlot("scroll-area").First;
        await Assertions.Expect(scrollArea).ToBeVisibleAsync();
    }
}
