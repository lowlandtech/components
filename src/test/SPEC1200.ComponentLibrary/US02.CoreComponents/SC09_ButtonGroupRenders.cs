using Bunit;
using LowlandTech.Components.Components.ButtonGroup;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for ButtonGroup component rendering and base functionality.
/// SC09 - Implement ButtonGroup components
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "09")]
public class SC09_ButtonGroupRenders : Scenario
{
    private BunitContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then I should have ButtonGroup, ButtonGroupText, and ButtonGroupSeparator components")]
    [Trait(Spec.UAC, "32")]
    public async Task UAC32_ShouldHaveAllButtonGroupComponents()
    {
        // Test that all components can be instantiated
        using var ctx = new BunitContext();
        
        // ButtonGroup
        var group = ctx.Render<ButtonGroup>(parameters => parameters
            .Add(p => p.ChildContent, "Test"));
        group.ShouldNotBeNull();
        
        // ButtonGroupText
        var text = ctx.Render<ButtonGroupText>(parameters => parameters
            .Add(p => p.ChildContent, "Text"));
        text.ShouldNotBeNull();
        
        // ButtonGroupSeparator
        var separator = ctx.Render<ButtonGroupSeparator>();
        separator.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And ButtonGroup should render a wrapper with role=\"group\" and base group class")]
    [Trait(Spec.UAC, "33")]
    public async Task UAC33_ButtonGroupShouldHaveRoleAndClass()
    {
        using var ctx = new BunitContext();
        var group = ctx.Render<ButtonGroup>(parameters => parameters
            .Add(p => p.ChildContent, "Content"));
        
        var div = group.Find("div");
        div.ClassList.ShouldContain("button-group");
        div.GetAttribute("role").ShouldBe("group");
    }

    [Fact(DisplayName = "And it should support horizontal and vertical orientations")]
    [Trait(Spec.UAC, "34")]
    public async Task UAC34_ShouldSupportOrientations()
    {
        // Test horizontal (default)
        using var ctx1 = new BunitContext();
        var horizontal = ctx1.Render<ButtonGroup>(parameters => parameters
            .Add(p => p.Orientation, ButtonGroupOrientation.Horizontal)
            .Add(p => p.ChildContent, "Content"));
        horizontal.Find("div").ClassList.ShouldContain("button-group");
        horizontal.Find("div").ClassList.ShouldNotContain("button-group-vertical");

        // Test vertical
        using var ctx2 = new BunitContext();
        var vertical = ctx2.Render<ButtonGroup>(parameters => parameters
            .Add(p => p.Orientation, ButtonGroupOrientation.Vertical)
            .Add(p => p.ChildContent, "Content"));
        vertical.Find("div").ClassList.ShouldContain("button-group");
        vertical.Find("div").ClassList.ShouldContain("button-group-vertical");
    }
}
