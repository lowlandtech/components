var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<ISonnerService, SonnerService>();
builder.Services.AddScoped<IPopoverService, PopoverService>();
builder.Services.AddScoped<IPageOverlayService, PageOverlayService>();
builder.Services.AddScoped<ShowcodeState>();
builder.Services.AddScoped<WorkStateService>();
builder.Services.AddDesignableComponents();

// Agent graph services — DRD pattern for session agent types
builder.Services.AddScoped<IAgentGraphService, AskAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, PlanAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, EditAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, RunAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, CreateAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, ExecuteAgentGraphService>();
builder.Services.AddScoped<IAgentGraphRegistry, AgentGraphRegistry>();
builder.Services.AddScoped<IGraphDiscoveryService, GraphDiscoveryService>();

// Scoped theming system — themes are stateless token bags (singleton), service holds per-session state (scoped)
builder.Services.AddSingleton<IComponentTheme, LightComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, DarkComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, OceanComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, RoseComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, ForestComponentTheme>();
builder.Services.AddScoped<ComponentThemeService>();

StoryRegistry.RegisterAllStories(typeof(ShowcodeLayout).Assembly);

await builder.Build().RunAsync();
