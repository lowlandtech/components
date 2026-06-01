namespace LowlandTech.Components.Tests.SPEC1010.Card.US02.ThemeSlot;

// ---------------------------------------------------------------------------
// Test theme: adds class slots for Card.Root
// ---------------------------------------------------------------------------
internal sealed class CardSlotTheme : IComponentTheme, IComponentClassTheme
{
    public string Name => "CardSlot";
    public IReadOnlyDictionary<string, string> Tokens { get; } =
        new Dictionary<string, string> { ["--background"] = "#000" };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            // Wildcard — applies to all Card.Root slots regardless of variant/size
            [new("Card", "Root")] = new("theme-card-root", ThemeClassMergeMode.Append),
        };
}

// ---------------------------------------------------------------------------

/// <summary>
/// SC01 — Card component respects ThemeProvider class slot injection.
/// </summary>
[Trait(Spec.SPEC, "1010")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Card component respects ThemeProvider class slots")]
public class SC01_CardThemeSlot : JsViewScenario
{
    private ThemeProviderContext BuildContext(string providerId = "p1")
    {
        var services = new ServiceCollection();
        services.AddSingleton<IComponentTheme>(new CardSlotTheme());
        services.AddScoped<ComponentThemeService>();
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ComponentThemeService>();
        svc.Register(providerId, "CardSlot");
        return new ThemeProviderContext(providerId, svc);
    }

    [Fact(DisplayName = "UAC01: No ThemeContext — renders without theme slot class")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NoContext_NoThemeClass()
    {
        var cut = Ctx.Render<CardSut>(p => p.AddChildContent("Body"));
        var classes = cut.Find("[data-slot='card']").ClassName;
        classes!.Contains("theme-card-root").ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: Wildcard slot applies — theme class added")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_WildcardSlotApplied()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<CardSut>(p => p
            .AddChildContent("Body")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='card']").ClassName;
        classes.ShouldContain("theme-card-root");
    }

    [Fact(DisplayName = "UAC03: Default card classes still present (Append mode)")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DefaultClassesPreservedInAppendMode()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<CardSut>(p => p
            .AddChildContent("Body")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='card']").ClassName;
        // Card has a default class containing "cn-card"
        classes.ShouldContain("cn-card");
        classes.ShouldContain("theme-card-root");
    }

    [Fact(DisplayName = "UAC04: Consumer Class is preserved")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ConsumerClassPreserved()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<CardSut>(p => p
            .Add(c => c.Class, "layout-card")
            .AddChildContent("Body")
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='card']").ClassName;
        classes.ShouldContain("layout-card");
    }
}
