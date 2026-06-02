namespace LowlandTech.Components.Tests.Infrastructure;

/// <summary>
/// ViewScenario variant that enables loose JSInterop mode for components
/// that call into JS. Now that ViewScenario itself uses loose mode, this
/// class is kept for backwards compatibility and simply extends it.
/// </summary>
public abstract class JsViewScenario : ViewScenario
{
}
