using Bunit;
using ProgressSut = LowlandTech.Components.Progress;

namespace LowlandTech.Components.Tests.SPEC1036.Progress.US01.View;

[Trait(Spec.SPEC, "1036")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Progress component")]
public class SC01_RenderProgress : ViewScenario
{
    [Fact(DisplayName = "UAC01: Progress renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-slot='progress']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Progress renders cn-progress class")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersCnProgressClass()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-slot='progress']").ClassList.ShouldContain("cn-progress");
    }

    [Fact(DisplayName = "UAC03: Progress renders role='progressbar'")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersProgressbarRole()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[role='progressbar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Progress renders with default Max value 100")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithDefaultMax()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Instance.Max.ShouldBe(100);
        cut.Find("[data-slot='progress']").GetAttribute("data-max").ShouldBe("100");
    }

    [Fact(DisplayName = "UAC05: Progress Max parameter can be customized")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_MaxCanBeCustomized()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.Max, 200));
        cut.Instance.Max.ShouldBe(200);
        cut.Find("[data-slot='progress']").GetAttribute("data-max").ShouldBe("200");
    }

    [Fact(DisplayName = "UAC06: Progress Value parameter sets data-value attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ValueSetsDataValue()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.Value, 50));
        cut.Find("[data-slot='progress']").GetAttribute("data-value").ShouldBe("50");
    }

    [Fact(DisplayName = "UAC07: Progress renders aria-valuenow with Value")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersAriaValueNow()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.Value, 75));
        cut.Find("[aria-valuenow='75']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: Progress renders aria-valuemin as 0")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersAriaValueMin()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[aria-valuemin='0']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: Progress renders aria-valuemax with Max value")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RendersAriaValueMax()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.Max, 150));
        cut.Find("[aria-valuemax='150']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: Progress renders Class parameter")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_RendersClass()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.Class, "custom-progress"));
        cut.Find("[data-slot='progress']").ClassList.ShouldContain("custom-progress");
    }

    [Fact(DisplayName = "UAC11: Progress renders AdditionalAttributes")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_RendersAdditionalAttributes()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
            p.Add(x => x.AdditionalAttributes, new Dictionary<string, object> { { "data-test", "test-value" } }));
        cut.Find("[data-test='test-value']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: Progress indicator renders data-slot attribute")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_IndicatorRendersDataSlot()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-slot='progress-indicator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: Progress indicator has cn-progress-indicator class")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_IndicatorHasClass()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-slot='progress-indicator']").ClassList.ShouldContain("cn-progress-indicator");
    }

    #region Indeterminate State Tests

    [Fact(DisplayName = "UAC14: Progress IsIndeterminate parameter renders indeterminate state")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_IndeterminateRendersCorrectState()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.IsIndeterminate, true));
        cut.Instance.IsIndeterminate.ShouldBeTrue();
        cut.Find("[data-state='indeterminate']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: Progress indeterminate renders aria-busy='true'")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_IndeterminateRendersAriaBusy()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.IsIndeterminate, true));
        cut.Find("[aria-busy='true']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC16: Progress indeterminate does not render aria-valuenow")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_IndeterminateDoesNotRenderValueNow()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.IsIndeterminate, true);
            p.Add(x => x.Value, 50);
        });
        cut.Find("[data-slot='progress']").HasAttribute("aria-valuenow").ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC17: Progress indeterminate renders aria-valuetext as 'Loading...'")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_IndeterminateRendersLoadingText()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.IsIndeterminate, true));
        cut.Find("[aria-valuetext='Loading...']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC18: Progress indeterminate adds cn-progress-indeterminate class")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_IndeterminateAddsClass()
    {
        var cut = Ctx.Render<ProgressSut>(p => p.Add(x => x.IsIndeterminate, true));
        cut.Find("[data-slot='progress']").ClassList.ShouldContain("cn-progress-indeterminate");
    }

    #endregion

    #region Value Calculation Tests

    [Fact(DisplayName = "UAC19: Progress calculates percentage correctly")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_CalculatesPercentage()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.Value, 25);
            p.Add(x => x.Max, 100);
        });
        cut.Find("[data-slot='progress']").GetAttribute("data-value").ShouldBe("25");
    }

    [Fact(DisplayName = "UAC20: Progress calculates percentage with custom Max")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_CalculatesPercentageWithCustomMax()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.Value, 50);
            p.Add(x => x.Max, 200);
        });
        cut.Find("[data-slot='progress']").GetAttribute("data-value").ShouldBe("25");
    }

    [Fact(DisplayName = "UAC21: Progress clamps value to 0 minimum")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_ClampsValueToZero()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.Value, -10);
            p.Add(x => x.Max, 100);
        });
        cut.Find("[data-slot='progress']").GetAttribute("data-value").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC22: Progress clamps value to 100 maximum")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_ClampsValueToMax()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.Value, 150);
            p.Add(x => x.Max, 100);
        });
        cut.Find("[data-slot='progress']").GetAttribute("data-value").ShouldBe("100");
    }

    [Fact(DisplayName = "UAC23: Progress renders complete state when Value equals Max")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_RendersCompleteState()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.Value, 100);
            p.Add(x => x.Max, 100);
        });
        cut.Find("[data-state='complete']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC24: Progress renders loading state when Value is less than Max")]
    [Trait(Spec.UAC, "24")]
    public void UAC24_RendersLoadingState()
    {
        var cut = Ctx.Render<ProgressSut>(p => 
        {
            p.Add(x => x.Value, 50);
            p.Add(x => x.Max, 100);
        });
        cut.Find("[data-state='loading']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC25: Progress renders loading state when Value is null")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_RendersLoadingStateWhenNull()
    {
        var cut = Ctx.Render<ProgressSut>();
        cut.Find("[data-state='loading']").ShouldNotBeNull();
    }

    #endregion
}
