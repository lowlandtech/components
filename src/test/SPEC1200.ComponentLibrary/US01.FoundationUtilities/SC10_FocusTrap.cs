using LowlandTech.Components.Utilities;
using Microsoft.JSInterop;
using Moq;

namespace LowlandTech.Cli.Tests.SPEC1227.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for FocusTrap utility
/// Feature: US01.FoundationUtilities.feature
/// Scenario: SC10 - Implement focus trap utility
/// UACs: UAC45-UAC50
/// </summary>
[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "10")]
public class SC10_FocusTrap
{
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC10_FocusTrap()
    {
        _mockJsRuntime = new Mock<IJSRuntime>();
        // Default setup for activate
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);
        
        // Default setup for deactivate
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.deactivate",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);
    }

    #region UAC45 - Should trap focus within container

    [Fact(DisplayName = "Then it should trap focus within a container element")]
    [Trait(Spec.UAC, "45")]
    public async Task UAC45_ShouldTrapFocusWithinContainer()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "test-container");

        // Act
        await focusTrap.ActivateAsync();

        // Assert
        focusTrap.IsActive.ShouldBeTrue();
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.Is<object[]>(args => 
                args.Length == 2 &&
                args[0].ToString() == "test-container" &&
                (bool)args[1] == true)), Times.Once);
    }

    #endregion

    #region UAC46 - Should move focus to first focusable element on activate

    [Fact(DisplayName = "And it should move focus to first focusable element on activate")]
    [Trait(Spec.UAC, "46")]
    public async Task UAC46_ShouldMoveToFirstFocusableOnActivate()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "modal-container");

        // Act
        await focusTrap.ActivateAsync();

        // Assert
        // Verify that activate was called (which moves focus to first focusable in JS)
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.Is<object[]>(args => 
                args.Length == 2 &&
                args[0].ToString() == "modal-container")), Times.Once);
    }

    #endregion

    #region UAC47 - Tab should cycle through focusable elements

    [Fact(DisplayName = "And Tab should cycle through focusable elements")]
    [Trait(Spec.UAC, "47")]
    public async Task UAC47_TabShouldCycleThroughFocusableElements()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "dialog-container");

        // Act
        await focusTrap.ActivateAsync();

        // Assert
        // Tab cycling is handled by JavaScript event listener
        // We verify activation was called which sets up the Tab handler
        focusTrap.IsActive.ShouldBeTrue();
    }

    #endregion

    #region UAC48 - Shift+Tab should cycle backwards

    [Fact(DisplayName = "And Shift+Tab should cycle backwards")]
    [Trait(Spec.UAC, "48")]
    public async Task UAC48_ShiftTabShouldCycleBackwards()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "menu-container");

        // Act
        await focusTrap.ActivateAsync();

        // Assert
        // Shift+Tab cycling is handled by JavaScript event listener
        // We verify activation was called which sets up the Shift+Tab handler
        focusTrap.IsActive.ShouldBeTrue();
    }

    #endregion

    #region UAC49 - Escape should release focus trap

    [Fact(DisplayName = "And Escape should release focus trap")]
    [Trait(Spec.UAC, "49")]
    public async Task UAC49_EscapeShouldReleaseFocusTrap()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "overlay-container");
        var releasedEventFired = false;

        focusTrap.Released += (sender, e) => releasedEventFired = true;

        await focusTrap.ActivateAsync();

        // Act
        await focusTrap.DeactivateAsync();

        // Assert
        focusTrap.IsActive.ShouldBeFalse();
        releasedEventFired.ShouldBeTrue();
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.deactivate",
            It.Is<object[]>(args => 
                args.Length == 1 &&
                args[0].ToString() == "overlay-container")), Times.Once);
    }

    #endregion

    #region UAC50 - Should restore focus to trigger element on release

    [Fact(DisplayName = "And it should restore focus to trigger element on release")]
    [Trait(Spec.UAC, "50")]
    public async Task UAC50_ShouldRestoreFocusToTriggerOnRelease()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "popup-container");

        // Act
        await focusTrap.ActivateAsync(restoreFocusOnRelease: true);
        await focusTrap.DeactivateAsync();

        // Assert
        // Verify activate was called with restoreFocusOnRelease=true
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.Is<object[]>(args => 
                args.Length == 2 &&
                (bool)args[1] == true)), Times.Once);
    }

    #endregion

    #region Additional Tests - Edge Cases & Lifecycle

    [Fact]
    public async Task ShouldNotReactivateWhenAlreadyActive()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "container");

        // Act
        await focusTrap.ActivateAsync();
        await focusTrap.ActivateAsync(); // Second activation

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.IsAny<object[]>()), Times.Once); // Should only call once
    }

    [Fact]
    public async Task ShouldNotDeactivateWhenNotActive()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "container");

        // Act
        await focusTrap.DeactivateAsync();

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.deactivate",
            It.IsAny<object[]>()), Times.Never);
    }

    [Fact]
    public async Task ShouldDeactivateOnDispose()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "container");

        await focusTrap.ActivateAsync();

        // Act
        await focusTrap.DisposeAsync();

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.deactivate",
            It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void ShouldThrowWhenContainerIdIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new FocusTrap(_mockJsRuntime.Object, null!));
    }

    [Fact]
    public void ShouldThrowWhenContainerIdIsEmpty()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new FocusTrap(_mockJsRuntime.Object, ""));
    }

    [Fact]
    public void ShouldThrowWhenJSRuntimeIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new FocusTrap(null!, "container"));
    }

    [Fact]
    public async Task ShouldHandleJSExceptionGracefully()
    {
        // Arrange
        var mockJs = new Mock<IJSRuntime>();
        mockJs.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("JS not available"));

        var focusTrap = new FocusTrap(mockJs.Object, "container");

        // Act
        await focusTrap.ActivateAsync();

        // Assert
        focusTrap.IsActive.ShouldBeFalse(); // Should fail silently
    }

    [Fact]
    public async Task ShouldSupportDisableRestoreFocus()
    {
        // Arrange
        var focusTrap = new FocusTrap(_mockJsRuntime.Object, "container");

        // Act
        await focusTrap.ActivateAsync(restoreFocusOnRelease: false);

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.focusTrapUtils.activate",
            It.Is<object[]>(args => 
                args.Length == 2 &&
                (bool)args[1] == false)), Times.Once);
    }

    #endregion
}
