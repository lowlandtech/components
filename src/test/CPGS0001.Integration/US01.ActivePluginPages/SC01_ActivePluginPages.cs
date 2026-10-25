namespace LowlandTech.Components.Tests.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// SC01 — Resolve active plugin pages.
///
/// <para>
/// A Components host exposes the pages of a plugin owner only while that owner
/// is active. A loaded type is not proof of active ownership: an owned page is
/// validated before it is published, the metadata comes from the real page
/// attribute the plugin declares, the host routes stay reserved, and every
/// lookup path, including the Plugins page catalog the shell route gateway
/// reads, stops offering the pages of an owner that withdraws.
/// </para>
/// <para>
/// Every Fact is skipped until the page owner bridge exists: the Components
/// plugin page owner contracts are the missing dependency. A skipped Fact is
/// unfinished inventory, never acceptance.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPGS0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers expose the plugin pages and the navigation of active plugin owners only, and keep the host routes reserved")]
public sealed class SC01_ActivePluginPages
{
    private const string ActiveOwner = "owner.active";
    private const string OtherOwner = "owner.other";

    private const string PageIdentity = "Test.FakePluginPage";
    private const string HiddenIdentity = "Test.FakeHiddenPluginPage";
    private const string UndescribedIdentity = "Test.FakeUndescribedPluginPage";
    private const string MissingIdentity = "Test.Missing";
    private const string WrongTypeIdentity = "Test.NotAComponent";

    private const string PageRoute = "/plugin/reports";
    private const string HiddenRoute = "/plugin/hidden";
    private const string ReservedRoute = "/";

    private object? _bridge;

    [Fact(
        DisplayName = "UAC01: an active page keeps the metadata it declares, a page that hides itself from the navigation stays routable and the page resolves to the component the host authorizes",
        Skip = "TODO: the Components plugin page owner bridge (ComponentPageOwnerBridge, ComponentPageBinding, ComponentPageOwnerSnapshot, ComponentPageBindingValidation, ComponentPageBindingErrorCode, ComponentPageNavigationEntry) is not implemented yet, so no owned page can be adapted from the real PluginPage metadata and projected onto the Plugins route and page primitives.")]
    [Trait(Spec.UAC, "01")]
    [Given("a published page of an active owner that carries the real PluginPage metadata, beside a page of the same owner that hides itself from the navigation")]
    [When("the navigation and the direct route resolution are evaluated")]
    [Then("the label, the icon, the group, the subgroup and the order are preserved, the hidden page leaves the navigation but stays routable, and the page resolves to the component the host authorizes")]
    public void UAC01_AnActivePageKeepsItsMetadataAndStaysRoutable() =>
        Bridge.ShouldSatisfyAllConditions(
            () => IsValid(Publish(ActiveOwner, PageIdentity, PageRoute)).ShouldBeTrue(
                "The page of the active owner was not accepted."),
            () => IsValid(Publish(ActiveOwner, HiddenIdentity, HiddenRoute)).ShouldBeTrue(
                "The page that hides itself from the navigation was not accepted."),
            () => ComponentsPageOwnerContracts.Enumerate(Bridge).ShouldContain(
                entry => entry.RoutePath == PageRoute && entry.SymbolicIdentity == PageIdentity,
                "The page of the active owner is not published."),
            () => ComponentsPageOwnerContracts.Resolve(Bridge, PageRoute).ShouldNotBeNull(
                "The route of the page of the active owner does not resolve."),
            () => ComponentsPageOwnerContracts.Navigation(Bridge).ShouldContain(
                entry => entry.RoutePath == PageRoute
                    && entry.Label == "Reports"
                    && entry.Icon == "table"
                    && entry.Order == 20
                    && entry.Group == "Operations"
                    && entry.SubGroup == "Daily",
                "The navigation does not preserve the metadata the page declares."),
            () => ComponentsPageOwnerContracts.Navigation(Bridge).ShouldNotContain(
                entry => entry.RoutePath == HiddenRoute,
                "The page that hides itself from the navigation is still offered in the navigation."),
            () => ComponentsPageOwnerContracts.Resolve(Bridge, HiddenRoute).ShouldNotBeNull(
                "The page that hides itself from the navigation is not routable any more."),
            () => ComponentsPageOwnerContracts.PageEntries(Bridge).ShouldContain(
                entry => entry.PluginKey == ActiveOwner
                    && entry.Component == PageIdentity
                    && entry.Label == "Reports"
                    && entry.Icon == "table"
                    && entry.Order == 20
                    && entry.ShowInNav
                    && entry.Group == "Operations"
                    && entry.SubGroup == "Daily",
                "The Plugins page catalog does not carry the adapted metadata of the page."));

