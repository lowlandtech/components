using Bunit;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Item component rendering and functionality.
/// SC06 - Implement Item component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "06")]
public class SC06_ItemRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a list item container with consistent spacing")]
    [Trait(Spec.UAC, "18")]
    public async Task UAC18_ShouldRenderListItemContainer()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Item>(parameters => parameters
            .Add(p => p.ChildContent, "<span>Item content</span>"));

        // Assert - should render item container with "item" class
        var item = cut.Find(".item");
        item.ShouldNotBeNull();

        // Assert - should render item content
        var content = cut.Find(".item-content");
        content.ShouldNotBeNull();
        content.TextContent.ShouldContain("Item content");
    }

    [Fact(DisplayName = "And it should support leading and trailing content slots")]
    [Trait(Spec.UAC, "19")]
    public async Task UAC19_ShouldSupportLeadingAndTrailing()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Item>(parameters => parameters
            .Add(p => p.Leading, "<span class=\"leading-icon\">📋</span>")
            .Add(p => p.ChildContent, "<span>Main content</span>")
            .Add(p => p.Trailing, "<span class=\"trailing-badge\">New</span>"));

        // Assert - should render leading content
        var leading = cut.Find(".item-leading");
        leading.ShouldNotBeNull();
        var leadingIcon = cut.Find(".leading-icon");
        leadingIcon.ShouldNotBeNull();

        // Assert - should render trailing content
        var trailing = cut.Find(".item-trailing");
        trailing.ShouldNotBeNull();
        var trailingBadge = cut.Find(".trailing-badge");
        trailingBadge.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "20")]
    public async Task UAC20_ShouldMergeCustomClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Item>(parameters => parameters
            .Add(p => p.Class, "custom-item highlighted")
            .Add(p => p.ChildContent, "<span>Content</span>"));

        // Assert - should have both base and custom classes
        var item = cut.Find(".item");
        item.ClassList.ShouldContain("item");
        item.ClassList.ShouldContain("custom-item");
        item.ClassList.ShouldContain("highlighted");
    }
}
