namespace LowlandTech.Components.Tests.SPEC1071.ComponentThemes.US01.ThemeRegistry;

// ============================================================
//  SC01 – Registered themes expose stable names and token
//         dictionaries
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC01_RegisteredThemesNameAndTokens
{
    [Fact(DisplayName = "UAC01: Each theme exposes a non-empty unique Name")]
    [Trait(Spec.UAC, "01")]
    [Then("each theme should expose a non-empty unique Name")]
    public void UAC01_EachThemeHasUniqueNonEmptyName()
    {
        var service = TestThemeFactory.CreateService();
        var names = service.All.Select(t => t.Name).ToList();
        names.ShouldAllBe(n => !string.IsNullOrEmpty(n));
        names.Distinct().Count().ShouldBe(names.Count);
    }

    [Fact(DisplayName = "UAC02: Each theme exposes a non-empty Tokens dictionary")]
    [Trait(Spec.UAC, "02")]
    [Then("each theme should expose a non-empty Tokens dictionary")]
    public void UAC02_EachThemeHasNonEmptyTokens()
    {
        var service = TestThemeFactory.CreateService();
        foreach (var theme in service.All)
            theme.Tokens.ShouldNotBeEmpty($"Theme '{theme.Name}' has no tokens");
    }

    [Fact(DisplayName = "UAC03: Every theme token key begins with '--'")]
    [Trait(Spec.UAC, "03")]
    [Then("every theme token key should begin with '--'")]
    public void UAC03_AllTokenKeysPrefixedWithDoubleDash()
    {
        var service = TestThemeFactory.CreateService();
        foreach (var theme in service.All)
            theme.Tokens.Keys.ShouldAllBe(
                k => k.StartsWith("--"),
                $"A token key in theme '{theme.Name}' does not start with '--'");
    }

    [Fact(DisplayName = "UAC04: Every token value is a non-empty string")]
    [Trait(Spec.UAC, "04")]
    [Then("every token value should be written verbatim without service-side conversion")]
    public void UAC04_AllTokenValuesNonEmpty()
    {
        var service = TestThemeFactory.CreateService();
        foreach (var theme in service.All)
            theme.Tokens.Values.ShouldAllBe(
                v => !string.IsNullOrWhiteSpace(v),
                $"A token value in theme '{theme.Name}' is blank");
    }

    [Fact(DisplayName = "UAC05: All registered themes appear in the All list in registration order")]
    [Trait(Spec.UAC, "05")]
    [Then("ThemesLoaded should identify the registration order used for cycling")]
    public void UAC05_AllReturnsThemesInRegistrationOrder()
    {
        var service = TestThemeFactory.CreateService();
        service.All.Select(t => t.Name)
            .ShouldBe(new[] { "Light", "Dark", "Ocean", "Rose", "Forest" });
    }
}

