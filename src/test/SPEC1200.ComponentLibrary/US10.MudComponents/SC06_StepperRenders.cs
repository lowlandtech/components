using Bunit;
using LowlandTech.Components.Components.Stepper;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US10.MudComponents;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "06")]
public class SC06_StepperRenders : Scenario
{
    private TestContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new TestContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "UAC22: Then I should have Stepper and Step components")]
    [Trait(Spec.UAC, "22")]
    public async Task UAC22_ShouldHaveStepperComponents()
    {
        await ArrangeAndActAsync();
        var stepper = _ctx.Render<Stepper>();
        var step = _ctx.Render<Step>();

        stepper.ShouldNotBeNull();
        step.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC23: And Stepper should render a wrapper with class stepper")]
    [Trait(Spec.UAC, "23")]
    public async Task UAC23_ShouldRenderStepperWrapper()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<Stepper>();

        var element = component.Find(".stepper");
        element.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC24: And Step should render a wrapper with class step")]
    [Trait(Spec.UAC, "24")]
    public async Task UAC24_ShouldRenderStepWrapper()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<Step>();

        var element = component.Find(".step");
        element.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC25: And the active step should include class step-active")]
    [Trait(Spec.UAC, "25")]
    public async Task UAC25_ShouldApplyActiveClass()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<Stepper>(builder => builder
            .Add(p => p.ActiveIndex, 0)
            .AddChildContent<Step>(step1 => step1
                .Add(p => p.Label, "Step 1")
            )
            .AddChildContent<Step>(step2 => step2
                .Add(p => p.Label, "Step 2")
            )
        );

        // The first step should have step-active
        var steps = component.FindAll(".step");
        steps.Count.ShouldBeGreaterThan(0);
        steps[0].ClassList.ShouldContain("step-active");
    }

    [Fact(DisplayName = "UAC26: And completed and disabled steps should include correct classes")]
    [Trait(Spec.UAC, "26")]
    public async Task UAC26_ShouldApplyCompletedAndDisabledClasses()
    {
        await ArrangeAndActAsync();
        
        using var ctx1 = new TestContext();
        var completed = ctx1.Render<Step>(parameters => parameters.Add(p => p.Completed, true));
        completed.Find(".step").ClassList.ShouldContain("step-complete");

        using var ctx2 = new TestContext();
        var disabled = ctx2.Render<Step>(parameters => parameters.Add(p => p.Disabled, true));
        disabled.Find(".step").ClassList.ShouldContain("step-disabled");
    }
}
