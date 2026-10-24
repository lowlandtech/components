namespace LowlandTech.Components.Tests.CBLK0001.Integration.US01.ActiveOwnedBlocks;

/// <summary>
/// SC01 — Resolve active owned session blocks.
///
/// <para>
/// A Components host resolves session blocks only for the plugin owners that
/// are active, through a reusable optional bridge. A loaded type is not proof
/// of active ownership: an owned descriptor is validated before it is
/// published, and every lookup path, including the assembly-scanning
/// fallback, stops offering it once its owner withdraws.
/// </para>
/// <para>
/// Every Fact is skipped until the block owner bridge exists: the Components
/// session block owner contracts are the missing dependency. A skipped Fact is
/// unfinished inventory, never acceptance.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CBLK0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers resolve only the session blocks of active plugin owners through a reusable optional bridge")]
public sealed class SC01_ActiveOwnedBlocks
{
    private const string ActiveOwner = "owner.active";
    private const string OtherOwner = "owner.other";
    private const string InactiveOwner = "owner.inactive";

    private const string PluginIdentity = "Test.FakePluginSessionBlock";
    private const string WrongTypeIdentity = "Test.NotAComponent";
    private const string MissingIdentity = "Test.Missing";

    private object? _bridge;

    [Fact(
        DisplayName = "UAC01: a duplicate, a conflicting or a wrong type owned descriptor is refused by name and the published lookup does not change",
        Skip = "TODO: the Components session block owner bridge (SessionBlockOwnerBridge, SessionBlockCandidate, SessionBlockOwnerSnapshot, SessionBlockCandidateValidation, SessionBlockCandidateErrorCode) is not implemented yet, so no owned candidate can be validated before publication.")]
    [Trait(Spec.UAC, "01")]
    [Given("an owned candidate that is published, and further owned candidates that duplicate its node type key, conflict with it, do not resolve or resolve to a type that is not a component")]
    [When("each candidate is validated before publication")]
    [Then("the refused candidates are reported by name and the published lookup keeps the first candidate")]
    public void UAC01_RefusedOwnedCandidatesKeepThePublishedLookup() =>
        Bridge.ShouldSatisfyAllConditions(
            () => ComponentsBlockOwnerContracts.ErrorCodeNames().ShouldContain(
                "DuplicateNodeTypeKey",
                "The candidate diagnostics do not name a duplicate node type key."),
            () => ComponentsBlockOwnerContracts.ErrorCodeNames().ShouldContain(
                "ConflictingNodeTypeKey",
                "The candidate diagnostics do not name a conflicting node type key."),
            () => ComponentsBlockOwnerContracts.ErrorCodeNames().ShouldContain(
                "UnresolvedComponent",
                "The candidate diagnostics do not name an unresolved component."),
            () => ComponentsBlockOwnerContracts.ErrorCodeNames().ShouldContain(
                "WrongComponentType",
                "The candidate diagnostics do not name a wrong component type."),
            () => IsValid(Publish(ActiveOwner, "plugin.order", PluginIdentity)).ShouldBeTrue(
                "The owned candidate of the active owner was not accepted."),
            () => Errors(Publish(ActiveOwner, "plugin.order", PluginIdentity)).ShouldContain(
                error => error.Code == "DuplicateNodeTypeKey" && error.Subject == "plugin.order",
                "The second candidate of the same owner was not refused as a duplicate node type key."),
            () => Errors(Publish(OtherOwner, "plugin.order", PluginIdentity)).ShouldContain(
                error => error.Code == "ConflictingNodeTypeKey" && error.Subject == "plugin.order",
                "The candidate of the second owner was not refused as a conflicting node type key."),
            () => Errors(Publish(ActiveOwner, "plugin.unresolved", MissingIdentity)).ShouldContain(
                error => error.Code == "UnresolvedComponent" && error.Subject == "Test.Missing",
                "The candidate whose symbolic identity does not resolve was not refused."),
            () => Errors(Publish(ActiveOwner, "plugin.wrong", WrongTypeIdentity)).ShouldContain(
                error => error.Code == "WrongComponentType" && error.Subject == WrongTypeIdentity,
                "The candidate whose component type is not a component was not refused."),
            () => Errors(Publish(ActiveOwner, "plugin.wrong", WrongTypeIdentity)).ShouldContain(
                error => error.Message.Contains(WrongTypeIdentity, StringComparison.Ordinal),
                "The refusal of the wrong component type does not report the offending identity."),
            () => Discovery.ResolveComponentType("plugin.order").ShouldBe(
                typeof(FakePluginSessionBlock),
                "A refused candidate changed the published lookup of the accepted candidate."),
            () => Discovery.ResolveComponentType("plugin.unresolved").ShouldBeNull(
                "A refused candidate was published."),
            () => Discovery.ResolveComponentType("plugin.wrong").ShouldBeNull(
                "A refused candidate was published."));

