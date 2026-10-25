namespace LowlandTech.Components.E2E.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// SC01 — Resolve active plugin pages, browser half.
///
/// <para>
/// The browser acceptance of this scope runs in a separate Interactive Server
/// RCL consumer that restores the Components packages from an offline feed. The
/// consumer publishes a plugin page through the page owner bridge, feeds the
/// active page identities to the Plugins route and page primitives and renders
/// the navigation of the shell route gateway. It also serves a reserved host
/// route of its own and a guarded page that authorizes a server action before
/// it renders any content.
/// </para>
/// <para>
/// The consumer proof belongs to the execution phase, where the consumer
/// harness is created. The scenario is skipped until it exists: the reference
/// consumer and the plugin page owner bridge are the missing dependencies.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPGS0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers expose the plugin pages and the navigation of active plugin owners only, and keep the host routes reserved")]
public sealed class SC01_ActivePluginPagesE2E
{
    [Fact(
        DisplayName = "UAC03: the withdrawal of the owner removes the navigation entry and denies the retained URL of its page, while the reserved host route stays available",
        Skip = "TODO: the reference Interactive Server RCL consumer of this scope and the Components plugin page owner bridge (ComponentPageOwnerBridge) do not exist yet, so the withdrawal of an owner cannot be driven against the Plugins route gateway of a published consumer.")]
    [Trait(Spec.UAC, "03")]
    [Given("a published consumer that serves a reserved host route and a plugin page of an active owner through the Plugins route gateway, with a guarded page that authorizes a server action before it renders content")]
    [When("the owner of the plugin page withdraws and the retained URL of that page is requested again")]
    [Then("the navigation entry of the withdrawn owner is gone, the retained URL is denied or reaches the host fallback, and the reserved host route stays available")]
    public void UAC03_TheWithdrawalRemovesTheNavigationEntryAndDeniesTheRetainedUrl()
    {
        // The scenario is written when the execution phase creates the
        // reference consumer. It publishes a plugin page of an active owner
        // through the bridge, feeds the active page identities to the Plugins
        // route and page primitives and renders the navigation of the shell
        // route gateway, beside a reserved host route of the consumer and a
        // guarded page that authorizes a server action before it renders
        // content.
        //
        // It then opens the consumer in a browser, waits until the navigation
        // offers the page of the owner, withdraws the owner and asserts that
        // the navigation entry is gone, that the retained URL of the page is
        // denied or reaches the host fallback, that the guarded page still
        // authorizes its server action and that the reserved host route is
        // still available.
        throw new NotSupportedException(
            "The reference consumer of CPGS0001-US01-SC01 is created in the execution phase.");
    }
}
