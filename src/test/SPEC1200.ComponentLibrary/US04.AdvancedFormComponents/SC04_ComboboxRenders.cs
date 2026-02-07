using Bunit;
using LowlandTech.Components.Components.Combobox;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Combobox component rendering and functionality.
/// SC04 - Implement Combobox component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "04")]
public class SC04_ComboboxRenders : Scenario
{
    private BunitContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
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

    [Fact(DisplayName = "Then it should combine an input with a selectable list")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_ShouldCombineInputWithList()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<Combobox>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxItem>(0);
                builder.AddAttribute(1, "Value", "item1");
                builder.AddAttribute(2, "Label", "Item 1");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Item 1")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(4);
                builder.AddAttribute(5, "Value", "item2");
                builder.AddAttribute(6, "Label", "Item 2");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Item 2")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(8);
                builder.AddAttribute(9, "Value", "item3");
                builder.AddAttribute(10, "Label", "Item 3");
                builder.AddAttribute(11, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Item 3")));
                builder.CloseComponent();
            }));

        // Assert - Verify combobox container exists
        var comboboxElement = component.Find(".combobox");
        comboboxElement.ShouldNotBeNull();

        // Verify input exists
        var inputElement = component.Find(".combobox-input");
        inputElement.ShouldNotBeNull();
        inputElement.TagName.ShouldBe("INPUT");
        inputElement.GetAttribute("type").ShouldBe("text");

        // Focus the input to open the dropdown
        await inputElement.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());

        // Verify dropdown appears
        var dropdownElement = component.Find(".combobox-dropdown");
        dropdownElement.ShouldNotBeNull();

        // Verify list exists
        var listElement = component.Find(".combobox-list");
        listElement.ShouldNotBeNull();

        // Verify items are rendered
        var items = component.FindAll(".combobox-item");
        items.Count.ShouldBe(3);
        items[0].TextContent.ShouldBe("Item 1");
        items[1].TextContent.ShouldBe("Item 2");
        items[2].TextContent.ShouldBe("Item 3");
    }

    [Fact(DisplayName = "And it should filter items based on user input")]
    [Trait(Spec.UAC, "12")]
    public async Task UAC12_ShouldFilterItems()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<Combobox>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxItem>(0);
                builder.AddAttribute(1, "Value", "calendar");
                builder.AddAttribute(2, "Label", "Calendar");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Calendar")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(4);
                builder.AddAttribute(5, "Value", "search");
                builder.AddAttribute(6, "Label", "Search");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Search")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(8);
                builder.AddAttribute(9, "Value", "settings");
                builder.AddAttribute(10, "Label", "Settings");
                builder.AddAttribute(11, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Settings")));
                builder.CloseComponent();
            }));

        var input = component.Find(".combobox-input");

        // Assert - Initially, focus should show all items
        await input.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());
        var items = component.FindAll(".combobox-item");
        items.Count.ShouldBe(3);

        // Type "cal" to filter
        await input.InputAsync("cal");
        items = component.FindAll(".combobox-item");
        items.Count.ShouldBe(1);
        items[0].TextContent.ShouldBe("Calendar");

        // Type "se" to filter
        await input.InputAsync("se");
        items = component.FindAll(".combobox-item");
        items.Count.ShouldBe(2); // Search and Settings both contain "se"
        
        // Type "search" to filter more specifically
        await input.InputAsync("search");
        items = component.FindAll(".combobox-item");
        items.Count.ShouldBe(1);
        items[0].TextContent.ShouldBe("Search");
    }

    [Fact(DisplayName = "And it should support an empty state when no matches are found")]
    [Trait(Spec.UAC, "13")]
    public async Task UAC13_ShouldSupportEmptyState()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<Combobox>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxItem>(0);
                builder.AddAttribute(1, "Value", "calendar");
                builder.AddAttribute(2, "Label", "Calendar");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Calendar")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(4);
                builder.AddAttribute(5, "Value", "search");
                builder.AddAttribute(6, "Label", "Search");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Search")));
                builder.CloseComponent();
            }));

        var input = component.Find(".combobox-input");

        // Focus to open dropdown
        await input.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());

        // Assert - Type something that doesn't match
        await input.InputAsync("xyz");

        // Should show empty state
        var emptyElement = component.Find(".combobox-empty");
        emptyElement.ShouldNotBeNull();
        emptyElement.TextContent.ShouldContain("No items found");

        // Items should not be visible
        var items = component.FindAll(".combobox-item");
        items.Count.ShouldBe(0);

        // Test with custom empty content
        var customComponent = ctx.Render<Combobox>(parameters => parameters
            .Add(p => p.EmptyContent, builder => builder.AddContent(0, "Custom empty message"))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxItem>(0);
                builder.AddAttribute(1, "Value", "test");
                builder.AddAttribute(2, "Label", "Test");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Test")));
                builder.CloseComponent();
            }));

        var customInput = customComponent.Find(".combobox-input");
        await customInput.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());
        await customInput.InputAsync("nomatch");

        var customEmpty = customComponent.Find(".combobox-empty");
        customEmpty.TextContent.ShouldContain("Custom empty message");
    }

    [Fact(DisplayName = "And it should expose a Value and ValueChanged for binding")]
    [Trait(Spec.UAC, "14")]
    public async Task UAC14_ShouldSupportValueBinding()
    {
        // Arrange
        using var ctx = new BunitContext();
        string? selectedValue = null;
        var valueChanged = false;

        // Act
        var component = ctx.Render<Combobox>(parameters => parameters
            .Add(p => p.Value, selectedValue)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value =>
            {
                selectedValue = value;
                valueChanged = true;
            }))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxItem>(0);
                builder.AddAttribute(1, "Value", "option1");
                builder.AddAttribute(2, "Label", "Option 1");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Option 1")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(4);
                builder.AddAttribute(5, "Value", "option2");
                builder.AddAttribute(6, "Label", "Option 2");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Option 2")));
                builder.CloseComponent();
            }));

        var input = component.Find(".combobox-input");
        await input.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());

        // Assert - Initially no value selected
        selectedValue.ShouldBeNull();
        valueChanged.ShouldBeFalse();

        // Click on first item
        var items = component.FindAll(".combobox-item");
        items[0].Click();

        // ValueChanged should have been invoked
        valueChanged.ShouldBeTrue();
        selectedValue.ShouldBe("option1");

        // Test with pre-selected value
        var componentWithValue = ctx.Render<Combobox>(parameters => parameters
            .Add(p => p.Value, "option2")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ComboboxItem>(0);
                builder.AddAttribute(1, "Value", "option1");
                builder.AddAttribute(2, "Label", "Option 1");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Option 1")));
                builder.CloseComponent();
                
                builder.OpenComponent<ComboboxItem>(4);
                builder.AddAttribute(5, "Value", "option2");
                builder.AddAttribute(6, "Label", "Option 2");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Option 2")));
                builder.CloseComponent();
            }));

        var input2 = componentWithValue.Find(".combobox-input");
        await input2.FocusAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());
        var items2 = componentWithValue.FindAll(".combobox-item");
        
        // Second item should be marked as selected
        items2[1].ClassList.ShouldContain("combobox-item-selected");
    }
}
