namespace LowlandTech.Components.Tests.SPEC1005.Badge.US02.ThemeSlot;

// ---------------------------------------------------------------------------
// Test theme: adds class slots for Badge.Root
// ---------------------------------------------------------------------------
internal sealed class BadgeSlotTheme : IComponentTheme, IComponentClassTheme
{
    public string Name => "BadgeSlot";
    public IReadOnlyDictionary<string, string> Tokens { get; } =
        new Dictionary<string, string> { ["--background"] = "#000" };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new("Badge", "Root", Variant: "default")] = new("theme-badge-default", ThemeClassMergeMode.Append),
            [new("Badge", "Root", Variant: "outline")] = new("theme-badge-outline", ThemeClassMergeMode.Replace),
            // Wildcard entry for any other variant
            [new("Badge", "Root")] = new("theme-badge-wildcard", ThemeClassMergeMode.Append),
        };
}

// ---------------------------------------------------------------------------

/// <summary>
/// SC01 — Badge component respects ThemeProvider class slot injection.
/// </summary>
[Trait(Spec.SPEC, "1005")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Badge component respects ThemeProvider class slots")]
public class SC01_BadgeThemeSlot : JsViewScenario
{
    private ThemeProviderContext BuildContext(string providerId = "p1")
    {
        var services = new ServiceCollection();
        services.AddSingleton<IComponentTheme>(new BadgeSlotTheme());
        services.AddScoped<ComponentThemeService>();
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ComponentThemeService>();
        svc.Register(providerId, "BadgeSlot");
        return new ThemeProviderContext(providerId, svc);
    }

    [Fact(DisplayName = "UAC01: No ThemeContext — renders without theme slot classes")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NoContext_DefaultClasses()
    {
        var cut = Ctx.Render<BadgeSut>(p => p
            .Add(b => b.Variant, "default")
            .AddChildContent("New"));
        var classes = cut.Find("[data-slot='badge']").ClassName;
        classes.ShouldNotBeNullOrWhiteSpace();
        classes!.Contains("theme-badge").ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: Exact variant Append — theme class added after default")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ExactVariantAppend()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<BadgeSut>(p => p
            .Add(b => b.Variant, "default")
            .AddChildContent("X")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='badge']").ClassName;
        classes.ShouldContain("theme-badge-default");
        classes.ShouldContain("cn-badge");
    }

    [Fact(DisplayName = "UAC03: Exact variant Replace — default class dropped")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ExactVariantReplace()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<BadgeSut>(p => p
            .Add(b => b.Variant, "outline")
            .AddChildContent("X")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='badge']").ClassName;
        classes.ShouldContain("theme-badge-outline");
        classes!.Contains("cn-badge").ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: Unregistered variant hits wildcard")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_WildcardAppliesForUnregisteredVariant()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<BadgeSut>(p => p
            .Add(b => b.Variant, "secondary")
            .AddChildContent("X")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='badge']").ClassName;
        classes.ShouldContain("theme-badge-wildcard");
    }

    [Fact(DisplayName = "UAC05: Consumer Class is preserved")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ConsumerClassPreserved()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<BadgeSut>(p => p
            .Add(b => b.Class, "layout-badge")
            .AddChildContent("X")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='badge']").ClassName;
        classes.ShouldContain("layout-badge");
    }
}
