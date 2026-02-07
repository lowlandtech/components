using Bunit;
using Shouldly;
using SeparatorComponent = LowlandTech.Components.Separator.Separator;
using LowlandTech.Components.Separator;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Separator component rendering and orientation.
/// SC04 - Implement Separator component
/// </summarSPEC, "12
[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "04")]
public class SC04_SeparatorRenders : Scenario
{
    private TestContext _ctx = null!;
    private IRenderedComponent<SeparatorComponent>? _separator;

    protected override Task GivenAsync()
    {
        _ctx = new TestContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _separator = _ctx.Render<SeparatorComponent>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a <div> element by default")]
    [Trait(Spec.UAC, "15")]
    public async Task UAC15_ShouldRenderDivElement()
    {
        _ctx = new TestContext();

        _separator = _ctx.Render<SeparatorComponent>();
        _separator.ShouldNotBeNull();

        var div = _separator!.Find("div");
        div.TagName.ShouldBe("DIV");
    }

    [Fact(DisplayName = "And it should apply base \"separator\" class")]
    [Trait(Spec.UAC, "16")]
    public async Task UAC16_ShouldApplyBaseClass()
    {
        _ctx = new TestContext();

        _separator = _ctx.Render<SeparatorComponent>();
        _separator.ShouldNotBeNull();

        var div = _separator!.Find("div");
        div.ClassList.ShouldContain("separator");
    }

    [Fact(DisplayName = "And it should support orientation (horizontal | vertical) mapped to \"separator-horizontal\" or \"separator-vertical\"")]
    [Trait(Spec.UAC, "17")]
    public async Task UAC17_ShouldSupportOrientation()
    {
        _ctx = new TestContext();

        var horizontal = _ctx.Render<SeparatorComponent>(parameters => parameters
            .Add(p => p.Orientation, SeparatorOrientation.Horizontal)
        );
        horizontal!.Find("div").ClassList.ShouldContain("separator-horizontal");

        _separator = _ctx.Render<SeparatorComponent>(parameters => parameters
            .Add(p => p.Orientation, SeparatorOrientation.Vertical)
        );
        _separator!.Find("div").ClassList.ShouldContain("separator-vertical");
    }

    [Fact(DisplayName = "And it should be aria-hidden by default")]
    [Trait(Spec.UAC, "18")]
    public async Task UAC18_ShouldBeAriaHidden()
    {
        _ctx = new TestContext();

        _separator = _ctx.Render<SeparatorComponent>();
        _separator.ShouldNotBeNull();

        var ariaAttr = _separator!.Find("div").GetAttribute("aria-hidden");
        ariaAttr.ShouldBe("true");
    }

    [Fact(DisplayName = "And it should merge Class parameter with generated classes")]
    [Trait(Spec.UAC, "19")]
    public async Task UAC19_ShouldMergeClassParameter()
    {
        _ctx = new TestContext();

        _separator = _ctx.Render<SeparatorComponent>(parameters => parameters
            .Add(p => p.Class, "custom-class")
        );
        _separator.ShouldNotBeNull();

        var div = _separator!.Find("div");
        div.ClassList.ShouldContain("separator");
        div.ClassList.ShouldContain("custom-class");
    }
}
