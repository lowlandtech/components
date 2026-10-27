namespace LowlandTech.Components.Tests.CBLK0002.Sessions.US01.Reachability;

/// <summary>
/// SC01 — A user reaches the plugin tab of the session surface with a real
/// pointer.
///
/// <para>
/// The scope is refined. The acceptance criteria below name the behavior of
/// the change, and their Facts are discoverable and skipped until the
/// implementation phase of this scope is released.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CBLK0002")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A user reaches the plugin tab of the right pane of the session surface")]
public class SC01_PluginTabReachability
{
    [Fact(DisplayName = "UAC01: the panels of the session surface claim no more than their group")]
    [Trait(Spec.UAC, "01")]
    [Given("the session surface with both side panels open")]
    [When("the sizes of its panels are read")]
    [Then("they claim no more than the width of the group, so no panel is shrunk by an over-subscribed group")]
    public void UAC01_PanelsClaimNoMoreThanTheirGroup()
    {
        Assert.Skip(
            "The middle panel of the session surface claims the whole width of the group: " +
            "src/blocks/Session/SessionBlock.razor gives it DefaultSize=\"100\" next to two side " +
            "panels of 16, so the group subscribes 132 percent of the width and every panel shrinks " +
            "by about a quarter. The scope is refined and released to the coordinator; the " +
            "implementation phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC02: the right pane keeps the width that its tab strip needs")]
    [Trait(Spec.UAC, "02")]
    [Given("the right pane of the session surface at 1280px")]
    [When("the width of the pane and the width of its tab strip are read")]
    [Then("the pane keeps the about 209px that the tab strip and the close button need, so the plugin tab lies inside the viewport of the pane")]
    public void UAC02_RightPaneKeepsTheWidthOfItsTabStrip()
    {
        Assert.Skip(
            "The right pane of the session surface lands at 141px because the panel group over-" +
            "subscribes its width, while the tab strip and the close button need about 209px, so " +
            "the plugin tab lies 6px outside the viewport of the pane. The scope is refined and " +
            "released to the coordinator; the implementation phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC03: a group with one side panel keeps the sum of its panels at 100 percent")]
    [Trait(Spec.UAC, "03")]
    [Given("the session surface with only one side panel open")]
    [When("the sizes of its panels are read")]
    [Then("the sum of the sizes is 100 percent")]
    public void UAC03_GroupWithOneSidePanelSumsToOneHundredPercent()
    {
        Assert.Skip(
            "The sizes of the panels of the session surface are fixed values that do not follow the " +
            "panels that are rendered, so a group with one side panel does not sum to 100 percent " +
            "either. The scope is refined and released to the coordinator; the implementation phase " +
            "of this scope is not released.");
    }

    [Fact(DisplayName = "UAC04: a real pointer reaches the plugin tab of the packaged session route")]
    [Trait(Spec.UAC, "04")]
    [Given("the packaged session route of the Harness acceptance")]
    [When("a real pointer clicks the plugin tab of the right pane")]
    [Then("the click reaches the tab instead of timing out")]
    public void UAC04_RealPointerReachesThePluginTab()
    {
        Assert.Skip(
            "The pointer acceptance of the packaged session route belongs to the Harness lane, " +
            "which owns the route and the browser. It stays blocked until the panels of this " +
            "repository give the tab its width; the scope is refined and released to the " +
            "coordinator.");
    }
}
