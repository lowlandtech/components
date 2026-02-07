using Bunit;
using LowlandTech.Components.Components.NavigationMenu;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US06.NavigationComponents;

/// <summary>
/// Tests for NavigationMenu component rendering and functionality.
/// SC05 - Implement NavigationMenu component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "05")]
public class SC05_NavigationMenuRenders : Scenario
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

    [Fact(DisplayName = "Then I should have NavigationMenu, NavigationMenuList, NavigationMenuItem, NavigationMenuTrigger, and NavigationMenuContent components")]
    [Trait(Spec.UAC, "18")]
    public async Task UAC18_ShouldHaveAllNavigationMenuComponents()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<NavigationMenu>(parameters => parameters
            .AddChildContent<NavigationMenuList>(list => list
                .AddChildContent<NavigationMenuItem>(item => item
                    .Add(i => i.Value, "item1")
                    .AddChildContent<NavigationMenuTrigger>(trigger => trigger
                        .AddChildContent("Menu Item")
                    )
                    .AddChildContent<NavigationMenuContent>(content => content
                        .AddChildContent("<div>Content</div>")
                    )
                )
            )
        );

        // Assert
        var nav = component.Find("nav");
        nav.ShouldNotBeNull();
        nav.ClassList.ShouldContain("navigation-menu");

        var list = component.Find("ul");
        list.ShouldNotBeNull();
        list.ClassList.ShouldContain("navigation-menu-list");

        var item = component.Find("li");
        item.ShouldNotBeNull();
        item.ClassList.ShouldContain("navigation-menu-item");

        var trigger = component.Find("button");
        trigger.ShouldNotBeNull();
        trigger.ClassList.ShouldContain("navigation-menu-trigger");

        var content = component.Find(".navigation-menu-content");
        content.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support keyboard navigation between items")]
    [Trait(Spec.UAC, "19")]
    public async Task UAC19_ShouldSupportKeyboardNavigation()
    {
        // Arrange
        using var ctx = new BunitContext();
        var component = ctx.Render<NavigationMenu>(parameters => parameters
            .AddChildContent<NavigationMenuList>(list => list
                .AddChildContent<NavigationMenuItem>(item => item
                    .Add(i => i.Value, "item1")
                    .AddChildContent<NavigationMenuTrigger>(trigger => trigger
                        .AddChildContent("Menu 1")
                    )
                    .AddChildContent<NavigationMenuContent>(content => content
                        .AddChildContent("<div>Content 1</div>")
                    )
                )
            )
        );

        // Act - Press Enter key
        var trigger = component.Find("button");
        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Menu should open
        var triggerDataState = trigger.GetAttribute("data-state");
        triggerDataState.ShouldBe("open");

        // Act - Press Escape key
        await trigger.KeyDownAsync(new KeyboardEventArgs { Key = "Escape" });

        // Assert - Menu should close
        var updatedTrigger = component.Find("button");
        var closedState = updatedTrigger.GetAttribute("data-state");
        closedState.ShouldBe("closed");
    }

    [Fact(DisplayName = "And it should support open and close animations")]
    [Trait(Spec.UAC, "20")]
    public async Task UAC20_ShouldSupportAnimations()
    {
        // Arrange
        using var ctx = new BunitContext();
        var component = ctx.Render<NavigationMenu>(parameters => parameters
            .AddChildContent<NavigationMenuList>(list => list
                .AddChildContent<NavigationMenuItem>(item => item
                    .Add(i => i.Value, "item1")
                    .AddChildContent<NavigationMenuTrigger>(trigger => trigger
                        .AddChildContent("Menu")
                    )
                    .AddChildContent<NavigationMenuContent>(content => content
                        .AddChildContent("<div>Content</div>")
                    )
                )
            )
        );

        // Assert - Initially closed
        var content = component.Find(".navigation-menu-content");
        var initialState = content.GetAttribute("data-state");
        initialState.ShouldBe("closed");

        // Act - Click trigger to open
        var trigger = component.Find("button");
        await trigger.ClickAsync(new MouseEventArgs());

        // Assert - Now open
        var openContent = component.Find(".navigation-menu-content");
        var openState = openContent.GetAttribute("data-state");
        openState.ShouldBe("open");

        // Act - Click trigger again to close
        await trigger.ClickAsync(new MouseEventArgs());

        // Assert - Closed again
        var closedContent = component.Find(".navigation-menu-content");
        var closedState = closedContent.GetAttribute("data-state");
        closedState.ShouldBe("closed");
    }

    [Fact(DisplayName = "And it should manage focus when menus open")]
    [Trait(Spec.UAC, "21")]
    public async Task UAC21_ShouldManageFocus()
    {
        // Arrange
        using var ctx = new BunitContext();
        var component = ctx.Render<NavigationMenu>(parameters => parameters
            .AddChildContent<NavigationMenuList>(list => list
                .AddChildContent<NavigationMenuItem>(item => item
                    .Add(i => i.Value, "item1")
                    .AddChildContent<NavigationMenuTrigger>(trigger => trigger
                        .AddChildContent("Menu")
                    )
                    .AddChildContent<NavigationMenuContent>(content => content
                        .AddChildContent("<div>Content</div>")
                    )
                )
            )
        );

        // Act - Click trigger to open menu
        var trigger = component.Find("button");
        await trigger.ClickAsync(new MouseEventArgs());

        // Assert - Trigger should have aria-expanded="true"
        var ariaExpanded = trigger.GetAttribute("aria-expanded");
        ariaExpanded.ShouldBe("true");

        // Assert - Content should have aria-hidden="false"
        var content = component.Find(".navigation-menu-content");
        var ariaHidden = content.GetAttribute("aria-hidden");
        ariaHidden.ShouldBe("false");

        // Act - Close menu
        await trigger.ClickAsync(new MouseEventArgs());

        // Assert - Aria attributes updated
        var closedTrigger = component.Find("button");
        var closedExpanded = closedTrigger.GetAttribute("aria-expanded");
        closedExpanded.ShouldBe("false");

        var closedContent = component.Find(".navigation-menu-content");
        var closedHidden = closedContent.GetAttribute("aria-hidden");
        closedHidden.ShouldBe("true");
    }

    [Fact(DisplayName = "And it should support nested content groups")]
    [Trait(Spec.UAC, "22")]
    public async Task UAC22_ShouldSupportNestedContent()
    {
        // Arrange & Act
        using var ctx = new BunitContext();
        var component = ctx.Render<NavigationMenu>(parameters => parameters
            .AddChildContent<NavigationMenuList>(list => list
                .AddChildContent<NavigationMenuItem>(item => item
                    .Add(i => i.Value, "parent")
                    .AddChildContent<NavigationMenuTrigger>(trigger => trigger
                        .AddChildContent("Parent Menu")
                    )
                    .AddChildContent<NavigationMenuContent>(content => content
                        .AddChildContent("<div class='nested-group'><span>Nested Item 1</span><span>Nested Item 2</span></div>")
                    )
                )
            )
        );

        // Act - Open the menu
        var trigger = component.Find("button");
        await trigger.ClickAsync(new MouseEventArgs());

        // Assert - Nested content should be rendered inside
        var content = component.Find(".navigation-menu-content");
        var nestedGroup = content.QuerySelector(".nested-group");
        nestedGroup.ShouldNotBeNull();

        var nestedItems = nestedGroup.QuerySelectorAll("span");
        nestedItems.Length.ShouldBe(2);
        nestedItems[0].TextContent.ShouldBe("Nested Item 1");
        nestedItems[1].TextContent.ShouldBe("Nested Item 2");
    }
}
