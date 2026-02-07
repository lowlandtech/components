using Bunit;
using LowlandTech.Components.Components.ScrollArea;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US07.LayoutComponents;

/// <summary>
/// SC03: ScrollArea component implementation
/// 
/// Feature: US07.LayoutComponents.feature
/// Scenario: SC03 - Implement ScrollArea component
/// 
/// Given I have the shadcn/ui ScrollArea component
/// When I port it to Blazor
/// Then it should render a scrollable viewport container
/// And it should render a custom scrollbar component
/// And it should support horizontal and vertical scrollbars
/// And it should merge the Class parameter with generated classes
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "03")]
public class SC03_ScrollAreaRenders
{
    [Fact(DisplayName = "ScrollArea renders scrollable viewport container")]
    [Trait(Spec.UAC, "10")]
    public void ScrollArea_RendersScrollableViewportContainer()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render ScrollArea with content
        var cut = ctx.Render<ScrollArea>(parameters => parameters
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "test-content");
                builder.AddContent(2, "Scrollable content");
                builder.CloseElement();
            }));

        // Then - Should render a scroll-area container
        var scrollArea = cut.Find(".scroll-area");
        Assert.NotNull(scrollArea);
        
        // Should have a viewport inside
        var viewport = cut.Find(".scroll-area-viewport");
        Assert.NotNull(viewport);
        
        // Should contain the child content
        var content = cut.Find(".test-content");
        Assert.NotNull(content);
        Assert.Contains("Scrollable content", content.TextContent);
    }

    [Fact(DisplayName = "ScrollArea renders custom scrollbar component")]
    [Trait(Spec.UAC, "11")]
    public void ScrollArea_RendersCustomScrollbarComponent()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render ScrollArea with vertical scrollbar enabled (default)
        var cut = ctx.Render<ScrollArea>(parameters => parameters
            .Add(p => p.ShowScrollBarVertical, true)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));

        // Then - Should render a scrollbar component
        var scrollbar = cut.Find(".scrollbar");
        Assert.NotNull(scrollbar);
        
        // Should have scrollbar-thumb
        var thumb = cut.Find(".scrollbar-thumb");
        Assert.NotNull(thumb);
    }

    [Fact(DisplayName = "ScrollArea supports horizontal and vertical scrollbars")]
    [Trait(Spec.UAC, "12")]
    public void ScrollArea_SupportsHorizontalAndVerticalScrollbars()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render ScrollArea with both scrollbars
        var cut = ctx.Render<ScrollArea>(parameters => parameters
            .Add(p => p.ShowScrollBarHorizontal, true)
            .Add(p => p.ShowScrollBarVertical, true)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));

        // Then - Should render both scrollbars
        var scrollbars = cut.FindAll(".scrollbar");
        Assert.Equal(2, scrollbars.Count);
        
        // Should have horizontal scrollbar
        var horizontalScrollbar = cut.Find("[data-orientation='horizontal']");
        Assert.NotNull(horizontalScrollbar);
        Assert.Contains("scrollbar-horizontal", horizontalScrollbar.GetAttribute("class"));
        
        // Should have vertical scrollbar
        var verticalScrollbar = cut.Find("[data-orientation='vertical']");
        Assert.NotNull(verticalScrollbar);
        Assert.Contains("scrollbar-vertical", verticalScrollbar.GetAttribute("class"));
    }

    [Fact(DisplayName = "ScrollArea merges Class parameter with generated classes")]
    [Trait(Spec.UAC, "13")]
    public void ScrollArea_MergesClassParameter()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render ScrollArea with custom class
        var cut = ctx.Render<ScrollArea>(parameters => parameters
            .Add(p => p.Class, "custom-scroll-class")
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));

        // Then - Should have both scroll-area and custom-scroll-class
        var scrollArea = cut.Find(".scroll-area");
        var classes = scrollArea.GetAttribute("class");
        Assert.Contains("scroll-area", classes);
        Assert.Contains("custom-scroll-class", classes);
    }
}
