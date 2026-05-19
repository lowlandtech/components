using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US03.LoginPanel;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "Render LoginPanel block")]
public class SC01_RenderLoginPanel : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.LoginPanel>();
        cut.Find("[data-slot='login-panel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and subtitle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndSubtitle()
    {
        var cut = Ctx.Render<Blocks.LoginPanel>(p =>
        {
            p.Add(x => x.Title, "Welcome back");
            p.Add(x => x.Subtitle, "Sign in to continue");
        });
        cut.Markup.ShouldContain("Welcome back");
        cut.Markup.ShouldContain("Sign in to continue");
    }

    [Fact(DisplayName = "UAC03: renders email and password inputs")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersInputs()
    {
        var cut = Ctx.Render<Blocks.LoginPanel>();
        cut.Find("input[type='email']").ShouldNotBeNull();
        cut.Find("input[type='password']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: renders submit button with text")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersSubmitButton()
    {
        var cut = Ctx.Render<Blocks.LoginPanel>(p => p.Add(x => x.SubmitText, "Log in"));
        cut.Find("button[type='submit']").TextContent.ShouldContain("Log in");
    }

    [Fact(DisplayName = "UAC05: displays error message when provided")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DisplaysErrorMessage()
    {
        var cut = Ctx.Render<Blocks.LoginPanel>(p => p.Add(x => x.ErrorMessage, "Invalid credentials"));
        cut.Find("[data-testid='login-panel-error']").TextContent.ShouldContain("Invalid credentials");
    }

    [Fact(DisplayName = "UAC06: invokes OnSubmit callback with credentials")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_InvokesOnSubmit()
    {
        Blocks.LoginPanel.LoginCredentials? captured = null;
        var cut = Ctx.Render<Blocks.LoginPanel>(p =>
        {
            p.Add(x => x.OnSubmit, (Blocks.LoginPanel.LoginCredentials creds) => captured = creds);
        });

        var email = cut.Find("input[type='email']");
        email.Change("test@example.com");

        var password = cut.Find("input[type='password']");
        password.Change("secret123");

        cut.Find("button[type='submit']").Click();

        captured.ShouldNotBeNull();
        captured.Email.ShouldBe("test@example.com");
        captured.Password.ShouldBe("secret123");
    }

    [Fact(DisplayName = "UAC07: does not inject feature-state services")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.LoginPanel>();
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("LoginPanel must not expose stateful properties");
    }
}
