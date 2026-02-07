using Bunit;
using LowlandTech.Components.Components.Accordion;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Tests.SPEC1227.ComponentLibrary.US06.NavigationComponents;

[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "02")]
public class SC02_Accordion : Scenario
{
    private TestContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new TestContext();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "And it should support single and multiple expansion modes")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldSupportMultipleModes()
    {
        using var ctx1 = new TestContext();
        var accordion1 = ctx1.Render<Accordion>(p => p.Add(x => x.OneOpen, true));
        accordion1.Instance.OneOpen.ShouldBeTrue();

        using var ctx2 = new TestContext();
        var accordion2 = ctx2.Render<Accordion>(p => p.Add(x => x.OneOpen, false));
        accordion2.Instance.OneOpen.ShouldBeFalse();

        using var ctx3 = new TestContext();
        var accordion3 = ctx3.Render<Accordion>(p => p.Add(x => x.Collapsible, true));
        accordion3.Instance.Collapsible.ShouldBeTrue();

        using var ctx4 = new TestContext();
        var accordion4 = ctx4.Render<Accordion>(p => p.Add(x => x.Collapsible, false));
        accordion4.Instance.Collapsible.ShouldBeFalse();
    }

    [Fact(DisplayName = "And it should support default open items")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldSupportDefaultOpenItems()
    {
        using var ctx = new TestContext();
        var component = ctx.Render<Accordion>(p => p.Add(x => x.DefaultOpenValue, 1));
        component.ShouldNotBeNull();
        component.Instance.DefaultOpenValue.HasValue.ShouldBeTrue();
        component.Instance.DefaultOpenValue.ShouldBe(1);
    }
}
