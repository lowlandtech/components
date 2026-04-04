using Bunit;
using LowlandTech.Components.Sonner;
using LowlandTech.Components.Tests.Infrastructure;
using SonnerSut = LowlandTech.Components.Sonner.Sonner;

namespace LowlandTech.Components.Tests.SPEC1046.Sonner.US01.View;

[Trait(Spec.SPEC, "1046")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Sonner component")]
public class SC01_RenderSonner : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sonner renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SonnerSut>();
        cut.Find("[data-slot='sonner-toaster']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Sonner renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SonnerSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Sonner renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SonnerSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='sonner-toaster']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Sonner defaults to system theme and bottom-right position")]
    [Trait(Spec.UAC, "04")]
    [Then("data-theme='system' and data-position='bottom-right' by default")]
    public void UAC04_DefaultThemeAndPosition()
    {
        var cut = Ctx.Render<SonnerSut>();
        var el = cut.Find("[data-slot='sonner-toaster']");
        el.GetAttribute("data-theme").ShouldBe("system");
        el.GetAttribute("data-position").ShouldBe("bottom-right");
    }

    [Fact(DisplayName = "UAC05: Sonner Theme and Position parameters propagate")]
    [Trait(Spec.UAC, "05")]
    [Then("custom Theme and Position are reflected in data attributes")]
    public void UAC05_ThemeAndPositionParameters()
    {
        var cut = Ctx.Render<SonnerSut>(p => p
            .Add(x => x.Theme, "dark")
            .Add(x => x.Position, "top-center"));
        var el = cut.Find("[data-slot='sonner-toaster']");
        el.GetAttribute("data-theme").ShouldBe("dark");
        el.GetAttribute("data-position").ShouldBe("top-center");
    }

    [Fact(DisplayName = "UAC06: Sonner renders toast when service has an active toast")]
    [Trait(Spec.UAC, "06")]
    [Then("a sonner-toast element is present with the toast message")]
    public void UAC06_RendersActiveToast()
    {
        var service = Ctx.Services.GetRequiredService<ISonnerService>();
        service.Show("Test message", ToastType.Success);

        var cut = Ctx.Render<SonnerSut>();
        var toast = cut.Find("[data-slot='sonner-toast']");
        toast.ShouldNotBeNull();
        toast.GetAttribute("data-type").ShouldBe("success");
        cut.Markup.ShouldContain("Test message");
    }

    [Fact(DisplayName = "UAC07: Sonner CSS custom properties for theming are present")]
    [Trait(Spec.UAC, "07")]
    [Then("the toaster element has inline CSS variables for theming")]
    public void UAC07_CssVariablesPresent()
    {
        var cut = Ctx.Render<SonnerSut>();
        cut.Find("[data-slot='sonner-toaster']").GetAttribute("style").ShouldContain("--normal-bg");
    }
}
