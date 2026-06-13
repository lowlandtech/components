namespace LowlandTech.Components;

public sealed record FakeWorkspace(Guid Id, string Name);
public sealed record FakeFlow(Guid Id, string Name);
public sealed record FakeStep(Guid Id, string Name);
public sealed record FakePackage(Guid Id, string Name);

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
