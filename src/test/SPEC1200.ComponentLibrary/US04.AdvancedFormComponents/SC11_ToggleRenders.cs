using Bunit;
using LowlandTech.Components.Components.Toggle;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Toggle component rendering and functionality.
/// SC11 - Implement Toggle component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "11")]
public class SC11_ToggleRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a button-like toggle control")]
    [Trait(Spec.UAC, "34")]
    public async Task UAC34_ShouldRenderToggleControl()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Toggle>(parameters => parameters
            .Add(p => p.Pressed, false)
            .Add(p => p.ChildContent, "Toggle")
        );
        var toggle = component.Find(".toggle");
        toggle.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support a pressed state via Value and ValueChanged")]
    [Trait(Spec.UAC, "35")]
    public async Task UAC35_ShouldSupportPressedState()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var isPressed = false;
        var component = ctx.Render<Toggle>(parameters => parameters
            .Add(p => p.Pressed, isPressed)
            .Add(p => p.ValueChanged, value => isPressed = value)
            .Add(p => p.ChildContent, "Toggle")
        );
        var toggle = component.Find(".toggle");
        toggle.ClassList.ShouldNotContain("toggle-pressed");
        toggle.Click();
        isPressed.ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should support a Disabled state")]
    [Trait(Spec.UAC, "36")]
    public async Task UAC36_ShouldSupportDisabled()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Toggle>(parameters => parameters
            .Add(p => p.Pressed, false)
            .Add(p => p.Disabled, true)
            .Add(p => p.ChildContent, "Toggle")
        );
        var toggle = component.Find(".toggle");
        toggle.HasAttribute("disabled").ShouldBeTrue();
        toggle.ClassList.ShouldContain("toggle-disabled");
    }
}
