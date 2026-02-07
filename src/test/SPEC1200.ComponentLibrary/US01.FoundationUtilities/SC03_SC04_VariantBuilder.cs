using LowlandTech.Components.Utilities;

namespace LowlandTech.Tests.SPEC1227.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for VariantBuilder (class-variance-authority port)
/// Covers SC03-SC04 from US01.FoundationUtilities.feature
/// </summary>
[Trait(Spec.SPEC, "0027")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "03")]
public class SC03_SC04_VariantBuilder
{
    #region SC03: Implement VariantBuilder (CVA port)

    [Fact(DisplayName = "Then it should accept base classes as a string")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_ShouldAcceptBaseClassesAsString()
    {
        // Arrange & Act
        var builder = new VariantBuilder("inline-flex items-center");

        // Assert
        builder.ShouldNotBeNull();
        var result = builder.Build();
        result.ShouldContain("inline-flex");
        result.ShouldContain("items-center");
    }

    [Fact(DisplayName = "And it should accept a dictionary of variant definitions")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ShouldAcceptDictionaryOfVariantDefinitions()
    {
        // Arrange
        var variants = new Dictionary<string, Dictionary<string, string>>
        {
            ["variant"] = new()
            {
                ["default"] = "bg-primary",
                ["outline"] = "border"
            }
        };

        // Act
        var builder = new VariantBuilder("base", variants);

        // Assert
        builder.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should accept default variant values")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ShouldAcceptDefaultVariantValues()
    {
        // Arrange
        var variants = new Dictionary<string, Dictionary<string, string>>
        {
            ["variant"] = new() { ["default"] = "bg-primary" }
        };
        var defaults = new Dictionary<string, string>
        {
            ["variant"] = "default"
        };

        // Act
        var builder = new VariantBuilder("base", variants, defaults);

        // Assert
        builder.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should have a Build method that accepts selected variants")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ShouldHaveBuildMethodThatAcceptsSelectedVariants()
    {
        // Arrange
        var builder = new VariantBuilder(
            "base",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["size"] = new() { ["sm"] = "h-9", ["lg"] = "h-11" }
            }
        );

        // Act
        var result = builder.Build(new Dictionary<string, string>
        {
            ["size"] = "sm"
        });

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain("h-9");
    }

    [Fact(DisplayName = "And Build should combine base classes with selected variant classes")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_BuildShouldCombineBaseClassesWithSelectedVariantClasses()
    {
        // Arrange
        var builder = new VariantBuilder(
            "inline-flex items-center",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["variant"] = new() { ["default"] = "bg-primary text-white" }
            }
        );

        // Act
        var result = builder.Build(new Dictionary<string, string>
        {
            ["variant"] = "default"
        });

        // Assert
        result.ShouldContain("inline-flex");
        result.ShouldContain("items-center");
        result.ShouldContain("bg-primary");
        result.ShouldContain("text-white");
    }

    [Fact(DisplayName = "And Build should use default variants when not specified")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_BuildShouldUseDefaultVariantsWhenNotSpecified()
    {
        // Arrange
        var builder = new VariantBuilder(
            "base",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["variant"] = new() { ["default"] = "bg-primary", ["outline"] = "border" },
                ["size"] = new() { ["default"] = "h-10", ["sm"] = "h-9" }
            },
            new Dictionary<string, string>
            {
                ["variant"] = "default",
                ["size"] = "default"
            }
        );

        // Act - Call Build with no variants
        var result = builder.Build();

        // Assert - Should use defaults
        result.ShouldContain("bg-primary");
        result.ShouldContain("h-10");
    }

    [Fact(DisplayName = "And Build should accept additional classes parameter")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_BuildShouldAcceptAdditionalClassesParameter()
    {
        // Arrange
        var builder = new VariantBuilder("base");

        // Act
        var result = builder.Build(null, "custom-class another-class");

        // Assert
        result.ShouldContain("custom-class");
        result.ShouldContain("another-class");
    }

    #endregion

    #region SC04: Test VariantBuilder with Button variants

    [Fact(DisplayName = "Then it should return classes including \"inline-flex items-center justify-center border border-input h-9 px-3\"")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_ShouldReturnClassesIncludingAllSelectedVariants()
    {
        // Arrange - Button variants configuration
        var buttonVariants = new VariantBuilder(
            "inline-flex items-center justify-center",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["variant"] = new()
                {
                    ["default"] = "bg-primary text-white",
                    ["outline"] = "border border-input"
                },
                ["size"] = new()
                {
                    ["default"] = "h-10 px-4",
                    ["sm"] = "h-9 px-3"
                }
            },
            new Dictionary<string, string>
            {
                ["variant"] = "default",
                ["size"] = "default"
            }
        );

        // Act - Call Build with variant="outline" and size="sm"
        var result = buttonVariants.Build(new Dictionary<string, string>
        {
            ["variant"] = "outline",
            ["size"] = "sm"
        });

        // Assert
        result.ShouldContain("inline-flex");
        result.ShouldContain("items-center");
        result.ShouldContain("justify-center");
        result.ShouldContain("border");
        result.ShouldContain("border-input");
        result.ShouldContain("h-9");
        result.ShouldContain("px-3");
    }

    [Fact(DisplayName = "Then it should return default variant classes")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_ShouldReturnDefaultVariantClasses()
    {
        // Arrange
        var buttonVariants = new VariantBuilder(
            "inline-flex",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["variant"] = new()
                {
                    ["default"] = "bg-primary",
                    ["outline"] = "border"
                },
                ["size"] = new()
                {
                    ["default"] = "h-10",
                    ["sm"] = "h-9"
                }
            },
            new Dictionary<string, string>
            {
                ["variant"] = "default",
                ["size"] = "default"
            }
        );

        // Act - Call Build with no variants specified
        var result = buttonVariants.Build();

        // Assert - Should return default variant classes
        result.ShouldContain("bg-primary");
        result.ShouldContain("h-10");
    }

    [Fact(DisplayName = "Then it should append \"custom-class\" to the result")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_ShouldAppendCustomClassToResult()
    {
        // Arrange
        var buttonVariants = new VariantBuilder(
            "inline-flex",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["variant"] = new() { ["default"] = "bg-primary" }
            },
            new Dictionary<string, string> { ["variant"] = "default" }
        );

        // Act - Call Build with additional classes "custom-class"
        var result = buttonVariants.Build(null, "custom-class");

        // Assert
        result.ShouldContain("custom-class");
    }

    #endregion

    #region Additional Tests for Edge Cases and Fluent API

    [Fact]
    public void ShouldHandleEmptyBaseClasses()
    {
        // Arrange & Act
        var builder = new VariantBuilder("");
        var result = builder.Build();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHandleNoVariants()
    {
        // Arrange
        var builder = new VariantBuilder("base-class");

        // Act
        var result = builder.Build();

        // Assert
        result.ShouldBe("base-class");
    }

    [Fact]
    public void ShouldHandleNonExistentVariantOption()
    {
        // Arrange
        var builder = new VariantBuilder(
            "base",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["size"] = new() { ["sm"] = "h-9", ["lg"] = "h-11" }
            }
        );

        // Act - Try to use a variant option that doesn't exist
        var result = builder.Build(new Dictionary<string, string>
        {
            ["size"] = "nonexistent"
        });

        // Assert - Should just return base classes
        result.ShouldBe("base");
    }

    [Fact]
    public void ShouldOverrideDefaultsWithSelectedVariants()
    {
        // Arrange
        var builder = new VariantBuilder(
            "base",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["size"] = new() { ["default"] = "h-10", ["sm"] = "h-9" }
            },
            new Dictionary<string, string> { ["size"] = "default" }
        );

        // Act - Override default with explicit selection
        var result = builder.Build(new Dictionary<string, string>
        {
            ["size"] = "sm"
        });

        // Assert - Should use selected, not default
        result.ShouldContain("h-9");
        result.ShouldNotContain("h-10");
    }

    [Fact]
    public void ShouldHandleConflictingClassesInVariants()
    {
        // Arrange - Both variants have padding classes
        var builder = new VariantBuilder(
            "base",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["size"] = new() { ["default"] = "p-2" },
                ["spacing"] = new() { ["default"] = "p-4" }
            },
            new Dictionary<string, string>
            {
                ["size"] = "default",
                ["spacing"] = "default"
            }
        );

        // Act
        var result = builder.Build();

        // Assert - Later variant should override (p-4 from spacing overrides p-2 from size)
        result.ShouldContain("p-4");
        result.ShouldNotContain("p-2");
    }

    [Fact]
    public void FluentAPI_WithVariant_ShouldAddVariant()
    {
        // Arrange & Act
        var builder = new VariantBuilder("base")
            .WithVariant("size", new Dictionary<string, string>
            {
                ["sm"] = "h-9",
                ["lg"] = "h-11"
            });

        var result = builder.Build(new Dictionary<string, string> { ["size"] = "sm" });

        // Assert
        result.ShouldContain("h-9");
    }

    [Fact]
    public void FluentAPI_WithDefault_ShouldSetDefault()
    {
        // Arrange & Act
        var builder = new VariantBuilder("base")
            .WithVariant("size", new Dictionary<string, string>
            {
                ["sm"] = "h-9",
                ["lg"] = "h-11"
            })
            .WithDefault("size", "lg");

        var result = builder.Build();

        // Assert - Should use default (lg)
        result.ShouldContain("h-11");
    }

    [Fact]
    public void FluentAPI_StringExtension_CreateVariantBuilder_ShouldWork()
    {
        // Arrange & Act
        var builder = "base-class".CreateVariantBuilder();

        // Assert
        builder.ShouldNotBeNull();
        var result = builder.Build();
        result.ShouldBe("base-class");
    }

    [Fact]
    public void ShouldHandleComplexRealWorldButtonExample()
    {
        // Arrange - Real-world button component
        var buttonVariants = new VariantBuilder(
            "inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50",
            new Dictionary<string, Dictionary<string, string>>
            {
                ["variant"] = new()
                {
                    ["default"] = "bg-primary text-primary-foreground shadow hover:bg-primary/90",
                    ["destructive"] = "bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90",
                    ["outline"] = "border border-input bg-background shadow-sm hover:bg-accent hover:text-accent-foreground",
                    ["secondary"] = "bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80",
                    ["ghost"] = "hover:bg-accent hover:text-accent-foreground",
                    ["link"] = "text-primary underline-offset-4 hover:underline"
                },
                ["size"] = new()
                {
                    ["default"] = "h-9 px-4 py-2",
                    ["sm"] = "h-8 rounded-md px-3 text-xs",
                    ["lg"] = "h-10 rounded-md px-8",
                    ["icon"] = "h-9 w-9"
                }
            },
            new Dictionary<string, string>
            {
                ["variant"] = "default",
                ["size"] = "default"
            }
        );

        // Act - Build different button variants
        var defaultButton = buttonVariants.Build();
        var outlineSmButton = buttonVariants.Build(new() { ["variant"] = "outline", ["size"] = "sm" });
        var destructiveLgButton = buttonVariants.Build(new() { ["variant"] = "destructive", ["size"] = "lg" });
        var customButton = buttonVariants.Build(new() { ["variant"] = "ghost", ["size"] = "icon" }, "custom-hover");

        // Assert - Default button
        defaultButton.ShouldContain("bg-primary");
        defaultButton.ShouldContain("h-9");

        // Assert - Outline small button
        outlineSmButton.ShouldContain("border");
        outlineSmButton.ShouldContain("h-8");
        outlineSmButton.ShouldContain("text-xs");

        // Assert - Destructive large button
        destructiveLgButton.ShouldContain("bg-destructive");
        destructiveLgButton.ShouldContain("h-10");
        destructiveLgButton.ShouldContain("px-8");

        // Assert - Custom button
        customButton.ShouldContain("hover:bg-accent");
        customButton.ShouldContain("h-9");
        customButton.ShouldContain("w-9");
        customButton.ShouldContain("custom-hover");
    }

    #endregion
}
