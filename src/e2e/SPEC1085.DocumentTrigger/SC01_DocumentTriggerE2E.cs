namespace LowlandTech.Components.E2E.SPEC1085.DocumentTrigger;

/// <summary>
/// SPEC1085 — the document view and the action trigger are observed in a browser.
///
/// <para>
/// The lane's own e2e project drives the demo host, which serves the components from source on an
/// ephemeral port. The story carries a scripted transition client, so applied, refused and
/// conflict are exercised without a network and without Graph's HTTP entry. Chromium is the
/// dedicated browser Playwright manages; no user browser is attached to.
/// </para>
/// </summary>
[Trait(Spec.SPEC, "1085")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A document and its trigger are proven in a browser")]
public sealed class SC01_DocumentTriggerE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    private const string Story = "Document";

    private ILocator Document => Page.Locator("[data-slot='document-view']");
    private ILocator Trigger => Page.Locator("[data-slot='transition-trigger']");
    private ILocator Result => Page.Locator("[data-trigger-result]");
    private IPage Page => Context.Pages[0];

    private async Task OpenAsync()
    {
        await Showcode.GoToStoryAsync(Story);
        await Assertions.Expect(Document).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC01: the declared labels and values render and no property key is shown")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_LabelsAndValuesRender()
    {
        await OpenAsync();

        await Assertions.Expect(Document).ToHaveAttributeAsync("data-document-type", "document.record");
        await Assertions.Expect(Document).ToHaveAttributeAsync("data-document-id", "3f2a9c11-0000-4000-8000-000000000007");

        var headline = Page.Locator("[data-field='headline']");
        await Assertions.Expect(headline).ToContainTextAsync("Title");
        await Assertions.Expect(headline).ToContainTextAsync("The tide turns");

        var text = await Document.InnerTextAsync();
        text.ShouldNotContain("headline");
        text.ShouldNotContain("detail");
    }

    [Fact(DisplayName = "UAC02: relations are read-only text and the declared action is offered")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_RelationsReadOnlyAndActionOffered()
    {
        await OpenAsync();

        var relation = Page.Locator("[data-field='parent']");
        await Assertions.Expect(relation).ToContainTextAsync("Belongs to");
        await Assertions.Expect(relation).ToContainTextAsync("Episode 1");
        await Assertions.Expect(relation).ToContainTextAsync("outgoing");
        (await Document.Locator("[data-field-input]").CountAsync()).ShouldBe(0);

        await Assertions.Expect(Page.Locator("[data-document-action='approve']")).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC03: running the action updates the region with the returned result")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_AppliedRenders()
    {
        await OpenAsync();
        await Page.Locator("[data-document-action='approve'] button").ClickAsync();

        await Assertions.Expect(Trigger).ToBeVisibleAsync();
        await Trigger.Locator("[data-trigger-submit] button").ClickAsync();

        await Assertions.Expect(Result).ToContainTextAsync("The record was saved.");
        await Assertions.Expect(Page.Locator("[data-result-case='document.applied']")).ToHaveAttributeAsync("data-severity", "success");
        await Assertions.Expect(Page.Locator("[data-story-outcome]")).ToHaveTextAsync("applied");
    }

    [Fact(DisplayName = "UAC04: a refusal renders its case against the field it names")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_RefusedRenders()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Refused", Exact = true }).ClickAsync();
        await Page.Locator("[data-document-action='approve'] button").ClickAsync();
        await Trigger.Locator("[data-trigger-submit] button").ClickAsync();

        await Assertions.Expect(Result).ToContainTextAsync("The record cannot be saved.");
        await Assertions.Expect(Page.Locator("[data-result-field='headline']")).ToContainTextAsync("The title is required.");
        await Assertions.Expect(Page.Locator("[data-story-outcome]")).ToHaveTextAsync("refused");
    }

    [Fact(DisplayName = "UAC05: a conflict shows the moved record and offers its action")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ConflictRenders()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Conflict", Exact = true }).ClickAsync();
        await Page.Locator("[data-document-action='approve'] button").ClickAsync();
        await Trigger.Locator("[data-trigger-submit] button").ClickAsync();

        await Assertions.Expect(Result).ToContainTextAsync("The record moved while you were editing.");
        await Assertions.Expect(Page.Locator("[data-result-actions='document.conflict'] button")).ToContainTextAsync("Try again");
        await Assertions.Expect(Page.Locator("[data-story-outcome]")).ToHaveTextAsync("conflict");
    }

    [Fact(DisplayName = "UAC06: a keyboard user reaches the result region and the outcome is announced")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ResultRegionIsFocusableAndPolitelyLive()
    {
        await OpenAsync();
        await Page.Locator("[data-document-action='approve'] button").ClickAsync();

        await Assertions.Expect(Result).ToHaveAttributeAsync("role", "status");
        await Assertions.Expect(Result).ToHaveAttributeAsync("aria-live", "polite");

        await Trigger.Locator("[data-trigger-submit] button").PressAsync("Enter");

        await Assertions.Expect(Result).ToContainTextAsync("The record was saved.");
        await Assertions.Expect(Result).ToBeFocusedAsync();
    }

    [Fact(DisplayName = "UAC07: the host page composes the two components and adds no document markup")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_HostAddsNoDocumentMarkup()
    {
        await OpenAsync();

        var host = Page.Locator("[data-slot='document-view']");
        await Assertions.Expect(host).ToHaveCountAsync(1);
        await Assertions.Expect(Page.Locator("[data-slot='transition-trigger']")).ToHaveCountAsync(0);

        var insideDocument = await host.Locator("form[data-slot='data-form']").CountAsync();
        insideDocument.ShouldBe(1);
        (await Page.Locator("[data-document-head]").CountAsync()).ShouldBe(1);
    }
}
