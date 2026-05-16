namespace LowlandTech.Components;

public sealed record AppLayoutContext(
    AppLayoutScenario Scenario,
    string SessionKey,
    bool HasTopBar,
    bool HasSidebar,
    bool HasAside,
    bool HasFooter);

