using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ThemeProvider = LowlandTech.Components.ThemeProvider.ThemeProvider;

namespace LowlandTech.Components.Tests.SPEC1071.ComponentThemes.US04.ProviderIsolation;

// ============================================================
//  SC01 – Sibling providers each render their own style
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "Provider Isolation")]
public class SC01_SiblingProvidersHaveIndependentStyles : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC01: tp-a style contains Ocean '--background:#f0f9ff'")]
    [Trait(Spec.UAC, "01")]
    [Then("tp-a should have Ocean background token in its style")]
    public void UAC01_TpAHasOceanBackground()
    {
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        cutA.Find("style").TextContent.ShouldContain("--background:#f0f9ff");
    }

    [Fact(DisplayName = "UAC02: tp-b style does not contain Ocean '--background:#f0f9ff'")]
    [Trait(Spec.UAC, "02")]
    [Then("tp-b should not have Ocean background token in its style")]
    public void UAC02_TpBDoesNotHaveOceanBackground()
    {
        var cutB = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-b")
            .Add(x => x.Theme, "Rose"));
        cutB.Find("style").TextContent.ShouldNotContain("--background:#f0f9ff");
    }

    [Fact(DisplayName = "UAC03: Both sibling providers are simultaneously active")]
    [Trait(Spec.UAC, "03")]
    [Then("both sibling providers should have distinct token styles")]
    public void UAC03_BothSiblingsActiveSimultaneously()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var cutB = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-b")
            .Add(x => x.Theme, "Rose"));
        service.GetNameForProvider("tp-a").ShouldBe("Ocean");
        service.GetNameForProvider("tp-b").ShouldBe("Rose");
    }

    [Fact(DisplayName = "UAC04-05: Each sibling provider has its own data-theme-provider attribute")]
    [Trait(Spec.UAC, "04")]
    [Then("each sibling should have its own data-theme-provider id")]
    public void UAC04_05_EachSiblingHasOwnId()
    {
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var cutB = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-b")
            .Add(x => x.Theme, "Rose"));
        cutA.Find("[data-theme-provider='tp-a']").ShouldNotBeNull();
        cutB.Find("[data-theme-provider='tp-b']").ShouldNotBeNull();
    }
}

// ============================================================
//  SC02 – Cycling tp-a does not affect tp-b
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "02")]
[UserStory("04", "Provider Isolation")]
public class SC02_CyclingOneSiblingLeavesOtherIntact : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC06: tp-a advances after Next; tp-b stays 'Rose'")]
    [Trait(Spec.UAC, "06")]
    [Then("tp-b should still be Rose after tp-a advances")]
    public void UAC06_TpBUnchangedWhenTpAAdvances()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-a", "Ocean");
        service.Register("tp-b", "Rose");
        service.Next("tp-a");
        service.GetNameForProvider("tp-b").ShouldBe("Rose");
    }

    [Fact(DisplayName = "UAC07: tp-a advances to 'Rose' after Next from 'Ocean'")]
    [Trait(Spec.UAC, "07")]
    [Then("tp-a should be 'Rose' after cycling from 'Ocean'")]
    public void UAC07_TpAAdvancesToRose()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-a", "Ocean");
        service.Register("tp-b", "Rose");
        service.Next("tp-a");
        service.GetNameForProvider("tp-a").ShouldBe("Rose");
    }

    [Fact(DisplayName = "UAC08: Style for tp-b unchanged in rendered output")]
    [Trait(Spec.UAC, "08")]
    [Then("tp-b style content should remain unchanged after tp-a is cycled")]
    public void UAC08_TpBStyleUnchanged()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var cutB = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-b")
            .Add(x => x.Theme, "Rose"));

        var styleBefore = cutB.Find("style").TextContent;
        service.Next("tp-a");
        cutB.Find("style").TextContent.ShouldBe(styleBefore);
    }

    [Fact(DisplayName = "UAC09: Style for tp-a updates in rendered output")]
    [Trait(Spec.UAC, "09")]
    [Then("tp-a style should reflect the new theme after cycling")]
    public void UAC09_TpAStyleUpdates()
    {
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();

        service.Next("tp-a"); // Ocean → Rose
        cutA.WaitForState(() => !cutA.Find("style").TextContent.Contains("--background:#f0f9ff"));
        cutA.Find("style").TextContent.ShouldNotContain("--background:#f0f9ff");
    }
}

