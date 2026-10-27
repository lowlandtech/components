namespace LowlandTech.Components.Tests.CBLK0003.Sessions.US01.StaleState;

/// <summary>
/// SC01 — The session surface recovers from a stale persisted state.
///
/// <para>
/// The scope is refined. The acceptance criteria below name the behavior of
/// the change, and their Facts are discoverable and skipped until the
/// implementation phase of this scope is released.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CBLK0003")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A user sees the panels of the session surface after a reset of the profile")]
public class SC01_SessionSurfaceStaleState
{
    [Fact(DisplayName = "UAC01: a persisted state that matches the sessions keeps the surface collapsed")]
    [Trait(Spec.UAC, "01")]
    [Given("a surface with a live session and a persisted collapse state")]
    [When("the surface renders")]
    [Then("the persisted state keeps the pane collapsed")]
    public void UAC01_MatchingPersistedStateKeepsTheSurfaceCollapsed()
    {
        Assert.Skip(
            "The control reads the persisted state of the panes and applies it without asking " +
            "whether the sessions of the surface still exist, so the behavior of a matching state " +
            "is not proven apart from the behavior of a stale one. The scope is refined and " +
            "released to the coordinator; the implementation phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC02: a stale persisted state does not keep the surface collapsed")]
    [Trait(Spec.UAC, "02")]
    [Given("a surface whose sessions are gone and a persisted collapse state")]
    [When("the surface renders")]
    [Then("the surface renders its panels and its handles instead of a single panel view")]
    public void UAC02_StalePersistedStateDoesNotKeepTheSurfaceCollapsed()
    {
        Assert.Skip(
            "The control of the panes of the session surface applies the persisted state " +
            "session-block:sidebar-open and session-block:right-sidebar-open without asking whether " +
            "the sessions of the surface still exist, so a stale state keeps the surface in a single " +
            "panel view. The scope is refined and released to the coordinator; the implementation " +
            "phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC03: the control recovers without the user clearing the storage")]
    [Trait(Spec.UAC, "03")]
    [Given("a surface that recovered from a stale persisted state")]
    [When("the surface renders again after a reload")]
    [Then("the recovery holds without the user clearing the storage of the browser")]
    public void UAC03_ControlRecoversWithoutTheUserClearingTheStorage()
    {
        Assert.Skip(
            "The control does not recover from a stale persisted state, so the recovery over a " +
            "reload is not proven. The scope is refined and released to the coordinator; the " +
            "implementation phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC04: the packaged session route renders its panels after a stale state")]
    [Trait(Spec.UAC, "04")]
    [Given("the packaged session route of the Harness acceptance")]
    [When("the route is reloaded with the stale state seeded")]
    [Then("it renders the three panels and the two handles and every linked asset answers 200")]
    public void UAC04_PackagedRouteRendersItsPanelsAfterAStaleState()
    {
        Assert.Skip(
            "The packaged session route belongs to the Harness lane, which owns the route and the " +
            "browser. It stays blocked until the control of this repository recovers from the stale " +
            "state; the scope is refined and released to the coordinator.");
    }
}
