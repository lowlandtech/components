using Bunit;
using LowlandTech.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LowlandTech.Components.Tests.Infrastructure;

public abstract class ViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public virtual ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
        Ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        Ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        Ctx.Services.AddSingleton<IPopoverService, PopoverService>();
        Ctx.Services.AddSingleton<ISonnerService, SonnerService>();
        Ctx.Services.AddSingleton<IPageOverlayService, PageOverlayService>();
        Ctx.Services.AddGraphDescriptors(typeof(INodeTypeDescriptor).Assembly);
        Ctx.Services.AddSingleton<IEdgeTypeService, EdgeTypeService>();
        Ctx.Services.AddSingleton<IExecutionEngine>(sp => new ComponentsExecutionEngine(sp.GetRequiredService<IEdgeTypeService>()) { UserId = "test-user" });
        Ctx.Services.AddSingleton<IGraphToolService, WorkspaceCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, FlowCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, StepCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, PackageCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, WorkitemCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, UserStoryCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, ScenarioCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, ScenarioStepCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, ActionCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, HandlerCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, EventCreatorTool>();
        Ctx.Services.AddSingleton<IAgentGraphService, AskAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, PlanAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, EditAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, RunAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, CreateAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, ExecuteAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphRegistry, AgentGraphRegistry>();
        Ctx.Services.AddSingleton<IGraphDiscoveryService, GraphDiscoveryService>();
        Ctx.Services.AddScoped<ISessionSurfaceDiscovery, SessionSurfaceDiscovery>();
        Ctx.Services.AddSingleton<ISessionBlockRegistry, DefaultSessionBlockRegistry>();
        Ctx.Services.AddSingleton<ISessionBlockRegistry, AssemblyScanningSessionBlockRegistry>();
        Ctx.Services.AddSingleton<ISessionBlockDiscovery, SessionBlockDiscovery>();
        Ctx.Services.AddSingleton<IAgentSelectionService, AgentSelectionService>();
        Ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}