// ============================================================
//  SC03 – Nested providers each have their own style elements
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "03")]
[UserStory("04", "Provider Isolation")]
public class SC03_NestedProvidersHaveIndependentStyles : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC10: Outer provider (Forest) has its own style element")]
    [Trait(Spec.UAC, "10")]
    [Then("the outer provider should have a style element with Forest tokens")]
    public void UAC10_OuterProviderHasStyle()
    {
        var cut = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-outer")
            .Add(x => x.Theme, "Forest")
            .AddChildContent<ThemeProvider>(inner => inner
                .Add(x => x.Id, "tp-inner")
                .Add(x => x.Theme, "Dark")));
        // Both providers emit <style> elements; FindAll to count
        cut.FindAll("style").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC11: Inner provider (Dark) has its own style element")]
    [Trait(Spec.UAC, "11")]
    [Then("the inner provider should have a style element with Dark tokens")]
    public void UAC11_InnerProviderHasStyle()
    {
        var cut = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-outer")
            .Add(x => x.Theme, "Forest")
            .AddChildContent<ThemeProvider>(inner => inner
                .Add(x => x.Id, "tp-inner")
                .Add(x => x.Theme, "Dark")));
        var styles = cut.FindAll("style");
        // One style should be scoped to tp-inner
        styles.ShouldContain(s => s.TextContent.Contains("[data-theme-provider=\"tp-inner\"]"));
    }

    [Fact(DisplayName = "UAC12: Both providers register independently in the service")]
    [Trait(Spec.UAC, "12")]
    [Then("service should know both tp-outer and tp-inner independently")]
    public void UAC12_BothProvidersRegisteredIndependently()
    {
        var cut = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-outer")
            .Add(x => x.Theme, "Forest")
            .AddChildContent<ThemeProvider>(inner => inner
                .Add(x => x.Id, "tp-inner")
                .Add(x => x.Theme, "Dark")));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.GetNameForProvider("tp-outer").ShouldBe("Forest");
        service.GetNameForProvider("tp-inner").ShouldBe("Dark");
    }

    [Fact(DisplayName = "UAC13: Outer style scoped to [data-theme-provider='tp-outer']")]
    [Trait(Spec.UAC, "13")]
    [Then("outer style element should be scoped to tp-outer")]
    public void UAC13_OuterStyleScopedCorrectly()
    {
        var cut = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-outer")
            .Add(x => x.Theme, "Forest")
            .AddChildContent<ThemeProvider>(inner => inner
                .Add(x => x.Id, "tp-inner")
                .Add(x => x.Theme, "Dark")));
        var styles = cut.FindAll("style");
        styles.ShouldContain(s => s.TextContent.Contains("[data-theme-provider=\"tp-outer\"]"));
    }
}