    [Fact(
        DisplayName = "UAC02: only the valid descriptors of active owners take part in the priority resolution and the host defaults stay available",
        Skip = "TODO: the Components session block owner bridge is not implemented yet, so no owned descriptor can take part in the resolution of the active owners.")]
    [Trait(Spec.UAC, "02")]
    [Given("owned candidates of two active owners with different priorities and one candidate of an inactive owner, beside the host defaults")]
    [When("the discovery queries the descriptors, the descriptor of a node type key and the component type of a key")]
    [Then("only the valid descriptors of the active owners take part, in a deterministic priority order, and the host defaults stay available")]
    public void UAC02_OnlyValidActiveDescriptorsTakePartInTheResolution() =>
        Bridge.ShouldSatisfyAllConditions(
            () => IsValid(Publish(ActiveOwner, "plugin.order", PluginIdentity, priority: 100)).ShouldBeTrue(
                "The owned candidate of the active owner was not accepted."),
            () => IsValid(Publish(OtherOwner, "plugin.report", PluginIdentity, priority: 50)).ShouldBeTrue(
                "The owned candidate of the second active owner was not accepted."),
            () => IsValid(Publish(InactiveOwner, "plugin.inactive", PluginIdentity, priority: 100)).ShouldBeTrue(
                "The owned candidate of the inactive owner was refused although only its owner is inactive."),
            () => ComponentsBlockOwnerContracts.IsOwnerActive(Bridge, ActiveOwner).ShouldBeTrue(
                "The active owner is not active in the delivered revision."),
            () => ComponentsBlockOwnerContracts.IsOwnerActive(Bridge, InactiveOwner).ShouldBeFalse(
                "The inactive owner is active in the delivered revision."),
            () => Discovery.ResolveComponentType("plugin.order").ShouldBe(
                typeof(FakePluginSessionBlock),
                "The descriptor of the active owner does not resolve."),
            () => Discovery.ResolveComponentType("plugin.report").ShouldBe(
                typeof(FakePluginSessionBlock),
                "The descriptor of the second active owner does not resolve."),
            () => Discovery.ResolveComponentType("plugin.inactive").ShouldBeNull(
                "The descriptor of the inactive owner takes part in the resolution."),
            () => Discovery.ResolveComponentType("session").ShouldBe(
                typeof(SessionCard),
                "The host default does not stay available beside the owned descriptors."),
            () => Discovery.ResolveComponentType("session.pin").ShouldBe(
                typeof(SessionContextPin),
                "The host default does not stay available beside the owned descriptors."),
            () => Discovery.GetDescriptors().Select(descriptor => descriptor.NodeTypeKey).ShouldContain(
                "plugin.order",
                "The descriptor of the active owner is missing."),
            () => Discovery.GetDescriptors().Select(descriptor => descriptor.NodeTypeKey).ShouldNotContain(
                "plugin.inactive",
                "The descriptor of the inactive owner takes part in the descriptors."),
            () => PriorityOrder().ShouldBe(
                ["plugin.order", "plugin.report"],
                "The owned descriptors do not resolve in a deterministic priority order."));

