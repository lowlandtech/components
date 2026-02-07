using Bunit;
using LowlandTech.Components.Components.NativeSelect;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for NativeSelect component rendering and functionality.
/// SC09 - Implement NativeSelect component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "09")]
public class SC09_NativeSelectRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a <select> element with class \"input\"")]
    [Trait(Spec.UAC, "28")]
    public async Task UAC28_ShouldRenderSelectElement()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<NativeSelect>(parameters => parameters
            .Add(p => p.ChildContent, "<option>Option 1</option>")
        );
        var selectElement = component.Find("select");
        selectElement.ShouldNotBeNull();
        selectElement.ClassList.ShouldContain("input");
    }

    [Fact(DisplayName = "And it should forward unmatched attributes to the underlying element")]
    [Trait(Spec.UAC, "29")]
    public async Task UAC29_ShouldForwardAttributes()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<NativeSelect>(parameters => parameters
            .Add(p => p.ChildContent, "<option>Option</option>")
            .AddUnmatched("data-testid", "my-select")
            .AddUnmatched("aria-label", "Select an option")
        );
        var selectElement = component.Find("select");
        selectElement.GetAttribute("data-testid").ShouldBe("my-select");
        selectElement.GetAttribute("aria-label").ShouldBe("Select an option");
    }

    [Fact(DisplayName = "And it should support a Disabled state")]
    [Trait(Spec.UAC, "30")]
    public async Task UAC30_ShouldSupportDisabled()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<NativeSelect>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ChildContent, "<option>Option</option>")
        );
        var selectElement = component.Find("select");
        selectElement.HasAttribute("disabled").ShouldBeTrue();
    }
}
