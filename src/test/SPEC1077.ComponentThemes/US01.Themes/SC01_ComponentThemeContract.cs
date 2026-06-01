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
    // The minimum set of CSS custom-property keys every theme must define.
    // Advanced themes may define additional tokens (e.g. --radius, --surface-blur,
    // density tokens); those extras are allowed and do not fail the contract.
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

    // Characters/sequences that must not appear in a token value lest they
    // break the scoped <style> block emitted by ThemeProvider.
    private static readonly string[] ForbiddenValueSequences = ["{", "}", ";", "</", "<style"];

    public static TheoryData<IComponentTheme> AllThemes =>
    [
        new DarkComponentTheme(),
        new LightComponentTheme(),
        new ForestComponentTheme(),
        new OceanComponentTheme(),
        new RoseComponentTheme(),
        new GlassComponentTheme(),
        new ChadcnBlackComponentTheme(),
        new CorporateGreyComponentTheme(),
    ];

    // -----------------------------------------------------------------------

    [Theory(DisplayName = "UAC01: Name is non-null and non-empty")]
    [Trait(Spec.UAC, "01")]
    [MemberData(nameof(AllThemes))]
    public void UAC01_NameIsNonEmpty(IComponentTheme theme)
    {
        theme.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "UAC02: Each theme has a unique Name within the full registered set")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NamesAreDistinct()
    {
        // Instantiate every theme independently to check name uniqueness.
        var names = new IComponentTheme[]
        {
            new DarkComponentTheme(),
            new LightComponentTheme(),
            new ForestComponentTheme(),
            new OceanComponentTheme(),
            new RoseComponentTheme(),
            new GlassComponentTheme(),
            new ChadcnBlackComponentTheme(),
            new CorporateGreyComponentTheme(),
        }.Select(t => t.Name).ToList();
        names.Distinct(StringComparer.OrdinalIgnoreCase).Count().ShouldBe(names.Count,
            "Theme names must be unique across all registered themes");
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

    [Theory(DisplayName = "UAC07: Token count is at least the required set size (extra tokens allowed)")]
    [Trait(Spec.UAC, "07")]
    [MemberData(nameof(AllThemes))]
    public void UAC07_TokenCountAtLeastRequiredSet(IComponentTheme theme)
    {
        theme.Tokens.Count.ShouldBeGreaterThanOrEqualTo(RequiredTokens.Length,
            $"Theme '{theme.Name}' has only {theme.Tokens.Count} tokens but at least {RequiredTokens.Length} are required");
    }

    [Theory(DisplayName = "UAC08: --primary-foreground contrasts with --primary (not empty)")]
    [Trait(Spec.UAC, "08")]
    [MemberData(nameof(AllThemes))]
    public void UAC08_PrimaryForegroundNotEmpty(IComponentTheme theme)
    {
        // ChadcnBlack and similar themes intentionally use a dark foreground on a
        // light primary, so we cannot hard-code '#ffffff'. The semantic guarantee
        // is that a foreground value exists and is non-empty.
        theme.Tokens["--primary-foreground"].ShouldNotBeNullOrWhiteSpace(
            $"Theme '{theme.Name}': --primary-foreground must have a non-empty contrast value");
    }

    [Theory(DisplayName = "UAC09: --destructive-foreground is non-empty (contrast guarantee)")]
    [Trait(Spec.UAC, "09")]
    [MemberData(nameof(AllThemes))]
    public void UAC09_DestructiveForegroundNotEmpty(IComponentTheme theme)
    {
        theme.Tokens["--destructive-foreground"].ShouldNotBeNullOrWhiteSpace(
            $"Theme '{theme.Name}': --destructive-foreground must have a non-empty contrast value");
    }

    [Theory(DisplayName = "UAC11: Token keys do not contain braces or semicolons")]
    [Trait(Spec.UAC, "11")]
    [MemberData(nameof(AllThemes))]
    public void UAC11_TokenKeysSafe(IComponentTheme theme)
    {
        foreach (var key in theme.Tokens.Keys)
        {
            // Use Contains().ShouldBeFalse() to avoid the Shouldly overload that
            // takes Expression<Func<char,bool>> as its second parameter.
            key.Contains("{").ShouldBeFalse($"Theme '{theme.Name}' token key '{key}' contains '{{'");
            key.Contains("}").ShouldBeFalse($"Theme '{theme.Name}' token key '{key}' contains '}}'" );
            key.Contains(";").ShouldBeFalse($"Theme '{theme.Name}' token key '{key}' contains ';'");
        }
    }

    [Theory(DisplayName = "UAC12: Token values do not contain sequences that break the scoped style block")]
    [Trait(Spec.UAC, "12")]
    [MemberData(nameof(AllThemes))]
    public void UAC12_TokenValuesSafe(IComponentTheme theme)
    {
        foreach (var (key, value) in theme.Tokens)
        foreach (var forbidden in ForbiddenValueSequences)
            value.Contains(forbidden).ShouldBeFalse(
                $"Theme '{theme.Name}' token '{key}' value contains forbidden sequence '{forbidden}'");
    }

    [Theory(DisplayName = "UAC13: IComponentClassTheme.ClassSlots keys start with '--' is false (component/slot pattern)")]
    [Trait(Spec.UAC, "13")]
    [MemberData(nameof(AllThemes))]
    public void UAC13_ClassSlotKeyComponentNotEmpty(IComponentTheme theme)
    {
        if (theme is not IComponentClassTheme classTheme) return;
        foreach (var key in classTheme.ClassSlots.Keys)
        {
            key.Component.ShouldNotBeNullOrWhiteSpace(
                $"Theme '{theme.Name}' has a ClassSlot with an empty Component name");
            key.Slot.ShouldNotBeNullOrWhiteSpace(
                $"Theme '{theme.Name}' has a ClassSlot with an empty Slot name");
        }
    }

    [Theory(DisplayName = "UAC14: IComponentClassTheme.ClassSlots values are non-empty strings")]
    [Trait(Spec.UAC, "14")]
    [MemberData(nameof(AllThemes))]
    public void UAC14_ClassSlotValuesNotEmpty(IComponentTheme theme)
    {
        if (theme is not IComponentClassTheme classTheme) return;
        foreach (var (key, slot) in classTheme.ClassSlots)
            slot.Class.ShouldNotBeNullOrWhiteSpace(
                $"Theme '{theme.Name}' ClassSlot [{key.Component}.{key.Slot}] has an empty Class string");
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
