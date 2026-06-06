using LowlandTech.Components;
using RepositoryAppComponent = LowlandTech.Components.RepositoryApp;

namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.Infrastructure;

public abstract class RepositoryScenario : ViewScenario
{
    protected RepositoryStateService StateService = null!;

    public override ValueTask InitializeAsync()
    {
        base.InitializeAsync();
        StateService = new RepositoryStateService(RepositoryUseCaseData.Primary());
        return ValueTask.CompletedTask;
    }

    protected IRenderedComponent<RepositoryAppComponent> RenderRepositoryApp()
        => Ctx.Render<RepositoryAppComponent>(p => p
            .Add(x => x.StateService, StateService)
            .Add(x => x.IsTest, true));

    protected IRenderedComponent<RepositoryAppComponent> RenderRepositoryAppWithState(RepositoryAppState state)
    {
        StateService = new RepositoryStateService(state);
        return RenderRepositoryApp();
    }
}
