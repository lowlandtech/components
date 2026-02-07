using Bunit;
using LowlandTech.Components.Components.Checkbox;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Tests.SPEC1227.ComponentLibrary.US03.BasicFormComponents;

/// <summary>
/// Tests for Checkbox component functionality.
/// SC01 - Implement Checkbox component
/// </summary>
[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
public class SC01_CheckboxRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Checkbox>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Checkbox>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render an <input type=\"checkbox\"> element by default")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldRenderCheckboxElement()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();
        
        var checkbox = _component!.Find("input[type='checkbox']");
        checkbox.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should apply the base \"checkbox\" class")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldApplyBaseClass()
    {
        await ArrangeAndActAsync();
        
        var checkbox = _component!.Find("input");
        checkbox.ClassList.ShouldContain("checkbox");
    }

    [Fact(DisplayName = "And it should support a Checked parameter with two-way binding")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ShouldSupportCheckedBinding()
    {
        var isChecked = false;
        
        using var ctx = new BunitContext();
        var component = ctx.Render<Checkbox>(parameters => parameters
            .Add(p => p.Checked, isChecked)
            .Add(p => p.CheckedChanged, EventCallback.Factory.Create<bool>(
                this, 
                value => isChecked = value
            ))
        );

        var checkbox = component.Find("input");
        
        // Initially unchecked
        checkbox.HasAttribute("checked").ShouldBeFalse();
        
        // Trigger change to checked
        await checkbox.ChangeAsync(new ChangeEventArgs { Value = true });
        
        // Verify state updated
        isChecked.ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should support an indeterminate state")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldSupportIndeterminate()
    {
        using var ctx = new BunitContext();
        
        // Setup JSInterop to handle indeterminate state setting
        ctx.JSInterop.SetupVoid("eval", _ => true);
        
        var component = ctx.Render<Checkbox>(parameters => parameters
            .Add(p => p.Indeterminate, true)
            .Add(p => p.Id, "test-checkbox")
        );

        var checkbox = component.Find("input");
        checkbox.ShouldNotBeNull();
        
        // Verify the component accepts the parameter
        component.Instance.Indeterminate.ShouldBeTrue();
        
        // Verify JS was invoked to set indeterminate
        ctx.JSInterop.VerifyInvoke("eval");
    }

    [Fact(DisplayName = "And it should support a Disabled state")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldSupportDisabled()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Checkbox>(parameters => parameters
            .Add(p => p.Disabled, true)
        );

        var checkbox = component.Find("input");
        checkbox.HasAttribute("disabled").ShouldBeTrue();
    }
}