    [Fact(
        DisplayName = "UAC02: a duplicate, a conflicting, an unresolved, a wrong type, an undescribed and a reserved page is refused by name and the published projection keeps the first valid page",
        Skip = "TODO: the Components plugin page owner bridge is not implemented yet, so no owned page can be validated before publication and no refused page can leave the published projection untouched.")]
    [Trait(Spec.UAC, "02")]
    [Given("a published page of an active owner, beside further pages that duplicate it, conflict with its route, do not resolve, resolve to a type that is not a component, carry no page metadata or claim a reserved host route")]
    [When("each page is validated before publication")]
    [Then("every refused page is reported by name and the published projection keeps the first valid page")]
    public void UAC02_RefusedPagesKeepThePublishedProjection() =>
        Bridge.ShouldSatisfyAllConditions(
            () => ComponentsPageOwnerContracts.ErrorCodeNames().ShouldContain(
                "DuplicateBinding",
                "The candidate diagnostics do not name a duplicate binding."),
            () => ComponentsPageOwnerContracts.ErrorCodeNames().ShouldContain(
                "ConflictingRoutePath",
                "The candidate diagnostics do not name a conflicting route path."),
            () => ComponentsPageOwnerContracts.ErrorCodeNames().ShouldContain(
                "UnresolvedComponent",
                "The candidate diagnostics do not name an unresolved component."),
            () => ComponentsPageOwnerContracts.ErrorCodeNames().ShouldContain(
                "WrongComponentType",
                "The candidate diagnostics do not name a wrong component type."),
            () => ComponentsPageOwnerContracts.ErrorCodeNames().ShouldContain(
                "MissingPageMetadata",
                "The candidate diagnostics do not name missing page metadata."),
            () => ComponentsPageOwnerContracts.ErrorCodeNames().ShouldContain(
                "ReservedHostRoute",
                "The candidate diagnostics do not name a reserved host route."),
            () => IsValid(Publish(ActiveOwner, PageIdentity, PageRoute)).ShouldBeTrue(
                "The page of the active owner was not accepted."),
            () => Errors(Publish(ActiveOwner, PageIdentity, PageRoute)).ShouldContain(
                error => error.Code == "DuplicateBinding" && error.Subject == PageIdentity,
                "The second page of the same owner was not refused as a duplicate binding."),
            () => Errors(Publish(OtherOwner, PageIdentity, PageRoute)).ShouldContain(
                error => error.Code == "ConflictingRoutePath" && error.Subject == PageRoute,
                "The page of the second owner was not refused as a conflicting route path."),
            () => Errors(Publish(ActiveOwner, MissingIdentity, "/plugin/missing")).ShouldContain(
                error => error.Code == "UnresolvedComponent" && error.Subject == MissingIdentity,
                "The page whose symbolic identity does not resolve was not refused."),
            () => Errors(Publish(ActiveOwner, WrongTypeIdentity, "/plugin/wrong")).ShouldContain(
                error => error.Code == "WrongComponentType" && error.Subject == WrongTypeIdentity,
                "The page whose component type is not a component was not refused."),
            () => Errors(Publish(ActiveOwner, UndescribedIdentity, "/plugin/undescribed")).ShouldContain(
                error => error.Code == "MissingPageMetadata" && error.Subject == UndescribedIdentity,
                "The page that carries no page metadata was not refused."),
            () => Errors(Publish(ActiveOwner, PageIdentity, ReservedRoute)).ShouldContain(
                error => error.Code == "ReservedHostRoute" && error.Subject == ReservedRoute,
                "The page that claims a reserved host route was not refused."),
            () => ComponentsPageOwnerContracts.Enumerate(Bridge).ShouldContain(
                entry => entry.RoutePath == PageRoute && entry.SymbolicIdentity == PageIdentity,
                "The published projection does not keep the first valid page."),
            () => ComponentsPageOwnerContracts.Enumerate(Bridge).ShouldNotContain(
                entry => entry.RoutePath == ReservedRoute,
                "The refused page replaced the published projection."),
            () => ComponentsPageOwnerContracts.Navigation(Bridge).ShouldContain(
                entry => entry.RoutePath == PageRoute && entry.Label == "Reports",
                "The navigation of the published projection does not keep the first valid page."),
            () => ComponentsPageOwnerContracts.Navigation(Bridge).ShouldNotContain(
                entry => entry.RoutePath == ReservedRoute,
                "The refused page took a place in the navigation."));

