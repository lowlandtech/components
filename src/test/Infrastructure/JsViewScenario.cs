using LowlandTech.Components.Sonner;

namespace LowlandTech.Components.Tests.Infrastructure;

/// <summary>
/// ViewScenario variant that enables loose JSInterop mode for components
/// that call into JS (e.g. ContainerGrid, ContainerGridSection).
/// </summary>
public abstract class JsViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public virtual ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
        Ctx.JSInterop.Mode = JSRuntimeMode.Loose;
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
