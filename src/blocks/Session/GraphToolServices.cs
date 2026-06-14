namespace LowlandTech.Components;

public sealed record FakeWorkspace(Guid Id, string Name);
public sealed record FakeFlow(Guid Id, string Name);
public sealed record FakeStep(Guid Id, string Name);
public sealed record FakePackage(Guid Id, string Name);
public sealed record FakeWorkitem(Guid Id, string Name);

/// <summary>
/// Components-environment creator for <c>workspaces.header</c> → <c>workspace</c>.
/// Real hosts can register a different <see cref="IGraphToolService"/> with the
/// same <see cref="ToolKey"/>.
/// </summary>
public sealed class WorkspaceCreatorTool : IGraphToolService
{
    public string ToolKey => "WorkspaceCreator";

    /// <summary>
    /// Optional hook for tests to observe tool execution without re-registering the service.
    /// </summary>
    public Action<object, string>? OnExecuted { get; set; }

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        var title = name ?? "New Workspace";
        var result = new FakeWorkspace(Guid.NewGuid(), title);
        OnExecuted?.Invoke(result, title);
        return Task.FromResult<object>(result);
    }
}

/// <summary>
/// Components-environment creator for <c>flow.header</c> → <c>flow</c>.
/// </summary>
public sealed class FlowCreatorTool : IGraphToolService
{
    public string ToolKey => "FlowCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeFlow(Guid.NewGuid(), name ?? "New Flow"));
    }
}

/// <summary>
/// Components-environment creator for <c>step.header</c> → <c>step</c>.
/// </summary>
public sealed class StepCreatorTool : IGraphToolService
{
    public string ToolKey => "StepCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeStep(Guid.NewGuid(), name ?? "New Step"));
    }
}

/// <summary>
/// Components-environment creator for <c>package.header</c> → <c>package</c>.
/// </summary>
public sealed class PackageCreatorTool : IGraphToolService
{
    public string ToolKey => "PackageCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakePackage(Guid.NewGuid(), name ?? "New Package"));
    }
}

/// <summary>
/// Components-environment creator for <c>workitems.header</c> → <c>workitem</c>.
/// </summary>
public sealed class WorkitemCreatorTool : IGraphToolService
{
    public string ToolKey => "WorkitemCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeWorkitem(Guid.NewGuid(), name ?? "New Workitem"));
    }
}

public sealed record FakeUserStory(Guid Id, string Name);
public sealed record FakeScenario(Guid Id, string Name);
public sealed record FakeScenarioStep(Guid Id, string Name);
public sealed record FakeAction(Guid Id, string Name);
public sealed record FakeHandler(Guid Id, string Name);
public sealed record FakeEvent(Guid Id, string Name);

/// <summary>
/// Components-environment creator for <c>use-case</c> → <c>user-story</c>.
/// </summary>
public sealed class UserStoryCreatorTool : IGraphToolService
{
    public string ToolKey => "UserStoryCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeUserStory(Guid.NewGuid(), name ?? "New User Story"));
    }
}

/// <summary>
/// Components-environment creator for <c>user-story</c> → <c>scenario</c>.
/// </summary>
public sealed class ScenarioCreatorTool : IGraphToolService
{
    public string ToolKey => "ScenarioCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeScenario(Guid.NewGuid(), name ?? "New Scenario"));
    }
}

/// <summary>
/// Components-environment creator for <c>scenario</c> → <c>scenario-step</c>.
/// </summary>
public sealed class ScenarioStepCreatorTool : IGraphToolService
{
    public string ToolKey => "ScenarioStepCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeScenarioStep(Guid.NewGuid(), name ?? "New Scenario Step"));
    }
}

/// <summary>
/// Components-environment creator for <c>scenario-step</c> → <c>action</c>.
/// </summary>
public sealed class ActionCreatorTool : IGraphToolService
{
    public string ToolKey => "ActionCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeAction(Guid.NewGuid(), name ?? "New Action"));
    }
}

/// <summary>
/// Components-environment creator for <c>scenario-step</c> → <c>handler</c>.
/// </summary>
public sealed class HandlerCreatorTool : IGraphToolService
{
    public string ToolKey => "HandlerCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeHandler(Guid.NewGuid(), name ?? "New Handler"));
    }
}

/// <summary>
/// Components-environment creator for <c>scenario-step</c> → <c>event</c>.
/// </summary>
public sealed class EventCreatorTool : IGraphToolService
{
    public string ToolKey => "EventCreator";

    public Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default)
    {
        var name = properties.TryGetValue("Title", out var value) ? value?.ToString() : null;
        return Task.FromResult<object>(new FakeEvent(Guid.NewGuid(), name ?? "New Event"));
    }
}
