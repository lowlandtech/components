using Bunit;
using LowlandTech.Components.Components.InputOtp;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for InputOtp component rendering and functionality.
/// SC02 - Implement InputOtp component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "02")]
public class SC02_InputOtpRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<InputOtp>? _inputOtpComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _inputOtpComponent = _ctx.Render<InputOtp>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a grouped set of input slots")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldRenderInputSlots()
    {
        using var ctx = new BunitContext();
        
        // Create InputOtp with default length (6)
        var component = ctx.Render<InputOtp>();
        
        // Should have wrapper with input-otp class
        var wrapper = component.Find(".input-otp");
        wrapper.ShouldNotBeNull();
        
        // Should have 6 input slots by default
        var inputs = component.FindAll("input.input-otp-slot");
        inputs.Count.ShouldBe(6);
        
        // Each input should have maxlength=1 and type=text
        foreach (var input in inputs)
        {
            input.GetAttribute("type").ShouldBe("text");
            input.GetAttribute("maxlength").ShouldBe("1");
        }
    }

    [Fact(DisplayName = "And it should support a Length parameter for the number of slots")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldSupportLengthParameter()
    {
        using var ctx = new BunitContext();
        
        // Create InputOtp with Length=4
        var component4 = ctx.Render<InputOtp>(parameters => parameters
            .Add(p => p.Length, 4));
        
        // Should have 4 input slots
        var inputs4 = component4.FindAll("input.input-otp-slot");
        inputs4.Count.ShouldBe(4);
        
        // Test with Length=8
        var component8 = ctx.Render<InputOtp>(parameters => parameters
            .Add(p => p.Length, 8));
        
        var inputs8 = component8.FindAll("input.input-otp-slot");
        inputs8.Count.ShouldBe(8);
    }

    [Fact(DisplayName = "And it should expose a Value and ValueChanged for binding")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ShouldSupportValueBinding()
    {
        using var ctx = new BunitContext();
        
        string? otpValue = null;
        
        // Create InputOtp with Value binding
        var component = ctx.Render<InputOtp>(parameters => parameters
            .Add(p => p.Length, 4)
            .Add(p => p.Value, otpValue)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, value => otpValue = value)));
        
        // Get the first input
        var firstInput = component.Find("input.input-otp-slot");
        
        // Simulate entering a digit
        await firstInput.InputAsync(new ChangeEventArgs { Value = "1" });
        
        // Value should have been updated
        otpValue.ShouldNotBeNull();
        otpValue.ShouldStartWith("1");
        
        // Test setting initial value - create new component with initial value
        var componentWithValue = ctx.Render<InputOtp>(parameters => parameters
            .Add(p => p.Length, 4)
            .Add(p => p.Value, "1234"));
        
        var inputs = componentWithValue.FindAll("input.input-otp-slot");
        inputs[0].GetAttribute("value").ShouldBe("1");
        inputs[1].GetAttribute("value").ShouldBe("2");
        inputs[2].GetAttribute("value").ShouldBe("3");
        inputs[3].GetAttribute("value").ShouldBe("4");
    }
}
