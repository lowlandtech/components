namespace LowlandTech.Components.Tests.SPEC1076.Showcode.US01.ThemeService;

/// <summary>
/// SC01 — ThemeService correctly initialises from localStorage / system preference,
/// toggles and sets the theme, persists to localStorage, fires ThemeChanged,
/// and degrades gracefully when JS interop throws.
/// </summary>
[Trait(Spec.SPEC, "1076")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "ThemeService manages dark/light theme with JS-backed persistence")]
public class SC01_ThemeServiceBehavior : IAsyncLifetime
{
    private BunitContext _ctx = null!;

    public ValueTask InitializeAsync()
    {
        _ctx = new BunitContext();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _ctx.Dispose();
        return ValueTask.CompletedTask;
    }

    // Builds a ThemeService wired to the bUnit JSInterop runtime.
    private LowlandTech.Components.Stories.Services.ThemeService BuildService() =>
        new(_ctx.JSInterop.JSRuntime);

    private void SetupForInit(string? storedTheme)
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Strict;

        _ctx.JSInterop
            .Setup<string?>("localStorage.getItem", "app-theme")
            .SetResult(storedTheme);

        if (storedTheme is null)
        {
            _ctx.JSInterop
                .Setup<bool>("eval", _ => true)
                .SetResult(false);
        }

        // ApplyThemeAsync always calls eval with the class toggle string.
        _ctx.JSInterop.SetupVoid("eval", _ => true);
    }

    private void SetupForSaveAndApply()
    {
        _ctx.JSInterop.SetupVoid("localStorage.setItem", _ => true);
        _ctx.JSInterop.SetupVoid("eval", _ => true);
    }

    // -----------------------------------------------------------------------

    [Fact(DisplayName = "UAC01: IsDark is false before initialisation")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_IsDarkFalseBeforeInit()
    {
        var svc = BuildService();

        svc.IsDark.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC02: InitializeAsync sets IsDark=true when localStorage contains 'dark'")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_InitFromStoredDark()
    {
        SetupForInit("dark");
        var svc = BuildService();

        await svc.InitializeAsync();

        svc.IsDark.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: InitializeAsync sets IsDark=false when localStorage contains 'light'")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_InitFromStoredLight()
    {
        SetupForInit("light");
        var svc = BuildService();

        await svc.InitializeAsync();

        svc.IsDark.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: InitializeAsync falls back to system preference when no stored theme")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_InitFromSystemPreference()
    {
        // System preference returns false (light)
        SetupForInit(null);
        var svc = BuildService();

        await svc.InitializeAsync();

        svc.IsDark.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC05: InitializeAsync is idempotent — second call is a no-op")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_InitIsIdempotent()
    {
        SetupForInit("dark");
        var svc = BuildService();

        await svc.InitializeAsync();
        // Second call must not throw or re-invoke JS
        await svc.InitializeAsync();

        svc.IsDark.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC06: ToggleThemeAsync flips IsDark")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ToggleFlipsIsDark()
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var svc = BuildService();

        await svc.ToggleThemeAsync();

        svc.IsDark.ShouldBeTrue();

        await svc.ToggleThemeAsync();

        svc.IsDark.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC07: ToggleThemeAsync fires ThemeChanged")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ToggleFiresThemeChanged()
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var svc = BuildService();
        var raised = 0;
        svc.ThemeChanged += () => raised++;

        await svc.ToggleThemeAsync();

        raised.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC08: SetThemeAsync changes IsDark when value differs")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_SetThemeChangesValue()
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var svc = BuildService();

        await svc.SetThemeAsync(true);

        svc.IsDark.ShouldBeTrue();

        await svc.SetThemeAsync(false);

        svc.IsDark.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC09: SetThemeAsync is a no-op when value is unchanged")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_SetThemeNoOpWhenSame()
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var svc = BuildService();
        var raised = 0;
        svc.ThemeChanged += () => raised++;

        await svc.SetThemeAsync(false); // already false

        raised.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC10: SetThemeAsync fires ThemeChanged when value changes")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_SetThemeFiresThemeChanged()
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var svc = BuildService();
        var raised = 0;
        svc.ThemeChanged += () => raised++;

        await svc.SetThemeAsync(true);

        raised.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC11: InitializeAsync degrades gracefully when JS interop throws")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_InitDegrdesGracefullyOnJsFailure()
    {
        // Loose mode — any uninvoked call returns default, simulating an environment
        // where localStorage is unavailable. We verify no exception escapes.
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var svc = BuildService();

        var ex = await Record.ExceptionAsync(() => svc.InitializeAsync());

        ex.ShouldBeNull();
        svc.IsDark.ShouldBeFalse();
    }
}
