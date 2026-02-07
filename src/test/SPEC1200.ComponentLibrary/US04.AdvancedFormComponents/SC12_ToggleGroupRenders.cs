using Bunit;
using LowlandTech.Components.Components.ToggleGroup;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for ToggleGroup component rendering and functionality.
/// SC12 - Implement ToggleGroup component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "12")]
public class SC12_ToggleGroupRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<ToggleGroup>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then I should have ToggleGroup and ToggleGroupItem components")]
    [Trait(Spec.UAC, "37")]
    public async Task UAC37_ShouldHaveToggleGroupComponents()
    {
        await ArrangeAndActAsync();

        _component = _ctx.Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, "single")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ToggleGroupItem>(0);
                builder.AddAttribute(1, "Value", "bold");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(3, "B")));
                builder.CloseComponent();
                
                builder.OpenComponent<ToggleGroupItem>(4);
                builder.AddAttribute(5, "Value", "italic");
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(7, "I")));
                builder.CloseComponent();
            }));

        var group = _component.Find(".toggle-group");
        group.ShouldNotBeNull();

        var items = _component.FindAll(".toggle-group-item");
        items.Count.ShouldBe(2);
    }

    [Fact(DisplayName = "And it should support single and multiple selection modes")]
    [Trait(Spec.UAC, "38")]
    public async Task UAC38_ShouldSupportSelectionModes()
    {
        await ArrangeAndActAsync();

        // Test single selection mode
        var singleValue = "";
        _component = _ctx.Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, "single")
            .Add(p => p.Value, "bold")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v => singleValue = v ?? ""))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ToggleGroupItem>(0);
                builder.AddAttribute(1, "Value", "bold");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(3, "B")));
                builder.CloseComponent();
                
                builder.OpenComponent<ToggleGroupItem>(4);
                builder.AddAttribute(5, "Value", "italic");
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(7, "I")));
                builder.CloseComponent();
            }));

        var items = _component.FindAll(".toggle-group-item");
        items[0].ClassList.ShouldContain("toggle-pressed");

        // Click second item - should switch selection
        items[1].Click();
        singleValue.ShouldBe("italic");

        // Test multiple selection mode
        var multiValue = "";
        var multiComponent = _ctx.Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, "multiple")
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v => multiValue = v ?? ""))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ToggleGroupItem>(0);
                builder.AddAttribute(1, "Value", "bold");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(3, "B")));
                builder.CloseComponent();
                
                builder.OpenComponent<ToggleGroupItem>(4);
                builder.AddAttribute(5, "Value", "italic");
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(7, "I")));
                builder.CloseComponent();
            }));

        var multiItems = multiComponent.FindAll(".toggle-group-item");
        multiItems[0].Click();
        multiValue.ShouldContain("bold");
        
        multiItems[1].Click();
        // Both should be selected
        multiValue.Split(',').Length.ShouldBe(2);
    }

    [Fact(DisplayName = "And it should support a default selected value")]
    [Trait(Spec.UAC, "39")]
    public async Task UAC39_ShouldSupportDefaultValue()
    {
        await ArrangeAndActAsync();

        _component = _ctx.Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, "single")
            .Add(p => p.DefaultValue, "bold")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ToggleGroupItem>(0);
                builder.AddAttribute(1, "Value", "bold");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(3, "B")));
                builder.CloseComponent();
                
                builder.OpenComponent<ToggleGroupItem>(4);
                builder.AddAttribute(5, "Value", "italic");
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(7, "I")));
                builder.CloseComponent();
            }));

        var items = _component.FindAll(".toggle-group-item");
        items[0].ClassList.ShouldContain("toggle-pressed");
    }

    [Fact(DisplayName = "And it should support a controlled value via Value and ValueChanged")]
    [Trait(Spec.UAC, "40")]
    public async Task UAC40_ShouldSupportControlledValue()
    {
        await ArrangeAndActAsync();

        var value = "bold";
        var valueChanged = false;

        _component = _ctx.Render<ToggleGroup>(parameters => parameters
            .Add(p => p.Type, "single")
            .Add(p => p.Value, value)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, v =>
            {
                value = v ?? "";
                valueChanged = true;
            }))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ToggleGroupItem>(0);
                builder.AddAttribute(1, "Value", "bold");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddContent(3, "B")));
                builder.CloseComponent();
                
                builder.OpenComponent<ToggleGroupItem>(4);
                builder.AddAttribute(5, "Value", "italic");
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(7, "I")));
                builder.CloseComponent();
            }));

        var items = _component.FindAll(".toggle-group-item");
        
        // Click italic item
        items[1].Click();
        
        valueChanged.ShouldBeTrue();
        value.ShouldBe("italic");
    }
}