    [Fact(
        DisplayName = "UAC03: a withdrawn owner cannot reappear through any lookup path and the host fallback stays available",
        Skip = "TODO: the Components session block owner bridge does not filter the assembly-scanning fallback yet, so a withdrawn plugin component can still reappear as a plugin contribution.")]
    [Trait(Spec.UAC, "03")]
    [Given("an owned descriptor that overrides a host default, published by an owner that withdraws")]
    [When("every lookup path is queried again after the withdrawal")]
    [Then("the plugin contribution cannot reappear and the host fallback stays available")]
    public void UAC03_AWithdrawnOwnerCannotReappearThroughAnyLookupPath() =>
        Bridge.ShouldSatisfyAllConditions(
            () => IsValid(Publish(ActiveOwner, "workitem", PluginIdentity, priority: 100)).ShouldBeTrue(
                "The owned descriptor that overrides the host default was not accepted."),
            () => IsValid(Publish(ActiveOwner, "plugin.order", PluginIdentity, priority: 100)).ShouldBeTrue(
                "The owned descriptor of the active owner was not accepted."),
            () => ScanningFallback.ResolveComponentType(typeof(FakePluginSessionBlock).Name).ShouldBe(
                typeof(FakePluginSessionBlock),
                "The assembly-scanning fallback does not publish the plugin component, so the bridge has nothing to filter."),
            () => Discovery.ResolveComponentType(typeof(FakePluginSessionBlock).Name).ShouldBeNull(
                "The plugin component is reachable under its own type name while its owner is active."),
            () => WithdrawOwner().ShouldBeTrue(
                "The bridge did not accept the withdrawal of the owner."),
            () => ComponentsBlockOwnerContracts.GetRevision(Bridge).ShouldBe(
                2,
                "The bridge does not project the revision of the withdrawal."),
            () => Discovery.ResolveComponentType("workitem").ShouldBe(
                typeof(SessionCard),
                "The host fallback of the overridden node type key is not available after the withdrawal."),
            () => Discovery.ResolveComponentType("plugin.order").ShouldBeNull(
                "The descriptor of the withdrawn owner reappeared."),
            () => Discovery.ResolveComponentType(typeof(FakePluginSessionBlock).Name).ShouldBeNull(
                "The withdrawn plugin component reappeared through the assembly-scanning fallback."),
            () => Discovery.GetDescriptors().Select(descriptor => descriptor.NodeTypeKey).ShouldNotContain(
                "plugin.order",
                "The descriptor of the withdrawn owner is still published."),
            () => Discovery.ResolveComponentType("session").ShouldBe(
                typeof(SessionCard),
                "The host defaults are not available after the withdrawal."));

    /// <summary>The trusted startup map that binds the symbolic identities.</summary>
    private static ComponentsStartupMap StartupMap =>
        new(
            [
                new ComponentsStartupBinding(ActiveOwner, PluginIdentity, typeof(FakePluginSessionBlock)),
                new ComponentsStartupBinding(ActiveOwner, WrongTypeIdentity, typeof(string)),
            ],
            [ActiveOwner, OtherOwner, InactiveOwner]);

    /// <summary>The bridge under test, over the host defaults and the fallback.</summary>
    private object Bridge =>
        _bridge ??= ComponentsBlockOwnerContracts.CreateBridge(
            StartupMap,
            ComponentsBlockOwnerContracts.CreateSnapshot(1, [ActiveOwner, OtherOwner]),
            [new DefaultSessionBlockRegistry(), new AssemblyScanningSessionBlockRegistry()]);

    /// <summary>The discovery that a host builds over the bridge.</summary>
    private ISessionBlockDiscovery Discovery =>
        new SessionBlockDiscovery([ComponentsBlockOwnerContracts.GetRegistry(Bridge)]);

    /// <summary>The discovery a host builds over the fallback alone.</summary>
    private static ISessionBlockDiscovery ScanningFallback =>
        new SessionBlockDiscovery([new AssemblyScanningSessionBlockRegistry()]);

    private object Publish(string ownerKey, string nodeTypeKey, string symbolicIdentity, int priority = 0) =>
        ComponentsBlockOwnerContracts.Publish(
            Bridge,
            ComponentsBlockOwnerContracts.CreateCandidate(ownerKey, nodeTypeKey, symbolicIdentity, priority));

    private static bool IsValid(object validation) =>
        ComponentsBlockOwnerContracts.IsValid(validation);

    private static IReadOnlyList<(string Code, string Subject, string Message)> Errors(object validation) =>
        ComponentsBlockOwnerContracts.ReadErrors(validation);

    private bool WithdrawOwner()
    {
        ComponentsBlockOwnerContracts.Deliver(
            Bridge,
            ComponentsBlockOwnerContracts.CreateSnapshot(2, []));

        return true;
    }

    private IReadOnlyList<string> PriorityOrder() =>
        Discovery.GetDescriptors()
            .Where(descriptor => descriptor.NodeTypeKey.StartsWith("plugin.", StringComparison.Ordinal))
            .Select(descriptor => descriptor.NodeTypeKey)
            .ToList();
}
