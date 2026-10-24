namespace LowlandTech.Components.Tests.CTHE0001.Integration.US01.ProviderThemes;

/// <summary>
/// SC01 — Resolve active provider-scoped themes.
///
/// <para>
/// A Components host resolves the themes of the plugin owners that are active,
/// through a reusable optional bridge. An installed theme is not proof of
/// active ownership: a theme binding is validated before it is published, a
/// provider only resolves the theme of an active owner, and the withdrawal of
/// an owner restores the host defaults of the providers it had selected.
/// </para>
/// <para>
/// Every Fact is skipped until the theme owner bridge exists: the Components
/// theme owner contracts are the missing dependency. A skipped Fact is
/// unfinished inventory, never acceptance.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CTHE0001")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Components consumers resolve only the themes of active plugin owners, per provider boundary, and restore the host defaults after a withdrawal")]
public sealed class SC01_ProviderThemes
{
    private const string ActiveOwner = "owner.active";
    private const string OtherOwner = "owner.other";
    private const string InactiveOwner = "owner.inactive";

    private const string ProviderA = "provider.a";
    private const string ProviderB = "provider.b";
    private const string ProviderC = "provider.c";
    private const string ThemeKey = "dark";

    private const string BaseClass = "base-class";
    private const string ConsumerClass = "consumer-class";
    private const string PluginClass = "plugin-class";
    private const string HostClass = "host-class";

    private const string Stylesheet = "_content/Sample.Theme/plugin.css";

    private static readonly ComponentSlotKey RootSlot = new("Button", "Root");

    private object? _bridge;

    [Fact(
        DisplayName = "UAC01: two independent providers resolve only their own theme and the consumer override keeps its precedence",
        Skip = "TODO: the Components theme owner bridge (ComponentThemeOwnerBridge, ComponentThemeBinding, ComponentThemeOwnerSnapshot) is not implemented yet, so no owned theme can be published to a provider boundary.")]
    [Trait(Spec.UAC, "01")]
    [Given("an active owner that publishes a plugin theme beside the host theme, and two independent provider boundaries")]
    [When("the plugin theme is selected for the first provider and both providers resolve classes, fragments and style families")]
    [Then("only the first provider resolves the plugin theme and the consumer override keeps its precedence")]
    public void UAC01_OnlyTheSelectedProviderResolvesTheOwnedTheme() =>
        Bridge.ShouldSatisfyAllConditions(
            () => IsValid(Publish(ActiveOwner, ProviderA)).ShouldBeTrue(
                "The binding of the active owner was not accepted."),
            () => ComponentsThemeOwnerContracts.Select(Bridge, ProviderA, ThemeKey).ShouldBeTrue(
                "The bridge did not select the theme of the active owner for the provider."),
            () => ThemeNameOf(ProviderA).ShouldBe(
                FakePluginTheme.ThemeName,
                "The provider does not resolve the theme it selected."),
            () => Service.Class(ProviderA, RootSlot, BaseClass, ConsumerClass).ShouldBe(
                $"{PluginClass} {ConsumerClass}",
                "The provider does not resolve the class of the owned theme, or the consumer override lost its precedence."),
            () => Service.Fragment(ProviderA, RootSlot).ShouldNotBeNull(
                "The provider does not resolve the fragment of the owned theme."),
            () => StyleFamilyOf(ProviderA).ShouldBe(
                "brand",
                "The provider does not resolve the style family of the owned theme."),
            () => ComponentsThemeOwnerContracts.StylesheetsFor(Bridge, ProviderA).ShouldBe(
                [Stylesheet],
                "The provider does not carry the stylesheet selection of the owned theme."),
            () => ThemeNameOf(ProviderB).ShouldBe(
                FakeHostTheme.ThemeName,
                "The second provider does not resolve its own theme anymore."),
            () => Service.Class(ProviderB, RootSlot, BaseClass, ConsumerClass).ShouldBe(
                $"{BaseClass} {HostClass} {ConsumerClass}",
                "The second provider resolves the theme of the owner, or the consumer override lost its precedence."),
            () => Service.Fragment(ProviderB, RootSlot).ShouldBeNull(
                "The second provider resolves the fragment of the owned theme."),
            () => StyleFamilyOf(ProviderB).ShouldBe(
                "default",
                "The second provider resolves the style family of the owned theme."),
            () => ComponentsThemeOwnerContracts.StylesheetsFor(Bridge, ProviderB).ShouldBeEmpty(
                "The second provider carries the stylesheet selection of the owned theme."),
            () => ComponentsThemeOwnerContracts.Enumerate(Bridge).Count.ShouldBe(
                1,
                "The bridge does not publish the binding of the active owner."));

