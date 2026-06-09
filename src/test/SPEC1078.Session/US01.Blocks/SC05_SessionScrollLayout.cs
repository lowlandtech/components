namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Session content scrolls while the command bar remains fixed at the bottom")]
public sealed class SC05_SessionScrollLayout : ViewScenario
{
    private static readonly SessionNodeInput SessionNode =
        new("orders-session", "session", "Orders");

    [Fact(DisplayName = "UAC01: Session content body owns vertical scrolling")]
    [Trait(Spec.UAC, "01")]
    [Given("an active session with content that can exceed the available height")]
    [When("the session content surface is rendered")]
    [Then("only the content body scrolls while its parent surfaces clip overflow")]
    public void UAC01_SessionContentBodyOwnsVerticalScrolling()
    {
        var cut = Ctx.Render<SessionContent>(parameters => parameters
            .Add(component => component.OpenNodes, [SessionNode])
            .Add(component => component.SelectedNode, SessionNode));

        var root = cut.Find("[data-slot='session-content']");
        root.ClassList.ShouldContain("min-h-0");
        root.ClassList.ShouldContain("overflow-hidden");

        var tabsContent = cut.Find("[data-slot='tabs-content']");
        tabsContent.ClassList.ShouldContain("min-h-0");
        tabsContent.ClassList.ShouldContain("overflow-hidden");

        var body = cut.Find("[data-slot='session-content-body']");
        body.ClassList.ShouldContain("min-h-0");
        body.ClassList.ShouldContain("overflow-y-auto");
        body.ClassList.ShouldContain("overflow-x-hidden");
        body.ClassList.ShouldNotContain("overflow-auto");
    }

    [Fact(DisplayName = "UAC02: Session command bar remains fixed below the scrolling context")]
    [Trait(Spec.UAC, "02")]
    [Given("an embedded SessionBlock containing a session context and command bar")]
    [When("the main session column is rendered")]
    [Then("the main column clips overflow and the command bar cannot shrink or scroll away")]
    public void UAC02_CommandBarRemainsFixedBelowScrollingContext()
    {
        var cut = Ctx.Render<SessionBlock>();

        var main = cut.Find("[data-slot='session-block-main-content']");
        main.ClassList.ShouldContain("min-h-0");
        main.ClassList.ShouldContain("overflow-hidden");

        var commandBar = cut.Find("[data-slot='session-command-bar']");
        commandBar.ClassList.ShouldContain("shrink-0");
    }
}
