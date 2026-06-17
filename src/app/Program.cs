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
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddScoped<ISessionSurfaceDiscovery, SessionSurfaceDiscovery>();
builder.Services.AddSingleton<ISessionBlockRegistry, DefaultSessionBlockRegistry>();
builder.Services.AddSingleton<ISessionBlockRegistry, AssemblyScanningSessionBlockRegistry>();
builder.Services.AddSingleton<ISessionBlockDiscovery, SessionBlockDiscovery>();
builder.Services.AddSingleton<IDynamicBlockRegistry, DefaultDynamicBlockRegistry>();
builder.Services.AddSingleton<IDynamicBlockDiscovery, DynamicBlockDiscovery>();
builder.Services.AddSingleton<IDynamicReducerArm, DynamicInitializerReducerArm>();
builder.Services.AddSingleton<IDynamicReducerArm, DynamicFieldReducerArm>();
builder.Services.AddSingleton<IDynamicReducerArm, DynamicEntityMutationReducerArm>();
builder.Services.AddDesignableComponents();

// Graph descriptors + edge-type-driven creation support
builder.Services.AddGraphDescriptors(typeof(INodeTypeDescriptor).Assembly);
builder.Services.AddScoped<IEdgeTypeService, EdgeTypeService>();
builder.Services.AddScoped<IExecutionEngine>(sp => new ComponentsExecutionEngine(sp.GetRequiredService<IEdgeTypeService>()) { UserId = "components-user" });
builder.Services.AddScoped<IGraphToolService, WorkspaceCreatorTool>();
builder.Services.AddScoped<IGraphToolService, TableCreatorTool>();
builder.Services.AddScoped<IGraphToolService, WorkflowCreatorTool>();
builder.Services.AddScoped<IGraphToolService, PageCreatorTool>();
builder.Services.AddScoped<IGraphToolService, FeatureCreatorTool>();
builder.Services.AddScoped<IGraphToolService, BranchCreatorTool>();
builder.Services.AddScoped<IGraphToolService, FlowCreatorTool>();
builder.Services.AddScoped<IGraphToolService, StepCreatorTool>();
builder.Services.AddScoped<IGraphToolService, PackageCreatorTool>();
builder.Services.AddScoped<IGraphToolService, WorkitemCreatorTool>();
builder.Services.AddScoped<IGraphToolService, UserStoryCreatorTool>();
builder.Services.AddScoped<IGraphToolService, ScenarioCreatorTool>();
builder.Services.AddScoped<IGraphToolService, ScenarioStepCreatorTool>();
builder.Services.AddScoped<IGraphToolService, ActionCreatorTool>();
builder.Services.AddScoped<IGraphToolService, HandlerCreatorTool>();
builder.Services.AddScoped<IGraphToolService, EventCreatorTool>();

// Agent graph services — DRD pattern for session agent types
builder.Services.AddScoped<IAgentGraphService, AskAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, PlanAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, EditAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, RunAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, CreateAgentGraphService>();
builder.Services.AddScoped<IAgentGraphService, ExecuteAgentGraphService>();
builder.Services.AddScoped<IAgentGraphRegistry, AgentGraphRegistry>();
builder.Services.AddScoped<IGraphDiscoveryService, GraphDiscoveryService>();
builder.Services.AddScoped<IAgentSelectionService, AgentSelectionService>();

// Scoped theming system — themes are stateless token bags (singleton), service holds per-session state (scoped)
builder.Services.AddSingleton<IComponentTheme, LightComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, DarkComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, OceanComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, RoseComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, ForestComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, GlassComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, ChadcnBlackComponentTheme>();
builder.Services.AddSingleton<IComponentTheme, CorporateGreyComponentTheme>();
builder.Services.AddScoped<ComponentThemeService>();

StoryRegistry.RegisterAllStories(typeof(ShowcodeLayout).Assembly);

await builder.Build().RunAsync();
