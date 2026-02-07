using Bunit;
using LowlandTech.Components.Components.Input;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US03.BasicFormComponents;

/// <summary>
/// Tests for Input component functionality.
/// SC02 - Implement Input component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "02")]
public class SC02_InputRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Input>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        // Default rendering - tests will override with specific parameters
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "UAC06: Then it should render an <input> element with class 'input'")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ShouldRenderInputElementWithClass()
    {
        await GivenAsync();
        
        _component = _ctx.Render<Input>();
        
        _component.ShouldNotBeNull();
        var input = _component.Find("input");
        input.ShouldNotBeNull();
        input.ClassList.ShouldContain("input");
    }

    [Fact(DisplayName = "UAC07: And it should support a Type parameter (text, email, password, number, search)")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldSupportTypeParameter()
    {
        await GivenAsync();

        // Test text type (default)
        _component = _ctx.Render<Input>();
        _component.Find("input").GetAttribute("type").ShouldBe("text");

        // Test email type
        _component = _ctx.Render<Input>(parameters => parameters
            .Add(p => p.Type, "email"));
        _component.Find("input").GetAttribute("type").ShouldBe("email");

        // Test password type
        _component = _ctx.Render<Input>(parameters => parameters
            .Add(p => p.Type, "password"));
        _component.Find("input").GetAttribute("type").ShouldBe("password");

        // Test number type
        _component = _ctx.Render<Input>(parameters => parameters
            .Add(p => p.Type, "number"));
        _component.Find("input").GetAttribute("type").ShouldBe("number");

        // Test search type
        _component = _ctx.Render<Input>(parameters => parameters
            .Add(p => p.Type, "search"));
        _component.Find("input").GetAttribute("type").ShouldBe("search");
    }

    [Fact(DisplayName = "UAC08: And it should support a Placeholder parameter")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldSupportPlaceholder()
    {
        await GivenAsync();

        _component = _ctx.Render<Input>(parameters => parameters
            .Add(p => p.Placeholder, "Enter your name"));

        var input = _component.Find("input");
        input.GetAttribute("placeholder").ShouldBe("Enter your name");
    }

    [Fact(DisplayName = "UAC09: And it should forward unmatched attributes to the underlying element")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_ShouldForwardUnmatchedAttributes()
    {
        await GivenAsync();

        _component = _ctx.Render<Input>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "custom-input" },
                { "disabled", true },
                { "maxlength", 50 }
            }));

        var input = _component.Find("input");
        input.GetAttribute("data-testid").ShouldBe("custom-input");
        input.HasAttribute("disabled").ShouldBeTrue();
        input.GetAttribute("maxlength").ShouldBe("50");
    }
}
