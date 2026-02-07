using Bunit;
using LowlandTech.Components.Components.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace LowlandTech.Tests.SPEC1227.ComponentLibrary.US05.OverlayComponents;

/// <summary>
/// Tests for Dialog component (SC01: Implement Dialog component)
/// Covers UAC01-UAC06 from US05.OverlayComponents.feature
/// </summary>
[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "01")]
public class SC01_Dialog : IDisposable
{
    private readonly TestContext _testContext;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC01_Dialog()
    {
        _testContext = new TestContext();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _testContext.Services.AddSingleton(_mockJsRuntime.Object);

        // Setup common JSInterop mocks for Portal and FocusTrap
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.Contains("portalUtils")),
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.Contains("focusTrapUtils")),
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

    #region UAC01: Dialog Components Structure

    [Fact(DisplayName = "Then I should have Dialog, DialogTrigger, DialogContent, DialogHeader, DialogTitle, DialogDescription, and DialogFooter components")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ShouldHaveAllDialogComponents()
    {
        // Arrange & Act - Render full dialog structure
        var cut = Render<Dialog>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Open Dialog");
                }));
                builder.CloseComponent();

                builder.OpenComponent<DialogContent>(2);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(content =>
                {
                    content.OpenComponent<DialogHeader>(0);
                    content.AddAttribute(1, "ChildContent", (RenderFragment)(header =>
                    {
                        header.OpenComponent<DialogTitle>(0);
                        header.AddAttribute(1, "ChildContent", (RenderFragment)(title =>
                        {
                            title.AddContent(0, "Dialog Title");
                        }));
                        header.CloseComponent();

                        header.OpenComponent<DialogDescription>(2);
                        header.AddAttribute(3, "ChildContent", (RenderFragment)(desc =>
                        {
                            desc.AddContent(0, "Dialog description");
                        }));
                        header.CloseComponent();
                    }));
                    content.CloseComponent();

                    content.OpenComponent<DialogFooter>(2);
                    content.AddAttribute(3, "ChildContent", (RenderFragment)(footer =>
                    {
                        footer.AddContent(0, "Footer content");
                    }));
                    content.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        // Assert - All component types should be instantiated successfully
        cut.ShouldNotBeNull();
        cut.Markup.ShouldContain("Open Dialog"); // DialogTrigger content
    }

    #endregion

    #region UAC02: Portal Rendering

    [Fact(DisplayName = "And dialog content should render via Portal")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_DialogContentShouldRenderViaPortal()
    {
        // Arrange
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, true) // Open the dialog
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Dialog Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Wait for rendering
        await Task.Delay(100); // Allow JSInterop calls to process

        // Assert - Should invoke Portal JSInterop
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()),
            Times.AtLeastOnce());
    }

    #endregion

    #region UAC03: Overlay/Backdrop

    [Fact(DisplayName = "And it should render an overlay/backdrop behind the dialog content")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ShouldRenderOverlayBackdrop()
    {
        // Arrange & Act
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, true) // Open the dialog
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Dialog Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - Should render overlay with dark background
        cut.Markup.ShouldContain("bg-black/80"); // Overlay background class
        cut.Markup.ShouldContain("fixed inset-0"); // Full viewport overlay
    }

    #endregion

    #region UAC04: Escape Key Handling

    [Fact(DisplayName = "And pressing Escape should close the dialog when it is open")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EscapeKeyShouldCloseDialog()
    {
        // Arrange
        var dialogOpenState = true;
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, dialogOpenState)
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(
                this, (bool newValue) => dialogOpenState = newValue))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Dialog Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Find the dialog content and trigger Escape key
        var dialogContent = cut.FindAll("[role='dialog']").FirstOrDefault();
        dialogContent.ShouldNotBeNull();
        
        // Trigger keydown event with Escape key
        dialogContent.KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Escape" });

        // Assert - Dialog should be closed (Open state changes trigger re-render)
        // Since we're testing the event handler, we verify the CloseDialog method was called
        // by checking that the dialog content is no longer visible
        cut.Instance.Open.ShouldBeFalse();
    }

    #endregion

    #region UAC05: Focus Trap

    [Fact(DisplayName = "And focus should be trapped within the dialog when open")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_FocusShouldBeTrappedWhenOpen()
    {
        // Arrange
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, true) // Open the dialog
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Dialog Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Wait for focus trap activation
        await Task.Delay(100);

        // Assert - Should invoke focus trap activate
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.IsAny<object[]>()),
            Times.AtLeastOnce());
    }

    #endregion

    #region UAC06: Return Focus to Trigger

    [Fact(DisplayName = "And focus should return to the trigger when the dialog closes")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_FocusShouldReturnToTriggerOnClose()
    {
        // Arrange - Open dialog first
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Dialog Content");
                }));
                builder.CloseComponent();
            }));

        await Task.Delay(100); // Wait for focus trap activation

        // Act - Close the dialog using InvokeAsync to handle Blazor dispatcher
        cut.InvokeAsync(() => cut.Instance.CloseDialog());
        cut.Render();

        await Task.Delay(100); // Wait for focus trap deactivation

        // Assert - Should invoke focus trap deactivate (which restores focus)
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.deactivate",
            It.IsAny<object[]>()),
            Times.AtLeastOnce());
    }

    #endregion

    #region Additional Integration Tests

    [Fact(DisplayName = "Dialog should support controlled Open state")]
    public void Dialog_ShouldSupportControlledOpenState()
    {
        // Arrange & Act - Render with Open=false
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Hidden Content");
                }));
                builder.CloseComponent();
            }));

        // Assert - DialogContent should not be rendered when closed
        cut.Markup.ShouldNotContain("Hidden Content");

        // Act - Open the dialog using InvokeAsync to handle Blazor dispatcher
        cut.InvokeAsync(() => cut.Instance.OpenDialog());
        cut.Render();

        // Assert - DialogContent should now be rendered
        cut.Markup.ShouldContain("Hidden Content");
    }

    [Fact(DisplayName = "DialogTrigger should toggle dialog open state")]
    public void DialogTrigger_ShouldToggleDialogOpenState()
    {
        // Arrange
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(trigger =>
                {
                    trigger.AddContent(0, "Open");
                }));
                builder.CloseComponent();
            }));

        // Act - Click the trigger
        var trigger = cut.Find("button");
        trigger.Click();

        // Assert - Dialog should be open
        cut.Instance.Open.ShouldBeTrue();
    }

    [Fact(DisplayName = "DialogContent should close on overlay click by default")]
    public void DialogContent_ShouldCloseOnOverlayClickByDefault()
    {
        // Arrange
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Click the overlay
        var overlay = cut.Find(".bg-black\\/80");
        overlay.Click();

        // Assert - Dialog should be closed
        cut.Instance.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "DialogContent should respect CloseOnOverlayClick parameter")]
    public void DialogContent_ShouldRespectCloseOnOverlayClickParameter()
    {
        // Arrange
        var cut = Render<Dialog>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogContent>(0);
                builder.AddAttribute(1, "CloseOnOverlayClick", false);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
                {
                    content.AddContent(0, "Content");
                }));
                builder.CloseComponent();
            }));

        // Act - Click the overlay
        var overlay = cut.Find(".bg-black\\/80");
        overlay.Click();

        // Assert - Dialog should still be open
        cut.Instance.Open.ShouldBeTrue();
    }

    [Fact(DisplayName = "DialogHeader should apply correct CSS classes")]
    public void DialogHeader_ShouldApplyCorrectCssClasses()
    {
        // Arrange & Act
        var cut = Render<DialogHeader>(parameters => parameters
            .AddChildContent("Header Content"));

        // Assert
        cut.Markup.ShouldContain("flex flex-col space-y-1.5");
    }

    [Fact(DisplayName = "DialogTitle should render as h2 with correct classes")]
    public void DialogTitle_ShouldRenderAsH2WithCorrectClasses()
    {
        // Arrange & Act
        var cut = Render<DialogTitle>(parameters => parameters
            .AddChildContent("Title"));

        // Assert
        cut.Markup.ShouldContain("<h2");
        cut.Markup.ShouldContain("text-lg font-semibold");
        cut.Markup.ShouldContain("Title");
    }

    [Fact(DisplayName = "DialogDescription should render as p with muted text")]
    public void DialogDescription_ShouldRenderAsParagraphWithMutedText()
    {
        // Arrange & Act
        var cut = Render<DialogDescription>(parameters => parameters
            .AddChildContent("Description text"));

        // Assert
        cut.Markup.ShouldContain("<p");
        cut.Markup.ShouldContain("text-gray-500");
        cut.Markup.ShouldContain("Description text");
    }

    [Fact(DisplayName = "DialogFooter should apply correct flexbox classes")]
    public void DialogFooter_ShouldApplyCorrectFlexboxClasses()
    {
        // Arrange & Act
        var cut = Render<DialogFooter>(parameters => parameters
            .AddChildContent("Footer content"));

        // Assert
        cut.Markup.ShouldContain("flex flex-col-reverse");
        cut.Markup.ShouldContain("sm:flex-row sm:justify-end");
    }

    #endregion
}
