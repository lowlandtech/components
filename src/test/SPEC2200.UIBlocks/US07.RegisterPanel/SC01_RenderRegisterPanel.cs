using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US07.RegisterPanel;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "01")]
[UserStory("07", "Render RegisterPanel block")]
public class SC01_RenderRegisterPanel : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.RegisterPanel>();
        cut.Find("[data-slot='register-panel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and subtitle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndSubtitle()
    {
        var cut = Ctx.Render<Blocks.RegisterPanel>(p =>
        {
            p.Add(x => x.Title, "Sign up");
            p.Add(x => x.Subtitle, "Create an account");
        });
        cut.Markup.ShouldContain("Sign up");
        cut.Markup.ShouldContain("Create an account");
    }

    [Fact(DisplayName = "UAC03: renders name, email and password inputs")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersInputs()
    {
        var cut = Ctx.Render<Blocks.RegisterPanel>();
        cut.Find("input[type='text']").ShouldNotBeNull();
        cut.Find("input[type='email']").ShouldNotBeNull();
        cut.Find("input[type='password']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: renders submit button with text")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersSubmitButton()
    {
        var cut = Ctx.Render<Blocks.RegisterPanel>(p => p.Add(x => x.SubmitText, "Join"));
        cut.Find("button[type='submit']").TextContent.ShouldContain("Join");
    }

    [Fact(DisplayName = "UAC05: displays error message when provided")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DisplaysErrorMessage()
    {
        var cut = Ctx.Render<Blocks.RegisterPanel>(p => p.Add(x => x.ErrorMessage, "Email taken"));
        cut.Find("[data-testid='register-panel-error']").TextContent.ShouldContain("Email taken");
    }

    [Fact(DisplayName = "UAC06: invokes OnSubmit callback with credentials")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_InvokesOnSubmit()
    {
        Blocks.RegisterPanel.RegisterCredentials? captured = null;
        var cut = Ctx.Render<Blocks.RegisterPanel>(p =>
        {
            p.Add(x => x.OnSubmit, (Blocks.RegisterPanel.RegisterCredentials creds) => captured = creds);
        });

        await cut.InvokeAsync(() => cut.Find("input[type='text']").Change("Jane Doe"));
        await cut.InvokeAsync(() => cut.Find("input[type='email']").Change("jane@example.com"));
        await cut.InvokeAsync(() => cut.Find("input[type='password']").Change("secret123"));

        await cut.InvokeAsync(() => cut.Find("button[type='submit']").Click());

        captured.ShouldNotBeNull();
        captured.Name.ShouldBe("Jane Doe");
        captured.Email.ShouldBe("jane@example.com");
        captured.Password.ShouldBe("secret123");
    }

    [Fact(DisplayName = "UAC07: does not inject feature-state services")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.RegisterPanel>();
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("RegisterPanel must not expose stateful properties");
    }
}
