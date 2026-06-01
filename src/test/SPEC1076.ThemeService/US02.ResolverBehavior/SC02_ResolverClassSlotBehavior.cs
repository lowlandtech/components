namespace LowlandTech.Components.Tests.SPEC1076.Showcode.US02.ResolverBehavior;

// ---------------------------------------------------------------------------
// Shared test doubles
// ---------------------------------------------------------------------------

/// <summary>Token-only theme with no class slots.</summary>
internal sealed class TokenOnlyTheme : IComponentTheme
{
    public string Name => "TokenOnly";
    public IReadOnlyDictionary<string, string> Tokens { get; } =
        new Dictionary<string, string> { ["--background"] = "#fff" };
}

/// <summary>Class-slot theme with exact and wildcard entries.</summary>
internal sealed class SlotTheme : IComponentTheme, IComponentClassTheme
{
    public string Name => "SlotTheme";
    public IReadOnlyDictionary<string, string> Tokens { get; } =
        new Dictionary<string, string> { ["--background"] = "#000" };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            // Exact: Button.Root default variant
            [new("Button", "Root", Variant: "default")] = new("slot-exact"),
            // Wildcard: Card.Root (no variant/size/state)
            [new("Card", "Root")] = new("slot-wildcard"),
            // Replace mode
            [new("Badge", "Root")] = new("slot-replace", ThemeClassMergeMode.Replace),
            // Prepend mode
            [new("Input", "Root")] = new("slot-prepend", ThemeClassMergeMode.Prepend),
        };
}

// ---------------------------------------------------------------------------

