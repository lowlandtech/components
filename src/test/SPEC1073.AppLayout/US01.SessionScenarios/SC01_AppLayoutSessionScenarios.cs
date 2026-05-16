namespace LowlandTech.Components.Tests.SPEC1073.AppLayout.US01.SessionScenarios;

[Trait(Spec.SPEC, "1073")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Session keyed app layout scenarios")]
public class SC01_AppLayoutSessionScenarios : ViewScenario
{
    private static RenderFragment Fragment(string text) => builder => builder.AddContent(0, text);

    private IRenderedComponent<AppLayout> RenderLayout(
        AppLayoutScenario scenario = AppLayoutScenario.Workspace,
        AppLayoutSession? session = null) =>
        Ctx.Render<AppLayout>(parameters => parameters
            .Add(component => component.Scenario, scenario)
            .Add(component => component.Session, session)
            .Add(component => component.TopBar, Fragment("Top bar"))
            .Add(component => component.Sidebar, Fragment("Sidebar"))
            .Add(component => component.Main, Fragment("Main content"))
            .Add(component => component.Aside, Fragment("Aside"))
            .Add(component => component.Footer, Fragment("Footer")));

    [Fact(DisplayName = "UAC01-UAC05: Workspace renders topbar sidebar and main Grid spans")]
    [Trait(Spec.UAC, "01")]
    [Trait(Spec.UAC, "02")]
    [Trait(Spec.UAC, "03")]
    [Trait(Spec.UAC, "04")]
    [Trait(Spec.UAC, "05")]
    public void WorkspaceScenarioRendersDockedLayout()
    {
        var cut = RenderLayout();

        cut.Find("[data-slot='app-layout']").GetAttribute("data-layout-scenario").ShouldBe("workspace");
        cut.Find(".lt-app-layout-topbar").TextContent.ShouldContain("Top bar");
        cut.Find(".lt-app-layout-sidebar").TextContent.ShouldContain("Sidebar");
        cut.Find(".lt-app-layout-main").TextContent.ShouldContain("Main content");

        cut.Find(".lt-app-layout-sidebar").GetAttribute("style").ShouldContain("--lt-md:3");
        cut.Find(".lt-app-layout-main").GetAttribute("style").ShouldContain("--lt-md:9");
    }

    [Fact(DisplayName = "UAC07-UAC10: Documentation scenario adds right aside")]
    [Trait(Spec.UAC, "07")]
    [Trait(Spec.UAC, "08")]
    [Trait(Spec.UAC, "09")]
    [Trait(Spec.UAC, "10")]
    public void DocumentationScenarioAddsAside()
    {
        var cut = RenderLayout(AppLayoutScenario.Documentation);

        cut.Find("[data-slot='app-layout']").GetAttribute("data-layout-scenario").ShouldBe("documentation");
        cut.Find(".lt-app-layout-topbar").TextContent.ShouldContain("Top bar");
        cut.Find(".lt-app-layout-sidebar").TextContent.ShouldContain("Sidebar");
        cut.Find(".lt-app-layout-main").TextContent.ShouldContain("Main content");
        cut.Find(".lt-app-layout-aside").TextContent.ShouldContain("Aside");
        cut.Find(".lt-app-layout-aside").GetAttribute("style").ShouldContain("--lt-md:3");
    }

    [Fact(DisplayName = "UAC11-UAC15: Focus and kiosk scenarios reduce navigation chrome")]
    [Trait(Spec.UAC, "11")]
    [Trait(Spec.UAC, "12")]
    [Trait(Spec.UAC, "13")]
    [Trait(Spec.UAC, "14")]
    [Trait(Spec.UAC, "15")]
    public void ReducedChromeScenariosHideNavigationSlots()
    {
        var focus = RenderLayout(AppLayoutScenario.Focus);

        focus.Find(".lt-app-layout-topbar").TextContent.ShouldContain("Top bar");
        focus.FindAll(".lt-app-layout-sidebar").Count.ShouldBe(0);
        focus.FindAll(".lt-app-layout-aside").Count.ShouldBe(0);
        focus.Find(".lt-app-layout-main").GetAttribute("style").ShouldContain("--lt-md:12");

        var kiosk = RenderLayout(AppLayoutScenario.Kiosk);

        kiosk.Find("[data-slot='app-layout']").GetAttribute("data-layout-scenario").ShouldBe("kiosk");
        kiosk.FindAll(".lt-app-layout-topbar").Count.ShouldBe(0);
        kiosk.FindAll(".lt-app-layout-sidebar").Count.ShouldBe(0);
        kiosk.FindAll(".lt-app-layout-aside").Count.ShouldBe(0);
        kiosk.Find(".lt-app-layout-main").TextContent.ShouldContain("Main content");
    }

    [Fact(DisplayName = "UAC16-UAC18: Session scenario overrides parameter scenario")]
    [Trait(Spec.UAC, "16")]
    [Trait(Spec.UAC, "17")]
    [Trait(Spec.UAC, "18")]
    public void SessionScenarioOverridesParameterScenario()
    {
        var session = new AppLayoutSession(AppLayoutScenario.Documentation, "demo-session");

        var cut = RenderLayout(AppLayoutScenario.Workspace, session);

        cut.Find("[data-slot='app-layout']").GetAttribute("data-session-key").ShouldBe("demo-session");
        cut.Find("[data-slot='app-layout']").GetAttribute("data-layout-scenario").ShouldBe("documentation");
        cut.Find(".lt-app-layout-aside").TextContent.ShouldContain("Aside");
    }

    [Fact(DisplayName = "UAC19-UAC21: Session changes update the rendered Grid reactively")]
    [Trait(Spec.UAC, "19")]
    [Trait(Spec.UAC, "20")]
    [Trait(Spec.UAC, "21")]
    public void SessionScenarioChangesRerenderLayout()
    {
        var session = new AppLayoutSession(AppLayoutScenario.Workspace, "demo-session");
        var cut = RenderLayout(session: session);

        session.SetScenario(AppLayoutScenario.Studio);

        cut.WaitForAssertion(() =>
        {
            cut.Find("[data-slot='app-layout']").GetAttribute("data-layout-scenario").ShouldBe("studio");
            cut.Find(".lt-app-layout-footer").TextContent.ShouldContain("Footer");
            cut.Find(".lt-app-layout-main").TextContent.ShouldContain("Main content");
        });
    }
}

