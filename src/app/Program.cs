var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<ISonnerService, SonnerService>();
builder.Services.AddScoped<ShowcodeState>();

// Scoped theming system — themes are stateless token bags (singleton), service holds per-session state (scoped)
builder.Services.AddSingleton<IComponentTheme, LightComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, DarkComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, OceanComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, RoseComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, ForestComponentTheme>();
builder.Services.AddScoped<ComponentThemeService>();

StoryRegistry.RegisterAllStories(typeof(ShowcodeLayout).Assembly);

await builder.Build().RunAsync();