    [Fact(
        DisplayName = "UAC02: an inactive or invalid theme binding is excluded or rejected before publication and the provider selections do not change",
        Skip = "TODO: the Components theme owner bridge does not validate a theme binding before publication yet, so an inactive or invalid binding can still be published.")]
    [Trait(Spec.UAC, "02")]
    [Given("a published binding of an active owner, a binding of an inactive owner and an invalid binding")]
    [When("both bindings are published and the themes are enumerated")]
    [Then("the inactive binding is excluded and the invalid binding is rejected before publication, and the provider selections do not change")]
    public void UAC02_InactiveAndInvalidBindingsAreNotPublished() =>
        Bridge.ShouldSatisfyAllConditions(
            () => ComponentsThemeOwnerContracts.ErrorCodeNames().ShouldContain(
                "InactiveOwner",
                "The binding diagnostics do not name an inactive owner."),
            () => ComponentsThemeOwnerContracts.ErrorCodeNames().ShouldContain(
                "MissingThemeName",
                "The binding diagnostics do not name a missing theme name."),
            () => ComponentsThemeOwnerContracts.ErrorCodeNames().ShouldContain(
                "InvalidStyleFamily",
                "The binding diagnostics do not name an invalid style family."),
            () => ComponentsThemeOwnerContracts.ErrorCodeNames().ShouldContain(
                "UndefinedMergeMode",
                "The binding diagnostics do not name an undefined merge mode."),
            () => ComponentsThemeOwnerContracts.ErrorCodeNames().ShouldContain(
                "InvalidStylesheetPath",
                "The binding diagnostics do not name an invalid stylesheet path."),
            () => IsValid(Publish(ActiveOwner, ProviderA)).ShouldBeTrue(
                "The binding of the active owner was not accepted."),
            () => ComponentsThemeOwnerContracts.Select(Bridge, ProviderA, ThemeKey).ShouldBeTrue(
                "The bridge did not select the theme of the active owner for the provider."),
            () => IsValid(Publish(InactiveOwner, ProviderC)).ShouldBeFalse(
                "The binding of the inactive owner was accepted."),
            () => Errors(Publish(InactiveOwner, ProviderC)).ShouldContain(
                error => error.Code == "InactiveOwner" && error.Subject == InactiveOwner,
                "The binding of the inactive owner was not refused as an inactive owner."),
            () => IsValid(Publish(ActiveOwner, ProviderC, theme: new FakeInvalidTheme())).ShouldBeFalse(
                "The invalid binding was accepted."),
            () => Errors(Publish(ActiveOwner, ProviderC, theme: new FakeInvalidTheme())).ShouldContain(
                error => error.Code == "MissingThemeName",
                "The binding without a theme name was not refused."),
            () => Errors(Publish(ActiveOwner, ProviderC, theme: new FakeInvalidTheme())).ShouldContain(
                error => error.Code == "InvalidStyleFamily",
                "The binding with an invalid style family was not refused."),
            () => Errors(Publish(ActiveOwner, ProviderC, theme: new FakeInvalidTheme())).ShouldContain(
                error => error.Code == "UndefinedMergeMode",
                "The binding with an undefined merge mode was not refused."),
            () => IsValid(Publish(ActiveOwner, ProviderC, stylesheets: ["../outside/plugin.css"])).ShouldBeFalse(
                "The binding with a stylesheet outside the package was accepted."),
            () => Errors(Publish(ActiveOwner, ProviderC, stylesheets: ["../outside/plugin.css"])).ShouldContain(
                error => error.Code == "InvalidStylesheetPath",
                "The binding with a stylesheet outside the package was not refused."),
            () => ComponentsThemeOwnerContracts.Enumerate(Bridge).ShouldNotContain(
                binding => SubjectOf(binding) == ProviderC,
                "A refused binding was published."),
            () => ThemeNameOf(ProviderA).ShouldBe(
                FakePluginTheme.ThemeName,
                "A refused binding changed the selection of the provider."),
            () => ComponentsThemeOwnerContracts.Select(Bridge, ProviderC, ThemeKey).ShouldBeFalse(
                "The bridge selected a theme for a provider whose binding was refused."));

