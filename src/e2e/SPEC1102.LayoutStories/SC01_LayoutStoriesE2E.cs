using LowlandTech.Components.E2E.Infrastructure;

namespace LowlandTech.Components.E2E.SPEC1102.LayoutStories;

/// <summary>
/// SPEC1102 US01 — E2E tests for every Layout group story.
/// </summary>
[Trait(Spec.SPEC, "1102")]
[Trait(Spec.US, "01")]
public sealed class SC01_LayoutStoriesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: Card story — card container and header are visible")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CardStory()
    {
        await Showcode.GoToStoryAsync("Card");

        var card = Showcode.DataSlot("card").First;
        await Assertions.Expect(card).ToBeVisibleAsync();

        var header = Showcode.DataSlot("card-header").First;
        await Assertions.Expect(header).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC02: Separator story — horizontal and vertical separators render")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_SeparatorStory()
    {
        await Showcode.GoToStoryAsync("Separator");

        var separator = Showcode.DataSlot("separator").First;
        await Assertions.Expect(separator).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC03: Skeleton story — skeleton elements render")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_SkeletonStory()
    {
        await Showcode.GoToStoryAsync("Skeleton");

        var skeleton = Showcode.DataSlot("skeleton").First;
        await Assertions.Expect(skeleton).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC04: Aspect Ratio story — aspect-ratio container renders")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_AspectRatioStory()
    {
        await Showcode.GoToStoryAsync("Aspect Ratio");

        var ar = Showcode.DataSlot("aspect-ratio").First;
        await Assertions.Expect(ar).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC05: Empty story — empty state container is visible")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_EmptyStory()
    {
        await Showcode.GoToStoryAsync("Empty");

        // Empty renders a centred div — the main area should have content.
        await Assertions.Expect(Showcode.Main).ToBeVisibleAsync();
        var text = await Showcode.Main.TextContentAsync();
        text.ShouldNotBeNullOrWhiteSpace();
    }
}
