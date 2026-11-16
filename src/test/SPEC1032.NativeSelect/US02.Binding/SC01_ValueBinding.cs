using Bunit;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using NativeSelectSut = LowlandTech.Components.NativeSelect;

namespace LowlandTech.Components.Tests.SPEC1032.NativeSelect.US02.Binding;

/// <summary>
/// US02 — Two-way value binding.
///
/// NativeSelect joins the controls that take part in <c>@bind-Value</c>: a caller sets the
/// selected value, the control writes the value the user chose, and it raises the change so a
/// binding can flow back.
/// </summary>
[Trait(Spec.SPEC, "1032")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Two-way value binding")]
public class SC01_ValueBinding : ViewScenario
{
    private const string Options =
        "<option value='a'>A</option><option value='b'>B</option>";

    [Fact(DisplayName = "UAC01: NativeSelect renders the value its caller set")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered select carries the supplied value")]
    public void UAC01_RendersSuppliedValue()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p
            .Add(x => x.Value, "b")
            .AddChildContent(Options));

        cut.Find("[data-slot='native-select']").GetAttribute("value").ShouldBe("b");
    }

    [Fact(DisplayName = "UAC02: a selection raises ValueChanged with the new value")]
    [Trait(Spec.UAC, "02")]
    [Then("the caller receives the newly selected value")]
    public void UAC02_ChangeRaisesValueChanged()
    {
        string? captured = null;

        var cut = Ctx.Render<NativeSelectSut>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, value => captured = value))
            .AddChildContent(Options));

        cut.Find("[data-slot='native-select']").Change("b");

        captured.ShouldBe("b");
    }

    [Fact(DisplayName = "UAC03: a selection writes the value the control shows")]
    [Trait(Spec.UAC, "03")]
    [Then("Value holds the new selection after the change")]
    public void UAC03_ChangeWritesValue()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p
            .Add(x => x.Value, "a")
            .AddChildContent(Options));

        cut.Find("[data-slot='native-select']").Change("b");

        cut.Instance.Value.ShouldBe("b");
    }

    [Fact(DisplayName = "UAC04: @bind-Value flows the selection back to the caller's field")]
    [Trait(Spec.UAC, "04")]
    [Then("the bound field holds the newly selected value")]
    public void UAC04_BindingFlowsBack()
    {
        var cut = Ctx.Render<BindingHost>();

        cut.Find("[data-slot='native-select']").Change("b");

        cut.Instance.Bound.ShouldBe("b");
    }

    /// <summary>The smallest caller that binds the control the way a page does.</summary>
    private sealed class BindingHost : ComponentBase
    {
        public string? Bound { get; private set; } = "a";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<NativeSelectSut>(0);
            builder.AddAttribute(1, nameof(NativeSelectSut.Value), Bound);
            builder.AddAttribute(2, nameof(NativeSelectSut.ValueChanged), EventCallback.Factory.Create<string?>(this, value =>
            {
                Bound = value;
                StateHasChanged();
            }));
            builder.AddAttribute(3, nameof(NativeSelectSut.ChildContent), (RenderFragment)(content =>
                content.AddMarkupContent(4, Options)));
            builder.CloseComponent();
        }
    }
}
