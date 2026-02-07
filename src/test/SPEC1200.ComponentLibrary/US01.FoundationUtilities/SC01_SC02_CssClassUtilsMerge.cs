using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for CssClassUtils.Merge (shadcn/ui cn() function port)
/// Covers SC01-SC02 from US01.FoundationUtilities.feature
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
public class SC01_SC02_CssClassUtilsMerge
{
    #region SC01: Implement CssClassUtils.Merge

    [Fact(DisplayName = "Then it should accept multiple string inputs")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ShouldAcceptMultipleStringInputs()
    {
        // Act
        var result = CssClassUtils.Merge("class1", "class2", "class3");

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("class1");
        result.ShouldContain("class2");
        result.ShouldContain("class3");
    }

    [Fact(DisplayName = "And it should accept null values and ignore them")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ShouldAcceptNullValuesAndIgnoreThem()
    {
        // Act
        var result = CssClassUtils.Merge("base", null, "extra", null);

        // Assert
        result.ShouldBe("base extra");
    }

    [Fact(DisplayName = "And it should merge class names into a single string")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ShouldMergeClassNamesIntoSingleString()
    {
        // Act
        var result = CssClassUtils.Merge("inline-flex", "items-center", "justify-center");

        // Assert
        result.ShouldBe("inline-flex items-center justify-center");
    }

    [Fact(DisplayName = "And it should handle Tailwind CSS class conflicts (later classes override earlier)")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ShouldHandleTailwindCssClassConflicts_LaterOverridesEarlier()
    {
        // Act - Padding conflict
        var result1 = CssClassUtils.Merge("p-2", "p-4");
        
        // Assert
        result1.ShouldBe("p-4", "later padding class should override earlier");

        // Act - Text color conflict
        var result2 = CssClassUtils.Merge("text-red-500", "text-blue-500");
        
        // Assert
        result2.ShouldBe("text-blue-500", "later text color should override earlier");
    }

    [Fact(DisplayName = "And later \"p-4\" should override earlier \"p-2\"")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ShouldHandlePaddingConflict_P4OverridesP2()
    {
        // Act
        var result = CssClassUtils.Merge("p-2", "p-4");

        // Assert
        result.ShouldBe("p-4");
    }

    [Fact(DisplayName = "And it should handle conditional classes")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ShouldHandleConditionalClasses()
    {
        // Arrange
        bool condition = true;
        string? conditionalClass = condition ? "active" : null;

        // Act
        var result = CssClassUtils.Merge("base", conditionalClass);

        // Assert
        result.ShouldBe("base active");

        // Act - condition false
        condition = false;
        conditionalClass = condition ? "active" : null;
        result = CssClassUtils.Merge("base", conditionalClass);

        // Assert
        result.ShouldBe("base");
    }

    [Fact(DisplayName = "And it should remove duplicate classes")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ShouldRemoveDuplicateClasses()
    {
        // Act
        var result = CssClassUtils.Merge("flex", "items-center", "flex", "gap-2");

        // Assert
        result.ShouldBe("flex items-center gap-2");
    }

    #endregion

    #region SC02: Test CssClassUtils.Merge with various inputs

    [Fact(DisplayName = "Then it should return \"base extra\"")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ShouldReturnBaseExtra_WhenCalledWithBaseAndExtra()
    {
        // Act
        var result = CssClassUtils.Merge("base", "extra");

        // Assert
        result.ShouldBe("base extra");
    }

    [Fact(DisplayName = "Then it should return \"p-4\" (later override)")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ShouldReturnP4_WhenCalledWithP2AndP4()
    {
        // Act
        var result = CssClassUtils.Merge("p-2", "p-4");

        // Assert
        result.ShouldBe("p-4");
    }

    [Fact(DisplayName = "Then it should return \"base extra\" (null ignored)")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ShouldReturnBaseExtra_WhenNullIsInMiddle()
    {
        // Act
        var result = CssClassUtils.Merge("base", null, "extra");

        // Assert
        result.ShouldBe("base extra");
    }

    [Fact(DisplayName = "Then it should return \"text-blue-500\" (conflicting color utilities)")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ShouldReturnTextBlue500_WhenConflictingColorUtilities()
    {
        // Act
        var result = CssClassUtils.Merge("text-red-500", "text-blue-500");

        // Assert
        result.ShouldBe("text-blue-500");
    }

    #endregion

    #region Additional Tests for Edge Cases

    [Fact]
    public void ShouldHandleEmptyAndWhitespaceInputs()
    {
        // Act & Assert
        CssClassUtils.Merge().ShouldBeEmpty();
        CssClassUtils.Merge("").ShouldBeEmpty();
        CssClassUtils.Merge("   ").ShouldBeEmpty();
        CssClassUtils.Merge(null, null).ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHandleClassesWithMultipleSpaces()
    {
        // Act
        var result = CssClassUtils.Merge("flex  items-center", "gap-2   justify-center");

        // Assert
        result.ShouldBe("flex items-center gap-2 justify-center");
    }

    [Fact]
    public void ShouldHandleComplexRealWorldExample()
    {
        // Arrange - Simulating a button component
        var baseClasses = "inline-flex items-center justify-center rounded-md";
        var variantClasses = "bg-primary text-primary-foreground";
        var sizeClasses = "h-10 px-4 py-2";
        var userClasses = "custom-shadow hover:bg-primary/90";
        var overrideClasses = "px-6"; // User wants more padding

        // Act
        var result = CssClassUtils.Merge(
            baseClasses,
            variantClasses,
            sizeClasses,
            userClasses,
            overrideClasses
        );

        // Assert
        result.ShouldContain("inline-flex");
        result.ShouldContain("items-center");
        result.ShouldContain("bg-primary");
        result.ShouldContain("h-10");
        result.ShouldContain("py-2");
        result.ShouldContain("px-6");
        result.ShouldNotContain("px-4"); // Should be overridden
        result.ShouldContain("custom-shadow");
    }

    #endregion

    #region MergeAttributes Tests

    [Fact]
    public void MergeAttributes_ShouldCombineMultipleDictionaries()
    {
        // Arrange
        var dict1 = new Dictionary<string, object> { ["id"] = "test", ["class"] = "base" };
        var dict2 = new Dictionary<string, object> { ["data-value"] = "123", ["class"] = "extra" };

        // Act
        var result = CssClassUtils.MergeAttributes(dict1, dict2);

        // Assert
        result.ShouldContainKey("id");
        result.ShouldContainKey("data-value");
        result.ShouldContainKey("class");
        result["class"].ToString().ShouldBe("base extra");
    }

    [Fact]
    public void MergeAttributes_ShouldHandleNullDictionaries()
    {
        // Arrange
        var dict1 = new Dictionary<string, object> { ["id"] = "test" };

        // Act
        var result = CssClassUtils.MergeAttributes(dict1, null, null);

        // Assert
        result.ShouldContainKey("id");
    }

    [Fact]
    public void MergeAttributes_ShouldMergeClassAttributesWithConflictResolution()
    {
        // Arrange
        var dict1 = new Dictionary<string, object> { ["class"] = "p-2 text-red-500" };
        var dict2 = new Dictionary<string, object> { ["class"] = "p-4 text-blue-500" };

        // Act
        var result = CssClassUtils.MergeAttributes(dict1, dict2);

        // Assert
        result["class"].ToString().ShouldBe("p-4 text-blue-500");
    }

    [Fact]
    public void MergeAttributes_ShouldOverrideNonClassAttributes()
    {
        // Arrange
        var dict1 = new Dictionary<string, object> { ["id"] = "old", ["data-value"] = "1" };
        var dict2 = new Dictionary<string, object> { ["id"] = "new", ["data-value"] = "2" };

        // Act
        var result = CssClassUtils.MergeAttributes(dict1, dict2);

        // Assert
        result["id"].ShouldBe("new");
        result["data-value"].ShouldBe("2");
    }

    #endregion
}
