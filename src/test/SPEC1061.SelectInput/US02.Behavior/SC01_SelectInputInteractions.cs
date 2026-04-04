using Bunit;
using Microsoft.AspNetCore.Components;
using LowlandTech.Components;
using LowlandTech.Components.SelectInput;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1061.SelectInput.US02.Behavior;

[Trait(Spec.SPEC, "1061")]
[Trait(Spec.US, "02")]
[UserStory("02", "SelectInput component behavior")]
public class SC01_SelectInputInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking trigger opens dropdown")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ClickingTriggerOpensDropdown()
    {
        var cut = Ctx.Render<SelectInput<string>>();
        
        cut.FindAll("[data-slot='select-content']").Count.ShouldBe(0);
        
        cut.Find("[data-slot='select-trigger']").Click();
        
        cut.Find("[data-slot='select-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Clicking trigger again closes dropdown")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ClickingTriggerAgainClosesDropdown()
    {
        var cut = Ctx.Render<SelectInput<string>>();
        
        var trigger = cut.Find("[data-slot='select-trigger']");
        trigger.Click();
        cut.Find("[data-slot='select-content']").ShouldNotBeNull();
        
        trigger.Click();
        cut.FindAll("[data-slot='select-content']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Selecting option triggers ValueChanged")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_SelectingOptionTriggersValueChanged()
    {
        var selectedValue = "";
        var options = new List<SelectOption<string>>
        {
            new("a", "Option A"),
            new("b", "Option B")
        };

        var cut = Ctx.Render<SelectInput<string>>(p => p
            .Add(x => x.Options, options)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => selectedValue = v))
        );

        cut.Find("[data-slot='select-trigger']").Click();
        await cut.FindAll("[data-slot='select-item']")[1].ClickAsync();

        selectedValue.ShouldBe("b");
    }

    [Fact(DisplayName = "UAC04: Selecting option closes dropdown")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SelectingOptionClosesDropdown()
    {
        var options = new List<SelectOption<string>>
        {
            new("a", "Option A"),
            new("b", "Option B")
        };

        var cut = Ctx.Render<SelectInput<string>>(p => p.Add(x => x.Options, options));

        cut.Find("[data-slot='select-trigger']").Click();
        cut.Find("[data-slot='select-content']").ShouldNotBeNull();

        cut.FindAll("[data-slot='select-item']")[0].Click();
        cut.FindAll("[data-slot='select-content']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: Clicking backdrop closes dropdown")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ClickingBackdropClosesDropdown()
    {
        var cut = Ctx.Render<SelectInput<string>>();

        cut.Find("[data-slot='select-trigger']").Click();
        cut.Find("[data-slot='select-content']").ShouldNotBeNull();

        cut.Find("[data-slot='select-backdrop']").Click();
        cut.FindAll("[data-slot='select-content']").Count.ShouldBe(0);
    }
}
