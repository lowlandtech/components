using LowlandTech.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Cli.Tests.SPEC1200.ComponentLibrary.US01.FoundationUtilities;

/// <summary>
/// Tests for ComponentBase extension methods
/// Feature: US01.FoundationUtilities.feature
/// Scenario: SC09 - Create ComponentBase extensions
/// UACs: UAC42-UAC44
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "09")]
public class SC09_ComponentBaseExtensions
{
    // Helper component for testing extension methods
    private class TestComponent : ComponentBase { }

    #region UAC42 - MergeAttributes helper

    [Fact(DisplayName = "Then I should have MergeAttributes helper for combining attribute dictionaries")]
    [Trait(Spec.UAC, "42")]
    public async Task UAC42_ShouldMergeMultipleAttributeDictionaries()
    {
        // Arrange
        var component = new TestComponent();
        var dict1 = new Dictionary<string, object>
        {
            { "id", "test-id" },
            { "class", "text-red-500" }
        };
        var dict2 = new Dictionary<string, object>
        {
            { "class", "text-blue-500" },
            { "data-test", "value" }
        };

        // Act
        var result = component.MergeAttributes(dict1, dict2);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result["id"].ShouldBe("test-id");
        result["class"].ShouldBe("text-blue-500"); // Later dict should override
        result["data-test"].ShouldBe("value");
    }

    [Fact]
    public async Task UAC42_MergeAttributes_ShouldHandleNullDictionaries()
    {
        // Arrange
        var component = new TestComponent();
        var dict = new Dictionary<string, object> { { "key", "value" } };

        // Act
        var result = component.MergeAttributes(null, dict, null);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result["key"].ShouldBe("value");
    }

    [Fact]
    public async Task UAC42_MergeAttributes_ShouldReturnNullWhenAllDictionariesAreNull()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.MergeAttributes(null, null);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task UAC42_MergeAttributes_ShouldReturnNullWhenNoDictionariesProvided()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.MergeAttributes();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task UAC42_MergeAttributes_ShouldHandleEmptyDictionaries()
    {
        // Arrange
        var component = new TestComponent();
        var empty1 = new Dictionary<string, object>();
        var empty2 = new Dictionary<string, object>();

        // Act
        var result = component.MergeAttributes(empty1, empty2);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task UAC42_MergeAttributes_ShouldOverrideWithLastValue()
    {
        // Arrange
        var component = new TestComponent();
        var dict1 = new Dictionary<string, object> { { "key", "first" } };
        var dict2 = new Dictionary<string, object> { { "key", "second" } };
        var dict3 = new Dictionary<string, object> { { "key", "third" } };

        // Act
        var result = component.MergeAttributes(dict1, dict2, dict3);

        // Assert
        result.ShouldNotBeNull();
        result["key"].ShouldBe("third");
    }

    #endregion

    #region UAC43 - SetDataAttribute helper

    [Fact(DisplayName = "And I should have SetDataAttribute helper for data-* attributes")]
    [Trait(Spec.UAC, "43")]
    public async Task UAC43_ShouldAddDataAttributeToExistingDictionary()
    {
        // Arrange
        var component = new TestComponent();
        var attributes = new Dictionary<string, object>
        {
            { "id", "test-id" }
        };

        // Act
        var result = component.SetDataAttribute(attributes, "test-id", "test-value");

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result["id"].ShouldBe("test-id");
        result["data-test-id"].ShouldBe("test-value");
    }

    [Fact]
    public async Task UAC43_ShouldCreateNewDictionaryWhenNull()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.SetDataAttribute(null, "test", "value");

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result["data-test"].ShouldBe("value");
    }

    [Fact]
    public async Task UAC43_ShouldHandleDataPrefixInName()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.SetDataAttribute(null, "data-test", "value");

        // Assert
        result["data-test"].ShouldBe("value");
        result.Keys.Count(k => k.StartsWith("data-")).ShouldBe(1); // Should not double-prefix
    }

    [Fact]
    public async Task UAC43_ShouldOverrideExistingDataAttribute()
    {
        // Arrange
        var component = new TestComponent();
        var attributes = new Dictionary<string, object>
        {
            { "data-test", "old-value" }
        };

        // Act
        var result = component.SetDataAttribute(attributes, "test", "new-value");

        // Assert
        result["data-test"].ShouldBe("new-value");
    }

    [Fact]
    public async Task UAC43_ShouldThrowOnNullOrEmptyName()
    {
        // Arrange
        var component = new TestComponent();

        // Act & Assert
        Should.Throw<ArgumentException>(() => component.SetDataAttribute(null, null!, "value"));
        Should.Throw<ArgumentException>(() => component.SetDataAttribute(null, "", "value"));
        Should.Throw<ArgumentException>(() => component.SetDataAttribute(null, "   ", "value"));
    }

    #endregion

    #region UAC44 - BuildClassString helper

    [Fact(DisplayName = "And I should have BuildClassString helper for class attribute construction")]
    [Trait(Spec.UAC, "44")]
    public async Task UAC44_ShouldBuildClassStringFromBaseAndAdditional()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.BuildClassString("text-red-500", "bg-blue-500", "p-4");

        // Assert
        result.ShouldBe("text-red-500 bg-blue-500 p-4");
    }

    [Fact]
    public async Task UAC44_ShouldHandleNullClasses()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.BuildClassString(null, "text-red-500", null, "p-4");

        // Assert
        result.ShouldBe("text-red-500 p-4");
    }

    [Fact]
    public async Task UAC44_ShouldUseTailwindMergeLogic()
    {
        // Arrange
        var component = new TestComponent();

        // Act - conflicting Tailwind classes
        var result = component.BuildClassString("text-red-500", "text-blue-500");

        // Assert
        result.ShouldBe("text-blue-500"); // Later class should win via CssClassUtils.Merge
    }

    [Fact]
    public async Task UAC44_ShouldExtractClassFromAttributesDictionary()
    {
        // Arrange
        var component = new TestComponent();
        var attributes = new Dictionary<string, object>
        {
            { "class", "bg-blue-500" }
        };

        // Act
        var result = component.BuildClassString("text-red-500", attributes);

        // Assert
        result.ShouldBe("text-red-500 bg-blue-500");
    }

    [Fact]
    public async Task UAC44_ShouldHandleNullAttributesDictionary()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.BuildClassString("text-red-500", (IReadOnlyDictionary<string, object>?)null);

        // Assert
        result.ShouldBe("text-red-500");
    }

    [Fact]
    public async Task UAC44_ShouldHandleAttributesDictionaryWithoutClass()
    {
        // Arrange
        var component = new TestComponent();
        var attributes = new Dictionary<string, object>
        {
            { "id", "test-id" }
        };

        // Act
        var result = component.BuildClassString("text-red-500", attributes);

        // Assert
        result.ShouldBe("text-red-500");
    }

    [Fact]
    public async Task UAC44_ShouldMergeConflictingClassesFromAttributes()
    {
        // Arrange
        var component = new TestComponent();
        var attributes = new Dictionary<string, object>
        {
            { "class", "text-blue-500" }
        };

        // Act
        var result = component.BuildClassString("text-red-500", attributes);

        // Assert
        result.ShouldBe("text-blue-500"); // Attribute class should override via CssClassUtils.Merge
    }

    [Fact]
    public async Task UAC44_ShouldReturnEmptyStringWhenAllClassesAreNull()
    {
        // Arrange
        var component = new TestComponent();

        // Act
        var result = component.BuildClassString(null, (string?)null, null);

        // Assert
        result.ShouldBe(string.Empty);
    }

    #endregion
}
