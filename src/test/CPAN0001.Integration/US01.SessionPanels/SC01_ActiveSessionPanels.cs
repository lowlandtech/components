namespace LowlandTech.Components.Tests.CPAN0001.Integration.US01.SessionPanels;

/// <summary>
/// SC01 — Project active session panels.
///
/// <para>
/// A Components host renders session panels only for the plugin owners that are
/// active, through a reusable optional bridge. The base DI-only mode, the
/// existing placements and the existing settings semantics stay intact.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPAN0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers render session panels only for active plugin owners through a reusable optional bridge")]
public sealed class SC01_ActiveSessionPanels : ViewScenario
{
    private const string ActiveOwner = "owner.active";
    private const string InactiveOwner = "owner.inactive";

    private readonly FakeSessionPluginSettingsStore _settings = new();
    private readonly List<(string Owner, ISessionPlugin Plugin)> _owned = [];
    private SessionPluginOwnerBridge? _bridge;
    private SessionPluginOwnerBridge? _secondBridge;
    private int _reloadRequests;

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        _owned.Add((ActiveOwner, Panel("owner.active.sidebar", SessionPluginPlacement.RightSidebar, order: 10)));
        _owned.Add((ActiveOwner, Panel(
            "owner.active.protected",
            SessionPluginPlacement.RightSidebar,
            order: 5,
            isProtected: true,
            isOrderLocked: true)));
        _owned.Add((ActiveOwner, Panel("owner.active.workspace", SessionPluginPlacement.WorkspaceTab, order: 20)));
        _owned.Add((ActiveOwner, Panel("owner.active.footer", SessionPluginPlacement.FooterTool, order: 30)));
        _owned.Add((InactiveOwner, Panel("owner.inactive.sidebar", SessionPluginPlacement.RightSidebar, order: 15)));

        // The host persisted the enablement and the order of its panels.
        await _settings.ToggleEnabledAsync("owner.active.sidebar", false);
        await _settings.ReorderTabsAsync(["owner.active.sidebar", "owner.active.protected"]);

