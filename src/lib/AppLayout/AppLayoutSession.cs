namespace LowlandTech.Components;

public sealed class AppLayoutSession
{
    public AppLayoutSession(AppLayoutScenario scenario = AppLayoutScenario.Workspace, string? key = null)
    {
        Scenario = scenario;
        Key = key ?? "default";
    }

    public string Key { get; }

    public AppLayoutScenario Scenario { get; private set; }

    public event EventHandler<AppLayoutScenario>? ScenarioChanged;

    public void SetScenario(AppLayoutScenario scenario)
    {
        if (Scenario == scenario)
            return;

        Scenario = scenario;
        ScenarioChanged?.Invoke(this, scenario);
    }
}

