using System.Text.RegularExpressions;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.CBLK0002.Sessions.US01.Reachability;

/// <summary>
/// SC01 — A user reaches the plugin tab of the session surface with a real
/// pointer.
///
/// <para>
/// The panels of the session surface share the width of their group, so the
/// middle panel claims the width that the side panels leave. A group that
/// subscribes more than the width of the group shrinks every panel of it, and
/// the right pane of the surface then lands below the width that its tab strip
/// needs, which puts the tab of the plugin surface under the close button of
/// the pane. The strip of the pane scrolls, so the tab stays inside the pane
/// and a pointer can reach it.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CBLK0002")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A user reaches the plugin tab of the right pane of the session surface")]
public class SC01_PluginTabReachability : ViewScenario
{
    private static readonly SessionListEntry[] Sessions =
        [new SessionListEntry("s1", "Workspaces", 0, "session")];

    [Fact(DisplayName = "UAC01: the panels of the session surface claim no more than their group")]
    [Trait(Spec.UAC, "01")]
    [Given("a session surface with both side panes open")]
    [When("the sizes of its panels are read")]
    [Then("they claim no more than the width of their group, so no panel is shrunk by an over-subscribed group")]
    public void UAC01_PanelsClaimNoMoreThanTheirGroup()
    {
        foreach (var markup in Surfaces())
        {
            var sizes = PanelSizes(markup);

            sizes.ShouldNotBeEmpty();
            sizes.Sum().ShouldBeLessThanOrEqualTo(100);
        }
    }

    [Fact(DisplayName = "UAC02: the middle panel claims the width that the side panes leave")]
    [Trait(Spec.UAC, "02")]
    [Given("a session surface with both side panes open")]
    [When("the sizes of its panels are read")]
    [Then("the panels of the group claim the whole width of the group")]
    public void UAC02_MiddlePanelClaimsTheWidthThatTheSidePanesLeave()
    {
        foreach (var markup in Surfaces())
        {
            var sizes = PanelSizes(markup);

            sizes.Sum().ShouldBe(100);
        }
    }

    [Fact(DisplayName = "UAC03: the strip of the right pane scrolls, so the plugin tab stays inside the pane")]
    [Trait(Spec.UAC, "03")]
    [Given("a session surface with a plugin surface in the right pane")]
    [When("the strip of the tabs of the right pane is read")]
    [Then("it scrolls, so the tab of the plugin surface stays inside the pane instead of ending under the close button")]
    public void UAC03_TabStripScrolls()
    {
        foreach (var markup in Surfaces())
        {
            var strip = Regex.Match(
                markup,
                @"<div[^>]*data-slot=""session-block-right-sidebar-tabs""[^>]*>",
                RegexOptions.Singleline);

            strip.Success.ShouldBeTrue("The right pane of the session surface has to render a strip of tabs.");
            strip.Value.ShouldContain("overflow-x-auto");

            Regex.IsMatch(
                markup,
                @"data-slot=""session-block-right-sidebar-tab""[^>]*data-tab-key=""plugins""",
                RegexOptions.Singleline).ShouldBeTrue(
                "The right pane of the session surface has to render the tab of the plugin surface.");
        }
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
            "which owns the route and the browser. The fixed package is published for it, and this " +
            "scope stays open until its pointer result answers.");
    }

    /// <summary>
    /// The session surface renders two variants: the standalone variant with a
    /// collapsible sidebar and the embedded variant without one. Both of them
    /// carry a right pane with a strip of tabs, so both of them have to keep
    /// the tab of the plugin surface inside the pane.
    /// </summary>
    private IReadOnlyList<string> Surfaces() =>
        [RenderSurface(hasSidebar: true), RenderSurface(hasSidebar: false)];

    private string RenderSurface(bool hasSidebar)
    {
        var stateService = new SessionStateService();
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), Sessions);

        return Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, hasSidebar)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, Sessions)
            .Add(x => x.RightSidebarContent, (RenderFragment)(builder =>
                builder.AddMarkupContent(0, "<div>plugin surface</div>"))))
            .Markup;
    }

    private static IReadOnlyList<int> PanelSizes(string markup)
    {
        var sizes = new List<int>();

        foreach (Match match in Regex.Matches(
                     markup, @"data-slot=""resizable-panel""[^>]*?calc\((\d+)%"))
        {
            sizes.Add(int.Parse(match.Groups[1].Value));
        }

        return sizes;
    }
}
