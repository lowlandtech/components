using Bunit;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Empty component rendering and functionality.
/// SC05 - Implement Empty component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "05")]
public class SC05_EmptyRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a placeholder state container")]
    [Trait(Spec.UAC, "15")]
    public async Task UAC15_ShouldRenderPlaceholderContainer()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Empty>();

        // Assert - should render empty container with "empty" class
        var empty = cut.Find(".empty");
        empty.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support title and description slots")]
    [Trait(Spec.UAC, "16")]
    public async Task UAC16_ShouldSupportTitleAndDescription()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Empty>(parameters => parameters
            .Add(p => p.Title, "<span>No items found</span>")
            .Add(p => p.Description, "<span>Try adjusting your search criteria</span>"));

        // Assert - should render title and description
        var title = cut.Find(".empty-title");
        title.ShouldNotBeNull();
        title.TextContent.ShouldContain("No items found");

        var description = cut.Find(".empty-description");
        description.ShouldNotBeNull();
        description.TextContent.ShouldContain("Try adjusting your search criteria");
    }

    [Fact(DisplayName = "And it should support an optional action slot")]
    [Trait(Spec.UAC, "17")]
    public async Task UAC17_ShouldSupportActionSlot()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Empty>(parameters => parameters
            .Add(p => p.Action, "<button class=\"test-button\">Add Item</button>"));

        // Assert - should render action slot
        var action = cut.Find(".empty-action");
        action.ShouldNotBeNull();
        
        var button = cut.Find(".test-button");
        button.ShouldNotBeNull();
        button.TextContent.ShouldBe("Add Item");
    }
}
