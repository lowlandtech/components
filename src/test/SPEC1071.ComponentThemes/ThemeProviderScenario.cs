using Bunit;
using Microsoft.Extensions.DependencyInjection;

using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace LowlandTech.Components.Tests.SPEC1071.ComponentThemes;

/// <summary>
/// Shared bUnit base class for SPEC1071 tests that render ThemeProvider or
/// ThemeToggle components.  Extends <see cref="JsViewScenario"/> (loose JS
/// interop) and pre-registers all five stub <see cref="IComponentTheme"/>
/// implementations plus a scoped <see cref="ComponentThemeService"/>.
/// </summary>
public abstract class ThemeProviderScenario : JsViewScenario
{
    public override ValueTask InitializeAsync()
    {
        var t = base.InitializeAsync();
        Ctx.Services.AddSingleton<IComponentTheme, TestLightTheme>();
        Ctx.Services.AddSingleton<IComponentTheme, TestDarkTheme>();
        Ctx.Services.AddSingleton<IComponentTheme, TestOceanTheme>();
        Ctx.Services.AddSingleton<IComponentTheme, TestRoseTheme>();
        Ctx.Services.AddSingleton<IComponentTheme, TestForestTheme>();
        Ctx.Services.AddScoped<ComponentThemeService>();
        return t;
    }
}
