using Bunit;
using LowlandTech.Components.Components.SwipeArea;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US10.UtilityComponents;

/// <summary>
/// SC04: SwipeArea component implementation
/// 
/// Feature: US10.UtilityComponents.feature
/// Scenario: SC04 - Implement SwipeArea component
/// 
/// Given I have a SwipeArea component specification
/// When I implement it as a Blazor component
/// Then it should render a wrapper element with class "swipe-area"
/// And it should fire OnSwipeMove with direction data on pointer move
/// And it should fire OnSwipeEnd when movement exceeds Sensitivity
/// And it should honor PreventDefault by invoking the required JS interop
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "04")]
public class SC04_SwipeAreaRenders : IDisposable
{
    private readonly BunitContext _testContext;
    private readonly Mock<IJSRuntime> _mockJsRuntime;
    private readonly Mock<IJSObjectReference> _mockJsModule;

    public SC04_SwipeAreaRenders()
    {
        _testContext = new BunitContext();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _mockJsModule = new Mock<IJSObjectReference>();
        _testContext.Services.AddSingleton(_mockJsRuntime.Object);

        // Setup JS module import
        _mockJsRuntime.Setup(x => x.InvokeAsync<IJSObjectReference>(
            "import",
            It.IsAny<object[]>()))
            .ReturnsAsync(_mockJsModule.Object);

        // Setup swipeUtils methods - using InvokeAsync<object> instead of InvokeVoidAsync extension method
        _mockJsModule.Setup(x => x.InvokeAsync<object?>(
            It.Is<string>(s => s.StartsWith("swipeUtils")),
            It.IsAny<object[]>()))
            .ReturnsAsync(null as object);
    }

    public void Dispose()
    {
        _testContext?.Dispose();
    }

    [Fact(DisplayName = "SwipeArea renders wrapper element with class swipe-area")]
    [Trait(Spec.UAC, "14")]
    public void SwipeArea_RendersWrapperWithClass()
    {
        // Given
        using var ctx = new BunitContext();
        ctx.Services.AddSingleton(_mockJsRuntime.Object);

        // When - Render SwipeArea
        var cut = ctx.Render<SwipeArea>(parameters => parameters
            .AddChildContent("<div>Swipe content</div>"));

        // Then - Should render wrapper with swipe-area class
        var wrapper = cut.Find(".swipe-area");
        Assert.NotNull(wrapper);

        // Should contain child content
        Assert.Contains("Swipe content", cut.Markup);
    }

    [Fact(DisplayName = "SwipeArea fires OnSwipeMove with direction data on pointer move")]
    [Trait(Spec.UAC, "15")]
    public async Task SwipeArea_FiresOnSwipeMove()
    {
        // Given
        using var ctx = new BunitContext();
        ctx.Services.AddSingleton(_mockJsRuntime.Object);
        SwipeEventArgs? capturedArgs = null;

        // When - Render with OnSwipeMove callback
        var cut = ctx.Render<SwipeArea>(parameters => parameters
            .Add(p => p.OnSwipeMove, EventCallback.Factory.Create<SwipeEventArgs>(
                this, (SwipeEventArgs args) => capturedArgs = args))
            .AddChildContent("<div>Content</div>"));

        var swipeArea = cut.Find(".swipe-area");

        // Simulate pointer down
        await swipeArea.PointerDownAsync(new PointerEventArgs { ClientX = 100, ClientY = 100 });

        // Simulate pointer move to the right
        await swipeArea.PointerMoveAsync(new PointerEventArgs { ClientX = 150, ClientY = 105 });

        // Then - Should fire OnSwipeMove with direction data
        Assert.NotNull(capturedArgs);
        Assert.Equal(SwipeDirection.Right, capturedArgs.Direction);
        Assert.True(capturedArgs.DeltaX > 0);
        Assert.True(capturedArgs.Distance > 0);
    }

    [Fact(DisplayName = "SwipeArea fires OnSwipeEnd when movement exceeds Sensitivity")]
    [Trait(Spec.UAC, "16")]
    public async Task SwipeArea_FiresOnSwipeEndWhenThresholdExceeded()
    {
        // Given
        using var ctx = new BunitContext();
        ctx.Services.AddSingleton(_mockJsRuntime.Object);
        SwipeEventArgs? capturedArgs = null;

        // When - Render with OnSwipeEnd callback and Sensitivity = 50
        var cut = ctx.Render<SwipeArea>(parameters => parameters
            .Add(p => p.Sensitivity, 50.0)
            .Add(p => p.OnSwipeEnd, EventCallback.Factory.Create<SwipeEventArgs>(
                this, (SwipeEventArgs args) => capturedArgs = args))
            .AddChildContent("<div>Content</div>"));

        var swipeArea = cut.Find(".swipe-area");

        // Simulate swipe that exceeds sensitivity
        await swipeArea.PointerDownAsync(new PointerEventArgs { ClientX = 100, ClientY = 100 });
        await swipeArea.PointerUpAsync(new PointerEventArgs { ClientX = 200, ClientY = 100 }); // 100px horizontal swipe

        // Then - Should fire OnSwipeEnd because distance (100px) > Sensitivity (50px)
        Assert.NotNull(capturedArgs);
        Assert.Equal(SwipeDirection.Right, capturedArgs.Direction);
        Assert.True(capturedArgs.Distance >= 50.0);
    }

    [Fact(DisplayName = "SwipeArea honors PreventDefault by invoking JS interop")]
    [Trait(Spec.UAC, "17")]
    public async Task SwipeArea_HonorsPreventDefault()
    {
        // Given & When - Render with PreventDefault = true
        var cut = _testContext.Render<SwipeArea>(parameters => parameters
            .Add(p => p.PreventDefault, true)
            .AddChildContent("<div>Content</div>"));

        // Wait for OnAfterRenderAsync to complete
        await Task.Delay(100);

        // Then - Should invoke swipeUtils.init via JS interop
        _mockJsRuntime.Verify(x => x.InvokeAsync<IJSObjectReference>(
            "import",
            It.IsAny<object[]>()),
            Times.AtLeastOnce());

        _mockJsModule.Verify(x => x.InvokeAsync<object>(
            "swipeUtils.init",
            It.IsAny<object[]>()),
            Times.AtLeastOnce());

        // Simulate pointer down to trigger preventDefault
        var swipeArea = cut.Find(".swipe-area");
        await swipeArea.PointerDownAsync(new PointerEventArgs { ClientX = 100, ClientY = 100 });

        // Should invoke preventDefault
        _mockJsModule.Verify(x => x.InvokeAsync<object>(
            "swipeUtils.preventDefault",
            It.IsAny<object[]>()),
            Times.AtLeastOnce());
    }
}
