using Bunit;
using LowlandTech.Components.Components.Stack;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US10.MudComponents;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "01")]
public class SC01_StackRenders : Scenario
{
    private TestContext _ctx = null!;
    private IRenderedComponent<Stack>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new TestContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Stack>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a div element with stack classes")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldRenderStackDiv()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();

        var element = _component!.Find("div");
        element.ShouldNotBeNull();
        element.ClassList.ShouldContain("stack");
    }

    [Fact(DisplayName = "Then it should apply stack-col class by default")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldApplyColumnDirectionByDefault()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();

        var element = _component!.Find(".stack");
        element.ClassList.ShouldContain("stack-col");
    }

    [Fact(DisplayName = "Then it should apply stack-row class when Row is true")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ShouldApplyRowDirectionWhenRowTrue()
    {
        using var ctx = new TestContext();
        var component = ctx.Render<Stack>(parameters => parameters
            .Add(p => p.Row, true)
        );

        var element = component.Find(".stack");
        element.ClassList.ShouldContain("stack-row");
        element.ClassList.ShouldNotContain("stack-col");
    }

    [Fact(DisplayName = "Then it should apply default gap-2 class")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldApplyDefaultGap()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();

        var element = _component!.Find(".stack");
        element.ClassList.ShouldContain("gap-2");
    }

    [Fact(DisplayName = "Then it should apply custom spacing gap class")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldApplyCustomSpacing()
    {
        using var ctx = new TestContext();
        var component = ctx.Render<Stack>(parameters => parameters
            .Add(p => p.Spacing, 4)
        );

        var element = component.Find(".stack");
        element.ClassList.ShouldContain("gap-4");
    }

    [Fact(DisplayName = "Then it should merge custom Class parameter")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ShouldMergeCustomClass()
    {
        using var ctx = new TestContext();
        var component = ctx.Render<Stack>(parameters => parameters
            .Add(p => p.Class, "custom-class")
        );

        var element = component.Find(".stack");
        element.ClassList.ShouldContain("custom-class");
        element.ClassList.ShouldContain("stack");
    }

    [Fact(DisplayName = "Then it should forward AdditionalAttributes")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldForwardAdditionalAttributes()
    {
        using var ctx = new TestContext();
        var component = ctx.Render<Stack>(parameters => parameters
            .AddUnmatched("data-testid", "my-stack")
        );

        var element = component.Find("[data-testid='my-stack']");
        element.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Then it should render child content")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldRenderChildContent()
    {
        using var ctx = new TestContext();
        var component = ctx.Render<Stack>(parameters => parameters
            .AddChildContent("<div class=\"child-item\">Item 1</div><div class=\"child-item\">Item 2</div>")
        );

        var children = component.FindAll(".child-item");
        children.Count.ShouldBe(2);
        children[0].TextContent.ShouldBe("Item 1");
        children[1].TextContent.ShouldBe("Item 2");
    }
}