        // The bridge is registered once the panels exist, because the bridge
        // takes the panels of the host when it is created.
        Ctx.Services.AddLogging();
        Ctx.Services.AddSingleton<ISessionPluginSettingsStore>(_settings);
        Ctx.Services.AddSingleton<ISessionPluginRegistry>(Registry);
    }

    [Fact(DisplayName = "UAC01: only the panels of active owners appear in the selected placement, in the ListAsync and the GetAllPlugins path")]
    [Trait(Spec.UAC, "01")]
    [Given("active and inactive panel owners contributing panels at RightSidebar, WorkspaceTab and FooterTool")]
    [When("a host queries its selected placement and lists all plugins")]
    [Then("only the active matching panels appear in both paths, on the existing stable slots")]
    public void UAC01_OnlyActiveOwnersAppearInTheSelectedPlacement() =>
        Registry.ShouldSatisfyAllConditions(
            () => typeof(SessionPluginHost).GetProperty("Placement").ShouldNotBeNull(
                "The session plugin host does not expose the host placement as a parameter."),
            () => typeof(SessionPluginHost).GetProperty("Placement")!.PropertyType.ShouldBe(
                typeof(SessionPluginPlacement),
                "The host placement parameter is not a session plugin placement."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).Select(item => item.Key).ShouldBe(
                ["owner.active.sidebar", "owner.active.protected"],
                "The right sidebar does not carry exactly the active panels of the selected placement."),
            () => ListAsync(SessionPluginPlacement.WorkspaceTab).Select(item => item.Key).ShouldBe(
                ["owner.active.workspace"],
                "The workspace tab does not carry exactly the active panels of the selected placement."),
            () => ListAsync(SessionPluginPlacement.FooterTool).Select(item => item.Key).ShouldBe(
                ["owner.active.footer"],
                "The footer tool does not carry exactly the active panels of the selected placement."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldAllBe(
                item => item.Key.StartsWith(ActiveOwner, StringComparison.Ordinal),
                "A panel of an inactive owner is projected onto the host."),
            () => Registry.GetAllPlugins().Select(plugin => plugin.Descriptor.Key).ShouldBe(
                ["owner.active.sidebar", "owner.active.protected", "owner.active.workspace", "owner.active.footer"],
                "The GetAllPlugins path does not carry exactly the panels of the active owners."),
            () => Registry.GetAllPlugins().ShouldAllBe(
                plugin => plugin.Descriptor.Placement == SessionPluginPlacement.RightSidebar
                          || plugin.Descriptor.Placement == SessionPluginPlacement.WorkspaceTab
                          || plugin.Descriptor.Placement == SessionPluginPlacement.FooterTool,
                "The bridge projects a panel onto a slot that is not one of the existing stable placements."),
            () => RenderHostShouldShowOnlyActivePanels().ShouldBeTrue(
                "The host renders a panel of an inactive owner."),
            () => RenderHostHonorsThePlacementParameter().ShouldBeTrue(
                "The host ignores the placement its host selected."));

    [Fact(DisplayName = "UAC02: badges, visibility, callbacks and ordering keep the existing settings semantics and a protected setting cannot survive a deactivation")]
    [Trait(Spec.UAC, "02")]
    [Given("persisted enablement and order with a protected and an order locked descriptor")]
    [When("the settings change and the owner is deactivated and reactivated")]
    [Then("badges, visibility, callbacks and ordering retain the existing semantics and the preferences survive the deactivation")]
    public void UAC02_SettingsSemanticsSurviveDeactivationAndReactivation() =>
        Registry.ShouldSatisfyAllConditions(
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldAllBe(
                item => item.Placement == SessionPluginPlacement.RightSidebar,
                "The bridged panel lost its placement."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldContain(
                item => item.Key == "owner.active.protected" && item.IsVisible,
                "A protected panel was hidden by the persisted enablement, which the existing semantics do not allow."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldContain(
                item => item.Key == "owner.active.sidebar" && !item.IsVisible,
                "A panel that the host disabled is still visible."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldContain(
                item => item.Key == "owner.active.protected" && item.Badge == "3",
                "The badge of a visible panel is not reported."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldContain(
                item => item.Key == "owner.active.sidebar" && item.Badge == null,
                "A panel that is not visible still reports a badge."),
            () => ListAsync(SessionPluginPlacement.RightSidebar).ShouldContain(
                item => item.Key == "owner.active.protected" && item.IsOrderLocked,
                "The order locked descriptor lost its flag."),
            () => _reloadRequests.ShouldBeGreaterThan(0,
                "The host was not asked to reload although a panel asked for it."),
            () => PreferencesSurviveDeactivationAndReactivation().ShouldBeTrue(
                "The deactivation of the owner erased the persisted preferences."),
            () => ProtectedPanelDoesNotSurviveDeactivation().ShouldBeTrue(
                "A protected panel survived the deactivation of its owner."));

    [Fact(DisplayName = "UAC03: a withdrawal clears the retained fragment in both consumers on the next revision and shows the host fallback")]
    [Trait(Spec.UAC, "03")]
    [Given("two live consumers and a retained selected fragment")]
    [When("the owner withdraws and a new revision is delivered")]
    [Then("both clear the fragment and show the host fallback, stale callbacks recheck host authority and no new handler is admitted")]
    public void UAC03_AWithdrawalClearsTheRetainedFragmentInBothConsumers() =>
        Registry.ShouldSatisfyAllConditions(
            () => BothConsumersDropTheWithdrawnOwner().ShouldBeTrue(
                "A consumer still projects the panels of the withdrawn owner."),
            () => HostFallsBackAfterWithdrawal().ShouldBeTrue(
                "The host keeps the retained fragment of the withdrawn owner."),
            () => ANewHandlerIsNotAdmittedAfterWithdrawal().ShouldBeTrue(
                "A new handler was admitted after the owner withdrew."),
            () => AlreadyAdmittedWorkMayFinish().ShouldBeTrue(
                "The work that was admitted before the withdrawal did not finish."));

    private SessionPluginOwnerBridge Registry => Bridge;

    private SessionPluginOwnerBridge Bridge =>
        _bridge ??= CreateBridge();

    private SessionPluginOwnerBridge SecondBridge =>
        _secondBridge ??= CreateBridge();

    private SessionPluginContext Context =>
        new(
            "session-1",
            [],
            [],
            new BlockContext(),
            new Dictionary<string, object?>())
        {
            OnReloadRequested = () =>
            {
                _reloadRequests++;
                return Task.CompletedTask;
            },
        };

    private SessionPluginOwnerBridge CreateBridge()
    {
        var bridge = new SessionPluginOwnerBridge(
            new SessionPluginOwnerSnapshot(1, [ActiveOwner]),
            _owned.Select(entry => entry.Plugin),
            _settings);

        foreach (var (owner, plugin) in _owned)
        {
            bridge.Register(owner, plugin);
        }

        return bridge;
    }

    private static FakeSessionPlugin Panel(
        string key,
        SessionPluginPlacement placement,
        int order,
        bool isProtected = false,
        bool isOrderLocked = false) =>
        new(
            new SessionPluginDescriptor(
                key,
                key,
                "plugin",
                order,
                placement,
                IsPinnedByDefault: false,
                IsEnabledByDefault: true,
                IsProtected: isProtected,
                IsOrderLocked: isOrderLocked))
        {
            Badge = context =>
            {
                context.OnReloadRequested?.Invoke();
                return "3";
            },
        };

    private IReadOnlyList<SessionPluginDescriptorRepresentation> ListAsync(
        SessionPluginPlacement placement) =>
        Registry.ListAsync(Context, placement).GetAwaiter().GetResult();

    private bool RenderHostShouldShowOnlyActivePanels()
    {
        var cut = Ctx.Render<SessionPluginHost>();

        var keys = cut.FindAll("[data-slot='session-plugin-tab']")
            .Select(tab => tab.GetAttribute("data-plugin-key"))
            .ToList();

        return keys.Count == 1
               && keys.Contains("owner.active.protected")
               && !keys.Contains("owner.inactive.sidebar");
    }

    private bool RenderHostHonorsThePlacementParameter()
    {
        var cut = Ctx.Render<SessionPluginHost>(parameters => parameters
            .Add(p => p.Placement, SessionPluginPlacement.WorkspaceTab));

        var keys = cut.FindAll("[data-slot='session-plugin-tab']")
            .Select(tab => tab.GetAttribute("data-plugin-key"))
            .ToList();

        return keys.Count == 1
               && keys.Contains("owner.active.workspace")
               && !keys.Contains("owner.active.protected");
    }

    private bool PreferencesSurviveDeactivationAndReactivation()
    {
        var enabledBefore = _settings.IsEnabledAsync("owner.active.sidebar").GetAwaiter().GetResult();
        var orderBefore = _settings.GetOrderAsync().GetAwaiter().GetResult();

        Bridge.Deliver(new SessionPluginOwnerSnapshot(2, []));
        Bridge.Deliver(new SessionPluginOwnerSnapshot(3, [ActiveOwner]));

        var enabledAfter = _settings.IsEnabledAsync("owner.active.sidebar").GetAwaiter().GetResult();
        var orderAfter = _settings.GetOrderAsync().GetAwaiter().GetResult();

        return enabledBefore == enabledAfter
               && orderBefore.SequenceEqual(orderAfter);
    }

    private bool ProtectedPanelDoesNotSurviveDeactivation()
    {
        Bridge.Deliver(new SessionPluginOwnerSnapshot(2, []));

        return Registry.GetAllPlugins().All(plugin => plugin.Descriptor.Key != "owner.active.protected");
    }

    private bool BothConsumersDropTheWithdrawnOwner()
    {
        var withdrawal = new SessionPluginOwnerSnapshot(2, []);

        Bridge.Deliver(withdrawal);
        SecondBridge.Deliver(withdrawal);

        var first = Registry.GetAllPlugins();
        var second = SecondBridge.GetAllPlugins();

        return first.Count == 0 && second.Count == 0;
    }

    private bool HostFallsBackAfterWithdrawal()
    {
        var cut = Ctx.Render<SessionPluginHost>();

        Bridge.Deliver(new SessionPluginOwnerSnapshot(2, []));

        cut.Render(parameters => parameters.Add(p => p.ActiveSessionId, "session-1"));

        return cut.FindAll("[data-slot='session-plugin-host-empty']").Count == 1
               && cut.FindAll("[data-slot='session-plugin-tab']").Count == 0;
    }

    private bool ANewHandlerIsNotAdmittedAfterWithdrawal()
    {
        var received = new List<long>();

        Bridge.Deliver(new SessionPluginOwnerSnapshot(2, []));

        try
        {
            Bridge.Subscribe(revision => received.Add(revision));
        }
        catch (InvalidOperationException)
        {
            // The bridge refused the admission of the new handler.
            return true;
        }

        Bridge.Deliver(new SessionPluginOwnerSnapshot(3, [ActiveOwner]));

        return received.Count == 0;
    }

    private bool AlreadyAdmittedWorkMayFinish()
    {
        var received = new List<long>();

        Bridge.Subscribe(revision => received.Add(revision));

        Bridge.Deliver(new SessionPluginOwnerSnapshot(2, []));

        return received.Count == 1 && received[0] == 2;
    }
}
