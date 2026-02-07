using LowlandTech.Components.Utilities;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for ThemeService (SC07-SC08: Dark-Only Theme Management)
/// Covers UAC32-UAC41 from US01.FoundationUtilities.feature
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "07")]
public class SC07_SC08_ThemeService
{
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC07_SC08_ThemeService()
    {
        _mockJsRuntime = new Mock<IJSRuntime>();
    }

    #region SC07: Theme Service Dark-Only Core (UAC32-UAC37)

    [Fact(DisplayName = "Then it should expose the current theme as \"dark\" only")]
    [Trait(Spec.UAC, "32")]
    public void UAC32_ShouldExposeThemeAsDarkOnly()
    {
        // Arrange
        var service = new ThemeService(_mockJsRuntime.Object);

        // Act & Assert
        service.Theme.ShouldBe("dark");
    }

    [Fact(DisplayName = "And it should have an OnThemeChanged event")]
    [Trait(Spec.UAC, "33")]
    public async Task UAC33_ShouldHaveOnThemeChangedEvent()
    {
        // Arrange
        var service = new ThemeService(_mockJsRuntime.Object);
        string? raisedTheme = null;
        service.OnThemeChanged += (sender, theme) => raisedTheme = theme;

        // Act
        await service.SetThemeAsync("dark");

        // Assert
        raisedTheme.ShouldBe("dark");
    }

    [Fact(DisplayName = "And applying the theme should trigger OnThemeChanged")]
    [Trait(Spec.UAC, "34")]
    public async Task UAC34_ApplyingThemeShouldTriggerEvent()
    {
        // Arrange
        var eventFired = false;
        var service = new ThemeService(_mockJsRuntime.Object);
        service.OnThemeChanged += (sender, theme) => eventFired = true;

        // Act
        await service.SetThemeAsync("dark");

        // Assert
        eventFired.ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should persist the dark theme to localStorage via JSInterop")]
    [Trait(Spec.UAC, "35")]
    public async Task UAC35_ShouldPersistDarkThemeViaJSInterop()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme", 
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act
        await service.SetThemeAsync("dark");

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme",
            It.Is<object[]>(args => args.Length == 1 && args[0].Equals("dark"))), 
            Times.Once);
    }

    [Fact(DisplayName = "And it should load the persisted theme on init, honoring only \"dark\"")]
    [Trait(Spec.UAC, "36")]
    public async Task UAC36_ShouldLoadPersistedDarkThemeOnInit()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<string?>(
            "LowlandTechComponents.themeUtils.getTheme", 
            It.IsAny<object[]>()))
            .ReturnsAsync("dark");

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.applyTheme",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act
        await service.InitializeAsync();

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.applyTheme",
            It.Is<object[]>(args => args.Length == 1 && args[0].Equals("dark"))),
            Times.Once);
    }

    [Fact(DisplayName = "And it should reject or ignore any non-dark theme value")]
    [Trait(Spec.UAC, "37")]
    public async Task UAC37_ShouldRejectNonDarkThemeValues()
    {
        // Arrange
        var service = new ThemeService(_mockJsRuntime.Object);

        // Act & Assert - Should throw for "light"
        await Should.ThrowAsync<ArgumentException>(async () => 
            await service.SetThemeAsync("light"));

        // Act & Assert - Should throw for "system"
        await Should.ThrowAsync<ArgumentException>(async () => 
            await service.SetThemeAsync("system"));

        // Act & Assert - Should throw for invalid values
        await Should.ThrowAsync<ArgumentException>(async () => 
            await service.SetThemeAsync("invalid"));
    }

    #endregion

    #region SC08: Theme Service Tests with bUnit (UAC38-UAC41)

    [Fact(DisplayName = "Then OnThemeChanged event should fire")]
    [Trait(Spec.UAC, "38")]
    public async Task UAC38_OnThemeChangedEventShouldFire()
    {
        // Arrange
        var service = new ThemeService(_mockJsRuntime.Object);
        var eventFired = false;
        string? eventTheme = null;

        service.OnThemeChanged += (sender, theme) =>
        {
            eventFired = true;
            eventTheme = theme;
        };

        // Act
        await service.SetThemeAsync("dark");

        // Assert
        eventFired.ShouldBeTrue();
        eventTheme.ShouldBe("dark");
    }

    [Fact(DisplayName = "And the Theme property should return \"dark\"")]
    [Trait(Spec.UAC, "39")]
    public void UAC39_ThemePropertyShouldReturnDark()
    {
        // Arrange
        var service = new ThemeService(_mockJsRuntime.Object);

        // Act & Assert
        service.Theme.ShouldBe("dark");
    }

    [Fact(DisplayName = "And it should invoke JS \"themeUtils.setTheme\" with \"dark\"")]
    [Trait(Spec.UAC, "40")]
    public async Task UAC40_ShouldInvokeJSSetThemeWithDark()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act
        await service.SetThemeAsync("dark");

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme",
            It.Is<object[]>(args => args.Length == 1 && args[0].Equals("dark"))),
            Times.Once);
    }

    [Fact(DisplayName = "Then the component should re-render when theme changes")]
    [Trait(Spec.UAC, "41")]
    public async Task UAC41_ComponentShouldReRenderWhenThemeChanges()
    {
        // Arrange
        var service = new ThemeService(_mockJsRuntime.Object);
        var renderCount = 0;

        service.OnThemeChanged += (sender, theme) => renderCount++;

        // Act - Apply theme multiple times
        await service.SetThemeAsync("dark");
        await service.SetThemeAsync("dark"); // Same theme again
        await service.SetThemeAsync("dark"); // Same theme again

        // Assert - Event should fire each time (components listening will re-render)
        renderCount.ShouldBe(3);
    }

    #endregion

    #region Additional Tests: JSInterop Graceful Degradation

    [Fact]
    public async Task ShouldHandleJSInteropNotAvailableDuringInit()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<string?>(
            "LowlandTechComponents.themeUtils.getTheme",
            It.IsAny<object[]>()))
            .ThrowsAsync(new InvalidOperationException("JSRuntime not available"));

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act & Assert
        await Should.NotThrowAsync(async () => await service.InitializeAsync());
        service.Theme.ShouldBe("dark"); // Should use default dark theme
    }

    [Fact]
    public async Task ShouldHandleJSInteropNotAvailableDuringSetTheme()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme",
            It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Not available"));

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act & Assert
        await Should.NotThrowAsync(async () => await service.SetThemeAsync("dark"));
        service.Theme.ShouldBe("dark"); // Should still expose dark theme
    }

    [Fact]
    public async Task ShouldIgnoreNonDarkPersistedTheme()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<string?>(
            "LowlandTechComponents.themeUtils.getTheme",
            It.IsAny<object[]>()))
            .ReturnsAsync("light"); // Persisted light theme (invalid)

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act
        await service.InitializeAsync();

        // Assert - Should default to dark and persist it
        service.Theme.ShouldBe("dark");
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.themeUtils.setTheme",
            It.Is<object[]>(args => args.Length == 1 && args[0].Equals("dark"))),
            Times.Once);
    }

    [Fact]
    public async Task ShouldNotInitializeTwice()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<string?>(
            "LowlandTechComponents.themeUtils.getTheme",
            It.IsAny<object[]>()))
            .ReturnsAsync("dark");

        var service = new ThemeService(_mockJsRuntime.Object);

        // Act
        await service.InitializeAsync();
        await service.InitializeAsync();

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<string?>(
            "LowlandTechComponents.themeUtils.getTheme",
            It.IsAny<object[]>()), 
            Times.Once);
    }

    #endregion
}
