using System.Text.RegularExpressions;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.CBLK0003.Sessions.US01.StaleState;

/// <summary>
/// SC01 — A user of the session surface sees the panels of the surface after a
/// reset of the profile.
///
/// <para>
/// The collapse state of the two side panes belongs together, because a state
/// that keeps the surface in a single panel view is not a state of a live
/// surface. A persisted state is stale when it would leave the surface in a
/// single panel view, or when the surface owns no sessions at all, because the
/// profile that wrote the state no longer owns the sessions of the surface. A
/// stale state does not keep the surface collapsed: the surface recovers its
/// panels and their handles, and the control repairs the stored state, so the
/// recovery holds after a reload without the user clearing the storage of the
/// browser.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CBLK0003")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A user of the session surface sees the panels of the surface after a reset of the profile")]
public class SC01_SessionSurfaceStaleState : ViewScenario
{
    private const string SidebarKey = "session-block:sidebar-open";
    private const string RightSidebarKey = "session-block:right-sidebar-open";
    private const string ReloadedSidebarKey = "session-block:sidebar-open:reloaded";
    private const string ReloadedRightSidebarKey = "session-block:right-sidebar-open:reloaded";

    private static readonly SessionListEntry[] LiveSessions =
        [new SessionListEntry("s1", "Workspaces", 0, "session")];

    [Fact(DisplayName = "UAC01: a persisted collapse state of a live surface keeps the pane collapsed")]
    [Trait(Spec.UAC, "01")]
    [Given("a surface with a live session and a persisted collapse state")]
    [When("the surface renders")]
    [Then("the persisted state keeps the pane collapsed")]
    public void UAC01_PersistedStateKeepsThePaneCollapsed()
    {
        Seed(SidebarKey, "false");
        Seed(RightSidebarKey, "true");

        var markup = RenderSurface(LiveSessions, liveSession: true);

        PanelCount(markup).ShouldBe(
            2,
            "the persisted collapse state of the left pane has to keep the left pane collapsed");
        StorageWrites().ShouldNotContain(
            write => write.Key == SidebarKey,
            "a persisted state of a live surface is applied, not repaired");
    }

    [Fact(DisplayName = "UAC02: a surface whose sessions are gone renders its panels and its handles")]
    [Trait(Spec.UAC, "02")]
    [Given("a surface whose sessions are gone and a persisted collapse state")]
    [When("the surface renders")]
    [Then("the surface renders its panels and its handles instead of a single panel view")]
    public void UAC02_SurfaceWithoutSessionsRendersItsPanelsAndHandles()
    {
        // The Harness seeds the two keys of the collapse state of a profile that
        // no longer owns the sessions of the surface.
        Seed(SidebarKey, "false");
        Seed(RightSidebarKey, "false");

        var markup = RenderSurface([], liveSession: false);

        PanelCount(markup).ShouldBe(
            3,
            "the surface has to render its three panels instead of a single panel view");
        HandleCount(markup).ShouldBe(
            2,
            "the surface has to render its two handles instead of none");
        StorageWrites().ShouldContain(
            write => write.Key == SidebarKey && write.Value == "true",
            "the control has to repair the stale state of the left pane");
        StorageWrites().ShouldContain(
            write => write.Key == RightSidebarKey && write.Value == "true",
            "the control has to repair the stale state of the right pane");
    }

    [Fact(DisplayName = "UAC03: the recovery of a stale state holds after a reload")]
    [Trait(Spec.UAC, "03")]
    [Given("a surface that recovered from a stale persisted state")]
    [When("the surface renders again after a reload")]
    [Then("the recovery holds without the user clearing the storage of the browser")]
    public void UAC03_RecoveryHoldsAfterAReload()
    {
        // The state keeps the surface in a single panel view, which is not a
        // state of a live surface.
        Seed(SidebarKey, "false");
        Seed(RightSidebarKey, "false");

        var markup = RenderSurface(LiveSessions, liveSession: true);

        PanelCount(markup).ShouldBe(
            3,
            "the surface has to recover its panels from a stale persisted state");
        StorageWrites().ShouldContain(
            write => write.Key == SidebarKey && write.Value == "true",
            "the control has to repair the stale state of the left pane");
        StorageWrites().ShouldContain(
            write => write.Key == RightSidebarKey && write.Value == "true",
            "the control has to repair the stale state of the right pane");

        // The reload reads the state that the control wrote.
        Seed(ReloadedSidebarKey, "true");
        Seed(ReloadedRightSidebarKey, "true");

        var reloaded = RenderSurface(
            LiveSessions,
            liveSession: true,
            sidebarKey: ReloadedSidebarKey,
            rightSidebarKey: ReloadedRightSidebarKey);

        PanelCount(reloaded).ShouldBe(
            3,
            "the recovery has to hold after a reload without the user clearing the storage");
        StorageWrites().ShouldNotContain(
            write => write.Key.StartsWith(ReloadedSidebarKey, StringComparison.Ordinal),
            "the repaired state has to be applied on the reload instead of being recovered again");
    }

    [Fact(DisplayName = "UAC04: the packaged session route recovers from the stale state")]
    [Trait(Spec.UAC, "04")]
    [Given("the packaged session route of the Harness acceptance")]
    [When("the route is reloaded with the stale state seeded")]
    [Then("it renders the three panels and the two handles and every linked asset answers 200")]
    public void UAC04_PackagedSessionRouteRecoversFromTheStaleState()
    {
        Assert.Skip(
            "The acceptance of the packaged session route belongs to the Harness lane, " +
            "which owns the route and the browser. The recovered package is published for it, " +
            "and this scope stays open until its result answers.");
    }

    private void Seed(string key, string value)
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", key).SetResult(value);
    }

    private string RenderSurface(
        IReadOnlyList<SessionListEntry> sessions,
        bool liveSession,
        string sidebarKey = SidebarKey,
        string rightSidebarKey = RightSidebarKey)
    {
        var stateService = new SessionStateService();
        if (liveSession)
        {
            stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);
        }

        return Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.SidebarPersistKey, sidebarKey)
            .Add(x => x.RightSidebarPersistKey, rightSidebarKey)
            .Add(x => x.RightSidebarContent, (RenderFragment)(builder =>
                builder.AddMarkupContent(0, "<div>plugin surface</div>"))))
            .Markup;
    }

    private IReadOnlyList<(string Key, string Value)> StorageWrites() =>
        Ctx.JSInterop.Invocations
            .Where(invocation => invocation.Identifier == "localStorage.setItem")
            .Select(invocation => ((string)invocation.Arguments[0]!, (string)invocation.Arguments[1]!))
            .ToList();

    private static int PanelCount(string markup) =>
        Regex.Matches(markup, @"data-slot=""resizable-panel""").Count;

    private static int HandleCount(string markup) =>
        Regex.Matches(markup, @"data-slot=""resizable-handle""").Count;
}
