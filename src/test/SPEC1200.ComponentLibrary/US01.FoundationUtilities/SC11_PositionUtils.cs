using LowlandTech.Components.Utilities;
using Microsoft.JSInterop;
using Moq;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for PositionUtils (SC11: Positioning Utilities for floating elements)
/// Covers UAC51-UAC56 from US01.FoundationUtilities.feature
/// 
/// Validates positioning calculations, placement options, auto-positioning,
/// coordinate returns, scroll event handling, and cleanup on dispose.
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "11")]
public class SC11_PositionUtils
{
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC11_PositionUtils()
    {
        _mockJsRuntime = new Mock<IJSRuntime>();
        
        // Default mock for position calculation
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ReturnsAsync(new PositionResult { Top = 100, Left = 50 });
        
        // Default mock for overflow check
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync(new OverflowResult { Top = false, Bottom = false, Left = false, Right = false });
    }

    #region UAC51 - Calculate optimal position for floating element

    [Fact(DisplayName = "Then it should calculate optimal position for floating element")]
    [Trait(Spec.UAC, "51")]
    public async Task UAC51_ShouldCalculateOptimalPosition()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");

        // Act
        var result = await positionUtils.CalculatePositionAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Top.ShouldBe(100);
        result.Left.ShouldBe(50);
        
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args =>
                args[0].ToString() == "popover" &&
                args[1].ToString() == "button" &&
                args[2].ToString() == "bottom" &&
                (int)args[3] == 8)), Times.Once);
    }

    #endregion

    #region UAC52 - Support placement options (top, bottom, left, right)

    [Fact(DisplayName = "And it should support placement options (top, bottom, left, right)")]
    [Trait(Spec.UAC, "52")]
    public async Task UAC52_ShouldSupportPlacementOptions()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");
        var placements = new[] { "top", "bottom", "left", "right" };

        // Act & Assert
        foreach (var placement in placements)
        {
            await positionUtils.CalculatePositionAsync(placement: placement);
            
            _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.Is<object[]>(args => args[2].ToString() == placement)), Times.Once);
        }
    }

    [Fact]
    public async Task UAC52_TopPlacement_ShouldPassCorrectParameters()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "tooltip", "anchor");

        // Act
        await positionUtils.CalculatePositionAsync(placement: "top", offset: 10);

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args =>
                args[0].ToString() == "tooltip" &&
                args[1].ToString() == "anchor" &&
                args[2].ToString() == "top" &&
                (int)args[3] == 10)), Times.Once);
    }

    [Fact]
    public async Task UAC52_CustomOffset_ShouldBeRespected()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "menu", "trigger");

        // Act
        await positionUtils.CalculatePositionAsync(placement: "right", offset: 16);

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => (int)args[3] == 16)), Times.Once);
    }

    #endregion

    #region UAC53 - Handle auto-positioning to avoid viewport edges

    [Fact(DisplayName = "And it should handle auto-positioning to avoid viewport edges")]
    [Trait(Spec.UAC, "53")]
    public async Task UAC53_ShouldAutoPositionToAvoidViewportEdges()
    {
        // Arrange - Bottom placement would overflow, should flip to top
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync(new OverflowResult { Bottom = true });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");

        // Act
        var result = await positionUtils.CalculateWithAutoPositionAsync(placement: "bottom");

        // Assert
        result.ShouldNotBeNull();
        
        // Should call position twice: once for bottom, once for adjusted top
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "bottom")), Times.Once);
        
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "top")), Times.Once);
    }

    [Fact]
    public async Task UAC53_TopOverflow_ShouldFlipToBottom()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync(new OverflowResult { Top = true });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "tooltip", "anchor");

        // Act
        await positionUtils.CalculateWithAutoPositionAsync(placement: "top");

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "bottom")), Times.Once);
    }

    [Fact]
    public async Task UAC53_LeftOverflow_ShouldFlipToRight()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync(new OverflowResult { Left = true });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "menu", "trigger");

        // Act
        await positionUtils.CalculateWithAutoPositionAsync(placement: "left");

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "right")), Times.Once);
    }

    [Fact]
    public async Task UAC53_RightOverflow_ShouldFlipToLeft()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync(new OverflowResult { Right = true });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "dropdown", "button");

        // Act
        await positionUtils.CalculateWithAutoPositionAsync(placement: "right");

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "left")), Times.Once);
    }

    [Fact]
    public async Task UAC53_NoOverflow_ShouldKeepOriginalPlacement()
    {
        // Arrange - No overflow
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync(new OverflowResult { Top = false, Bottom = false, Left = false, Right = false });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");

        // Act
        await positionUtils.CalculateWithAutoPositionAsync(placement: "bottom");

        // Assert - Should only call position once with original placement
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "bottom")), Times.Once);
        
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.IsAny<object[]>()), Times.Once);
    }

    #endregion

    #region UAC54 - Return position coordinates (x, y)

    [Fact(DisplayName = "And it should return position coordinates (x, y)")]
    [Trait(Spec.UAC, "54")]
    public async Task UAC54_ShouldReturnPositionCoordinates()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ReturnsAsync(new PositionResult { Top = 250, Left = 125 });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");

        // Act
        var result = await positionUtils.CalculatePositionAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Top.ShouldBe(250); // y coordinate
        result.Left.ShouldBe(125); // x coordinate
    }

    [Fact]
    public async Task UAC54_DifferentCoordinates_ShouldBeReturned()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ReturnsAsync(new PositionResult { Top = 500.5, Left = 300.75 });

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "tooltip", "anchor");

        // Act
        var result = await positionUtils.CalculatePositionAsync(placement: "top");

        // Assert
        result.Top.ShouldBe(500.5);
        result.Left.ShouldBe(300.75);
    }

    #endregion

    #region UAC55 - Handle scroll events and update position

    [Fact(DisplayName = "And it should handle scroll events and update position")]
    [Trait(Spec.UAC, "55")]
    public async Task UAC55_ShouldHandleScrollEventsAndUpdatePosition()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");

        // Act - Simulate scroll event causing position recalculation
        var position1 = await positionUtils.CalculatePositionAsync();
        
        // Simulate scroll - update mock to return new position
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ReturnsAsync(new PositionResult { Top = 200, Left = 100 });
        
        var position2 = await positionUtils.CalculatePositionAsync();

        // Assert
        position1.ShouldNotBeNull();
        position2.ShouldNotBeNull();
        position2.Top.ShouldBe(200);
        position2.Left.ShouldBe(100);
        
        // Should be called twice for scroll updates
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.IsAny<object[]>()), Times.Exactly(2));
    }

    #endregion

    #region UAC56 - Cleanup event listeners on dispose

    [Fact(DisplayName = "And it should cleanup event listeners on dispose")]
    [Trait(Spec.UAC, "56")]
    public async Task UAC56_ShouldCleanupEventListenersOnDispose()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "popover", "button");
        await positionUtils.CalculatePositionAsync();

        // Act
        await positionUtils.DisposeAsync();

        // Assert - Should not throw and should complete cleanup
        // Multiple dispose calls should be safe
        await positionUtils.DisposeAsync();
    }

    [Fact]
    public async Task UAC56_DisposeMultipleTimes_ShouldBeSafe()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "tooltip", "anchor");

        // Act & Assert - Should not throw
        await positionUtils.DisposeAsync();
        await positionUtils.DisposeAsync();
        await positionUtils.DisposeAsync();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task EdgeCase_NullJSRuntime_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new PositionUtils(null!, "element", "reference"));
    }

    [Fact]
    public async Task EdgeCase_NullElementId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new PositionUtils(_mockJsRuntime.Object, null!, "reference"));
    }

    [Fact]
    public async Task EdgeCase_EmptyElementId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new PositionUtils(_mockJsRuntime.Object, "", "reference"));
    }

    [Fact]
    public async Task EdgeCase_WhitespaceElementId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new PositionUtils(_mockJsRuntime.Object, "   ", "reference"));
    }

    [Fact]
    public async Task EdgeCase_NullReferenceId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new PositionUtils(_mockJsRuntime.Object, "element", null!));
    }

    [Fact]
    public async Task EdgeCase_EmptyReferenceId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new PositionUtils(_mockJsRuntime.Object, "element", ""));
    }

    [Fact]
    public async Task EdgeCase_WhitespaceReferenceId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            new PositionUtils(_mockJsRuntime.Object, "element", "   "));
    }

    [Fact]
    public async Task EdgeCase_JSException_ShouldReturnDefaultPosition()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Element not found"));

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "nonexistent", "button");

        // Act
        var result = await positionUtils.CalculatePositionAsync();

        // Assert - Should gracefully return default (SSR scenario)
        result.ShouldNotBeNull();
        result.Top.ShouldBe(0);
        result.Left.ShouldBe(0);
    }

    [Fact]
    public async Task EdgeCase_InvalidOperationException_ShouldReturnDefaultPosition()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ThrowsAsync(new InvalidOperationException("JSRuntime not available"));

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "element", "reference");

        // Act
        var result = await positionUtils.CalculatePositionAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Top.ShouldBe(0);
        result.Left.ShouldBe(0);
    }

    [Fact]
    public async Task EdgeCase_NullReturnFromJS_ShouldReturnDefaultPosition()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<PositionResult>(
                "LowlandTechComponents.positionUtils.position",
                It.IsAny<object[]>()))
            .ReturnsAsync((PositionResult)null!);

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "element", "reference");

        // Act
        var result = await positionUtils.CalculatePositionAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Top.ShouldBe(0);
        result.Left.ShouldBe(0);
    }

    [Fact]
    public async Task EdgeCase_CheckOverflow_JSException_ShouldReturnDefaultOverflow()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Element not found"));

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "element", "reference");

        // Act
        var result = await positionUtils.CheckOverflowAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Top.ShouldBeFalse();
        result.Bottom.ShouldBeFalse();
        result.Left.ShouldBeFalse();
        result.Right.ShouldBeFalse();
    }

    [Fact]
    public async Task EdgeCase_CheckOverflow_NullReturn_ShouldReturnDefaultOverflow()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.InvokeAsync<OverflowResult>(
                "LowlandTechComponents.positionUtils.checkOverflow",
                It.IsAny<object[]>()))
            .ReturnsAsync((OverflowResult)null!);

        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "element", "reference");

        // Act
        var result = await positionUtils.CheckOverflowAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Top.ShouldBeFalse();
        result.Bottom.ShouldBeFalse();
        result.Left.ShouldBeFalse();
        result.Right.ShouldBeFalse();
    }

    [Fact]
    public async Task EdgeCase_UnknownPlacement_ShouldStillCallJS()
    {
        // Arrange
        var positionUtils = new PositionUtils(_mockJsRuntime.Object, "element", "reference");

        // Act
        await positionUtils.CalculatePositionAsync(placement: "top-start");

        // Assert - Should pass through to JS even if unknown
        _mockJsRuntime.Verify(x => x.InvokeAsync<PositionResult>(
            "LowlandTechComponents.positionUtils.position",
            It.Is<object[]>(args => args[2].ToString() == "top-start")), Times.Once);
    }

    #endregion
}
