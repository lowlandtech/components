using LowlandTech.Components.E2E.Infrastructure;

namespace LowlandTech.Components.E2E.SPEC1100.FormStories;

/// <summary>
/// SPEC1100 US01 — E2E tests for every Form group story in the Showcode app.
/// Each [Fact] navigates to a story via the sidebar, interacts with the
/// primary interactive element(s), and asserts the expected DOM state.
/// </summary>
[Trait(Spec.SPEC, "1100")]
[Trait(Spec.US, "01")]
public sealed class SC01_FormStoriesE2E(ShowcodeCollection collection) : ShowcodeScenario(collection)
{
    [Fact(DisplayName = "UAC01: Button story — all variant buttons render and are clickable")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ButtonStory()
    {
        await Showcode.GoToStoryAsync("Button");

        // At least the first button variant should be visible.
        var buttons = Showcode.ComponentButtons;
        await Assertions.Expect(buttons.First).ToBeVisibleAsync();

        // Click the default button — no error or exception should occur.
        await buttons.First.ClickAsync();

        // The button should still be in the DOM after the click.
        await Assertions.Expect(buttons.First).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC02: Button Group story — buttons render and are clickable")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ButtonGroupStory()
    {
        await Showcode.GoToStoryAsync("Button Group");

        var buttons = Showcode.ComponentButtons;
        await Assertions.Expect(buttons.First).ToBeVisibleAsync();
        await buttons.First.ClickAsync();
        await Assertions.Expect(buttons.First).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC03: Input story — text can be typed into an input")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_InputStory()
    {
        await Showcode.GoToStoryAsync("Input");

        var input = Showcode.TextInputs.First;
        await Assertions.Expect(input).ToBeVisibleAsync();

        await input.ClearAsync();
        await input.FillAsync("hello playwright");

        var value = await input.InputValueAsync();
        value.ShouldBe("hello playwright");
    }

    [Fact(DisplayName = "UAC04: Textarea story — text can be typed into a textarea")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_TextareaStory()
    {
        await Showcode.GoToStoryAsync("Textarea");

        var textarea = Showcode.Textareas.First;
        await Assertions.Expect(textarea).ToBeVisibleAsync();

        await textarea.ClearAsync();
        await textarea.FillAsync("playwright test content");

        var value = await textarea.InputValueAsync();
        value.ShouldBe("playwright test content");
    }

    [Fact(DisplayName = "UAC05: Checkbox story — checkbox can be checked and unchecked")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_CheckboxStory()
    {
        await Showcode.GoToStoryAsync("Checkbox");

        var checkbox = Showcode.Checkboxes.First;
        await Assertions.Expect(checkbox).ToBeVisibleAsync();

        // Click to check — verify via data-state (Blazor sets this as lowercase).
        await checkbox.ClickAsync();
        (await checkbox.GetAttributeAsync("data-state")).ShouldBe("checked");

        // Click to uncheck.
        await checkbox.ClickAsync();
        (await checkbox.GetAttributeAsync("data-state")).ShouldBe("unchecked");
    }

    [Fact(DisplayName = "UAC06: Switch story — switch renders and is interactive")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_SwitchStory()
    {
        await Showcode.GoToStoryAsync("Switch");

        // Switch renders as <button role="switch" data-slot="switch">, not as input[type='checkbox'].
        var switchEl = Showcode.Page.Locator("main [data-slot='switch']").First;
        await Assertions.Expect(switchEl).ToBeVisibleAsync();

        var wasChecked = await switchEl.IsCheckedAsync();
        await switchEl.ClickAsync();
        (await switchEl.IsCheckedAsync()).ShouldBe(!wasChecked);
    }

    [Fact(DisplayName = "UAC07: Slider story — slider renders with ARIA value attributes")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_SliderStory()
    {
        await Showcode.GoToStoryAsync("Slider");

        var slider = Showcode.RangeInputs.First;
        await Assertions.Expect(slider).ToBeVisibleAsync();

        // Slider renders as a custom div with role="slider" and ARIA value attrs.
        var min = await slider.GetAttributeAsync("aria-valuemin");
        var max = await slider.GetAttributeAsync("aria-valuemax");
        var now = await slider.GetAttributeAsync("aria-valuenow");
        min.ShouldNotBeNullOrWhiteSpace();
        max.ShouldNotBeNullOrWhiteSpace();
        now.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "UAC08: Label story — label text is visible")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_LabelStory()
    {
        await Showcode.GoToStoryAsync("Label");

        var label = Showcode.Page.Locator("main label").First;
        await Assertions.Expect(label).ToBeVisibleAsync();
    }

    [Fact(DisplayName = "UAC09: Radio Group story — radio items can be selected")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_RadioGroupStory()
    {
        await Showcode.GoToStoryAsync("Radio Group");

        // RadioGroupItem renders as a div with data-slot="radio-group-item" and data-state.
        var radios = Showcode.RadioInputs;
        await Assertions.Expect(radios.First).ToBeVisibleAsync();

        // Select the second radio item.
        var second = radios.Nth(1);
        await second.ClickAsync();
        (await second.GetAttributeAsync("data-state")).ShouldBe("checked");
    }

    [Fact(DisplayName = "UAC10: Toggle story — toggle buttons render and can be clicked")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ToggleStory()
    {
        await Showcode.GoToStoryAsync("Toggle");

        // Toggle renders as a button.
        var toggleBtn = Showcode.ComponentButtons.First;
        await Assertions.Expect(toggleBtn).ToBeVisibleAsync();
        await toggleBtn.ClickAsync();
        await Assertions.Expect(toggleBtn).ToBeVisibleAsync();
    }
}
