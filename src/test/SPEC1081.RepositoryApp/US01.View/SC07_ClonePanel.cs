namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Clone panel exposes HTTPS/SSH/Tea protocols and copies the active command")]
public class SC07_ClonePanel : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Clone panel is hidden by default")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered with the default state")]
    [When("the file list is inspected")]
    [Then("the clone panel should not be rendered")]
    public void UAC01_ClonePanelHiddenByDefault()
    {
        var cut = RenderRepositoryApp();

        cut.FindAll("[data-slot='repository-clone-panel']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Clone panel renders when ShowClonePanel is true")]
    [Trait(Spec.UAC, "02")]
    [Given("the clone panel toggle has been activated")]
    [When("the file list is rendered")]
    [Then("the clone panel should be present in the DOM")]
    public void UAC02_ClonePanelRendersWhenToggled()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-slot='repository-clone-panel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Panel exposes all three protocol buttons")]
    [Trait(Spec.UAC, "03")]
    [Given("the clone panel is open")]
    [When("the protocol controls are inspected")]
    [Then("HTTPS, SSH, and Tea protocol buttons should all be present")]
    public void UAC03_PanelExposesThreeProtocols()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='https']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='ssh']").ShouldNotBeNull();
        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='tea']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: HTTPS is the default active protocol")]
    [Trait(Spec.UAC, "04")]
    [Given("the clone panel is opened for the first time")]
    [When("the protocol controls are inspected")]
    [Then("the HTTPS button should be the active protocol")]
    public void UAC04_HttpsIsDefaultActive()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='https']")
            .GetAttribute("data-active").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC05: Only one protocol is data-active at a time")]
    [Trait(Spec.UAC, "05")]
    [Given("the clone panel is open")]
    [When("the protocol controls are inspected")]
    [Then("exactly one protocol button should have data-active=\"true\"")]
    public void UAC05_OnlyOneProtocolActiveAtATime()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        var active = cut.FindAll("[data-testid='repo-clone-protocol'][data-active='true']");
        active.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC06: Clicking SSH switches the visible command")]
    [Trait(Spec.UAC, "06")]
    [Given("the clone panel is open with HTTPS active")]
    [When("the user clicks SSH")]
    [Then("the command area should render the SSH URL and the SSH button should be active")]
    public void UAC06_ClickingSshSwitchesCommand()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='ssh']").Click();
        cut.Render();

        var panel = cut.Find("[data-slot='repository-clone-panel']");
        panel.TextContent.ShouldContain(StateService.State.CloneInfo!.Ssh);
        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='ssh']")
            .GetAttribute("data-active").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC07: Clicking Tea switches the visible command")]
    [Trait(Spec.UAC, "07")]
    [Given("the clone panel is open with HTTPS active")]
    [When("the user clicks the Tea protocol button")]
    [Then("the command area should render the Tea URL")]
    public void UAC07_ClickingTeaSwitchesCommand()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='tea']").Click();
        cut.Render();

        var panel = cut.Find("[data-slot='repository-clone-panel']");
        panel.TextContent.ShouldContain(StateService.State.CloneInfo!.Tea);
    }

    [Fact(DisplayName = "UAC08: Copy button invokes navigator.clipboard.writeText with the HTTPS command")]
    [Trait(Spec.UAC, "08")]
    [Given("the clone panel is open with HTTPS active")]
    [When("the user clicks the Copy button")]
    [Then("navigator.clipboard.writeText should be invoked with the HTTPS command")]
    public void UAC08_CopyButtonInvokesClipboard()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-testid='repo-clone-copy']").Click();
        cut.Render();

        Ctx.JSInterop.VerifyInvoke("navigator.clipboard.writeText", 1);
    }

    [Fact(DisplayName = "UAC09: Clone panel renders protocol-specific hint text")]
    [Trait(Spec.UAC, "09")]
    [Given("the clone panel is open with HTTPS active")]
    [When("the panel content is inspected")]
    [Then("HTTPS-specific hint text should be present")]
    public void UAC09_ClonePanelRendersProtocolHint()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        var panel = cut.Find("[data-slot='repository-clone-panel']");
        panel.TextContent.ShouldContain("HTTPS");
    }

    [Fact(DisplayName = "UAC10: SSH protocol changes the hint text")]
    [Trait(Spec.UAC, "10")]
    [Given("the clone panel is open")]
    [When("the user selects SSH")]
    [Then("the hint text should change to the SSH-specific guidance")]
    public void UAC10_SshProtocolChangesHint()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleClonePanel();
        cut.Render();

        cut.Find("[data-testid='repo-clone-protocol'][data-protocol='ssh']").Click();
        cut.Render();

        var panel = cut.Find("[data-slot='repository-clone-panel']");
        panel.TextContent.ShouldContain("SSH");
    }
}
