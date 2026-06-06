namespace LowlandTech.Components;

/// <summary>
/// <see cref="BlockBase"/>-derived component base for generated entity pages.
/// Adds a declarative lifecycle-hook pipeline so <c>.Custom.cs</c> partials
/// can register behavior via attributes (<see cref="OnInitializedAttribute"/>,
/// <see cref="OnParametersSetAttribute"/>, <see cref="OnStateChangedAttribute"/>,
/// <see cref="OnAfterRenderAttribute"/>, <see cref="OnDisposeAttribute"/>)
/// without overriding virtual methods.
///
/// <para>
/// One reflection pass on first init collects every attributed method across
/// the component's type hierarchy into <see cref="Action"/> lists. The lists
/// fire on the matching lifecycle event. This lets the generator emit its
/// own setup/teardown methods (Store subscription, initial load dispatch)
/// AND lets plugin authors add feature-specific hooks in Custom.cs without
/// touching the generated code — they annotate methods and the base class
/// picks them up automatically.
/// </para>
///
/// <para>
/// Hook discovery uses <c>BindingFlags.Instance | Public | NonPublic</c>
/// and includes inherited methods, so both partials (generated +
/// hand-edit) contribute. Methods must be parameterless; attribute-bearing
/// methods with parameters are silently ignored.
/// </para>
/// </summary>
public abstract class BlockPageBase : BlockBase, IDisposable
{
    private List<Action>?       _onInitialized;
    private List<Func<Task>>?   _onInitializedAsync;
    private List<Action>?       _onParametersSet;
    private List<Action>?       _onStateChanged;
    private List<Action>?       _onAfterRender;
    private List<Action>?       _onDispose;
    private bool _hooksDiscovered;

    private void DiscoverHooks()
    {
        if (_hooksDiscovered) return;
        _hooksDiscovered = true;

        var type = GetType();
        // BindingFlags.FlattenHierarchy only works for static members, so walk
        // the inheritance chain manually to pick up inherited attributed methods.
        while (type is not null && type != typeof(object))
        {
            var methods = type.GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            foreach (var m in methods)
            {
                if (m.GetParameters().Length != 0) continue;
                if (m.ReturnType == typeof(void))
                {
                    if (m.GetCustomAttribute<OnInitializedAttribute>() is not null)
                        (_onInitialized ??= []).Add(() => m.Invoke(this, null));
                    if (m.GetCustomAttribute<OnParametersSetAttribute>() is not null)
                        (_onParametersSet ??= []).Add(() => m.Invoke(this, null));
                    if (m.GetCustomAttribute<OnStateChangedAttribute>() is not null)
                        (_onStateChanged ??= []).Add(() => m.Invoke(this, null));
                    if (m.GetCustomAttribute<OnAfterRenderAttribute>() is not null)
                        (_onAfterRender ??= []).Add(() => m.Invoke(this, null));
                    if (m.GetCustomAttribute<OnDisposeAttribute>() is not null)
                        (_onDispose ??= []).Add(() => m.Invoke(this, null));
                }
                else if (m.ReturnType == typeof(Task))
                {
                    if (m.GetCustomAttribute<OnInitializedAttribute>() is not null)
                        (_onInitializedAsync ??= []).Add(() => (Task)m.Invoke(this, null)!);
                }
            }
            type = type.BaseType;
        }
    }

    /// <summary>Hook wiring runs here so attributed methods see fully-set
    /// parameters (OnInitialized runs after parameters are applied).
    /// Sync hooks fire inline; async hooks fire fire-and-forget via InvokeAsync
    /// so the component can immediately continue to render while the async work
    /// (e.g. HTTP data fetch) proceeds in the background.</summary>
    protected override void OnInitialized()
    {
        DiscoverHooks();
        base.OnInitialized();
        InvokeAll(_onInitialized);
        if (_onInitializedAsync is not null)
            foreach (var a in _onInitializedAsync.ToArray())
                _ = InvokeAsync(a);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        InvokeAll(_onParametersSet);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        InvokeAll(_onAfterRender);
    }

    /// <summary>Call from the backing <c>Store&lt;T&gt;</c>'s <c>StateChanged</c>
    /// handler to fan out to every <c>[OnStateChanged]</c>-attributed method
    /// and schedule a re-render. Generated pages wire a one-line bridge —
    /// authors don't call this directly.</summary>
    protected void RaiseStateChanged()
    {
        InvokeAll(_onStateChanged);
        InvokeAsync(StateHasChanged);
    }

    public override void Dispose()
    {
        InvokeAll(_onDispose);
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    private static void InvokeAll(List<Action>? actions)
    {
        if (actions is null) return;
        // Copy so a hook that adds/removes hooks during invocation doesn't
        // mutate the list we're walking.
        foreach (var a in actions.ToArray()) a();
    }
}
