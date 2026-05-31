using Bunit;
using LowlandTech.Components.Card;
using LowlandTech.Components.Popover;
using LowlandTech.Components.Session;
using LowlandTech.Components.Session.Agents;
using LowlandTech.Components.Sonner;
using Microsoft.Extensions.DependencyInjection;

namespace LowlandTech.Components.Tests.Infrastructure;

public abstract class ViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public virtual ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
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
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}
