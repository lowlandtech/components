using Bunit;
using LowlandTech.Components.Components.Command;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Command component rendering and functionality.
/// SC02 - Implement Command component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "02")]
public class SC02_CommandRenders : Scenario
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

    [Fact(DisplayName = "Then I should have Command, CommandInput, CommandList, and CommandItem components")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldHaveCommandComponents()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<Command>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CommandInput>(0);
                builder.CloseComponent();
                
                builder.OpenComponent<CommandList>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(listBuilder =>
                {
                    listBuilder.OpenComponent<CommandItem>(0);
                    listBuilder.AddAttribute(1, "Label", "Test Item");
                    listBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Test Item");
                    }));
                    listBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert
        // Verify Command container exists
        var commandElement = component.Find(".command");
        commandElement.ShouldNotBeNull();
        commandElement.ClassList.ShouldContain("command");

        // Verify CommandInput exists
        var inputElement = component.Find(".command-input");
        inputElement.ShouldNotBeNull();
        inputElement.TagName.ShouldBe("INPUT");

        // Verify CommandList exists
        var listElement = component.Find(".command-list");
        listElement.ShouldNotBeNull();

        // Verify CommandItem exists
        var itemElement = component.Find(".command-item");
        itemElement.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support filtering items based on input")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ShouldSupportFiltering()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<Command>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CommandInput>(0);
                builder.AddAttribute(1, "Placeholder", "Search...");
                builder.CloseComponent();
                
                builder.OpenComponent<CommandList>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(listBuilder =>
                {
                    listBuilder.OpenComponent<CommandItem>(0);
                    listBuilder.AddAttribute(1, "Label", "Calendar");
                    listBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Calendar")));
                    listBuilder.CloseComponent();
                    
                    listBuilder.OpenComponent<CommandItem>(3);
                    listBuilder.AddAttribute(4, "Label", "Search");
                    listBuilder.AddAttribute(5, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Search")));
                    listBuilder.CloseComponent();
                    
                    listBuilder.OpenComponent<CommandItem>(6);
                    listBuilder.AddAttribute(7, "Label", "Settings");
                    listBuilder.AddAttribute(8, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Settings")));
                    listBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert
        // Initially, all items should be visible
        var items = component.FindAll(".command-item");
        items.Count.ShouldBe(3);

        // Simulate typing "cal" in the input
        var input = component.Find(".command-input");
        await input.InputAsync("cal");

        // Only "Calendar" should be visible (case-insensitive)
        items = component.FindAll(".command-item");
        items.Count.ShouldBe(1);
        items[0].TextContent.ShouldContain("Calendar");

        // Test filtering with "set"
        await input.InputAsync("set");
        items = component.FindAll(".command-item");
        items.Count.ShouldBe(1);
        items[0].TextContent.ShouldContain("Settings");
    }

    [Fact(DisplayName = "And it should support keyboard navigation within the list")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldSupportKeyboardNavigation()
    {
        // Arrange
        using var ctx = new BunitContext();
        var selectedItem = string.Empty;

        // Act
        var component = ctx.Render<Command>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CommandInput>(0);
                builder.CloseComponent();
                
                builder.OpenComponent<CommandList>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(listBuilder =>
                {
                    listBuilder.OpenComponent<CommandItem>(0);
                    listBuilder.AddAttribute(1, "Label", "First");
                    listBuilder.AddAttribute(2, "OnSelect", EventCallback.Factory.Create(this, () => { selectedItem = "First"; }));
                    listBuilder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "First")));
                    listBuilder.CloseComponent();
                    
                    listBuilder.OpenComponent<CommandItem>(4);
                    listBuilder.AddAttribute(5, "Label", "Second");
                    listBuilder.AddAttribute(6, "OnSelect", EventCallback.Factory.Create(this, () => { selectedItem = "Second"; }));
                    listBuilder.AddAttribute(7, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Second")));
                    listBuilder.CloseComponent();
                    
                    listBuilder.OpenComponent<CommandItem>(8);
                    listBuilder.AddAttribute(9, "Label", "Third");
                    listBuilder.AddAttribute(10, "OnSelect", EventCallback.Factory.Create(this, () => { selectedItem = "Third"; }));
                    listBuilder.AddAttribute(11, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Third")));
                    listBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        var input = component.Find(".command-input");

        // Assert
        // Test ArrowDown navigation
        await input.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        var items = component.FindAll(".command-item");
        items[0].ClassList.ShouldContain("command-item-selected");

        // ArrowDown again
        await input.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });
        items = component.FindAll(".command-item");
        items[1].ClassList.ShouldContain("command-item-selected");

        // Test ArrowUp navigation
        await input.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowUp" });
        items = component.FindAll(".command-item");
        items[0].ClassList.ShouldContain("command-item-selected");

        // Test Enter key selection
        await input.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        selectedItem.ShouldBe("First");

        // Test Escape clears filter
        await input.InputAsync("test");
        await input.KeyDownAsync(new KeyboardEventArgs { Key = "Escape" });
        
        // Verify filter was cleared by checking that input value is empty
        var currentValue = input.GetAttribute("value");
        currentValue.ShouldBeNullOrEmpty();
    }

    [Fact(DisplayName = "And it should support empty and loading states")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldSupportEmptyAndLoadingStates()
    {
        // Test loading state
        using var ctx1 = new BunitContext();
        var loadingComponent = ctx1.Render<Command>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CommandInput>(0);
                builder.CloseComponent();
                
                builder.OpenComponent<CommandList>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(listBuilder =>
                {
                    listBuilder.OpenComponent<CommandItem>(0);
                    listBuilder.AddAttribute(1, "Label", "Item");
                    listBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Item")));
                    listBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert loading state
        var loadingElement = loadingComponent.Find(".command-loading");
        loadingElement.ShouldNotBeNull();
        loadingElement.TextContent.ShouldContain("Loading");

        // Items should not be visible during loading
        var items = loadingComponent.FindAll(".command-item");
        items.Count.ShouldBe(0);

        // Test empty state (no items match filter)
        using var ctx2 = new BunitContext();
        var emptyComponent = ctx2.Render<Command>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CommandInput>(0);
                builder.CloseComponent();
                
                builder.OpenComponent<CommandList>(1);
                builder.AddAttribute(2, "EmptyMessage", "No results found.");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(listBuilder =>
                {
                    listBuilder.OpenComponent<CommandItem>(0);
                    listBuilder.AddAttribute(1, "Label", "Calendar");
                    listBuilder.AddAttribute(2, "ChildContent", (RenderFragment)(itemBuilder => itemBuilder.AddContent(0, "Calendar")));
                    listBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Filter to something that doesn't match
        var input = emptyComponent.Find(".command-input");
        await input.InputAsync("xyz");

        // Should show empty message
        var emptyElement = emptyComponent.Find(".command-empty");
        emptyElement.ShouldNotBeNull();
        emptyElement.TextContent.ShouldContain("No results found");

        // Items should not be visible when empty
        items = emptyComponent.FindAll(".command-item");
        items.Count.ShouldBe(0);
    }
}
