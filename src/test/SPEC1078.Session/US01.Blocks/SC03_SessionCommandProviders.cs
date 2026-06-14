namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Session command bar selects and persists completion providers")]
public sealed class SC03_SessionCommandProviders : ViewScenario
{
    private static readonly SessionNodeInput WorkspacesNode = new("workspaces", "session", "Workspaces");

    private IAgentSelectionService AgentSelection => Ctx.Services.GetRequiredService<IAgentSelectionService>();

    [Fact(DisplayName = "UAC01: /providers opens the provider picker without submitting a prompt")]
    [Trait(Spec.UAC, "01")]
    [Given("an active SessionCommandBar with no completion menu open")]
    [When("the user types the exact slash command /providers")]
    [Then("the provider popup opens and the slash command is not submitted")]
    public void UAC01_ProvidersSlashCommandOpensPicker()
    {
        SessionCommandSubmitted? submitted = null;
        var cut = RenderCommandBar(submittedCallback: command => submitted = command);

        cut.Find("[data-slot='session-command-input']").Input("/providers");

        cut.Find("[data-slot='session-command-menu'][data-command='providers']").ShouldNotBeNull();
        cut.FindAll("[data-slot='session-provider-option']").Count.ShouldBe(2);
        submitted.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC02: selecting Kimi opens its model picker and persists the provider")]
    [Trait(Spec.UAC, "02")]
    [Given("the /providers popup is open with Ollama active")]
    [When("the user selects Kimi")]
    [Then("the Kimi model popup opens and localStorage receives the selected provider")]
    public void UAC02_SelectingProviderPersistsSelection()
    {
        var cut = RenderCommandBar();
        cut.Find("[data-slot='session-command-input']").Input("/providers");

        cut.Find("[data-provider='kimi']").Click();

        cut.Find("[data-slot='session-command-menu'][data-command='models']").ShouldNotBeNull();
        cut.Find("[data-model='kimi-k2.5']").ShouldNotBeNull();
        Ctx.JSInterop.Invocations
            .Where(invocation => invocation.Identifier == "localStorage.setItem")
            .Select(invocation => invocation.Arguments[1]?.ToString())
            .Any(json => json?.Contains("\"ProviderKey\":\"kimi\"", StringComparison.Ordinal) == true)
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: model and API key are attached to the submitted A2A command")]
    [Trait(Spec.UAC, "03")]
    [Given("Kimi is selected with a model and API key")]
    [When("the user submits a normal session prompt")]
    [Then("the command carries the completion selection separately from the prompt")]
    public void UAC03_SubmitCarriesCompletionSelection()
    {
        SessionCommandSubmitted? submitted = null;
        var cut = RenderCommandBar(submittedCallback: command => submitted = command);
        cut.Find("[data-slot='session-command-input']").Input("/providers");
        cut.Find("[data-provider='kimi']").Click();
        cut.Find("[data-slot='session-provider-api-key']").Input("sk-test");
        cut.Find("[data-slot='session-provider-api-key-save']").Click();
        cut.Find("[data-slot='session-command-input']").Input("Summarize the active session");

        cut.Find("[data-slot='session-command-submit']").Click();

        submitted.ShouldNotBeNull();
        submitted.Input.ShouldBe("Summarize the active session");
        submitted.Completion.ShouldNotBeNull();
        submitted.Completion.ProviderKey.ShouldBe("kimi");
        submitted.Completion.ModelKey.ShouldBe("kimi-k2.5");
        submitted.Completion.ApiKey.ShouldBe("sk-test");
    }

    [Fact(DisplayName = "UAC04: completion selection rehydrates from localStorage after reload")]
    [Trait(Spec.UAC, "04")]
    [Given("localStorage contains a previously selected Kimi model and API key")]
    [When("SessionCommandBar renders after page reentry")]
    [Then("the active completion chip shows the restored Kimi model")]
    public void UAC04_SelectionRehydratesFromLocalStorage()
    {
        Ctx.JSInterop.Setup<string?>(
                "localStorage.getItem",
                "session-command-bar:completion")
            .SetResult("""{"ProviderKey":"kimi","ModelKey":"kimi-k2.5","ApiKey":"sk-restored"}""");

        var cut = RenderCommandBar(setupStorage: false);

        cut.WaitForAssertion(() =>
        {
            var selection = cut.Find("[data-slot='session-completion-selection']");
            selection.TextContent.ShouldContain("Kimi");
            selection.TextContent.ShouldContain("Kimi K2.5");
        });
    }

    [Fact(DisplayName = "UAC05: model selection uses the Select component")]
    [Trait(Spec.UAC, "05")]
    [Given("a rendered SessionCommandBar")]
    [When("inspecting the completion selection element")]
    [Then("it is rendered as a Select with the expected trigger and value slots")]
    public void UAC05_SelectionIsSelectComponent()
    {
        var cut = RenderCommandBar();

        var selection = cut.Find("[data-slot='session-completion-selection']");
        selection.GetAttribute("data-slot").ShouldContain("session-completion-selection");
        cut.Find("[data-slot='session-completion-selection'] [data-slot='select-trigger']").ShouldNotBeNull();
        cut.Find("[data-slot='session-completion-selection'] [data-slot='select-value']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: selecting a model from the dropdown updates the current selection")]
    [Trait(Spec.UAC, "06")]
    [Given("the model dropdown is open")]
    [When("the user clicks a model item")]
    [Then("the service selection changes to that provider and model and persists")]
    public void UAC06_DropdownSelectionUpdatesService()
    {
        var cut = RenderCommandBarWithPopover();

        cut.Find("[data-slot='session-completion-selection'] [data-slot='select-trigger']").Click();

        cut.WaitForAssertion(() =>
        {
            cut.Markup.ShouldContain("data-slot=\"select-content\"");
        });

        cut.Find("[data-slot='select-content'] [data-value='kimi::kimi-k2-thinking']").Click();

        AgentSelection.CurrentSelection.ProviderKey.ShouldBe("kimi");
        AgentSelection.CurrentSelection.ModelKey.ShouldBe("kimi-k2-thinking");
        Ctx.JSInterop.Invocations
            .Where(invocation => invocation.Identifier == "localStorage.setItem")
            .Select(invocation => invocation.Arguments[1]?.ToString())
            .Any(json => json?.Contains("\"ModelKey\":\"kimi-k2-thinking\"", StringComparison.Ordinal) == true)
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC07: switching providers preserves the API key when the new provider requires one")]
    [Trait(Spec.UAC, "07")]
    [Given("Kimi is selected with an API key")]
    [When("the user switches to another Kimi model")]
    [Then("the API key is retained on the current selection")]
    public void UAC07_ApiKeyPreservedWithinProvider()
    {
        var cut = RenderCommandBar();
        cut.Find("[data-slot='session-command-input']").Input("/providers");
        cut.Find("[data-provider='kimi']").Click();
        cut.Find("[data-slot='session-provider-api-key']").Input("sk-retained");
        cut.Find("[data-slot='session-provider-api-key-save']").Click();

        cut.Find("[data-slot='session-command-input']").Input("/models");
        cut.Find("[data-model='kimi-k2-thinking']").Click();

        AgentSelection.CurrentSelection.ProviderKey.ShouldBe("kimi");
        AgentSelection.CurrentSelection.ModelKey.ShouldBe("kimi-k2-thinking");
        AgentSelection.CurrentSelection.ApiKey.ShouldBe("sk-retained");
    }

    private IRenderedComponent<SessionCommandBar> RenderCommandBar(
        Action<SessionCommandSubmitted>? submittedCallback = null,
        bool setupStorage = true)
    {
        if (setupStorage)
        {
            Ctx.JSInterop.Setup<string?>(
                    "localStorage.getItem",
                    "session-command-bar:completion")
                .SetResult(null);
        }
        Ctx.JSInterop.SetupVoid("localStorage.setItem", _ => true);

        return Ctx.Render<SessionCommandBar>(parameters =>
        {
            parameters
                .Add(component => component.SelectedNode, WorkspacesNode)
                .Add(component => component.Value, string.Empty);

            if (submittedCallback is not null)
                parameters.Add(component => component.OnSubmit, submittedCallback);
        });
    }

    private IRenderedComponent<CommandBarPopoverWrapper> RenderCommandBarWithPopover()
    {
        Ctx.JSInterop.Setup<string?>(
                "localStorage.getItem",
                "session-command-bar:completion")
            .SetResult(null);
        Ctx.JSInterop.SetupVoid("localStorage.setItem", _ => true);

        return Ctx.Render<CommandBarPopoverWrapper>(parameters =>
        {
            parameters.Add(wrapper => wrapper.SelectedNode, WorkspacesNode);
            parameters.Add(wrapper => wrapper.Value, string.Empty);
        });
    }

    private sealed class CommandBarPopoverWrapper : ComponentBase
    {
        [Parameter] public SessionNodeInput SelectedNode { get; set; } = default!;
        [Parameter] public string Value { get; set; } = string.Empty;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<SessionCommandBar>(1);
            builder.AddAttribute(2, nameof(SessionCommandBar.SelectedNode), SelectedNode);
            builder.AddAttribute(3, nameof(SessionCommandBar.Value), Value);
            builder.CloseComponent();
        }
    }
}
