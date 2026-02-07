using Bunit;
using LowlandTech.Components.Components.Switch;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Switch component rendering and functionality.
/// SC06 - Implement Switch component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "06")]
public class SC06_SwitchRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a toggle control with class \"switch\"")]
    [Trait(Spec.UAC, "19")]
    public async Task UAC19_ShouldRenderSwitchControl()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Switch>(parameters => parameters
            .Add(p => p.Checked, false)
        );
        var toggle = component.Find(".switch");
        toggle.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support Checked and CheckedChanged for binding")]
    [Trait(Spec.UAC, "20")]
    public async Task UAC20_ShouldSupportCheckedBinding()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var isChecked = false;
        var component = ctx.Render<Switch>(parameters => parameters
            .Add(p => p.Checked, isChecked)
            .Add(p => p.CheckedChanged, value => isChecked = value)
        );
        var toggle = component.Find(".switch");
        toggle.Click();
        isChecked.ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should support a Disabled state")]
    [Trait(Spec.UAC, "21")]
    public async Task UAC21_ShouldSupportDisabled()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Switch>(parameters => parameters
            .Add(p => p.Checked, false)
            .Add(p => p.Disabled, true)
        );
        var toggle = component.Find(".switch");
        toggle.HasAttribute("disabled").ShouldBeTrue();
        toggle.ClassList.ShouldContain("switch-disabled");
    }
}
