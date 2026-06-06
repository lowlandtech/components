using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1080.Work.Infrastructure;

public abstract class WorkScenario : ViewScenario
{
    protected WorkStateService StateService = null!;

    public override ValueTask InitializeAsync()
    {
        base.InitializeAsync();
        StateService = new WorkStateService();
        return ValueTask.CompletedTask;
    }

    protected IRenderedComponent<WorkApp> RenderWorkApp()
        => Ctx.Render<WorkApp>(p => p
            .Add(x => x.StateService, StateService)
            .Add(x => x.IsTest, true));

    protected IRenderedComponent<WorkApp> RenderWorkAppWithState(WorkAppState state)
    {
        StateService = new WorkStateService(state);
        return RenderWorkApp();
    }
}
