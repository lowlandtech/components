namespace LowlandTech.Components.Tests.SPEC1075.Showcode.US01.SourceRegistry;

/// <summary>
/// SC01 — StorySourceRegistry correctly retrieves source code, returns a
/// fallback for unknown keys, normalises line endings, and builds
/// <see cref="StoryCard.CodeTab"/> instances.
/// </summary>
[Trait(Spec.SPEC, "1075")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "StorySourceRegistry resolves source code and builds CodeTab instances")]
public class SC01_StorySourceRegistryBehavior
{
    // A key that is guaranteed to exist in the generated Sources dictionary.
    private const string KnownKey = "Showcode/Examples/Data/DataGrid/OrderRow.cs";

    // -----------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: GetCode returns non-empty string for a known source key")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_GetCodeKnownKeyReturnsSource()
    {
        var code = StorySourceRegistry.GetCode(KnownKey);

        code.ShouldNotBeNullOrWhiteSpace();
        code.ShouldNotStartWith("// Story source");
    }

    [Fact(DisplayName = "UAC02: GetCode returns fallback comment for an unknown key")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_GetCodeUnknownKeyReturnsFallback()
    {
        var code = StorySourceRegistry.GetCode("this/key/does/not/exist");

        code.ShouldStartWith("// Story source");
        code.ShouldContain("this/key/does/not/exist");
    }

    [Fact(DisplayName = "UAC03: GetCode normalises CRLF to LF")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_GetCodeNormalisesCrLf()
    {
        // All generated sources use \n; after normalisation there must be no \r\n pairs.
        var code = StorySourceRegistry.GetCode(KnownKey);

        code.ShouldNotContain("\r\n");
    }

    [Fact(DisplayName = "UAC04: GetCode trims trailing whitespace/newlines")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_GetCodeTrimsTrailingWhitespace()
    {
        var code = StorySourceRegistry.GetCode(KnownKey);

        // TrimEnd() should produce the same string — no trailing whitespace.
        code.ShouldBe(code.TrimEnd());
    }

    [Fact(DisplayName = "UAC05: CodeTab returns a StoryCard.CodeTab with the supplied value")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_CodeTabValueIsPreserved()
    {
        var tab = StorySourceRegistry.CodeTab("tab-1", "OrderRow", KnownKey);

        tab.Value.ShouldBe("tab-1");
    }

    [Fact(DisplayName = "UAC06: CodeTab returns a StoryCard.CodeTab with the supplied name")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_CodeTabNameIsPreserved()
    {
        var tab = StorySourceRegistry.CodeTab("tab-1", "OrderRow", KnownKey);

        tab.Name.ShouldBe("OrderRow");
    }

    [Fact(DisplayName = "UAC07: CodeTab.Code equals GetCode for the same key")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CodeTabCodeMatchesGetCode()
    {
        var tab = StorySourceRegistry.CodeTab("tab-1", "OrderRow", KnownKey);
        var direct = StorySourceRegistry.GetCode(KnownKey);

        tab.Code.ShouldBe(direct);
    }

    [Fact(DisplayName = "UAC08: CodeTab defaults language to 'razor'")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_CodeTabDefaultLanguageIsRazor()
    {
        var tab = StorySourceRegistry.CodeTab("tab-1", "OrderRow", KnownKey);

        tab.Language.ShouldBe("razor");
    }

    [Fact(DisplayName = "UAC09: CodeTab respects an explicit language override")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_CodeTabExplicitLanguageIsRespected()
    {
        var tab = StorySourceRegistry.CodeTab("tab-1", "OrderRow", KnownKey, "csharp");

        tab.Language.ShouldBe("csharp");
    }

    [Fact(DisplayName = "UAC10: CodeTab with unknown key produces fallback code, not null")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_CodeTabUnknownKeyStillReturnsTab()
    {
        var tab = StorySourceRegistry.CodeTab("tab-x", "Missing", "no/such/key");

        tab.ShouldNotBeNull();
        tab.Code.ShouldStartWith("// Story source");
    }
}
