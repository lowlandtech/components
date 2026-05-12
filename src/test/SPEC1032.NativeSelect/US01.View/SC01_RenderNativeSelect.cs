using Bunit;
using NativeSelectSut = LowlandTech.Components.NativeSelect.NativeSelect;

namespace LowlandTech.Components.Tests.SPEC1032.NativeSelect.US01.View;

[Trait(Spec.SPEC, "1032")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render NativeSelect component")]
public class SC01_RenderNativeSelect : ViewScenario
{
    [Fact(DisplayName = "UAC01: NativeSelect renders inner select data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot='native-select'")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<NativeSelectSut>();
        cut.Find("[data-slot='native-select']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: NativeSelect renders ChildContent inside the select")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: NativeSelect renders Class on the wrapper element")]
    [Trait(Spec.UAC, "03")]
    [Then("the wrapper element has the supplied CSS class")]
    public void UAC03_RendersClassOnWrapper()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='native-select-wrapper']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: NativeSelect default size is 'default'")]
    [Trait(Spec.UAC, "04")]
    [Then("both wrapper and select carry data-size='default'")]
    public void UAC04_DefaultSize()
    {
        var cut = Ctx.Render<NativeSelectSut>();
        cut.Find("[data-slot='native-select-wrapper']").GetAttribute("data-size").ShouldBe("default");
        cut.Find("[data-slot='native-select']").GetAttribute("data-size").ShouldBe("default");
    }

    [Fact(DisplayName = "UAC05: NativeSelect size='sm' propagates to both elements")]
    [Trait(Spec.UAC, "05")]
    [Then("wrapper and select both have data-size='sm'")]
    public void UAC05_SmSize()
    {
        var cut = Ctx.Render<NativeSelectSut>(p => p.Add(x => x.size, "sm"));
        cut.Find("[data-slot='native-select-wrapper']").GetAttribute("data-size").ShouldBe("sm");
        cut.Find("[data-slot='native-select']").GetAttribute("data-size").ShouldBe("sm");
    }

    [Fact(DisplayName = "UAC06: NativeSelect renders the chevron icon")]
    [Trait(Spec.UAC, "06")]
    [Then("a native-select-icon svg is present")]
    public void UAC06_RendersIcon()
    {
        var cut = Ctx.Render<NativeSelectSut>();
        cut.Find("[data-slot='native-select-icon']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: NativeSelect wrapper has base CSS classes")]
    [Trait(Spec.UAC, "07")]
    [Then("the wrapper contains cn-native-select-wrapper class")]
    public void UAC07_WrapperBaseClasses()
    {
        var cut = Ctx.Render<NativeSelectSut>();
        cut.Find("[data-slot='native-select-wrapper']").ClassList.ShouldContain("cn-native-select-wrapper");
    }
}
