using Bunit;
using LowlandTech.Components.Components.RadioGroup;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for RadioGroup component rendering and functionality.
/// SC05 - Implement RadioGroup component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "05")]
public class SC05_RadioGroupRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<RadioGroup<string>>? _radioGroupComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _radioGroupComponent = _ctx.Render<RadioGroup<string>>(parameters => parameters
            .AddChildContent("<span>Test</span>")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have RadioGroup and RadioGroupItem components")]
    [Trait(Spec.UAC, "15")]
    public async Task UAC15_ShouldHaveRadioGroupComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // RadioGroup
        var radioGroup = ctx.Render<RadioGroup<string>>(parameters => parameters
            .AddChildContent("<span>Test</span>"));
        radioGroup.ShouldNotBeNull();
        
        // RadioGroupItem - needs to be rendered within RadioGroup for cascading value
        var groupWithItem = ctx.Render<RadioGroup<string>>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RadioGroupItem<string>>(0);
                builder.AddAttribute(1, "Value", "test");
                builder.CloseComponent();
            }));
        groupWithItem.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support a default selected value")]
    [Trait(Spec.UAC, "16")]
    public async Task UAC16_ShouldSupportDefaultValue()
    {
        using var ctx = new BunitContext();
        
        // Create RadioGroup with default Value
        var component = ctx.Render<RadioGroup<string>>(parameters => parameters
            .Add(p => p.Value, "option2")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RadioGroupItem<string>>(0);
                builder.AddAttribute(1, "Value", "option1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Option 1")));
                builder.CloseComponent();
                
                builder.OpenComponent<RadioGroupItem<string>>(3);
                builder.AddAttribute(4, "Value", "option2");
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Option 2")));
                builder.CloseComponent();
            }));
        
        // Find the checked radio input
        var radioInputs = component.FindAll("input[type='radio']");
        radioInputs.Count.ShouldBe(2);
        
        // Second radio should be checked
        var secondRadio = radioInputs[1];
        secondRadio.HasAttribute("checked").ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should support a controlled value via Value and ValueChanged")]
    [Trait(Spec.UAC, "17")]
    public async Task UAC17_ShouldSupportControlledValue()
    {
        using var ctx = new BunitContext();
        
        string? selectedValue = "option1";
        
        // Create RadioGroup with two-way binding
        var component = ctx.Render<RadioGroup<string>>(parameters => parameters
            .Add(p => p.Value, selectedValue)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, value => selectedValue = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RadioGroupItem<string>>(0);
                builder.AddAttribute(1, "Value", "option1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Option 1")));
                builder.CloseComponent();
                
                builder.OpenComponent<RadioGroupItem<string>>(3);
                builder.AddAttribute(4, "Value", "option2");
                builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Option 2")));
                builder.CloseComponent();
            }));
        
        // Click the second radio item
        var secondRadioItem = component.FindAll(".radio-group-item")[1];
        await secondRadioItem.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        
        // Value should have changed
        selectedValue.ShouldBe("option2");
    }

    [Fact(DisplayName = "And it should support a Disabled state for items")]
    [Trait(Spec.UAC, "18")]
    public async Task UAC18_ShouldSupportDisabledItems()
    {
        using var ctx = new BunitContext();
        
        // Create RadioGroup with one disabled item
        var component = ctx.Render<RadioGroup<string>>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RadioGroupItem<string>>(0);
                builder.AddAttribute(1, "Value", "option1");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Option 1")));
                builder.CloseComponent();
                
                builder.OpenComponent<RadioGroupItem<string>>(3);
                builder.AddAttribute(4, "Value", "option2");
                builder.AddAttribute(5, "Disabled", true);
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Option 2 (Disabled)")));
                builder.CloseComponent();
            }));
        
        // Find radio inputs
        var radioInputs = component.FindAll("input[type='radio']");
        radioInputs.Count.ShouldBe(2);
        
        // Second radio should be disabled
        var secondRadio = radioInputs[1];
        secondRadio.HasAttribute("disabled").ShouldBeTrue();
        
        // Second radio item should have disabled class
        var secondRadioItem = component.FindAll(".radio-group-item")[1];
        secondRadioItem.ClassList.ShouldContain("radio-group-item-disabled");
    }
}