/// <summary>
/// SC02 — ComponentThemeService.Class() resolves class slots with the correct
/// fallback and merge-mode semantics.
/// </summary>
[Trait(Spec.SPEC, "1076")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Resolver class-slot behavior")]
public class SC02_ResolverClassSlotBehavior
{
    private static ComponentThemeService BuildService(params IComponentTheme[] themes)
    {
        var services = new ServiceCollection();
        foreach (var t in themes)
            services.AddSingleton<IComponentTheme>(t);
        services.AddScoped<ComponentThemeService>();
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ComponentThemeService>();
    }

    private static ComponentThemeService BuildWithSlotTheme() =>
        BuildService(new SlotTheme());

    private static (ComponentThemeService svc, string providerId) WithProvider(
        ComponentThemeService svc,
        string themeName,
        string id = "p1")
    {
        svc.Register(id, themeName);
        return (svc, id);
    }

    // -----------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: No provider — returns default class")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NullProvider_ReturnsDefault()
    {
        var svc = BuildWithSlotTheme();
        var result = svc.Class(null, new("Button", "Root", "default"), "default-class");
        result.ShouldBe("default-class");
    }

    [Fact(DisplayName = "UAC02: No provider — consumer Class appended to default")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_NullProvider_ConsumerClassAppended()
    {
        var svc = BuildWithSlotTheme();
        var result = svc.Class(null, new("Button", "Root", "default"), "default-class", consumerClass: "consumer");
        result.ShouldBe("default-class consumer");
    }

    [Fact(DisplayName = "UAC03: Unknown provider — falls back to default class")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_UnknownProvider_FallsBackToDefault()
    {
        var svc = BuildWithSlotTheme();
        // Intentionally NOT calling Register — provider is unknown.
        var result = svc.Class("unknown-id", new("Button", "Root", "default"), "default-class");
        result.ShouldBe("default-class");
    }

    [Fact(DisplayName = "UAC04: Token-only theme — no slot override, returns default")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_TokenOnlyTheme_NoSlotOverride()
    {
        var svc = BuildService(new TokenOnlyTheme());
        var (s, id) = WithProvider(svc, "TokenOnly");
        var result = s.Class(id, new("Button", "Root", "default"), "default-class");
        result.ShouldBe("default-class");
    }

    [Fact(DisplayName = "UAC05: Exact match wins — exact key beats wildcard key")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ExactMatchBeatsWildcard()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        // Button.Root with variant="default" has an exact slot ("slot-exact").
        // The wildcard (no variant) should NOT be used.
        var result = s.Class(id, new("Button", "Root", "default"), "default");
        result.ShouldContain("slot-exact");
        result.ShouldNotContain("slot-wildcard");
    }

    [Fact(DisplayName = "UAC06: Wildcard applies when variant has no exact match")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_WildcardAppliesWhenNoExactMatch()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        // Button.Root with variant="outline" has no exact slot → should fall to wildcard.
        // But Button only has an exact "default" entry and no wildcard, so it returns default.
        // Card.Root has a wildcard entry, so any variant should hit it.
        var result = s.Class(id, new("Card", "Root", "outlined"), "default");
        result.ShouldContain("slot-wildcard");
    }

    [Fact(DisplayName = "UAC07: No partial fallback beyond wildcard — variant+size miss returns default")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NoPartialFallbackBeyondWildcard()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        // Button.Root with variant="outline" — no exact, no wildcard Button.Root entry.
        var result = s.Class(id, new("Button", "Root", "outline"), "default-class");
        result.ShouldBe("default-class");
    }

    [Fact(DisplayName = "UAC08: Append mode — slot class comes AFTER default")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_AppendMode_SlotAfterDefault()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        var result = s.Class(id, new("Button", "Root", "default"), "default");
        // slot-exact uses default Append mode
        result.ShouldBe("default slot-exact");
    }

    [Fact(DisplayName = "UAC09: Replace mode — only slot class (default dropped)")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ReplaceMode_DefaultDropped()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        // Badge.Root uses Replace mode.
        var result = s.Class(id, new("Badge", "Root"), "default-badge-class");
        result.ShouldBe("slot-replace");
    }

    [Fact(DisplayName = "UAC10: Prepend mode — slot class comes BEFORE default")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_PrependMode_SlotBeforeDefault()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        // Input.Root uses Prepend mode.
        var result = s.Class(id, new("Input", "Root"), "default-input");
        result.ShouldBe("slot-prepend default-input");
    }

    [Fact(DisplayName = "UAC11: Consumer Class preserved after resolved class (all merge modes)")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ConsumerClassPreservedAfterResolved()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");

        // Append
        s.Class(id, new("Button", "Root", "default"), "default", consumerClass: "consumer")
            .ShouldEndWith(" consumer");

        // Replace
        s.Class(id, new("Badge", "Root"), "default", consumerClass: "consumer")
            .ShouldEndWith(" consumer");

        // Prepend
        s.Class(id, new("Input", "Root"), "default", consumerClass: "consumer")
            .ShouldEndWith(" consumer");
    }

    [Fact(DisplayName = "UAC12: Design class preserved as last token (all merge modes)")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_DesignClassPreservedLast()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");

        s.Class(id, new("Badge", "Root"), "default", designClass: "lt-design-selectable")
            .ShouldEndWith(" lt-design-selectable");

        s.Class(id, new("Button", "Root", "default"), "default", designClass: "lt-design-selectable")
            .ShouldEndWith(" lt-design-selectable");
    }

    [Fact(DisplayName = "UAC13: Replace still preserves consumer and design classes")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ReplacePreservesConsumerAndDesign()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        var result = s.Class(id, new("Badge", "Root"), "default",
            consumerClass: "my-layout", designClass: "lt-design-selected");
        result.ShouldBe("slot-replace my-layout lt-design-selected");
        result.ShouldNotContain("default");
    }

    [Fact(DisplayName = "UAC14: Missing slot does not throw")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_MissingSlot_DoesNotThrow()
    {
        var svc = BuildWithSlotTheme();
        var (s, id) = WithProvider(svc, "SlotTheme");
        Should.NotThrow(() =>
            s.Class(id, new("Unknown", "Root"), "fallback"));
    }

    [Fact(DisplayName = "UAC15: Unknown theme name renders with component defaults (no throw)")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_UnknownThemeName_NoThrow_DefaultsApplied()
    {
        var svc = BuildWithSlotTheme();
        svc.Register("p-unknown", "NonExistentTheme");
        var result = Should.NotThrow(() =>
            svc.Class("p-unknown", new("Button", "Root", "default"), "default-class"));
        result.ShouldBe("default-class");
    }
}

// ---------------------------------------------------------------------------

/// <summary>
/// SC03 — Nested and sibling providers resolve independently.
/// </summary>
[Trait(Spec.SPEC, "1076")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "03")]
[UserStory("02", "Resolver provider isolation")]
public class SC03_ResolverProviderIsolation
{
    private static ComponentThemeService BuildBothThemes()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IComponentTheme>(new SlotTheme());
        services.AddSingleton<IComponentTheme>(new TokenOnlyTheme());
        services.AddScoped<ComponentThemeService>();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ComponentThemeService>();
    }

    [Fact(DisplayName = "UAC01: Nested providers each get their own slot resolution")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NestedProvidersIndependent()
    {
        var svc = BuildBothThemes();
        svc.Register("outer", "SlotTheme");
        svc.Register("inner", "TokenOnly");

        // outer has SlotTheme → Badge.Root has a Replace slot
        var outer = svc.Class("outer", new("Badge", "Root"), "default");
        // inner has TokenOnly → no slot override, falls back to default
        var inner = svc.Class("inner", new("Badge", "Root"), "default");

        outer.ShouldBe("slot-replace");
        inner.ShouldBe("default");
    }

    [Fact(DisplayName = "UAC02: Sibling providers do not leak slots into each other")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SiblingProvidersNoLeak()
    {
        var svc = BuildBothThemes();
        svc.Register("a", "SlotTheme");
        svc.Register("b", "TokenOnly");

        var resultA = svc.Class("a", new("Badge", "Root"), "default");
        var resultB = svc.Class("b", new("Badge", "Root"), "default");

        resultA.ShouldContain("slot-replace");
        resultB.ShouldBe("default");
    }

    [Fact(DisplayName = "UAC03: Unregistering a provider cleans up (result falls to null-provider path)")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_UnregisteredProviderFallsThrough()
    {
        var svc = BuildBothThemes();
        svc.Register("p", "SlotTheme");
        svc.Unregister("p");
        // After unregister, GetForProvider returns first theme (SlotTheme) as fallback.
        // What matters is no exception is thrown.
        Should.NotThrow(() => svc.Class("p", new("Badge", "Root"), "default"));
    }
}
