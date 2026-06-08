namespace LowlandTech.Components.Tests.SPEC1078.SessionApp;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "03")]
[UserStory("03", "SessionApp runtime pipelines")]
public class SC03_StateHydratesFromGraphOnMount : ViewScenario
{
    private sealed class FakeGraphSession : IGraphSession
    {
        private readonly Dictionary<Guid, Node> _nodes = new();

        public void SetNode(Guid id, Node node) => _nodes[id] = node;

        public Task<T?> GetAsync<T>(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>>? query = null,
            CancellationToken ct = default) where T : class
        {
            if (typeof(T) == typeof(Node) && _nodes.TryGetValue(id, out var node))
                return Task.FromResult((T?)(object?)node);
            return Task.FromResult<T?>(null);
        }

        public IQueryable<T> Query<T>() where T : class
            => throw new NotSupportedException();

        public Task<List<T>> ListAsync<T>(
            Func<IQueryable<T>, IQueryable<T>>? query = null,
            CancellationToken ct = default) where T : class
            => throw new NotSupportedException();

        public Task<T> AddAsync<T>(T entity, CancellationToken ct = default) where T : class
            => throw new NotSupportedException();

        public Task<T?> UpdateAsync<T>(T entity, CancellationToken ct = default) where T : class
            => throw new NotSupportedException();

        public Task<bool> DeleteAsync<T>(Guid id, CancellationToken ct = default) where T : class
            => throw new NotSupportedException();

        public Task<object?> GetAsync(Type entityType, Guid id, CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<object>> ListAsync(Type entityType, CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<object> AddAsync(Type entityType, object entity, CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<object?> UpdateAsync(Type entityType, object entity, CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<bool> DeleteAsync(Type entityType, Guid id, CancellationToken ct = default)
            => throw new NotSupportedException();
    }

    [Fact(DisplayName = "UAC01: SessionApp hydrates state from IGraphSession on mount")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_StateHydratesFromGraph()
    {
        var sessionId = Guid.NewGuid();
        var graphSession = new FakeGraphSession();
        graphSession.SetNode(sessionId, new Node
        {
            Id = sessionId,
            Properties =
            [
                new Property { Key = "agentType", Value = "coder" },
                new Property { Key = "runtimeKey", Value = "claude" }
            ]
        });
        Ctx.Services.AddSingleton<IGraphSession>(graphSession);

        var cut = Ctx.Render<SessionAppSut>(parameters => parameters
            .Add(p => p.SessionId, sessionId.ToString()));
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.IsHydrated.ShouldBeTrue();
        cut.Instance.Context.ShouldNotBeNull();
        cut.Instance.Context.State["agentType"].ShouldBe("coder");
        cut.Instance.Context.State["runtimeKey"].ShouldBe("claude");
    }

    [Fact(DisplayName = "UAC02: Missing node results in IsHydrated false")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_MissingNodeNotHydrated()
    {
        var sessionId = Guid.NewGuid();
        Ctx.Services.AddSingleton<IGraphSession>(new FakeGraphSession());

        var cut = Ctx.Render<SessionAppSut>(parameters => parameters
            .Add(p => p.SessionId, sessionId.ToString()));
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.IsHydrated.ShouldBeFalse();
        cut.Instance.Context.ShouldNotBeNull();
        cut.Instance.Context.State.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC03: No SessionId skips hydration")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_NoSessionIdSkipsHydration()
    {
        var graphSession = new FakeGraphSession();
        Ctx.Services.AddSingleton<IGraphSession>(graphSession);

        var cut = Ctx.Render<SessionAppSut>();
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.IsHydrated.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: No IGraphSession skips hydration gracefully")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_NoGraphSessionSkipsHydration()
    {
        var cut = Ctx.Render<SessionAppSut>(parameters => parameters
            .Add(p => p.SessionId, Guid.NewGuid().ToString()));
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.Instance.IsHydrated.ShouldBeFalse();
        cut.Instance.PipelineComplete.ShouldBeTrue();
    }
}
