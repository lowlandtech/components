namespace LowlandTech.Components.Tests.SPEC1074.Showcode.US01.State;

/// <summary>
/// SC01 — ShowcodeState correctly manages current story selection, table mode,
/// and fires the <c>Changed</c> event on every mutating operation.
/// </summary>
[Trait(Spec.SPEC, "1074")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "ShowcodeState manages selection and table-mode transitions")]
public class SC01_ShowcodeStateBehavior
{
    private static StoryMetadata AStory(string slug = "test_story") =>
        new() { Slug = slug, Name = slug, Type = typeof(StoryComponent) };

    // -----------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: Initial state — Current is null and IsTableMode is false")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_InitialState()
    {
        var state = new ShowcodeState();

        state.Current.ShouldBeNull();
        state.IsTableMode.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: Select sets Current to the supplied story")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SelectSetsCurrent()
    {
        var state = new ShowcodeState();
        var story = AStory();

        state.Select(story);

        state.Current.ShouldBe(story);
    }

    [Fact(DisplayName = "UAC03: Select raises Changed")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectRaisesChanged()
    {
        var state = new ShowcodeState();
        var raised = 0;
        state.Changed += () => raised++;

        state.Select(AStory());

        raised.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC04: ShowTable sets IsTableMode to true and clears Current")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ShowTableSetsTableMode()
    {
        var state = new ShowcodeState();
        state.Select(AStory());

        state.ShowTable();

        state.IsTableMode.ShouldBeTrue();
        state.Current.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC05: ShowTable raises Changed")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ShowTableRaisesChanged()
    {
        var state = new ShowcodeState();
        var raised = 0;
        state.Changed += () => raised++;

        state.ShowTable();

        raised.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC06: Clear resets Current and IsTableMode")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ClearResetsState()
    {
        var state = new ShowcodeState();
        state.ShowTable();

        state.Clear();

        state.Current.ShouldBeNull();
        state.IsTableMode.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC07: Clear raises Changed")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ClearRaisesChanged()
    {
        var state = new ShowcodeState();
        var raised = 0;
        state.Changed += () => raised++;

        state.Clear();

        raised.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC08: SelectFirst selects the first registered story when one exists")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_SelectFirstPicksFirstStory()
    {
        // Ensure at least one story is registered (uses the shared StoryRegistry)
        StoryRegistry.Register(typeof(FixtureStory));
        var state = new ShowcodeState();

        state.SelectFirst();

        state.Current.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: SelectFirst raises Changed when a story is found")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_SelectFirstRaisesChanged()
    {
        StoryRegistry.Register(typeof(FixtureStory));
        var state = new ShowcodeState();
        var raised = 0;
        state.Changed += () => raised++;

        state.SelectFirst();

        raised.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC10: Selecting a second story replaces Current")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_SelectReplacesCurrent()
    {
        var state = new ShowcodeState();
        var first = AStory("first");
        var second = AStory("second");

        state.Select(first);
        state.Select(second);

        state.Current.ShouldBe(second);
    }

    // Minimal StoryComponent subclass so SelectFirst has something to find.
    [DisplayName("ShowcodeStateFixture/Story")]
    private class FixtureStory : StoryComponent { }
}