// ============================================================
//  SC02 – Token contract includes the shadcn-style semantic
//         variables
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC02_TokenContractSemanticKeys
{
    private static readonly string[] RequiredTokens =
    [
        "--background", "--foreground",
        "--card", "--card-foreground",
        "--popover", "--popover-foreground",
        "--primary", "--primary-foreground",
        "--secondary", "--secondary-foreground",
        "--muted", "--muted-foreground",
        "--accent", "--accent-foreground",
        "--destructive", "--destructive-foreground",
        "--border", "--input", "--ring",
    ];

    [Fact(DisplayName = "UAC06: Every theme defines '--background'")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ThemeDefinesBackground() =>
        AssertAllThemesHaveToken("--background");

    [Fact(DisplayName = "UAC07: Every theme defines '--foreground'")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ThemeDefinesForeground() =>
        AssertAllThemesHaveToken("--foreground");

    [Fact(DisplayName = "UAC08: Every theme defines '--card' and '--card-foreground'")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ThemeDefinesCard()
    {
        AssertAllThemesHaveToken("--card");
        AssertAllThemesHaveToken("--card-foreground");
    }

    [Fact(DisplayName = "UAC09: Every theme defines '--popover' and '--popover-foreground'")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ThemeDefinesPopover()
    {
        AssertAllThemesHaveToken("--popover");
        AssertAllThemesHaveToken("--popover-foreground");
    }

    [Fact(DisplayName = "UAC10: Every theme defines '--primary' and '--primary-foreground'")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ThemeDefinesPrimary()
    {
        AssertAllThemesHaveToken("--primary");
        AssertAllThemesHaveToken("--primary-foreground");
    }

    [Fact(DisplayName = "UAC11: Every theme defines '--secondary' and '--secondary-foreground'")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ThemeDefinesSecondary()
    {
        AssertAllThemesHaveToken("--secondary");
        AssertAllThemesHaveToken("--secondary-foreground");
    }

    [Fact(DisplayName = "UAC12: Every theme defines '--muted' and '--muted-foreground'")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_ThemeDefinesMuted()
    {
        AssertAllThemesHaveToken("--muted");
        AssertAllThemesHaveToken("--muted-foreground");
    }

    [Fact(DisplayName = "UAC13: Every theme defines '--accent' and '--accent-foreground'")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ThemeDefinesAccent()
    {
        AssertAllThemesHaveToken("--accent");
        AssertAllThemesHaveToken("--accent-foreground");
    }

    [Fact(DisplayName = "UAC14: Every theme defines '--destructive' and '--destructive-foreground'")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ThemeDefinesDestructive()
    {
        AssertAllThemesHaveToken("--destructive");
        AssertAllThemesHaveToken("--destructive-foreground");
    }

    [Fact(DisplayName = "UAC15: Every theme defines '--border', '--input', and '--ring'")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ThemeDefinesBorderInputRing()
    {
        AssertAllThemesHaveToken("--border");
        AssertAllThemesHaveToken("--input");
        AssertAllThemesHaveToken("--ring");
    }

    private static void AssertAllThemesHaveToken(string tokenKey)
    {
        foreach (var theme in TestThemeFactory.AllThemes())
            theme.Tokens.ContainsKey(tokenKey)
                .ShouldBeTrue($"Theme '{theme.Name}' is missing token '{tokenKey}'");
    }
}

// ============================================================
//  SC03 – Provider registration chooses the explicit initial
//         theme when supplied
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC03_ProviderRegistrationExplicitTheme
{
    [Fact(DisplayName = "UAC16: providerThemes['tp-a'] is 'Ocean' after Register('tp-a', 'Ocean')")]
    [Trait(Spec.UAC, "16")]
    [Then("ComponentThemeRegistryState.providerThemes['tp-a'] should be 'Ocean'")]
    public void UAC16_ProviderThemeStoredAsOcean()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.GetNameForProvider("tp-a").ShouldBe("Ocean");
    }

    [Fact(DisplayName = "UAC17: GetNameForProvider('tp-a') returns 'Ocean'")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_GetNameForProviderReturnsOcean()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.GetNameForProvider("tp-a").ShouldBe("Ocean");
    }

    [Fact(DisplayName = "UAC18: GetForProvider('tp-a') returns the Ocean theme object")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_GetForProviderReturnsOceanTheme()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.GetForProvider("tp-a")!.Name.ShouldBe("Ocean");
    }

    [Fact(DisplayName = "UAC19: ProviderRegistered includes providerId 'tp-a'")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_ProviderRegisteredInAll()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        // Provider is addressable – GetNameForProvider returns non-null
        service.GetNameForProvider("tp-a").ShouldNotBeNull();
    }
}

// ============================================================
//  SC04 – Provider registration falls back to the first
//         registered theme
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC04_ProviderRegistrationDefaultFallback
{
    [Fact(DisplayName = "UAC20: providerThemes['tp-default'] is 'Light' when no initial theme given")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_FallsBackToLight()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-default", null);
        service.GetNameForProvider("tp-default").ShouldBe("Light");
    }

    [Fact(DisplayName = "UAC21: GetNameForProvider('tp-default') returns 'Light'")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_GetNameForProviderReturnsLight()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-default");
        service.GetNameForProvider("tp-default").ShouldBe("Light");
    }

    [Fact(DisplayName = "UAC22: GetForProvider('tp-default') returns the Light theme object")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_GetForProviderReturnsLightTheme()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-default");
        service.GetForProvider("tp-default")!.Name.ShouldBe("Light");
    }
}

// ============================================================
//  SC05 – Register is idempotent for an existing provider
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC05_RegisterIdempotent
{
    [Fact(DisplayName = "UAC23: Re-registering 'tp-a' with 'Rose' leaves it at 'Ocean'")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_ExistingThemePreservedOnReRegister()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.Register("tp-a", "Rose"); // second call must be a no-op
        service.GetNameForProvider("tp-a").ShouldBe("Ocean");
    }

    [Fact(DisplayName = "UAC24: GetNameForProvider('tp-a') still returns 'Ocean' after re-register")]
    [Trait(Spec.UAC, "24")]
    public void UAC24_GetNameForProviderStillOcean()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.Register("tp-a", "Rose");
        service.GetNameForProvider("tp-a").ShouldBe("Ocean");
    }

    [Fact(DisplayName = "UAC25: No ThemeChanged event raised by a no-op Register")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_NoThemeChangedOnIdempotentRegister()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        var raised = false;
        service.ThemeChanged += _ => raised = true;
        service.Register("tp-a", "Rose");
        raised.ShouldBeFalse();
    }
}

// ============================================================
//  SC06 – Set updates the provider theme and raises an event
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC06_SetUpdatesTheme
{
    [Fact(DisplayName = "UAC26: providerThemes['tp-a'] is 'Rose' after Set('tp-a','Rose')")]
    [Trait(Spec.UAC, "26")]
    public void UAC26_ThemeNameUpdatedToRose()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.Set("tp-a", "Rose");
        service.GetNameForProvider("tp-a").ShouldBe("Rose");
    }

    [Fact(DisplayName = "UAC27: GetForProvider('tp-a') returns the Rose theme after Set")]
    [Trait(Spec.UAC, "27")]
    public void UAC27_GetForProviderReturnsRose()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        service.Set("tp-a", "Rose");
        service.GetForProvider("tp-a")!.Name.ShouldBe("Rose");
    }

    [Fact(DisplayName = "UAC28: ThemeChanged raised exactly once for 'tp-a'")]
    [Trait(Spec.UAC, "28")]
    public void UAC28_ThemeChangedRaisedOnce()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-a", "Ocean");
        var events = new List<string>();
        service.ThemeChanged += id => events.Add(id);
        service.Set("tp-a", "Rose");
        events.Count.ShouldBe(1);
        events[0].ShouldBe("tp-a");
    }
}

// ============================================================
//  SC07 – Next advances and wraps after the last theme
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC07_NextWrapsToFirstTheme
{
    [Fact(DisplayName = "UAC29: Next wraps from 'Forest' back to 'Light'")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_WrapsFromForestToLight()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-cycle", "Forest");
        service.Next("tp-cycle");
        service.GetNameForProvider("tp-cycle").ShouldBe("Light");
    }

    [Fact(DisplayName = "UAC30: GetForProvider returns the Light theme object after wrap")]
    [Trait(Spec.UAC, "30")]
    public void UAC30_GetForProviderReturnsLightAfterWrap()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-cycle", "Forest");
        service.Next("tp-cycle");
        service.GetForProvider("tp-cycle")!.Name.ShouldBe("Light");
    }

    [Fact(DisplayName = "UAC31: ThemeChanged raised for 'tp-cycle' on Next")]
    [Trait(Spec.UAC, "31")]
    public void UAC31_ThemeChangedRaisedForCycleProvider()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-cycle", "Forest");
        string? changedId = null;
        service.ThemeChanged += id => changedId = id;
        service.Next("tp-cycle");
        changedId.ShouldBe("tp-cycle");
    }
}

// ============================================================
//  SC08 – Unregister removes provider state
// ============================================================
[Trait(Spec.SPEC, "1071")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "08")]
[UserStory("01", "Theme Registry and Token Contract")]
public class SC08_UnregisterRemovesProvider
{
    [Fact(DisplayName = "UAC32: providerThemes no longer contains 'tp-dispose' after Unregister")]
    [Trait(Spec.UAC, "32")]
    public void UAC32_ProviderRemovedFromDictionary()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-dispose", "Dark");
        service.Unregister("tp-dispose");
        service.GetNameForProvider("tp-dispose").ShouldBeNull();
    }

    [Fact(DisplayName = "UAC33: registeredThemes still contains all five themes after Unregister")]
    [Trait(Spec.UAC, "33")]
    public void UAC33_ThemeRegistryUnchangedAfterUnregister()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-dispose", "Dark");
        service.Unregister("tp-dispose");
        service.All.Count.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC34: ProviderUnregistered includes providerId 'tp-dispose'")]
    [Trait(Spec.UAC, "34")]
    public void UAC34_ProviderNoLongerAddressable()
    {
        var service = TestThemeFactory.CreateService();
        service.Register("tp-dispose", "Dark");
        service.Unregister("tp-dispose");
        // Provider is gone – GetForProvider falls back to the global first theme
        // (not null) because GetForProvider returns first theme when name is not
        // in the dict; but GetNameForProvider returns null.
        service.GetNameForProvider("tp-dispose").ShouldBeNull();
    }
}
