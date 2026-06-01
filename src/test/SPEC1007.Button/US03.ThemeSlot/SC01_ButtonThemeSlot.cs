namespace LowlandTech.Components.Tests.SPEC1007.Button.US03.ThemeSlot;

// ---------------------------------------------------------------------------
// Test theme: adds class slots for Button.Root in default and outline variants
// ---------------------------------------------------------------------------
internal sealed class ButtonSlotTheme : IComponentTheme, IComponentClassTheme
{
    public string Name => "ButtonSlot";
    public IReadOnlyDictionary<string, string> Tokens { get; } =
        new Dictionary<string, string> { ["--background"] = "#000" };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new("Button", "Root", Variant: "default")] = new("theme-default", ThemeClassMergeMode.Append),
            [new("Button", "Root", Variant: "outline")] = new("theme-outline", ThemeClassMergeMode.Replace),
            [new("Button", "Root")] = new("theme-wildcard", ThemeClassMergeMode.Append),
        };
}

// ---------------------------------------------------------------------------

/// <summary>
/// SC01 — Button component respects ThemeProvider class slot injection when rendered
/// inside a cascading ThemeProviderContext.
/// </summary>
[Trait(Spec.SPEC, "1007")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "Button component respects ThemeProvider class slots")]
public class SC01_ButtonThemeSlot : JsViewScenario
{
    private ThemeProviderContext BuildContext(string providerId = "p1")
    {
        var services = new ServiceCollection();
        services.AddSingleton<IComponentTheme>(new ButtonSlotTheme());
        services.AddScoped<ComponentThemeService>();
        // Use a long-lived provider so the scoped service outlives the test method.
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ComponentThemeService>();
        svc.Register(providerId, "ButtonSlot");
        return new ThemeProviderContext(providerId, svc);
    }

    // -----------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: No ThemeContext — renders default classes only")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NoContext_DefaultClasses()
    {
        var cut = Ctx.Render<LowlandTech.Components.Button.Button>(p => p
            .Add(b => b.Variant, "default")
            .Add(b => b.ChildContent, (RenderFragment)(rb => rb.AddContent(0, "Click"))));
        var classes = cut.Find("[data-slot='button']").ClassName;
        classes.ShouldNotBeNullOrWhiteSpace();
        classes.ShouldContain("btn");
    }

    [Fact(DisplayName = "UAC02: Exact variant slot — class appended to default")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ExactVariantSlot_Append()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<LowlandTech.Components.Button.Button>(p => p
            .Add(b => b.Variant, "default")
            .Add(b => b.ChildContent, (RenderFragment)(rb => rb.AddContent(0, "X")))
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='button']").ClassName;
        classes.ShouldContain("theme-default");
        classes.ShouldContain("btn");
    }

    [Fact(DisplayName = "UAC03: Exact variant Replace slot — default class dropped")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ExactVariantSlot_Replace()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<LowlandTech.Components.Button.Button>(p => p
            .Add(b => b.Variant, "outline")
            .Add(b => b.ChildContent, (RenderFragment)(rb => rb.AddContent(0, "X")))
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='button']").ClassName;
        classes.ShouldContain("theme-outline");
        // Replace mode: default "btn" class should NOT be present
        classes!.Contains("btn").ShouldBeFalse("Replace mode should drop the default class");
    }

    [Fact(DisplayName = "UAC04: No exact slot, wildcard applies")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_WildcardSlotApplies()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<LowlandTech.Components.Button.Button>(p => p
            .Add(b => b.Variant, "ghost")   // no exact ghost slot — hits wildcard
            .Add(b => b.ChildContent, (RenderFragment)(rb => rb.AddContent(0, "X")))
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='button']").ClassName;
        classes.ShouldContain("theme-wildcard");
    }

    [Fact(DisplayName = "UAC05: Consumer Class parameter preserved at the end")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ConsumerClassPreserved()
    {
        var ctx = BuildContext();
        var cut = Ctx.Render<LowlandTech.Components.Button.Button>(p => p
            .Add(b => b.Variant, "default")
            .Add(b => b.Class, "my-custom")
            .Add(b => b.ChildContent, (RenderFragment)(rb => rb.AddContent(0, "X")))
            .AddCascadingValue(ctx));
        var classes = cut.Find("[data-slot='button']").ClassName;
        classes.ShouldContain("my-custom");
    }
}