// ============================================================
//  SC04 – Cycling the inner provider leaves the outer unchanged
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "04")]
[UserStory("04", "Provider Isolation")]
public class SC04_CyclingInnerDoesNotAffectOuter : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC14: tp-outer stays 'Forest' after tp-inner is cycled")]
    [Trait(Spec.UAC, "14")]
    [Then("tp-outer should remain Forest after cycling tp-inner")]
    public void UAC14_OuterUnchangedWhenInnerCycles()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-outer", "Forest");
        service.Register("tp-inner", "Dark");
        service.Next("tp-inner");
        service.GetNameForProvider("tp-outer").ShouldBe("Forest");
    }

    [Fact(DisplayName = "UAC15: tp-inner advances after Next")]
    [Trait(Spec.UAC, "15")]
    [Then("tp-inner should advance after calling Next")]
    public void UAC15_InnerAdvancesAfterNext()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-outer", "Forest");
        service.Register("tp-inner", "Dark");
        service.Next("tp-inner");
        service.GetNameForProvider("tp-inner").ShouldNotBe("Dark");
    }

    [Fact(DisplayName = "UAC16: Only one ThemeChanged raised for tp-inner")]
    [Trait(Spec.UAC, "16")]
    [Then("ThemeChanged should only fire for tp-inner")]
    public void UAC16_OnlyOneThemeChangedRaised()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-outer", "Forest");
        service.Register("tp-inner", "Dark");
        var events = new List<string>();
        service.ThemeChanged += id => events.Add(id);
        service.Next("tp-inner");
        events.Count.ShouldBe(1);
        events[0].ShouldBe("tp-inner");
    }

    [Fact(DisplayName = "UAC17: tp-outer provider re-render not triggered by tp-inner change")]
    [Trait(Spec.UAC, "17")]
    [Then("tp-outer rendered output should be unchanged after tp-inner cycles")]
    public void UAC17_OuterRenderUnchanged()
    {
        var cut = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-outer")
            .Add(x => x.Theme, "Forest")
            .AddChildContent<ThemeProvider>(inner => inner
                .Add(x => x.Id, "tp-inner")
                .Add(x => x.Theme, "Dark")));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        var outerStyleBefore = cut.FindAll("style")
            .First(s => s.TextContent.Contains("tp-outer"))
            .TextContent;
        service.Next("tp-inner");
        cut.FindAll("style")
            .First(s => s.TextContent.Contains("tp-outer"))
            .TextContent
            .ShouldBe(outerStyleBefore);
    }
}

// ============================================================
//  SC05 – Cycling the outer provider leaves the inner unchanged
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "05")]
[UserStory("04", "Provider Isolation")]
public class SC05_CyclingOuterDoesNotAffectInner : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC18: tp-inner stays 'Dark' after tp-outer is cycled")]
    [Trait(Spec.UAC, "18")]
    [Then("tp-inner should remain Dark after cycling tp-outer")]
    public void UAC18_InnerUnchangedWhenOuterCycles()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-outer", "Forest");
        service.Register("tp-inner", "Dark");
        service.Next("tp-outer");
        service.GetNameForProvider("tp-inner").ShouldBe("Dark");
    }

    [Fact(DisplayName = "UAC19: tp-outer advances after Next")]
    [Trait(Spec.UAC, "19")]
    [Then("tp-outer should advance after calling Next")]
    public void UAC19_OuterAdvancesAfterNext()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-outer", "Forest");
        service.Register("tp-inner", "Dark");
        service.Next("tp-outer");
        service.GetNameForProvider("tp-outer").ShouldNotBe("Forest");
    }

    [Fact(DisplayName = "UAC20: Only one ThemeChanged raised for tp-outer")]
    [Trait(Spec.UAC, "20")]
    [Then("ThemeChanged should only fire for tp-outer")]
    public void UAC20_OnlyOneThemeChangedRaised()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-outer", "Forest");
        service.Register("tp-inner", "Dark");
        var events = new List<string>();
        service.ThemeChanged += id => events.Add(id);
        service.Next("tp-outer");
        events.Count.ShouldBe(1);
        events[0].ShouldBe("tp-outer");
    }

    [Fact(DisplayName = "UAC21: tp-inner rendered output unchanged after tp-outer cycles")]
    [Trait(Spec.UAC, "21")]
    [Then("tp-inner rendered output should be unchanged after tp-outer cycles")]
    public void UAC21_InnerRenderUnchanged()
    {
        var cut = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-outer")
            .Add(x => x.Theme, "Forest")
            .AddChildContent<ThemeProvider>(inner => inner
                .Add(x => x.Id, "tp-inner")
                .Add(x => x.Theme, "Dark")));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        var innerStyleBefore = cut.FindAll("style")
            .First(s => s.TextContent.Contains("tp-inner"))
            .TextContent;
        service.Next("tp-outer");
        cut.WaitForState(() =>
            !cut.FindAll("style")
                .First(s => s.TextContent.Contains("tp-outer"))
                .TextContent.Contains("Forest"));
        cut.FindAll("style")
            .First(s => s.TextContent.Contains("tp-inner"))
            .TextContent
            .ShouldBe(innerStyleBefore);
    }
}

