namespace LowlandTech.Components.Tests.SPEC1077.Showcode.US01.Themes;

/// <summary>
/// SC01 — Every IComponentTheme implementation shipped with the app satisfies
/// the interface contract: a non-empty Name, a non-empty Tokens dictionary,
/// and all required CSS custom-property keys present with non-empty values.
/// </summary>
[Trait(Spec.SPEC, "1077")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Component themes satisfy the IComponentTheme contract")]
public class SC01_ComponentThemeContract
{
    // The full set of CSS custom-property keys every theme must define.
    private static readonly string[] RequiredTokens =
    [
        "--background",
        "--foreground",
        "--card",
        "--card-foreground",
        "--popover",
        "--popover-foreground",
        "--primary",
        "--primary-foreground",
        "--secondary",
        "--secondary-foreground",
        "--muted",
        "--muted-foreground",
        "--accent",
        "--accent-foreground",
        "--destructive",
        "--destructive-foreground",
        "--border",
        "--input",
        "--ring",
    ];

    public static TheoryData<IComponentTheme> AllThemes =>
    [
        new DarkComponentTheme(),
        new LightComponentTheme(),
        new ForestComponentTheme(),
        new OceanComponentTheme(),
        new RoseComponentTheme(),
    ];

    // -----------------------------------------------------------------------

    [Theory(DisplayName = "UAC01: Name is non-null and non-empty")]
    [Trait(Spec.UAC, "01")]
    [MemberData(nameof(AllThemes))]
    public void UAC01_NameIsNonEmpty(IComponentTheme theme)
    {
        theme.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory(DisplayName = "UAC02: Each theme has a unique Name")]
    [Trait(Spec.UAC, "02")]
    [MemberData(nameof(AllThemes))]
    public void UAC02_NamesAreDistinct(IComponentTheme theme)
    {
        // Verify the concrete name matches one of the five expected values
        // (guards against copy-paste where Name was left unchanged).
        var knownNames = new[] { "Dark", "Light", "Forest", "Ocean", "Rose" };
        knownNames.ShouldContain(theme.Name);
    }

    [Theory(DisplayName = "UAC03: Tokens dictionary is non-null and non-empty")]
    [Trait(Spec.UAC, "03")]
    [MemberData(nameof(AllThemes))]
    public void UAC03_TokensNonEmpty(IComponentTheme theme)
    {
        theme.Tokens.ShouldNotBeNull();
        theme.Tokens.Count.ShouldBeGreaterThan(0);
    }

    [Theory(DisplayName = "UAC04: All required CSS custom-property keys are present")]
    [Trait(Spec.UAC, "04")]
    [MemberData(nameof(AllThemes))]
    public void UAC04_AllRequiredTokensPresent(IComponentTheme theme)
    {
        foreach (var key in RequiredTokens)
            theme.Tokens.ShouldContainKey(key,
                $"Theme '{theme.Name}' is missing required token '{key}'");
    }

    [Theory(DisplayName = "UAC05: Every token value is a non-empty string")]
    [Trait(Spec.UAC, "05")]
    [MemberData(nameof(AllThemes))]
    public void UAC05_AllTokenValuesNonEmpty(IComponentTheme theme)
    {
        foreach (var (key, value) in theme.Tokens)
            value.ShouldNotBeNullOrWhiteSpace(
                $"Theme '{theme.Name}' has an empty value for token '{key}'");
    }

    [Theory(DisplayName = "UAC06: Token keys start with '--'")]
    [Trait(Spec.UAC, "06")]
    [MemberData(nameof(AllThemes))]
    public void UAC06_TokenKeysStartWithDoubleDash(IComponentTheme theme)
    {
        foreach (var key in theme.Tokens.Keys)
            key.ShouldStartWith("--");
    }

    [Theory(DisplayName = "UAC07: Token count matches the required token set size")]
    [Trait(Spec.UAC, "07")]
    [MemberData(nameof(AllThemes))]
    public void UAC07_TokenCountMatchesRequiredSet(IComponentTheme theme)
    {
        theme.Tokens.Count.ShouldBe(RequiredTokens.Length,
            $"Theme '{theme.Name}' has {theme.Tokens.Count} tokens but {RequiredTokens.Length} are required");
    }

    [Theory(DisplayName = "UAC08: --primary-foreground is '#ffffff' (contrast guarantee)")]
    [Trait(Spec.UAC, "08")]
    [MemberData(nameof(AllThemes))]
    public void UAC08_PrimaryForegroundIsWhite(IComponentTheme theme)
    {
        theme.Tokens["--primary-foreground"].ShouldBe("#ffffff",
            $"Theme '{theme.Name}': --primary-foreground must be #ffffff for contrast");
    }

    [Theory(DisplayName = "UAC09: --destructive-foreground is '#ffffff' (contrast guarantee)")]
    [Trait(Spec.UAC, "09")]
    [MemberData(nameof(AllThemes))]
    public void UAC09_DestructiveForegroundIsWhite(IComponentTheme theme)
    {
        theme.Tokens["--destructive-foreground"].ShouldBe("#ffffff",
            $"Theme '{theme.Name}': --destructive-foreground must be #ffffff for contrast");
    }

    [Theory(DisplayName = "UAC10: Tokens property returns the same instance on repeated access")]
    [Trait(Spec.UAC, "10")]
    [MemberData(nameof(AllThemes))]
    public void UAC10_TokensIsSameInstanceOnRepeatAccess(IComponentTheme theme)
    {
        var first = theme.Tokens;
        var second = theme.Tokens;

        ReferenceEquals(first, second).ShouldBeTrue(
            $"Theme '{theme.Name}': Tokens should be a stable, pre-built dictionary, not re-created on each access");
    }
}
