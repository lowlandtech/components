namespace LowlandTech.Components.E2E.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// SC01 — Resolve active provider-scoped themes, browser half.
///
/// <para>
/// The browser acceptance of this scope runs in a separate Interactive Server
/// RCL consumer that restores the Components packages from an offline feed. The
/// consumer registers a host theme and a theme a plugin owner contributes,
/// publishes the owned theme through the bridge, selects it in a provider
/// boundary and renders the stylesheet selection of that boundary. Two
/// browsers open the consumer, so two circuits are live, and the owner
/// withdraws while both are open.
/// </para>
/// <para>
/// The consumer proof belongs to the execution phase, where the consumer
/// harness is created. The scenario is skipped until it exists: the reference
/// consumer and the theme owner bridge are the missing dependencies.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CTHE0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers resolve only the themes of active plugin owners, per provider boundary, and restore the host defaults after a withdrawal")]
public sealed class SC01_ProviderThemesE2E
{
    [Fact(
        DisplayName = "UAC03: the withdrawal of the owner restores the host defaults in two live circuits, detaches the plugin stylesheet and leaves the deployed stylesheet intact",
        Skip = "TODO: the reference Interactive Server RCL consumer of this scope and the Components theme owner bridge (ComponentThemeOwnerBridge) do not exist yet, so the withdrawal of an owner cannot be driven through two live circuits.")]
    [Trait(Spec.UAC, "03")]
    [Given("a published consumer that renders a provider boundary with the theme of an active plugin owner and the stylesheet selection of that boundary")]
    [When("a second consumer opens the same host and the owner withdraws")]
    [Then("both circuits render the host defaults, the plugin stylesheet is detached and the deployed stylesheet is still served")]
    public void UAC03_TheWithdrawalRestoresTheHostDefaultsInTwoCircuits()
    {
        // The scenario is written when the execution phase creates the
        // reference consumer. It publishes the theme of an active owner
        // through the bridge, selects it in a provider boundary of the
        // consumer and renders the stylesheet selection of that boundary.
        //
        // It then opens the consumer in two browser contexts, so two circuits
        // are live, waits until both circuits are live, withdraws the owner
        // and asserts that both circuits render the host default class and
        // the host default style family, that the plugin stylesheet link is
        // detached in both circuits and that the deployed stylesheet of the
        // package is still served with 200.
        throw new NotSupportedException(
            "The reference consumer of CTHE0001-US01-SC01 is created in the execution phase.");
    }
}
