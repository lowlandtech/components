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
        Ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}
