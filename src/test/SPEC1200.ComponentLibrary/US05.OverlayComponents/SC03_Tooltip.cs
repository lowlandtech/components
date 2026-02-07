using Bunit;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace LowlandTech.Tests.SPEC1227.ComponentLibrary.US05.OverlayComponents;

/// <summary>
/// Tests for Tooltip component (SC03: Tooltip Component)
/// Covers UAC12-UAC15 from US05.OverlayComponents.feature
/// </summary>
[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "03")]
public class SC03_Tooltip : IDisposable
{
    private readonly TestContext _testContext;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC03_Tooltip()
    {
        _testContext = new TestContext();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _testContext.Services.AddSingleton(_mockJsRuntime.Object);

        // Setup JSInterop mocks for Portal
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.Contains("portalUtils")),
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

    #region UAC12: Component Structure

    [Fact(DisplayName = "Then I should have TooltipProvider, Tooltip, TooltipTrigger, and TooltipContent components")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_ShouldHaveAllTooltipComponents()
    {
        // Arrange & Act - Render complete Tooltip structure
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Hover me");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Tooltip text");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - All components should render
        cut.ShouldNotBeNull();
        
        // Verify TooltipTrigger renders (button with content)
        var triggerButton = cut.Find("button");
        triggerButton.ShouldNotBeNull();
        triggerButton.TextContent.ShouldContain("Hover me");
        
        // TooltipContent should not render when Open=false (default)
        var tooltipDivs = cut.FindAll("[role='tooltip']");
        tooltipDivs.Count.ShouldBe(0);
    }

    #endregion

    #region UAC13: Configurable Delays

    [Fact(DisplayName = "And it should support configurable open and close delays")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ShouldSupportConfigurableDelays()
    {
        // Arrange & Act - Render TooltipProvider with custom delays
        var cut = Render<TooltipProvider>(parameters => parameters
            .Add(p => p.DelayMs, 500)
            .Add(p => p.CloseDelayMs, 200)
            .AddChildContent("<span>Content</span>"));

        // Assert
        var provider = cut.Instance;
        provider.ShouldNotBeNull();
        
        // Verify delays are configurable via parameters
        provider.DelayMs.ShouldBe(500);
        provider.CloseDelayMs.ShouldBe(200);
        
        // Verify internal methods return correct delays
        provider.GetOpenDelay().ShouldBe(500);
        provider.GetCloseDelay().ShouldBe(200);
    }

    [Fact]
    public void UAC13_ShouldUseDefaultDelaysWhenNotSpecified()
    {
        // Arrange & Act - Render TooltipProvider without specifying delays
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent("<span>Content</span>"));

        // Assert - Should use defaults (700ms open, 300ms close)
        var provider = cut.Instance;
        provider.GetOpenDelay().ShouldBe(700);
        provider.GetCloseDelay().ShouldBe(300);
    }

    #endregion

    #region UAC14: Portal Rendering

