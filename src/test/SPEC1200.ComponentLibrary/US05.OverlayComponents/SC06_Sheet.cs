using Bunit;
using LowlandTech.Components.Components.Sheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US05.OverlayComponents;

/// <summary>
/// Tests for Sheet component (SC06: Sheet Component)
/// Covers UAC26-UAC30 from US05.OverlayComponents.feature
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "06")]
public class SC06_Sheet : IDisposable
{
    private readonly BunitContext _testContext;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC06_Sheet()
    {
        _testContext = new BunitContext();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _testContext.Services.AddSingleton(_mockJsRuntime.Object);

        // Setup JSInterop mocks for Portal and FocusTrap
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

    #region UAC26: Component Structure

    [Fact(DisplayName = "Then I should have Sheet, SheetTrigger, and SheetContent components")]
    [Trait(Spec.UAC, "26")]
    public void UAC26_ShouldHaveAllSheetComponents()
    {
        // Arrange & Act - Render full sheet structure
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Open Sheet");
                }));
                builder.CloseComponent();

                builder.OpenComponent<SheetContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.OpenComponent<SheetHeader>(0);
                    content.AddAttribute(1, "ChildContent", (RenderFragment)(header =>
                    {
                        header.OpenComponent<SheetTitle>(0);
                        header.AddAttribute(1, "ChildContent", (RenderFragment)(title =>
                        {
                            title.AddContent(0, "Settings");
                        }));
                        header.CloseComponent();

                        header.OpenComponent<SheetDescription>(2);
                        header.AddAttribute(3, "ChildContent", (RenderFragment)(desc =>
                        {
                            desc.AddContent(0, "Configure your settings");
                        }));
                        header.CloseComponent();
                    }));
                    content.CloseComponent();

                    content.AddContent(2, "Content");

                    content.OpenComponent<SheetFooter>(3);
                    content.AddAttribute(4, "ChildContent", (RenderFragment)(footer =>
                    {
                        footer.AddContent(0, "Footer");
                    }));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - Verify components are available
        cut.ShouldNotBeNull();
        cut.Instance.ShouldNotBeNull();
        cut.Instance.ShouldBeOfType<Sheet>();
    }

    #endregion

    #region UAC27: Portal Rendering

    [Fact(DisplayName = "And sheet content should render via Portal")]
    [Trait(Spec.UAC, "27")]
    public void UAC27_SheetContentShouldRenderViaPortal()
    {
        // Arrange & Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Test Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Content should be rendered when open
        cut.Markup.ShouldContain("Test Content");
        
        // Verify JSInterop was called for Portal
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()), Times.AtLeastOnce);
    }

    #endregion

    #region UAC28: Side Placement

    [Theory(DisplayName = "And it should support side placement (top, right, bottom, left)")]
    [Trait(Spec.UAC, "28")]
    [InlineData(SheetSide.Top, "top")]
    [InlineData(SheetSide.Right, "right")]
    [InlineData(SheetSide.Bottom, "bottom")]
    [InlineData(SheetSide.Left, "left")]
    public void UAC28_ShouldSupportSidePlacement(SheetSide side, string expectedDataSide)
    {
        // Arrange & Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "Side", side);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Verify data-side attribute
        var contentElement = cut.Find("[data-slot='sheet-content']");
        contentElement.ShouldNotBeNull();
        contentElement.GetAttribute("data-side").ShouldBe(expectedDataSide);
    }

    #endregion

    #region UAC29: Close on Outside Click and Escape

    [Fact(DisplayName = "And it should close on outside click")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_ShouldCloseOnOutsideClick()
    {
        // Arrange
        bool isClosed = false;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, (open) => isClosed = !open))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "CloseOnOverlayClick", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Click on overlay
        var overlay = cut.Find("[data-slot='sheet-overlay']");
        overlay.Click();

        // Assert
        isClosed.ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should close on Escape key")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_ShouldCloseOnEscape()
    {
        // Arrange
        bool isClosed = false;
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, (open) => isClosed = !open))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "CloseOnEscape", true);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Press Escape key
        var contentElement = cut.Find("[data-slot='sheet-content']");
        contentElement.KeyDown(new KeyboardEventArgs { Key = "Escape" });

        // Assert
        isClosed.ShouldBeTrue();
    }

    #endregion

    #region UAC30: Focus Trap

    [Fact(DisplayName = "And focus should be trapped within the sheet when open")]
    [Trait(Spec.UAC, "30")]
    public void UAC30_ShouldTrapFocusWhenOpen()
    {
        // Arrange & Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Verify FocusTrap.ActivateAsync was called
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.IsAny<object[]>()), Times.AtLeastOnce);
    }

    #endregion

    #region Integration Tests

    [Fact(DisplayName = "SheetTrigger should open the sheet when clicked")]
    public void ShouldOpenSheetOnTriggerClick()
    {
        // Arrange
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Open");
                }));
                builder.CloseComponent();

                builder.OpenComponent<SheetContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Sheet Content");
                }));
                builder.CloseComponent();
            }));

        // Act
        var trigger = cut.Find("button[data-slot='sheet-trigger']");
        trigger.Click();

        // Assert
        cut.Instance.Open.ShouldBeTrue();
        cut.Markup.ShouldContain("Sheet Content");
    }

    [Fact(DisplayName = "Sheet should respect controlled open state")]
    public void ShouldRespectControlledOpenState()
    {
        // Arrange & Act - Render with Open=false
        var cutClosed = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Initially closed
        cutClosed.Instance.Open.ShouldBeFalse();
        cutClosed.Markup.ShouldNotContain("Content");

        // Arrange & Act - Render with Open=true
        var cutOpen = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Now open
        cutOpen.Instance.Open.ShouldBeTrue();
        cutOpen.Markup.ShouldContain("Content");
    }

    [Fact(DisplayName = "Sheet should render all layout components correctly")]
    public void ShouldRenderAllLayoutComponents()
    {
        // Arrange & Act
        var cut = Render<Sheet>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.OpenComponent<SheetHeader>(0);
                    content.AddAttribute(1, "ChildContent", (RenderFragment)(header =>
                    {
                        header.OpenComponent<SheetTitle>(0);
                        header.AddAttribute(1, "ChildContent", (RenderFragment)(title =>
                        {
                            title.AddContent(0, "Test Title");
                        }));
                        header.CloseComponent();

                        header.OpenComponent<SheetDescription>(2);
                        header.AddAttribute(3, "ChildContent", (RenderFragment)(desc =>
                        {
                            desc.AddContent(0, "Test Description");
                        }));
                        header.CloseComponent();
                    }));
                    content.CloseComponent();

                    content.AddMarkupContent(2, "<div>Main Content</div>");

                    content.OpenComponent<SheetFooter>(3);
                    content.AddAttribute(4, "ChildContent", (RenderFragment)(footer =>
                    {
                        footer.AddContent(0, "Footer Content");
                    }));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert
        var header = cut.Find("[data-slot='sheet-header']");
        header.ShouldNotBeNull();
        
        var title = cut.Find("[data-slot='sheet-title']");
        title.ShouldNotBeNull();
        title.TextContent.ShouldContain("Test Title");
        
        var description = cut.Find("[data-slot='sheet-description']");
        description.ShouldNotBeNull();
        description.TextContent.ShouldContain("Test Description");
        
        var footer = cut.Find("[data-slot='sheet-footer']");
        footer.ShouldNotBeNull();
        footer.TextContent.ShouldContain("Footer Content");
        
        cut.Markup.ShouldContain("Main Content");
    }

    #endregion
}
