using Bunit;
using LowlandTech.Components.Components.Portal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for Portal component (SC05-SC06: Portal Component)
/// Covers UAC22-UAC31 from US01.FoundationUtilities.feature
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "05")]
public class SC05_SC06_Portal : IDisposable
{
    private readonly TestContext _testContext;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public SC05_SC06_Portal()
    {
        _testContext = new TestContext();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _testContext.Services.AddSingleton(_mockJsRuntime.Object);
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

    #region SC05: Portal Component Implementation (UAC22-UAC27)

    [Fact(DisplayName = "Then it should accept ChildContent RenderFragment parameter")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_ShouldAcceptChildContentParameter()
    {
        // Arrange & Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test Content</div>"));

        // Assert
        cut.ShouldNotBeNull();
        cut.Markup.ShouldContain("Test Content");
    }

    [Fact(DisplayName = "And it should inject IJSRuntime")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_ShouldInjectIJSRuntime()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.Contains("portalUtils.create")),
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Assert - Should invoke JS after render
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "And it should render content to document.body via JSInterop")]
    [Trait(Spec.UAC, "24")]
    public void UAC24_ShouldRenderContentToDocumentBodyViaJSInterop()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .Add(p => p.Target, "body")
            .AddChildContent("<div>Portal Content</div>"));

        // Assert - Should call create with "body" as target
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.Is<object[]>(args => args.Length >= 2 && args[1].Equals("body"))),
            Times.Once);
    }

    [Fact(DisplayName = "And it should create a unique portal container div")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_ShouldCreateUniquePortalContainerDiv()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        string? capturedPortalId = null;

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .Callback<string, object[]>((method, args) =>
            {
                if (args.Length > 0)
                {
                    capturedPortalId = args[0]?.ToString();
                }
            })
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Assert - Portal ID should be unique (GUID-like)
        capturedPortalId.ShouldNotBeNull();
        capturedPortalId.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "And it should cleanup the container on dispose")]
    [Trait(Spec.UAC, "26")]
    public async Task UAC26_ShouldCleanupContainerOnDispose()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.destroy",
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Get the component instance and dispose it
        var portalInstance = cut.Instance as IAsyncDisposable;

        // Act
        await portalInstance!.DisposeAsync();

        // Assert
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.destroy",
            It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "And it should handle server-side rendering gracefully")]
    [Trait(Spec.UAC, "27")]
    public void UAC27_ShouldHandleServerSideRenderingGracefully()
    {
        // Arrange - Simulate SSR by throwing JSException
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.IsAny<string>(),
            It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("JSInterop not available"));

        // Act & Assert - Should not throw
        Should.NotThrow(() =>
        {
            var cut = Render<Portal>(parameters => parameters
                .AddChildContent("<div>Test Content</div>"));

            // Content should still render (fallback)
            cut.Markup.ShouldContain("Test Content");
        });
    }

    #endregion

    #region SC06: Portal Component bUnit Tests (UAC28-UAC31)

    [Fact(DisplayName = "Then the component should render without errors")]
    [Trait(Spec.UAC, "28")]
    public void UAC28_ShouldRenderWithoutErrors()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            It.IsAny<string>(),
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div class='test-content'>Portal Test</div>"));

        // Assert
        cut.ShouldNotBeNull();
        cut.Markup.ShouldContain("test-content");
        cut.Markup.ShouldContain("Portal Test");
    }

    [Fact(DisplayName = "And it should invoke JS \"portalUtils.create\" on first render")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_ShouldInvokePortalUtilsCreateOnFirstRender()
    {
        // Arrange
        var createCalled = false;
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .Callback(() => createCalled = true)
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Assert
        createCalled.ShouldBeTrue();
    }

    [Fact(DisplayName = "And it should pass the portal ID and content element reference")]
    [Trait(Spec.UAC, "30")]
    public void UAC30_ShouldPassPortalIdAndContentElementReference()
    {
        // Arrange
        object[]? capturedArgs = null;

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .Callback<string, object[]>((method, args) => capturedArgs = args)
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Assert
        capturedArgs.ShouldNotBeNull();
        capturedArgs.Length.ShouldBeGreaterThanOrEqualTo(1);
        // First arg should be portal ID (non-empty string)
        capturedArgs[0].ShouldBeOfType<string>();
        (capturedArgs[0] as string).ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "And the portal container should be created in the body (verify via JS mock)")]
    [Trait(Spec.UAC, "31")]
    public void UAC31_PortalContainerShouldBeCreatedInBody()
    {
        // Arrange
        string? targetElement = null;

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .Callback<string, object[]>((method, args) =>
            {
                if (args.Length >= 2)
                {
                    targetElement = args[1]?.ToString();
                }
            })
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Assert - Default target should be "body"
        targetElement.ShouldBe("body");
    }

    #endregion

    #region Additional Tests: Custom Target and ClassName

    [Fact]
    public void ShouldAllowCustomTarget()
    {
        // Arrange
        string? targetElement = null;

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .Callback<string, object[]>((method, args) =>
            {
                if (args.Length >= 2)
                {
                    targetElement = args[1]?.ToString();
                }
            })
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .Add(p => p.Target, "#custom-target")
            .AddChildContent("<div>Test</div>"));

        // Assert
        targetElement.ShouldBe("#custom-target");
    }

    [Fact]
    public void ShouldPassClassNameToPortalContainer()
    {
        // Arrange
        string? className = null;

        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .Callback<string, object[]>((method, args) =>
            {
                if (args.Length >= 3)
                {
                    className = args[2]?.ToString();
                }
            })
            .ReturnsAsync(null as object);

        // Act
        var cut = Render<Portal>(parameters => parameters
            .Add(p => p.ClassName, "custom-portal-class")
            .AddChildContent("<div>Test</div>"));

        // Assert
        className.ShouldBe("custom-portal-class");
    }

    [Fact]
    public async Task ShouldNotCallDestroyIfNotInitialized()
    {
        // Arrange
        _mockJsRuntime.Setup(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.create",
            It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Not available"));

        var cut = Render<Portal>(parameters => parameters
            .AddChildContent("<div>Test</div>"));

        // Get the component instance and dispose it
        var portalInstance = cut.Instance as IAsyncDisposable;

        // Act
        await portalInstance!.DisposeAsync();

        // Assert - destroy should NOT be called since create failed
        _mockJsRuntime.Verify(x => x.InvokeAsync<object?>(
            "LowlandTechComponents.portalUtils.destroy",
            It.IsAny<object[]>()), Times.Never);
    }

    #endregion
}
