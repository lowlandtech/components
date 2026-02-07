using Bunit;
using LowlandTech.Components.Components.Slider;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Slider component rendering and functionality.
/// SC07 - Implement Slider component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "07")]
public class SC07_SliderRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Slider>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Slider>(parameters => parameters
            .Add(p => p.Min, 0)
            .Add(p => p.Max, 100)
            .Add(p => p.Step, 1)
            .Add(p => p.Value, 50));
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a track and thumb")]
    [Trait(Spec.UAC, "22")]
    public async Task UAC22_ShouldRenderTrackAndThumb()
    {
        await ArrangeAndActAsync();

        var slider = _component!.Find(".slider");
        slider.ShouldNotBeNull();

        var track = _component.Find(".slider-track");
        track.ShouldNotBeNull();

        var range = _component.Find(".slider-range");
        range.ShouldNotBeNull();

        var thumb = _component.Find(".slider-thumb");
        thumb.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support Min, Max, and Step parameters")]
    [Trait(Spec.UAC, "23")]
    public async Task UAC23_ShouldSupportMinMaxStep()
    {
        await ArrangeAndActAsync();

        var input = _component!.Find("input[type='range']");
        input.ShouldNotBeNull();
        
        input.GetAttribute("min").ShouldBe("0");
        input.GetAttribute("max").ShouldBe("100");
        input.GetAttribute("step").ShouldBe("1");
        input.GetAttribute("value").ShouldBe("50");
    }

    [Fact(DisplayName = "And it should support Value and ValueChanged for binding")]
    [Trait(Spec.UAC, "24")]
    public async Task UAC24_ShouldSupportValueBinding()
    {
        _ctx = new BunitContext();
        double changedValue = 0;
        var valueChanged = false;

        _component = _ctx.Render<Slider>(parameters => parameters
            .Add(p => p.Min, 0)
            .Add(p => p.Max, 100)
            .Add(p => p.Step, 10)
            .Add(p => p.Value, 50)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<double>(this, value =>
            {
                changedValue = value;
                valueChanged = true;
            })));

        var input = _component.Find("input[type='range']");
        input.ShouldNotBeNull();

        // Simulate changing the value to 60
        input.Input("60");

        valueChanged.ShouldBeTrue();
        changedValue.ShouldBe(60);
    }
}