    [Fact(DisplayName = "And tooltip content should render via Portal")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_TooltipContentShouldRenderViaPortal()
    {
        // Arrange & Act - Render Tooltip with Open=true to show content
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", true); // Open to render content
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Tooltip content");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - TooltipContent should use Portal component
        var portalComponents = cut.FindComponents<LowlandTech.Components.Components.Portal.Portal>();
        portalComponents.Count.ShouldBeGreaterThan(0);
        
        // Verify JSInterop was called to create portal
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.Contains("portalUtils.create")),
            It.IsAny<object[]>()), 
            Times.AtLeastOnce);
    }

    [Fact]
    public void UAC14_TooltipContentShouldNotRenderWhenClosed()
    {
        // Arrange & Act - Render Tooltip with Open=false
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", false); // Closed
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Tooltip content");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Portal should not render content when closed
        var tooltipDiv = cut.FindAll("[role='tooltip']");
        tooltipDiv.Count.ShouldBe(0);
    }

    #endregion

    #region UAC15: Positioning

    [Fact(DisplayName = "And it should position relative to the trigger element")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ShouldSupportPositioning()
    {
        // Arrange & Act - Render TooltipContent with positioning parameters
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "Side", "bottom");
                    tooltip.AddAttribute(4, "SideOffset", 8);
                    tooltip.AddAttribute(5, "Align", "start");
                    tooltip.AddAttribute(6, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Tooltip content");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - TooltipContent should accept positioning parameters
        var contentComponents = cut.FindComponents<TooltipContent>();
        contentComponents.Count.ShouldBe(1);
        var contentInstance = contentComponents[0].Instance;
        contentInstance.Side.ShouldBe("bottom");
        contentInstance.SideOffset.ShouldBe(8);
        contentInstance.Align.ShouldBe("start");
        
        // Verify tooltip renders with fixed positioning
        var tooltipDiv = cut.Find("[role='tooltip']");
        tooltipDiv.ShouldNotBeNull();
        tooltipDiv.GetAttribute("style").ShouldContain("position: fixed");
    }

    [Fact]
    public void UAC15_ShouldUseDefaultPositioningWhenNotSpecified()
    {
        // Arrange & Act - Render TooltipContent without positioning params
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Tooltip");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Should use defaults (top, 4px offset, center align)
        var contentComponents = cut.FindComponents<TooltipContent>();
        var contentInstance = contentComponents[0].Instance;
        contentInstance.Side.ShouldBe("top");
        contentInstance.SideOffset.ShouldBe(4);
        contentInstance.Align.ShouldBe("center");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ShouldCascadeTooltipToChildComponents()
    {
        // Arrange & Act - Render complete structure
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", false);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Content");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Cascading should work
        var tooltipComponents = cut.FindComponents<Tooltip>();
        tooltipComponents.Count.ShouldBe(1);
        tooltipComponents[0].Instance.Open.ShouldBe(false);
    }

    [Fact]
    public void ShouldRenderTooltipTriggerAsButton()
    {
        // Arrange & Act
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Hover me");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Trigger should render as button
        var button = cut.Find("button");
        button.ShouldNotBeNull();
        button.GetAttribute("type").ShouldBe("button");
        button.TextContent.ShouldContain("Hover me");
    }

    [Fact]
    public void ShouldApplyTooltipRole()
    {
        // Arrange & Act
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Tooltip text");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Tooltip content should have role="tooltip"
        var tooltipDiv = cut.Find("[role='tooltip']");
        tooltipDiv.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldMergeCssClasses()
    {
        // Arrange & Act
        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "Class", "custom-class");
                    tooltip.AddAttribute(4, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Content");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Should merge custom class with base classes
        var tooltipDiv = cut.Find("[role='tooltip']");
        var classes = tooltipDiv.GetAttribute("class");
        classes.ShouldContain("custom-class");
        classes.ShouldContain("z-50"); // Base class
        classes.ShouldContain("rounded-md"); // Base class
    }

    [Fact]
    public void ShouldForwardUnmatchedAttributesToTrigger()
    {
        // Arrange & Act
        var additionalAttrs = new Dictionary<string, object>
        {
            { "data-testid", "tooltip-trigger" }
        };

        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "AdditionalAttributes", additionalAttrs);
                    tooltip.AddAttribute(2, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Should forward attributes
        var button = cut.Find("button");
        button.GetAttribute("data-testid").ShouldBe("tooltip-trigger");
    }

    [Fact]
    public void ShouldForwardUnmatchedAttributesToContent()
    {
        // Arrange & Act
        var additionalAttrs = new Dictionary<string, object>
        {
            { "data-testid", "tooltip-content" }
        };

        var cut = Render<TooltipProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Tooltip>(0);
                builder.AddAttribute(1, "Open", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(tooltip =>
                {
                    tooltip.OpenComponent<TooltipTrigger>(0);
                    tooltip.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                    {
                        trigger.AddContent(0, "Trigger");
                    }));
                    tooltip.CloseComponent();

                    tooltip.OpenComponent<TooltipContent>(2);
                    tooltip.AddAttribute(3, "AdditionalAttributes", additionalAttrs);
                    tooltip.AddAttribute(4, "ChildContent", (RenderFragment)(content =>
                    {
                        content.AddContent(0, "Content");
                    }));
                    tooltip.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Should forward attributes
        var tooltipDiv = cut.Find("[role='tooltip']");
        tooltipDiv.GetAttribute("data-testid").ShouldBe("tooltip-content");
    }

    #endregion
}
