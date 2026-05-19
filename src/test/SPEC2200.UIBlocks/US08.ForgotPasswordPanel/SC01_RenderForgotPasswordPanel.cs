using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US08.ForgotPasswordPanel;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "01")]
[UserStory("08", "Render ForgotPasswordPanel block")]
public class SC01_RenderForgotPasswordPanel : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>();
        cut.Find("[data-slot='forgot-password-panel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and subtitle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndSubtitle()
    {
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>(p =>
        {
            p.Add(x => x.Title, "Reset");
            p.Add(x => x.Subtitle, "Enter your email");
        });
        cut.Markup.ShouldContain("Reset");
        cut.Markup.ShouldContain("Enter your email");
    }

    [Fact(DisplayName = "UAC03: renders email input")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersEmailInput()
    {
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>();
        cut.Find("input[type='email']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: displays success message when provided")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DisplaysSuccessMessage()
    {
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>(p => p.Add(x => x.SuccessMessage, "Check your inbox"));
        cut.Find("[data-testid='forgot-password-success']").TextContent.ShouldContain("Check your inbox");
    }

    [Fact(DisplayName = "UAC05: displays error message when provided")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DisplaysErrorMessage()
    {
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>(p => p.Add(x => x.ErrorMessage, "Not found"));
        cut.Find("[data-testid='forgot-password-error']").TextContent.ShouldContain("Not found");
    }

    [Fact(DisplayName = "UAC06: invokes OnSubmit callback with email")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_InvokesOnSubmit()
    {
        string? captured = null;
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>(p =>
        {
            p.Add(x => x.OnSubmit, (string email) => captured = email);
        });

        cut.Find("input[type='email']").Change("user@example.com");
        cut.Find("button[type='submit']").Click();

        captured.ShouldBe("user@example.com");
    }

    [Fact(DisplayName = "UAC07: does not inject feature-state services")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.ForgotPasswordPanel>();
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("ForgotPasswordPanel must not expose stateful properties");
    }
}
