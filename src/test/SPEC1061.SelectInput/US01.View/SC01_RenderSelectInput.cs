using Bunit;
using LowlandTech.Components;
using LowlandTech.Components.SelectInput;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1061.SelectInput.US01.View;

[Trait(Spec.SPEC, "1061")]
[Trait(Spec.US, "01")]
[UserStory("01", "SelectInput component rendering")]
public class SC01_RenderSelectInput : ViewScenario
{
    [Fact(DisplayName = "UAC01: SelectInput renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SelectInputRendersWithDataSlot()
    {
        var cut = Ctx.Render<SelectInput<string>>();
        cut.Find("[data-slot='select-input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SelectInput renders trigger button")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SelectInputRendersTriggerButton()
    {
        var cut = Ctx.Render<SelectInput<string>>();
        var trigger = cut.Find("[data-slot='select-trigger']");
        trigger.ShouldNotBeNull();
        trigger.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC03: SelectInput renders placeholder when no value")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectInputRendersPlaceholder()
    {
        var cut = Ctx.Render<SelectInput<string>>(p => p.Add(x => x.Placeholder, "Choose..."));
        cut.Find(".select-value").TextContent.ShouldContain("Choose...");
    }

    [Fact(DisplayName = "UAC04: SelectInput content is hidden by default")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SelectInputContentHiddenByDefault()
    {
        var cut = Ctx.Render<SelectInput<string>>();
        cut.FindAll("[data-slot='select-content']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: SelectInput renders options when open")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SelectInputRendersOptionsWhenOpen()
    {
        var options = new List<SelectOption<string>>
        {
            new("a", "Option A"),
            new("b", "Option B")
        };

        var cut = Ctx.Render<SelectInput<string>>(p => p
            .Add(x => x.Options, options)
        );

        // Click trigger to open
        cut.Find("[data-slot='select-trigger']").Click();

        var items = cut.FindAll("[data-slot='select-item']");
        items.Count.ShouldBe(2);
        items[0].TextContent.ShouldContain("Option A");
        items[1].TextContent.ShouldContain("Option B");
    }

    [Fact(DisplayName = "UAC06: SelectInput shows selected value")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SelectInputShowsSelectedValue()
    {
        var options = new List<SelectOption<string>>
        {
            new("a", "Option A"),
            new("b", "Option B")
        };

        var cut = Ctx.Render<SelectInput<string>>(p => p
            .Add(x => x.Options, options)
            .Add(x => x.Value, "b")
        );

        cut.Find(".select-value").TextContent.ShouldContain("Option B");
    }

    [Fact(DisplayName = "UAC07: SelectInput item has selected class when selected")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_SelectInputItemHasSelectedClass()
    {
        var options = new List<SelectOption<string>>
        {
            new("a", "Option A"),
            new("b", "Option B")
        };

        var cut = Ctx.Render<SelectInput<string>>(p => p
            .Add(x => x.Options, options)
            .Add(x => x.Value, "b")
        );

        cut.Find("[data-slot='select-trigger']").Click();

        var items = cut.FindAll("[data-slot='select-item']");
        items[0].ClassList.ShouldNotContain("selected");
        items[1].ClassList.ShouldContain("selected");
    }
}
