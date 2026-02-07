using Bunit;
using LowlandTech.Components.Components.Textarea;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US03.BasicFormComponents;

/// <summary>
/// Tests for Textarea component functionality.
/// SC03 - Implement Textarea component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "03")]
public class SC03_TextareaRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Textarea>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        // Default rendering - tests will override with specific parameters
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "UAC10: Then it should render a <textarea> element with class 'input'")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldRenderTextareaElementWithClass()
    {
        await GivenAsync();
        
        _component = _ctx.Render<Textarea>();
        
        _component.ShouldNotBeNull();
        var textarea = _component.Find("textarea");
        textarea.ShouldNotBeNull();
        textarea.ClassList.ShouldContain("input");
    }

    [Fact(DisplayName = "UAC11: And it should support Rows and Placeholder parameters")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_ShouldSupportRowsAndPlaceholder()
    {
        await GivenAsync();

        _component = _ctx.Render<Textarea>(parameters => parameters
            .Add(p => p.Rows, 5)
            .Add(p => p.Placeholder, "Enter your comments here"));

        var textarea = _component.Find("textarea");
        textarea.GetAttribute("rows").ShouldBe("5");
        textarea.GetAttribute("placeholder").ShouldBe("Enter your comments here");
    }

    [Fact(DisplayName = "UAC12: And it should support Disabled and ReadOnly states")]
    [Trait(Spec.UAC, "12")]
    public async Task UAC12_ShouldSupportDisabledAndReadOnly()
    {
        await GivenAsync();

        // Test Disabled state
        _component = _ctx.Render<Textarea>(parameters => parameters
            .Add(p => p.Disabled, true));

        var textarea = _component.Find("textarea");
        textarea.HasAttribute("disabled").ShouldBeTrue();

        // Test ReadOnly state
        _component = _ctx.Render<Textarea>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        textarea = _component.Find("textarea");
        textarea.HasAttribute("readonly").ShouldBeTrue();

        // Test both states
        _component = _ctx.Render<Textarea>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ReadOnly, true));

        textarea = _component.Find("textarea");
        textarea.HasAttribute("disabled").ShouldBeTrue();
        textarea.HasAttribute("readonly").ShouldBeTrue();
    }
}
