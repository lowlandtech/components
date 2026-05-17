using LowlandTech.Components.E2E.Infrastructure;

namespace LowlandTech.Components.E2E.SPEC1103.FeedbackStories;

/// <summary>
/// SPEC1103 US01 — E2E tests for every Feedback group story.
/// </summary>
[Trait(Spec.SPEC, "1103")]
[Trait(Spec.US, "01")]
public sealed class SC01_FeedbackStoriesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: Alert story — alert elements with titles render")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AlertStory()
    {
        await Showcode.GoToStoryAsync("Alert");

        var alert = Showcode.DataSlot("alert").First;
        await Assertions.Expect(alert).ToBeVisibleAsync();

        var title = Showcode.DataSlot("alert-title").First;
        await Assertions.Expect(title).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC02: Badge story — badge elements render with variant classes")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_BadgeStory()
    {
        await Showcode.GoToStoryAsync("Badge");

        var badge = Showcode.DataSlot("badge").First;
        await Assertions.Expect(badge).ToBeVisibleAsync();

        // Multiple badge variants should be present.
        var badges = Showcode.DataSlot("badge");
        (await badges.CountAsync()).ShouldBeGreaterThan(1);
    }

    [Fact(DisplayName = "UAC03: Progress story — progress bar renders with expected value")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ProgressStory()
    {
        await Showcode.GoToStoryAsync("Progress");

        var progress = Showcode.DataSlot("progress").First;
        await Assertions.Expect(progress).ToBeVisibleAsync();

        // The progress element should have an aria value attribute.
        var ariaValue = await progress.GetAttributeAsync("aria-valuenow");
        ariaValue.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Spinner story — spinner is visible")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_SpinnerStory()
    {
        await Showcode.GoToStoryAsync("Spinner");

        // Spinner renders as <span data-slot="spinner" role="status">.
        var spinner = Showcode.DataSlot("spinner").First;
        await Assertions.Expect(spinner).ToBeVisibleAsync();
    }
}
