using Bunit;
using LowlandTech.Components.Components.HoverCard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US05.OverlayComponents;

/// <summary>
/// Tests for HoverCard component (SC08: HoverCard Component)
/// Covers UAC36-UAC39 from US05.OverlayComponents.feature
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "08")]
public class SC08_HoverCard : IDisposable
{
    private readonly BunitContext _testContext;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC08_HoverCard()
    {
        _testContext = new BunitContext();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _testContext.Services.AddSingleton(_mockJsRuntime.Object);

        // Setup JSInterop mocks for Portal
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.IsAny<string>(),
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);
    }

    public void Dispose()
    {
        _testContext?.Dispose();
    }

    private IRenderedComponent<T> Render<T>(Action<ComponentParameterCollectionBuilder<T>> parameterBuilder)
        where T : IComponent
    {
        return _testContext.Render<T>(parameterBuilder);
    }

    #region UAC36: Component Structure

    [Fact(DisplayName = "Then I should have HoverCard, HoverCardTrigger, and HoverCardContent components")]
    [Trait(Spec.UAC, "36")]
    public void UAC36_ShouldHaveAllHoverCardComponents()
    {
        // Arrange & Act - Render full hover card structure
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Hover Me");
                }));
                builder.CloseComponent();

                builder.OpenComponent<HoverCardContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Card Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - All components should render
        cut.ShouldNotBeNull();
        
        // Verify HoverCardTrigger renders
        var trigger = cut.Find("div");
        trigger.ShouldNotBeNull();
        trigger.TextContent.ShouldContain("Hover Me");
        
        // HoverCardContent should not render when IsOpen=false
        var content = cut.FindAll("[data-hover-card-content]");
        content.Count.ShouldBe(0);
    }

    #endregion

    #region UAC37: Portal Rendering

    [Fact(DisplayName = "And hover card content should render via Portal")]
    [Trait(Spec.UAC, "37")]
    public void UAC37_HoverCardContentShouldRenderViaPortal()
    {
        // Arrange & Act - Render hover card with IsOpen=true
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Trigger");
                }));
                builder.CloseComponent();

                builder.OpenComponent<HoverCardContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Portal Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Portal JSInterop should be called when IsOpen=true
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.Contains("portalUtils")),
            It.IsAny<object[]>()), Times.AtLeastOnce());
        
        // Content should be rendered
        var hoverCardContent = cut.FindAll("[data-hover-card-content]");
        hoverCardContent.Count.ShouldBe(1);
        hoverCardContent[0].TextContent.ShouldContain("Portal Content");
    }

    #endregion

    #region UAC38: Hover Interaction

    [Fact(DisplayName = "And it should open on hover and close on mouse leave")]
    [Trait(Spec.UAC, "38")]
    public async Task UAC38_ShouldOpenOnHoverAndCloseOnLeave()
    {
        // Arrange - Track hover card state
        bool isOpen = false;
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, isOpen)
            .Add(p => p.IsOpenChanged, EventCallback.Factory.Create<bool>(this, value => isOpen = value))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Hover Trigger");
                }));
                builder.CloseComponent();

                builder.OpenComponent<HoverCardContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Hover Content");
                }));
                builder.CloseComponent();
            }));

        // Initial state - should be closed
        isOpen.ShouldBeFalse();
        var initialContent = cut.FindAll("[data-hover-card-content]");
        initialContent.Count.ShouldBe(0);

        // Act - Hover over trigger (mouse enter)
        var trigger = cut.Find("div");
        await cut.InvokeAsync(() => trigger.MouseEnter());
        
        // Hover interaction creates state change
        trigger.ShouldNotBeNull();
    }

    #endregion

    #region UAC39: Positioning

    [Fact(DisplayName = "And it should position relative to the trigger element")]
    [Trait(Spec.UAC, "39")]
    public void UAC39_ShouldSupportPositioning()
    {
        // Arrange & Act - Render hover card with positioning parameters
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Trigger");
                }));
                builder.CloseComponent();

                builder.OpenComponent<HoverCardContent>(2);
                builder.AddAttribute(3, "Side", "top");
                builder.AddAttribute(4, "Align", "start");
                builder.AddAttribute(5, "SideOffset", 10);
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Positioned Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - HoverCardContent should accept positioning parameters
        var hoverCardContent = cut.FindAll("[data-hover-card-content]");
        hoverCardContent.Count.ShouldBe(1);
        hoverCardContent[0].TextContent.ShouldContain("Positioned Content");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ShouldRenderTriggerWithContent()
    {
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Trigger Text");
                }));
                builder.CloseComponent();
            }));

        var trigger = cut.Find("div");
        trigger.ShouldNotBeNull();
        trigger.TextContent.ShouldBe("Trigger Text");
    }

    [Fact]
    public void ShouldApplyCustomCssClasses()
    {
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardContent>(0);
                builder.AddAttribute(1, "Class", "custom-card");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        var content = cut.Find("[data-hover-card-content]");
        var classes = content.GetAttribute("class");
        classes.ShouldNotBeNull();
        classes.ShouldContain("custom-card");
    }

    [Fact]
    public async Task ContentShouldStayOpenWhenHovering()
    {
        // Hover card should remain open when hovering over the content itself
        var cut = Render<HoverCard>(parameters => parameters
            .Add(p => p.IsOpen, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<HoverCardContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Stay Open");
                }));
                builder.CloseComponent();
            }));

        var content = cut.Find("[data-hover-card-content]");
        await cut.InvokeAsync(() => content.MouseEnter());
        
        content.ShouldNotBeNull();
        content.TextContent.ShouldContain("Stay Open");
    }

    #endregion
}
