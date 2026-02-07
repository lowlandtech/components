using Bunit;
using LowlandTech.Components.Components.Alert;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Alert component rendering and base functionality.
/// SC05 - Implement Alert components
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "05")]
public class SC05_AlertRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Alert>? _alertComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _alertComponent = _ctx.Render<Alert>(parameters => parameters
            .Add(p => p.ChildContent, "Alert content")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have Alert, AlertTitle, and AlertDescription components")]
    [Trait(Spec.UAC, "18")]
    public async Task UAC18_ShouldHaveAllAlertComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // Alert
        var alert = ctx.Render<Alert>(parameters => parameters
            .Add(p => p.ChildContent, "Test"));
        alert.ShouldNotBeNull();
        
        // AlertTitle
        var title = ctx.Render<AlertTitle>(parameters => parameters
            .Add(p => p.ChildContent, "Title"));
        title.ShouldNotBeNull();
        
        // AlertDescription
        var description = ctx.Render<AlertDescription>(parameters => parameters
            .Add(p => p.ChildContent, "Description"));
        description.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And Alert should render a wrapper with class \"alert\"")]
    [Trait(Spec.UAC, "19")]
    public async Task UAC19_AlertShouldHaveAlertClass()
    {
        await ArrangeAndActAsync();
        
        var alert = _alertComponent!.Find("div");
        alert.ClassList.ShouldContain("alert");
    }

    [Fact(DisplayName = "And AlertTitle and AlertDescription should render wrappers with classes \"alert-title\" and \"alert-description\"")]
    [Trait(Spec.UAC, "20")]
    public async Task UAC20_SubcomponentsShouldHaveRespectiveClasses()
    {
        using var ctx = new BunitContext();
        
        // Test AlertTitle
        var title = ctx.Render<AlertTitle>(parameters => parameters
            .Add(p => p.ChildContent, "Title"));
        title.Find("h5").ClassList.ShouldContain("alert-title");
        
        // Test AlertDescription
        var description = ctx.Render<AlertDescription>(parameters => parameters
            .Add(p => p.ChildContent, "Description"));
        description.Find("div").ClassList.ShouldContain("alert-description");
    }

    [Fact(DisplayName = "And Alert should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "21")]
    public async Task UAC21_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Alert>(parameters => parameters
            .Add(p => p.Class, "custom-alert-class")
        );

        var alert = component.Find("div");
        alert.ClassList.ShouldContain("alert");
        alert.ClassList.ShouldContain("custom-alert-class");
    }
}
