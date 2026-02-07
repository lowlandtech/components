using Bunit;
using LowlandTech.Components.Components.Carousel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US07.LayoutComponents;

/// <summary>
/// SC05: Carousel component implementation
/// 
/// Feature: US07.LayoutComponents.feature
/// Scenario: SC05 - Implement Carousel component
/// 
/// Given I have a shadcn/ui Carousel component
/// When I port it to Blazor
/// Then I should have Carousel, CarouselContent, CarouselItem, CarouselPrevious, and CarouselNext components
/// And it should support keyboard navigation between items
/// And it should support optional looping behavior
/// And it should merge the Class parameter with generated classes
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "05")]
public class SC05_CarouselRenders : IDisposable
{
    private readonly BunitContext _testContext;

    public SC05_CarouselRenders()
    {
        _testContext = new BunitContext();
    }

    public void Dispose()
    {
        _testContext?.Dispose();
    }

    [Fact(DisplayName = "Carousel renders with all five components")]
    [Trait(Spec.UAC, "17")]
    public void Carousel_RendersWithAllFiveComponents()
    {
        // Given & When - Render Carousel with all subcomponents
        var cut = _testContext.Render<Carousel>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CarouselPrevious>(0);
                builder.CloseComponent();

                builder.OpenComponent<CarouselContent>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<CarouselItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 1");
                    }));
                    contentBuilder.CloseComponent();

                    contentBuilder.OpenComponent<CarouselItem>(2);
                    contentBuilder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 2");
                    }));
                    contentBuilder.CloseComponent();

                    contentBuilder.OpenComponent<CarouselItem>(4);
                    contentBuilder.AddAttribute(5, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 3");
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<CarouselNext>(3);
                builder.CloseComponent();
            }));

        // Then - Should have all five components
        var carousel = cut.Find(".carousel");
        Assert.NotNull(carousel);

        var previousButton = cut.Find(".carousel-previous");
        Assert.NotNull(previousButton);

        var content = cut.Find(".carousel-content");
        Assert.NotNull(content);

        var items = cut.FindAll(".carousel-item");
        Assert.Equal(3, items.Count);

        var nextButton = cut.Find(".carousel-next");
        Assert.NotNull(nextButton);
    }

    [Fact(DisplayName = "Carousel supports keyboard navigation between items")]
    [Trait(Spec.UAC, "18")]
    public async Task Carousel_SupportsKeyboardNavigation()
    {
        // Given - Carousel with three items
        var cut = _testContext.Render<Carousel>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CarouselContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<CarouselItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 1");
                    }));
                    contentBuilder.CloseComponent();

                    contentBuilder.OpenComponent<CarouselItem>(2);
                    contentBuilder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 2");
                    }));
                    contentBuilder.CloseComponent();

                    contentBuilder.OpenComponent<CarouselItem>(4);
                    contentBuilder.AddAttribute(5, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 3");
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        var carousel = cut.Find(".carousel");
        
        // Initially at index 0
        var items = cut.FindAll(".carousel-item");
        Assert.Contains("carousel-item-active", items[0].ClassName);
        Assert.DoesNotContain("carousel-item-active", items[1].ClassName);

        // When - Press ArrowRight
        await carousel.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowRight" });

        // Then - Should move to index 1
        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll(".carousel-item");
            Assert.DoesNotContain("carousel-item-active", updatedItems[0].ClassName);
            Assert.Contains("carousel-item-active", updatedItems[1].ClassName);
        });

        // When - Press Home
        await carousel.KeyDownAsync(new KeyboardEventArgs { Key = "Home" });

        // Then - Should move to index 0
        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll(".carousel-item");
            Assert.Contains("carousel-item-active", updatedItems[0].ClassName);
            Assert.DoesNotContain("carousel-item-active", updatedItems[1].ClassName);
        });

        // When - Press End
        await carousel.KeyDownAsync(new KeyboardEventArgs { Key = "End" });

        // Then - Should move to last index
        cut.WaitForAssertion(() =>
        {
            var updatedItems = cut.FindAll(".carousel-item");
            Assert.Contains("carousel-item-active", updatedItems[2].ClassName);
        });
    }

    [Fact(DisplayName = "Carousel supports optional looping behavior")]
    [Trait(Spec.UAC, "19")]
    public async Task Carousel_SupportsLooping()
    {
        // Given - Carousel with Loop enabled
        var cut = _testContext.Render<Carousel>(parameters => parameters
            .Add(p => p.Loop, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CarouselPrevious>(0);
                builder.CloseComponent();

                builder.OpenComponent<CarouselContent>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<CarouselItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 1");
                    }));
                    contentBuilder.CloseComponent();

                    contentBuilder.OpenComponent<CarouselItem>(2);
                    contentBuilder.AddAttribute(3, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 2");
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<CarouselNext>(3);
                builder.CloseComponent();
            }));

        // Then - Previous button should be enabled even at first item (loop mode)
        var previousButton = cut.Find(".carousel-previous");
        Assert.False(previousButton.HasAttribute("disabled"));

        // When - Click previous at first item
        await previousButton.ClickAsync(new MouseEventArgs());

        // Then - Should loop to last item
        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll(".carousel-item");
            Assert.Contains("carousel-item-active", items[1].ClassName);
        });

        // Given - Carousel without Loop
        var cutNoLoop = _testContext.Render<Carousel>(parameters => parameters
            .Add(p => p.Loop, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CarouselPrevious>(0);
                builder.CloseComponent();

                builder.OpenComponent<CarouselContent>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<CarouselItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 1");
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<CarouselNext>(3);
                builder.CloseComponent();
            }));

        // Then - Previous button should be disabled at first item (no loop)
        var previousButtonNoLoop = cutNoLoop.Find(".carousel-previous");
        Assert.True(previousButtonNoLoop.HasAttribute("disabled"));
    }

    [Fact(DisplayName = "Carousel merges the Class parameter with generated classes")]
    [Trait(Spec.UAC, "20")]
    public void Carousel_MergesClassParameter()
    {
        // Given & When - Render Carousel with custom Class
        var cut = _testContext.Render<Carousel>(parameters => parameters
            .Add(p => p.Class, "custom-carousel-class")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CarouselContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.OpenComponent<CarouselItem>(0);
                    contentBuilder.AddAttribute(1, "ChildContent", (RenderFragment)(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item 1");
                    }));
                    contentBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Then - Should have both generated and custom classes
        var carousel = cut.Find(".carousel");
        Assert.Contains("carousel", carousel.ClassName);
        Assert.Contains("custom-carousel-class", carousel.ClassName);
    }
}
