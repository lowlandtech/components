namespace LowlandTech.Components.Tests;

/// <summary>
/// Base class for all test scenarios providing the arrange-act-assert pattern.
/// Corresponds to Given/When/Then steps in Gherkin feature files.
/// </summary>
public abstract class Scenario : IAsyncLifetime
{
    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual Task DisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Arrange phase - set up test preconditions and test data.
    /// Corresponds to "Given" steps in Gherkin feature files.
    /// </summary>
    protected virtual Task GivenAsync() => Task.CompletedTask;

    /// <summary>
    /// Act phase - execute the action being tested.
    /// Corresponds to "When" steps in Gherkin feature files.
    /// </summary>
    protected virtual Task WhenAsync() => Task.CompletedTask;

    /// <summary>
    /// Execute both arrange and act phases. Call this from each test method
    /// before performing assertions (the "Then" phase).
    /// </summary>
    protected async Task ArrangeAndActAsync()
    {
        await GivenAsync();
        await WhenAsync();
    }
}
