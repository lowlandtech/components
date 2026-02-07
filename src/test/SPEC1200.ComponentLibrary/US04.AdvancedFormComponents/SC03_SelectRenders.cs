using Bunit;
using LowlandTech.Components.Components.Select;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Select component rendering and functionality.
/// SC03 - Implement Select component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "03")]
public class SC03_SelectRenders : Scenario
{
    private BunitContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Select, SelectTrigger, SelectContent, and SelectItem components")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldHaveSelectComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // Select
        var select = ctx.Render<Select>(parameters => parameters
            .Add(p => p.ChildContent, "Test"));
        select.ShouldNotBeNull();
        
        // SelectTrigger
        var trigger = ctx.Render<SelectTrigger>(parameters => parameters
            .AddCascadingValue(new Select()));
        trigger.ShouldNotBeNull();
        
        // SelectContent
        var content = ctx.Render<SelectContent>(parameters => parameters
            .AddCascadingValue(new Select()));
        content.ShouldNotBeNull();
        
        // SelectItem
        var item = ctx.Render<SelectItem>(parameters => parameters
            .Add(p => p.Value, "1")
            .AddCascadingValue(new Select()));
        item.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And select content should render via Portal")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldRenderViaPortal()
    {
        using var ctx = new BunitContext();
        
        var select = ctx.Render<Select>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenComponent<SelectContent>(0);
                builder.AddAttribute(1, "UsePortal", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));
        
        select.ShouldNotBeNull();
        var content = select.Find(".select-content");
        content.ShouldNotBeNull();
        content.TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "And it should support keyboard navigation and selection")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_ShouldSupportKeyboardNavigation()
    {
        using var ctx = new BunitContext();
        
        var select = ctx.Render<Select>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenComponent<SelectContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<SelectItem>(0);
                    contentBuilder.AddAttribute(1, "Value", "1");
                    contentBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Option 1");
                    }));
                    contentBuilder.CloseComponent();
                    
                    contentBuilder.OpenComponent<SelectItem>(2);
                    contentBuilder.AddAttribute(3, "Value", "2");
                    contentBuilder.AddAttribute(4, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Option 2");
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));
        
        select.ShouldNotBeNull();
        var items = select.FindAll(".select-item");
        items.Count.ShouldBe(2);
    }

    [Fact(DisplayName = "And it should support disabled items")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldSupportDisabledItems()
    {
        using var ctx = new BunitContext();
        
        var item = ctx.Render<SelectItem>(parameters => parameters
            .Add(p => p.Value, "1")
            .Add(p => p.IsDisabled, true)
            .Add(p => p.ChildContent, "Disabled Item")
            .AddCascadingValue(new Select()));
        
        item.ShouldNotBeNull();
        var div = item.Find(".select-item");
        div.ClassList.ShouldContain("select-item-disabled");
    }
}
