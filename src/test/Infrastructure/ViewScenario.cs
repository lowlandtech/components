using Bunit;

namespace LowlandTech.Components.Tests.Infrastructure;

public abstract class ViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}
