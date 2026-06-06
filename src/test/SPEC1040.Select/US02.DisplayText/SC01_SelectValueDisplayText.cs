using SelectSut = LowlandTech.Components.Select;

namespace LowlandTech.Components.Tests.SPEC1040.Select.US02.DisplayText;

[Trait(Spec.SPEC, "1040")]
[Trait(Spec.SC, "01")]
[UserStory("02", "SelectValue shows item display text, not raw value id")]
public class SC01_SelectValueDisplayText : ViewScenario
{
    // ── helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Renders a full Select with a trigger/value and a closed SelectContent
    /// containing items. Mirrors the real-world usage where the dropdown is
    /// hidden on first load but items must still register their text.
    /// </summary>
    private IRenderedComponent<SelectSut> RenderSelect(
        string value,
        (string Value, string Text)[] items)
    {
        return Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.Value, value);
            p.AddChildContent<SelectTrigger>(tp =>
                tp.AddChildContent<SelectValue>());
            p.AddChildContent<SelectContent>(cp =>
            {
                foreach (var (v, t) in items)
                    cp.AddChildContent<SelectItem>(ip =>
                        ip.Add(x => x.Value, v)
                          .Add(x => x.Text, t)
                          .AddChildContent(t));
            });
        });
    }

    // ── UAC01 – initial render shows Text, not Value id ───────────────────

    [Fact(DisplayName = "UAC01: SelectValue shows item Text on initial render, not raw id")]
    [Trait(Spec.UAC, "01")]
    [Given("a Select with Value='nl' and an item Text='Nederlands'")]
    [Then("SelectValue displays 'Nederlands', not 'nl'")]
    public void UAC01_ShowsItemTextNotRawId()
    {
        var cut = RenderSelect("nl",
        [
            ("en", "English"),
            ("nl", "Nederlands"),
            ("de", "Deutsch"),
        ]);

        var value = cut.Find("[data-slot='select-value']");
        value.TextContent.ShouldBe("Nederlands");
        value.TextContent.ShouldNotBe("nl");
    }

    // ── UAC02 – Text parameter takes precedence over ChildContent ─────────

    [Fact(DisplayName = "UAC02: SelectValue uses explicit Text parameter over ChildContent fallback")]
    [Trait(Spec.UAC, "02")]
    [Given("a SelectItem with Value='en' and Text='English'")]
    [Then("SelectValue resolves 'English' from the Text parameter")]
    public void UAC02_ExplicitTextParameterIsUsed()
    {
        var cut = RenderSelect("en",
        [
            ("en", "English"),
        ]);

        cut.Find("[data-slot='select-value']").TextContent.ShouldBe("English");
    }

    // ── UAC03 – closed dropdown still registers items ─────────────────────

    [Fact(DisplayName = "UAC03: Items inside a closed SelectContent still register display text")]
    [Trait(Spec.UAC, "03")]
    [Given("a Select where IsOpen is false (dropdown hidden)")]
    [Then("SelectValue still resolves the item's display text on first render")]
    public void UAC03_ClosedContentRegistersItemText()
    {
        // IsOpen defaults to false — this is the regression case where
        // SelectContent was gated behind @if (IsOpen) and items never
        // ran OnParametersSet.
        var cut = RenderSelect("sv",
        [
            ("sv", "Svenska"),
        ]);

        cut.Find("[data-slot='select-value']").TextContent.ShouldBe("Svenska");
    }

    // ── UAC04 – after interaction SelectValue updates to new selection ─────

    [Fact(DisplayName = "UAC04: SelectValue updates display text after user picks a new item")]
    [Trait(Spec.UAC, "04")]
    [Given("a Select with Value='en' and the dropdown is open")]
    [When("the user clicks the 'Deutsch' item")]
    [Then("SelectValue displays 'Deutsch'")]
    public async Task UAC04_DisplayTextUpdatesAfterPick()
    {
        string? selected = null;

        var cut = Ctx.Render<SelectSut>(p =>
        {
            p.Add(x => x.Value, "en");
            p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => selected = v));
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<SelectTrigger>(tp =>
                tp.AddChildContent<SelectValue>());
            p.AddChildContent<SelectContent>(cp =>
            {
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "en").Add(x => x.Text, "English").AddChildContent("English"));
                cp.AddChildContent<SelectItem>(ip =>
                    ip.Add(x => x.Value, "de").Add(x => x.Text, "Deutsch").AddChildContent("Deutsch"));
            });
        });

        await cut.Find("[data-value='de']").ClickAsync(new());

        selected.ShouldBe("de");
        cut.Find("[data-slot='select-value']").TextContent.ShouldBe("Deutsch");
    }

    // ── UAC05 – unrecognised value falls back gracefully ──────────────────

    [Fact(DisplayName = "UAC05: SelectValue falls back to raw value when no matching item exists")]
    [Trait(Spec.UAC, "05")]
    [Given("a Select with Value='xx' and no item for that value")]
    [Then("SelectValue displays the raw value 'xx' as a graceful fallback")]
    public void UAC05_FallsBackToRawValueWhenNoItemMatch()
    {
        var cut = RenderSelect("xx",
        [
            ("en", "English"),
            ("nl", "Nederlands"),
        ]);

        cut.Find("[data-slot='select-value']").TextContent.ShouldBe("xx");
    }

    // ── UAC06 – multiple items, correct one is resolved ───────────────────

    [Fact(DisplayName = "UAC06: SelectValue resolves the correct item when multiple items are registered")]
    [Trait(Spec.UAC, "06")]
    [Given("a Select with 10 culture items and Value='fr'")]
    [Then("SelectValue displays 'Français', not any other item's label")]
    public void UAC06_CorrectItemResolvedFromMultiple()
    {
        var cut = RenderSelect("fr",
        [
            ("en", "English"),
            ("de", "Deutsch"),
            ("fr", "Français"),
            ("es", "Español"),
            ("it", "Italiano"),
            ("nl", "Nederlands"),
            ("pt", "Português"),
            ("pl", "Polski"),
            ("sv", "Svenska"),
            ("da", "Dansk"),
        ]);

        cut.Find("[data-slot='select-value']").TextContent.ShouldBe("Français");
    }
}
