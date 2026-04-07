using Bunit;
using LowlandTech.Components.Popover;
using LowlandTech.Components.Sonner;
using Microsoft.Extensions.DependencyInjection;

namespace LowlandTech.Components.Tests.Infrastructure;

public abstract class ViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public virtual ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
        Ctx.Services.AddSingleton<IPopoverService, PopoverService>();
        Ctx.Services.AddSingleton<ISonnerService, SonnerService>();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}