    [Fact(
        DisplayName = "UAC03: a withdrawn owner loses its navigation entry, its direct route and its page catalog entry, and the host routes stay reserved",
        Skip = "TODO: the Components plugin page owner bridge does not project the active owners only yet, so a withdrawn owner can still offer its navigation entry, its direct route and its page catalog entry.")]
    [Trait(Spec.UAC, "03")]
    [Given("a published page of an owner that withdraws, beside the reserved host route of the host")]
    [When("every lookup path is queried again after the withdrawal")]
    [Then("the navigation entry, the direct route and the page catalog entry of the withdrawn owner are gone and the host route stays reserved")]
    public void UAC03_AWithdrawnOwnerLosesItsNavigationEntryAndItsDirectRoute() =>
        Bridge.ShouldSatisfyAllConditions(
            () => IsValid(Publish(ActiveOwner, PageIdentity, PageRoute)).ShouldBeTrue(
                "The page of the owner was not accepted."),
            () => ComponentsPageOwnerContracts.Resolve(Bridge, PageRoute).ShouldNotBeNull(
                "The route of the page of the owner does not resolve before the withdrawal."),
            () => ComponentsPageOwnerContracts.Navigation(Bridge).ShouldContain(
                entry => entry.RoutePath == PageRoute,
                "The navigation does not offer the page of the owner before the withdrawal."),
            () => WithdrawOwner().ShouldBeTrue(
                "The bridge did not accept the withdrawal of the owner."),
            () => ComponentsPageOwnerContracts.GetRevision(Bridge).ShouldBe(
                2,
                "The bridge does not project the revision of the withdrawal."),
            () => ComponentsPageOwnerContracts.IsOwnerActive(Bridge, ActiveOwner).ShouldBeFalse(
                "The owner is still active in the delivered revision."),
            () => ComponentsPageOwnerContracts.Resolve(Bridge, PageRoute).ShouldBeNull(
                "The direct route of the withdrawn owner still resolves."),
            () => ComponentsPageOwnerContracts.Navigation(Bridge).ShouldNotContain(
                entry => entry.RoutePath == PageRoute,
                "The navigation still offers the page of the withdrawn owner."),
            () => ComponentsPageOwnerContracts.Enumerate(Bridge).ShouldNotContain(
                entry => entry.RoutePath == PageRoute,
                "The withdrawn owner is still published."),
            () => ComponentsPageOwnerContracts.PageEntries(Bridge).ShouldNotContain(
                entry => entry.Component == PageIdentity,
                "The Plugins page catalog still carries the page of the withdrawn owner."),
            () => ComponentsPageOwnerContracts.IsOwnerActive(Bridge, OtherOwner).ShouldBeTrue(
                "The withdrawal of one owner deactivated another owner."));

    /// <summary>The trusted startup map that binds the symbolic identities.</summary>
    private static ComponentsStartupMap StartupMap =>
        new(
            [
                new ComponentsStartupBinding(ActiveOwner, PageIdentity, typeof(FakePluginPage)),
                new ComponentsStartupBinding(ActiveOwner, HiddenIdentity, typeof(FakeHiddenPluginPage)),
                new ComponentsStartupBinding(ActiveOwner, UndescribedIdentity, typeof(FakeUndescribedPluginPage)),
                new ComponentsStartupBinding(ActiveOwner, WrongTypeIdentity, typeof(string)),
            ],
            [ActiveOwner, OtherOwner]);

    /// <summary>The bridge under test, over the trusted startup map.</summary>
    private object Bridge =>
        _bridge ??= ComponentsPageOwnerContracts.CreateBridge(
            StartupMap,
            ComponentsPageOwnerContracts.CreateSnapshot(1, [ActiveOwner, OtherOwner]),
            [ReservedRoute]);

    private object Publish(string ownerKey, string symbolicIdentity, string routePath) =>
        ComponentsPageOwnerContracts.Publish(
            Bridge,
            ComponentsPageOwnerContracts.CreateBinding(ownerKey, symbolicIdentity, routePath));

    private static bool IsValid(object validation) =>
        ComponentsPageOwnerContracts.IsValid(validation);

    private static IReadOnlyList<(string Code, string Subject, string Message)> Errors(object validation) =>
        ComponentsPageOwnerContracts.ReadErrors(validation);

    private bool WithdrawOwner()
    {
        ComponentsPageOwnerContracts.Deliver(
            Bridge,
            ComponentsPageOwnerContracts.CreateSnapshot(2, [OtherOwner]));

        return true;
    }
}
