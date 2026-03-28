using Bunit;
using LowlandTech.Components.Popover;
using Microsoft.Extensions.DependencyInjection;

namespace LowlandTech.Components.Tests.Infrastructure;

public abstract class ViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
        Ctx.Services.AddSingleton<IPopoverService, PopoverService>();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}
