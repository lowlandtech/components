namespace LowlandTech.Components.Tests.CPKG0002.Packaging.US01.Feed;

/// <summary>
/// SC01 — A scenario consumes the published batch of the shared feed.
///
/// <para>
/// The CPKG0001 packaging harness packs the packages of this repository into a
/// private feed and restores a temporary consumer against that feed alone.
/// The switch this scope refines copies the published identities of the shared
/// feed into the scenario feed instead of packing them, so that a scenario
/// consumes the published batch rather than a fresh pack.
/// </para>
///
/// <para>
/// The scope is refined. The acceptance criteria below name the behavior of
/// the switch, and their Facts are discoverable and skipped until the
/// implementation phase of this scope is released.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "CPKG0002")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A scenario consumes the published batch of the shared feed instead of a fresh pack")]
public class SC01_PublishedFeedConsumption
{
    [Fact(DisplayName = "UAC01: the packaging harness selects the published batch of the shared feed")]
    [Trait(Spec.UAC, "01")]
    [Given("the packaging harness of this repository")]
    [When("its parameters are read")]
    [Then("it offers a switch that copies the published identities of the shared feed into the scenario feed instead of packing them")]
    public void UAC01_HarnessSelectsThePublishedBatch()
    {
        Assert.Skip(
            "The CPKG0001 packaging harness has no switch that copies the published identities of the " +
            "shared feed into the scenario feed instead of packing them. The scope is refined and " +
            "released to the coordinator; the implementation phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC02: a scenario consumes the published identities of the shared feed")]
    [Trait(Spec.UAC, "02")]
    [Given("a scenario that selects the published batch of the shared feed")]
    [When("the scenario runs and the consumer restores")]
    [Then("the consumer resolves the published versions of the packages and not a fresh pack")]
    public void UAC02_ScenarioConsumesThePublishedIdentities()
    {
        Assert.Skip(
            "The packaging harness has no switch that copies the published identities of the shared " +
            "feed into the scenario feed, so no scenario can consume the published batch yet. The " +
            "scope is refined and released to the coordinator; the implementation phase of this " +
            "scope is not released.");
    }

    [Fact(DisplayName = "UAC03: the default path of the harness still packs the packages of this repository")]
    [Trait(Spec.UAC, "03")]
    [Given("a scenario that does not select the published batch")]
    [When("the scenario runs")]
    [Then("the packages of this repository are packed into the scenario feed and the gate of the closed CPKG0001 scope is unchanged")]
    public void UAC03_DefaultPathStillPacksThePackagesOfThisRepository()
    {
        Assert.Skip(
            "The switch does not exist yet, so the default path of the harness is not proven to be " +
            "unchanged against it. The scope is refined and released to the coordinator; the " +
            "implementation phase of this scope is not released.");
    }

    [Fact(DisplayName = "UAC04: the copied published batch travels with its dependency closure")]
    [Trait(Spec.UAC, "04")]
    [Given("a scenario that selects the published batch of the shared feed")]
    [When("the scenario feed is assembled")]
    [Then("the dependency closure of the copied packages travels with them and the consumer never reaches outside the scenario feed")]
    public void UAC04_CopiedBatchTravelsWithItsDependencyClosure()
    {
        Assert.Skip(
            "The packaging harness has no switch that copies the published identities of the shared " +
            "feed into the scenario feed, so the closure of a copied batch is not proven to travel " +
            "with it. The scope is refined and released to the coordinator; the implementation phase " +
            "of this scope is not released.");
    }
}