// ============================================================
//  SC06 – Disposing one sibling does not break the other
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "06")]
[UserStory("04", "Provider Isolation")]
public class SC06_DisposedSiblingDoesNotBreakSibling : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC22: tp-a removed from service after disposal")]
    [Trait(Spec.UAC, "22")]
    [Then("tp-a should be removed from service after disposal")]
    public void UAC22_TpARemovedAfterDispose()
    {
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        cutA.Instance.Dispose();
        service.GetNameForProvider("tp-a").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC23: tp-b still addressable after tp-a disposed")]
    [Trait(Spec.UAC, "23")]
    [Then("tp-b should still be addressable after tp-a is disposed")]
    public void UAC23_TpBStillAddressableAfterTpADisposed()
    {
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-b", "Rose");
        cutA.Instance.Dispose();
        service.GetNameForProvider("tp-b").ShouldBe("Rose");
    }

    [Fact(DisplayName = "UAC24-25: tp-b can still be cycled after tp-a disposed")]
    [Trait(Spec.UAC, "24")]
    [Then("tp-b should still cycle after tp-a is disposed")]
    public void UAC24_25_TpBCyclesAfterTpADisposed()
    {
        var cutA = Ctx.Render<ThemeProvider>(p => p
            .Add(x => x.Id, "tp-a")
            .Add(x => x.Theme, "Ocean"));
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("tp-b", "Rose");
        cutA.Instance.Dispose();
        service.Next("tp-b");
        service.GetNameForProvider("tp-b").ShouldBe("Forest");
    }
}

// ============================================================
//  SC07 – Two providers sharing the same Id share one entry
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "07")]
[UserStory("04", "Provider Isolation")]
public class SC07_SharedIdUsesOneEntry : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC26: Second provider with same id is a no-op (idempotent)")]
    [Trait(Spec.UAC, "26")]
    [Then("registering the same id twice should be idempotent")]
    public void UAC26_SecondProviderSameIdIsIdempotent()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("shared", "Ocean");
        service.Register("shared", "Dark"); // second call is no-op
        service.GetNameForProvider("shared").ShouldBe("Ocean");
    }

    [Fact(DisplayName = "UAC27: ThemeChanged for 'shared' fires once after Set")]
    [Trait(Spec.UAC, "27")]
    [Then("ThemeChanged should fire exactly once for the shared id on Set")]
    public void UAC27_ThemeChangedFiresOnceForShared()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("shared", "Ocean");
        service.Register("shared", "Dark");
        var events = new List<string>();
        service.ThemeChanged += id => events.Add(id);
        service.Set("shared", "Rose");
        events.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC28: One style entry reflects the shared theme")]
    [Trait(Spec.UAC, "28")]
    [Then("there should be one service entry for the shared provider id")]
    public void UAC28_OneServiceEntryForShared()
    {
        var service = Ctx.Services.GetRequiredService<ComponentThemeService>();
        service.Register("shared", "Ocean");
        service.Register("shared", "Dark");
        service.GetNameForProvider("shared").ShouldBe("Ocean");
        service.GetForProvider("shared")!.Name.ShouldBe("Ocean");
    }
}

// ============================================================
//  SC08 – Providers with compound child content render cleanly
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "08")]
[UserStory("04", "Provider Isolation")]
public class SC08_ProviderWithChildContentRenders : ThemeProviderScenario
{
    [Fact(DisplayName = "UAC29-32: Provider with Button and Badge children renders without error")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_32_ProviderWithChildrenRendersWithoutError()
    {
        var exception = Record.Exception(() =>
        {
            var cut = Ctx.Render<ThemeProvider>(p => p
                .Add(x => x.Id, "tp-content")
                .Add(x => x.Theme, "Ocean")
                .AddChildContent("<button type='button'>click</button><span class='badge'>new</span>"));
            cut.Find("[data-theme-provider='tp-content']").ShouldNotBeNull();
        });
        exception.ShouldBeNull();
    }
}
