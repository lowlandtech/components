using Bunit;
using LowlandTech.Components.Components.Resizable;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US07.LayoutComponents;

/// <summary>
/// SC02: Resizable component implementation
/// 
/// Feature: US07.LayoutComponents.feature
/// Scenario: SC02 - Implement Resizable component
/// 
/// Given I have the shadcn/ui Resizable component
/// When I port it to Blazor
/// Then I should have ResizablePanelGroup, ResizablePanel, and ResizableHandle components
/// And it should support horizontal and vertical orientations
/// And it should support default panel sizes
/// And it should emit size changes when panels are resized
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "02")]
public class SC02_ResizableRenders
{
    [Fact(DisplayName = "Resizable has ResizablePanelGroup, ResizablePanel, and ResizableHandle components")]
    [Trait(Spec.UAC, "06")]
    public void Resizable_HasAllRequiredComponents()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render ResizablePanelGroup with panels and handle
        var cut = ctx.Render<ResizablePanelGroup>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, "DefaultSize", 50.0);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(panelBuilder =>
                {
                    panelBuilder.OpenElement(0, "div");
                    panelBuilder.AddContent(1, "Panel 1");
                    panelBuilder.CloseElement();
                }));
                builder.CloseComponent();

                builder.OpenComponent<ResizableHandle>(3);
                builder.CloseComponent();

                builder.OpenComponent<ResizablePanel>(4);
                builder.AddAttribute(5, "DefaultSize", 50.0);
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(panelBuilder =>
                {
                    panelBuilder.OpenElement(0, "div");
                    panelBuilder.AddContent(1, "Panel 2");
                    panelBuilder.CloseElement();
                }));
                builder.CloseComponent();
            }));

        // Then - Should render all three components
        var panelGroup = cut.Find(".resizable-panel-group");
        Assert.NotNull(panelGroup);

        var panels = cut.FindAll(".resizable-panel");
        Assert.Equal(2, panels.Count);

        var handle = cut.Find(".resizable-handle");
        Assert.NotNull(handle);

        // Should contain panel content
        Assert.Contains("Panel 1", cut.Markup);
        Assert.Contains("Panel 2", cut.Markup);
    }

    [Fact(DisplayName = "Resizable supports horizontal and vertical orientations")]
    [Trait(Spec.UAC, "07")]
    public void Resizable_SupportsOrientations()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render horizontal orientation
        var horizontalCut = ctx.Render<ResizablePanelGroup>(parameters => parameters
            .Add(p => p.Orientation, ResizableOrientation.Horizontal)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, "DefaultSize", 50.0);
                builder.CloseComponent();
            }));

        // Then - Should have horizontal orientation attribute
        var horizontalGroup = horizontalCut.Find(".resizable-panel-group");
        Assert.Contains("horizontal", horizontalGroup.GetAttribute("class"));
        Assert.Equal("horizontal", horizontalGroup.GetAttribute("data-orientation"));

        // When - Render vertical orientation
        var verticalCut = ctx.Render<ResizablePanelGroup>(parameters => parameters
            .Add(p => p.Orientation, ResizableOrientation.Vertical)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, "DefaultSize", 50.0);
                builder.CloseComponent();
            }));

        // Then - Should have vertical orientation attribute
        var verticalGroup = verticalCut.Find(".resizable-panel-group");
        Assert.Contains("vertical", verticalGroup.GetAttribute("class"));
        Assert.Equal("vertical", verticalGroup.GetAttribute("data-orientation"));
    }

    [Fact(DisplayName = "Resizable supports default panel sizes")]
    [Trait(Spec.UAC, "08")]
    public void Resizable_SupportsDefaultPanelSizes()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render panels with custom default sizes
        var cut = ctx.Render<ResizablePanelGroup>(parameters => parameters
            .Add(p => p.Orientation, ResizableOrientation.Horizontal)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, "DefaultSize", 30.0);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(panelBuilder =>
                {
                    panelBuilder.AddContent(0, "Small Panel");
                }));
                builder.CloseComponent();

                builder.OpenComponent<ResizableHandle>(3);
                builder.CloseComponent();

                builder.OpenComponent<ResizablePanel>(4);
                builder.AddAttribute(5, "DefaultSize", 70.0);
                builder.AddAttribute(6, "ChildContent", (RenderFragment)(panelBuilder =>
                {
                    panelBuilder.AddContent(0, "Large Panel");
                }));
                builder.CloseComponent();
            }));

        // Then - Should render panels with default sizes
        var panels = cut.FindAll(".resizable-panel");
        Assert.Equal(2, panels.Count);

        // First panel should have 30% width
        var firstPanelStyle = panels[0].GetAttribute("style");
        Assert.NotNull(firstPanelStyle);
        Assert.Contains("width: 30%", firstPanelStyle);

        // Second panel should have 70% width
        var secondPanelStyle = panels[1].GetAttribute("style");
        Assert.NotNull(secondPanelStyle);
        Assert.Contains("width: 70%", secondPanelStyle);
    }

    [Fact(DisplayName = "Resizable emits size changes when panels are resized")]
    [Trait(Spec.UAC, "09")]
    public async Task Resizable_EmitsSizeChanges()
    {
        // Given
        using var ctx = new BunitContext();
        Dictionary<int, double>? resizeEventArgs = null;

        // When - Render with OnResize callback
        var cut = ctx.Render<ResizablePanelGroup>(parameters => parameters
            .Add(p => p.Orientation, ResizableOrientation.Horizontal)
            .Add(p => p.OnResize, EventCallback.Factory.Create<Dictionary<int, double>>(
                this, (Dictionary<int, double> args) => resizeEventArgs = args))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, "DefaultSize", 50.0);
                builder.CloseComponent();

                builder.OpenComponent<ResizableHandle>(2);
                builder.CloseComponent();

                builder.OpenComponent<ResizablePanel>(3);
                builder.AddAttribute(4, "DefaultSize", 50.0);
                builder.CloseComponent();
            }));

        // Get the component instance to simulate resize
        var instance = cut.Instance as ResizablePanelGroupBase;
        Assert.NotNull(instance);

        // Simulate a panel resize
        await instance.UpdatePanelSize(0, 60.0);

        // Then - Should emit resize event with new sizes
        Assert.NotNull(resizeEventArgs);
        Assert.Contains(0, resizeEventArgs.Keys);
        Assert.Equal(60.0, resizeEventArgs[0]);
    }
}
