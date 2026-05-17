namespace LowlandTech.Components.Tests.SPEC1073.Showcode.US01.Registry;

/// <summary>
/// SC01 — StoryRegistry correctly registers story types, builds the group tree,
/// and exposes the look-up helpers <c>GetAll</c>, <c>GetBySlug</c>, and <c>GetFirst</c>.
/// </summary>
[Trait(Spec.SPEC, "1073")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "StoryRegistry registers stories and builds the group tree")]
public class SC01_StoryRegistryBehavior
{
    // Each test uses its own isolated StoryGroup / dictionary via a local
    // helper so the global StoryRegistry.Root state does not bleed between tests.
    // We exercise the static public surface via RegisterAllStories against a
    // synthetic assembly-scoped type, and validate the results on the shared
    // registry (acceptable because slug keys are derived from FullName and
    // the fixture types below are unique to this test class).

    private static string SlugOf<T>() =>
        typeof(T).FullName!.Replace(".", "_").ToLowerInvariant();

    // -----------------------------------------------------------------------
    // Fixture story types – must subclass StoryComponent to be picked up.
    // DisplayName drives the path in the group tree.
    // -----------------------------------------------------------------------
    [DisplayName("RegistryFixture/Alpha")]
    private class AlphaStory : StoryComponent { }

    [DisplayName("RegistryFixture/Beta")]
    private class BetaStory : StoryComponent { }

    [DisplayName("RegistryFixture/Nested/Deep")]
    private class DeepNestedStory : StoryComponent { }

    // Not a StoryComponent — must be ignored by RegisterAllStories.
    private class NotAStory { }

    // -----------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: Register adds the story to GetAll")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RegisterAddsToGetAll()
    {
        StoryRegistry.Register(typeof(AlphaStory));

        StoryRegistry.GetAll().ShouldContain(m => m.Slug == SlugOf<AlphaStory>());
    }

    [Fact(DisplayName = "UAC02: GetBySlug returns the registered metadata")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_GetBySlugReturnsMetadata()
    {
        StoryRegistry.Register(typeof(BetaStory));
        var slug = SlugOf<BetaStory>();

        var result = StoryRegistry.GetBySlug(slug);

        result.ShouldNotBeNull();
        result.Slug.ShouldBe(slug);
        result.Type.ShouldBe(typeof(BetaStory));
    }

    [Fact(DisplayName = "UAC03: GetBySlug returns null for an unknown slug")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_GetBySlugMissReturnsNull()
    {
        StoryRegistry.GetBySlug("this_slug_does_not_exist_ever").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC04: GetFirst returns a non-null story when at least one is registered")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_GetFirstNonNull()
    {
        StoryRegistry.Register(typeof(AlphaStory));

        StoryRegistry.GetFirst().ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: DisplayName path segments drive the group tree")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_GroupTreeReflectsDisplayName()
    {
        StoryRegistry.Register(typeof(AlphaStory));

        // "RegistryFixture/Alpha" → Root > RegistryFixture (group) > Alpha (story)
        var group = StoryRegistry.Root.Children
            .FirstOrDefault(g => g.Name == "RegistryFixture");

        group.ShouldNotBeNull("Expected a 'RegistryFixture' child group on Root");
        group!.Stories.ShouldContain(m => m.Name == "Alpha");
    }

    [Fact(DisplayName = "UAC06: Deeply nested DisplayName creates intermediate groups")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DeepNestingCreatesIntermediateGroups()
    {
        StoryRegistry.Register(typeof(DeepNestedStory));

        // "RegistryFixture/Nested/Deep"
        var l1 = StoryRegistry.Root.Children
            .FirstOrDefault(g => g.Name == "RegistryFixture");
        l1.ShouldNotBeNull();

        var l2 = l1!.Children.FirstOrDefault(g => g.Name == "Nested");
        l2.ShouldNotBeNull("Expected an intermediate 'Nested' group");

        l2!.Stories.ShouldContain(m => m.Name == "Deep");
    }

    [Fact(DisplayName = "UAC07: RegisterAllStories skips non-StoryComponent types")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RegisterAllStoriesSkipsNonStoryComponents()
    {
        StoryRegistry.RegisterAllStories(typeof(NotAStory).Assembly);

        StoryRegistry.GetAll().ShouldNotContain(m => m.Type == typeof(NotAStory));
    }

    [Fact(DisplayName = "UAC08: RegisterAllStories registers concrete StoryComponent subclasses")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RegisterAllStoriesPicksUpConcreteSubclasses()
    {
        StoryRegistry.RegisterAllStories(typeof(AlphaStory).Assembly);

        StoryRegistry.GetAll().ShouldContain(m => m.Type == typeof(AlphaStory));
        StoryRegistry.GetAll().ShouldContain(m => m.Type == typeof(BetaStory));
    }

    [Fact(DisplayName = "UAC09: Slug is derived from the type's FullName")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_SlugDerivedFromFullName()
    {
        StoryRegistry.Register(typeof(AlphaStory));
        var expected = typeof(AlphaStory).FullName!.Replace(".", "_").ToLowerInvariant();

        StoryRegistry.GetBySlug(expected).ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: StoryMetadata.Name uses the last path segment of DisplayName")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_MetadataNameIsLastSegment()
    {
        StoryRegistry.Register(typeof(AlphaStory));
        var slug = SlugOf<AlphaStory>();

        var meta = StoryRegistry.GetBySlug(slug);

        meta.ShouldNotBeNull();
        meta!.Name.ShouldBe("Alpha");
    }
}