    [Fact(
        DisplayName = "UAC03: the withdrawal of an owner restores the host defaults and detaches its stylesheet selection while the other providers keep their state",
        Skip = "TODO: the Components theme owner bridge does not restore the host defaults after a withdrawal yet, so a withdrawn owner keeps its theme selected.")]
    [Trait(Spec.UAC, "03")]
    [Given("two owners with an active theme each, selected for two providers")]
    [When("one owner withdraws and the themes are enumerated and resolved again")]
    [Then("the provider of the withdrawn owner resolves the host defaults, its stylesheet selection is detached and the other provider keeps its state")]
    public void UAC03_AWithdrawnOwnerRestoresTheHostDefaults() =>
        Bridge.ShouldSatisfyAllConditions(
            () => IsValid(Publish(ActiveOwner, ProviderA)).ShouldBeTrue(
                "The binding of the first owner was not accepted."),
            () => IsValid(Publish(OtherOwner, ProviderB)).ShouldBeTrue(
                "The binding of the second owner was not accepted."),
            () => ComponentsThemeOwnerContracts.Select(Bridge, ProviderA, ThemeKey).ShouldBeTrue(
                "The bridge did not select the theme of the first owner."),
            () => ComponentsThemeOwnerContracts.Select(Bridge, ProviderB, ThemeKey).ShouldBeTrue(
                "The bridge did not select the theme of the second owner."),
            () => Withdraw(ActiveOwner).ShouldBeTrue(
                "The bridge did not accept the withdrawal of the owner."),
            () => ComponentsThemeOwnerContracts.GetRevision(Bridge).ShouldBe(
                2,
                "The bridge does not project the revision of the withdrawal."),
            () => ThemeNameOf(ProviderA).ShouldBe(
                FakeHostTheme.ThemeName,
                "The provider of the withdrawn owner does not resolve the host default again."),
            () => Service.Class(ProviderA, RootSlot, BaseClass, ConsumerClass).ShouldBe(
                $"{BaseClass} {HostClass} {ConsumerClass}",
                "The provider of the withdrawn owner still resolves the class of the withdrawn theme."),
            () => StyleFamilyOf(ProviderA).ShouldBe(
                "default",
                "The provider of the withdrawn owner still resolves the style family of the withdrawn theme."),
            () => ComponentsThemeOwnerContracts.StylesheetsFor(Bridge, ProviderA).ShouldBeEmpty(
                "The stylesheet selection of the withdrawn owner is still attached."),
            () => ThemeNameOf(ProviderB).ShouldBe(
                FakePluginTheme.ThemeName,
                "The provider of the owner that is still active lost its theme."),
            () => ComponentsThemeOwnerContracts.StylesheetsFor(Bridge, ProviderB).ShouldBe(
                [Stylesheet],
                "The stylesheet selection of the owner that is still active was detached."),
            () => ComponentsThemeOwnerContracts.Enumerate(Bridge).Count.ShouldBe(
                1,
                "The bridge still publishes the binding of the withdrawn owner."));

    /// <summary>The host theme, registered before the plugin theme.</summary>
    private static FakeHostTheme HostTheme { get; } = new();

    /// <summary>The theme the plugin owner contributes.</summary>
    private static FakePluginTheme PluginTheme { get; } = new();

    /// <summary>The resolver of the host, with the host theme registered first.</summary>
    private ComponentThemeService Service { get; } = new([HostTheme, PluginTheme]);

    /// <summary>The bridge under test, over the authority of the host.</summary>
    private object Bridge =>
        _bridge ??= ComponentsThemeOwnerContracts.CreateBridge(
            Service,
            ComponentsThemeOwnerContracts.CreateSnapshot(1, [ActiveOwner, OtherOwner]));

    private object Publish(
        string ownerKey,
        string providerId,
        IComponentTheme? theme = null,
        IReadOnlyList<string>? stylesheets = null) =>
        ComponentsThemeOwnerContracts.Publish(
            Bridge,
            ComponentsThemeOwnerContracts.CreateBinding(
                ownerKey,
                providerId,
                ThemeKey,
                theme ?? PluginTheme,
                stylesheets ?? [Stylesheet]));

    private bool Withdraw(string ownerKey)
    {
        var active = ownerKey == ActiveOwner ? new[] { OtherOwner } : new[] { ActiveOwner };

        ComponentsThemeOwnerContracts.Deliver(
            Bridge,
            ComponentsThemeOwnerContracts.CreateSnapshot(2, active));

        return true;
    }

    private static bool IsValid(object validation) =>
        ComponentsThemeOwnerContracts.IsValid(validation);

    private static IReadOnlyList<(string Code, string Subject, string Message)> Errors(object validation) =>
        ComponentsThemeOwnerContracts.ReadErrors(validation);

    private string ThemeNameOf(string providerId) =>
        Service.GetForProvider(providerId)?.Name ?? string.Empty;

    private string StyleFamilyOf(string providerId) =>
        (Service.GetForProvider(providerId) as IThemeStyleFamily)?.StyleFamily ?? string.Empty;

    private static string SubjectOf(object binding) =>
        (string)binding.GetType().GetProperty("ProviderId")!.GetValue(binding)!;
}
