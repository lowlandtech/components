using Bunit;
using LowlandTech.Components.Components.Collapsible;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US06.NavigationComponents;

/// <summary>
/// SC08: Collapsible component implementation
/// 
/// Feature: US06.NavigationComponents.feature
/// Scenario: SC08 - Implement Collapsible component
/// 
/// Given I have a shadcn/ui Collapsible component
/// When I port it to Blazor
/// Then I should have Collapsible, CollapsibleTrigger, and CollapsibleContent components
/// And it should support controlled open state via parameters
/// And it should support a default open state
/// And it should support keyboard activation
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "08")]
public class SC08_CollapsibleRenders : IDisposable
{
    private readonly BunitContext _testContext;

    public SC08_CollapsibleRenders()
    {
        _testContext = new BunitContext();
    }

    public void Dispose()
    {
        _testContext?.Dispose();
    }

    [Fact(DisplayName = "Collapsible renders with all three components")]
    [Trait(Spec.UAC, "32")]
    public void Collapsible_RendersWithAllThreeComponents()
    {
        // Given & When - Render Collapsible with Trigger and Content
        var cut = _testContext.Render<Collapsible>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CollapsibleTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, "Toggle");
                }));
                builder.CloseComponent();

                builder.OpenComponent<CollapsibleContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.AddContent(0, "Collapsible content");
                }));
                builder.CloseComponent();
            }));

        // Then - Should have all three components
        var collapsibleWrapper = cut.Find(".collapsible");
        Assert.NotNull(collapsibleWrapper);

        var trigger = cut.Find(".collapsible-trigger");
        Assert.NotNull(trigger);
        Assert.Contains("Toggle", trigger.TextContent);

        var content = cut.Find(".collapsible-content");
        Assert.NotNull(content);
        Assert.Contains("Collapsible content", content.TextContent);
    }

    [Fact(DisplayName = "Collapsible supports controlled open state via parameters")]
    [Trait(Spec.UAC, "33")]
    public async Task Collapsible_SupportsControlledOpenState()
    {
        // Given - Controlled mode with Open parameter
        bool isOpen = false;
        bool openStateChanged = false;

        var cut = _testContext.Render<Collapsible>(parameters => parameters
            .Add(p => p.Open, isOpen)
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, newValue =>
            {
                isOpen = newValue;
                openStateChanged = true;
            }))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CollapsibleTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, "Toggle");
                }));
                builder.CloseComponent();

                builder.OpenComponent<CollapsibleContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // When - Click trigger
        var trigger = cut.Find(".collapsible-trigger");
        await trigger.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        // Then - Should notify parent of state change
        Assert.True(openStateChanged);
        Assert.True(isOpen);
    }

    [Fact(DisplayName = "Collapsible supports default open state")]
    [Trait(Spec.UAC, "34")]
    public void Collapsible_SupportsDefaultOpenState()
    {
        // Given & When - Uncontrolled mode with DefaultOpen = true
        var cut = _testContext.Render<Collapsible>(parameters => parameters
            .Add(p => p.DefaultOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CollapsibleTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, "Toggle");
                }));
                builder.CloseComponent();

                builder.OpenComponent<CollapsibleContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Then - Content should be visible
        var content = cut.Find(".collapsible-content");
        Assert.NotNull(content);
        Assert.Contains("Content", content.TextContent);

        // And wrapper should have open class
        var wrapper = cut.Find(".collapsible");
        Assert.Contains("collapsible-open", wrapper.ClassName);
    }

    [Fact(DisplayName = "Collapsible supports keyboard activation")]
    [Trait(Spec.UAC, "35")]
    public async Task Collapsible_SupportsKeyboardActivation()
    {
        // Given - Collapsible starting closed
        var cut = _testContext.Render<Collapsible>(parameters => parameters
            .Add(p => p.DefaultOpen, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CollapsibleTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(triggerBuilder =>
                {
                    triggerBuilder.AddContent(0, "Toggle");
                }));
                builder.CloseComponent();

                builder.OpenComponent<CollapsibleContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(contentBuilder =>
                {
                    contentBuilder.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // When - Press Enter key on trigger
        var trigger = cut.Find(".collapsible-trigger");
        await trigger.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

        // Then - Content should become visible
        cut.WaitForAssertion(() =>
        {
            var content = cut.Find(".collapsible-content");
            Assert.NotNull(content);
        }, timeout: TimeSpan.FromSeconds(2));

        // When - Press Space key on trigger
        await trigger.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = " " });

        // Then - Content should be hidden
        cut.WaitForAssertion(() =>
        {
            Assert.Throws<ElementNotFoundException>(() => cut.Find(".collapsible-content"));
        }, timeout: TimeSpan.FromSeconds(2));
    }
}
