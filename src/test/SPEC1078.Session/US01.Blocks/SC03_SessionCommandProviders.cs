namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Session command bar selects and persists completion providers")]
public sealed class SC03_SessionCommandProviders : ViewScenario
{
    private static readonly SessionNodeInput OrdersNode = new("orders", "session", "Orders");

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
                .Add(component => component.SelectedNode, OrdersNode)
                .Add(component => component.Value, string.Empty);

            if (submittedCallback is not null)
                parameters.Add(component => component.OnSubmit, submittedCallback);
        });
    }
}
