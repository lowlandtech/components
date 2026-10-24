using LowlandTech.Components;
using LowlandTech.ThemeConsumer;
using LowlandTech.ThemeConsumer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddLocalization();

// The host registers its own theme first, so the fallback of the resolver is
// the host theme and the withdrawal of a plugin owner restores it.
builder.Services.AddSingleton<ConsumerHostTheme>();
builder.Services.AddSingleton<ConsumerPluginTheme>();

builder.Services.AddSingleton<IComponentTheme>(
    serviceProvider => serviceProvider.GetRequiredService<ConsumerHostTheme>());

builder.Services.AddSingleton<IComponentTheme>(
    serviceProvider => serviceProvider.GetRequiredService<ConsumerPluginTheme>());

// The resolver is scoped, exactly as the demo host registers it, so every
// circuit resolves its own provider state and a withdrawal never reaches a
// circuit that did not select the theme.
builder.Services.AddScoped<ComponentThemeService>();

// The authority of the host over the plugin owners is the only state the
// circuits share.
builder.Services.AddSingleton<ThemeAuthority>();

// The bridge is scoped as well: it projects the authority onto the resolver of
// its own circuit. There is no global provider selection store.
builder.Services.AddScoped(serviceProvider => new ComponentThemeOwnerBridge(
    serviceProvider.GetRequiredService<ComponentThemeService>(),
    serviceProvider.GetRequiredService<ThemeAuthority>().Snapshot));

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// The host withdraws the owner of the theme: the next revision of the
// authority names no active owner, and every live circuit follows it.
app.MapPost("/withdraw", (ThemeAuthority authority) =>
{
    authority.Deliver(new ComponentThemeOwnerSnapshot(2, []));

    return Results.NoContent();
});

// The host activates the owner again, so a browser can prove that a
// reactivation restores the theme and its stylesheet selection.
app.MapPost("/activate", (ThemeAuthority authority) =>
{
    authority.Deliver(new ComponentThemeOwnerSnapshot(3, [ThemeAuthority.OwnerKey]));

    return Results.NoContent();
});

app.Run();
