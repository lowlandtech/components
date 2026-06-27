using System.Globalization;
using System.Xml.Linq;
using LowlandTech.Components.Core;
using LowlandTech.Components.Tests.SPEC1072.DataForm.Models;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US07.Localization;

using DataFormSut = LowlandTech.Components.DataForm;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "01")]
[UserStory("07", "DataForm and FormField localization")]
public class SC01_DataFormLocalization : ViewScenario
{
    [Fact(DisplayName = "UAC01: Default culture renders English default actions")]
    [Trait(Spec.UAC, "01")]
    [Given("a DataForm with default actions")]
    [When("the current UI culture is the default")]
    [Then("the submit and reset buttons display English labels")]
    public void UAC01_DefaultCultureRendersEnglishActions()
    {
        var cut = RenderForm(culture: null);

        var actions = cut.Find("[data-slot='data-form-actions']");
        actions.TextContent.ShouldContain("Submit");
        actions.TextContent.ShouldContain("Reset");
    }

    [Fact(DisplayName = "UAC02: Danish culture renders translated default actions")]
    [Trait(Spec.UAC, "02")]
    [Given("a DataForm with default actions")]
    [When("the current UI culture is Danish")]
    [Then("the submit and reset buttons display Danish labels")]
    public void UAC02_DanishCultureRendersDanishActions()
    {
        var cut = RenderForm(culture: new CultureInfo("da-DK"));

        var actions = cut.Find("[data-slot='data-form-actions']");
        actions.TextContent.ShouldContain("Send");
        actions.TextContent.ShouldContain("Nulstil");
    }

    [Fact(DisplayName = "UAC03: Guid field placeholder is localized")]
    [Trait(Spec.UAC, "03")]
    [Given("a Guid FormField in Danish")]
    [When("the field is rendered")]
    [Then("the placeholder is the localized GUID pattern")]
    public void UAC03_GuidPlaceholderLocalized()
    {
        var cut = RenderFormField(culture: new CultureInfo("da-DK"), FormFieldType.Guid);

        var placeholder = cut.Find("[data-field-input='field']").GetAttribute("placeholder");
        placeholder.ShouldBe("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
    }

    [Fact(DisplayName = "UAC04: NodeRef field placeholder is localized")]
    [Trait(Spec.UAC, "04")]
    [Given("a NodeRef FormField in Danish")]
    [When("the field is rendered")]
    [Then("the placeholder is the Danish translation")]
    public void UAC04_NodeRefPlaceholderLocalized()
    {
        var cut = RenderFormField(culture: new CultureInfo("da-DK"), FormFieldType.NodeRef);

        var placeholder = cut.Find("[data-field-input='field']").GetAttribute("placeholder");
        placeholder.ShouldBe("Nodereference...");
    }

    [Fact(DisplayName = "UAC05: Enum field select placeholder is localized")]
    [Trait(Spec.UAC, "05")]
    [Given("an Enum FormField in Danish")]
    [When("the field is rendered")]
    [Then("the select trigger placeholder is the Danish translation")]
    public void UAC05_EnumPlaceholderLocalized()
    {
        var cut = RenderFormField(
            culture: new CultureInfo("da-DK"),
            FormFieldType.Enum,
            enumType: typeof(CustomerStatus));

        cut.Find("[data-field-input='field']").TextContent.ShouldContain("Vælg...");
    }

    [Fact(DisplayName = "UAC06: All locale DataFormStrings.resx files have the same keys as English")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_AllLocaleFilesHaveSameKeys()
    {
        AssertResxCatalogParity("DataFormStrings");
    }

    private static void AssertResxCatalogParity(string className)
    {
        var resourcesDir = FindComponentResourcesDirectory();
        var enPath = Path.Combine(resourcesDir, $"{className}.en.resx");
        File.Exists(enPath).ShouldBeTrue();

        var enKeys = LoadResxKeys(enPath);
        enKeys.Count.ShouldBeGreaterThan(0);

        foreach (var locale in new[] { "da", "de", "es", "fr", "it", "nl", "pl", "pt", "sv" })
        {
            var locPath = Path.Combine(resourcesDir, $"{className}.{locale}.resx");
            File.Exists(locPath).ShouldBeTrue();
            var locKeys = LoadResxKeys(locPath);
            locKeys.ShouldBe(enKeys, ignoreOrder: true);
        }
    }

    private static string FindComponentResourcesDirectory()
    {
        // Use assembly location to handle shadow copies (NCrunch, etc.)
        var assemblyPath = typeof(SC01_DataFormLocalization).Assembly.Location;
        var startDir = new DirectoryInfo(Path.GetDirectoryName(assemblyPath)!);
        var current = startDir;

        while (current is not null)
        {
            var rootCandidate = Path.Combine(current.FullName, "src", "components", "lib", "Resources");
            if (Directory.Exists(rootCandidate))
            {
                var resxFiles = Directory.GetFiles(rootCandidate, "*.resx");
                if (resxFiles.Length > 0)
                {
                    return rootCandidate;
                }
            }

            var componentsDir = new DirectoryInfo(Path.Combine(current.FullName, "components"));
            if (componentsDir.Exists)
            {
                var compCandidate = Path.Combine(componentsDir.FullName, "lib", "Resources");
                if (Directory.Exists(compCandidate))
                {
                    var resxFiles = Directory.GetFiles(compCandidate, "*.resx");
                    if (resxFiles.Length > 0)
                    {
                        return compCandidate;
                    }
                }
            }

            var directCandidate = Path.Combine(current.FullName, "Resources");
            if (Directory.Exists(directCandidate))
            {
                var resxFiles = Directory.GetFiles(directCandidate, "*.resx");
                if (resxFiles.Length > 0)
                {
                    return directCandidate;
                }
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException($"Could not locate component Resources directory starting from {startDir}");
    }

    private static HashSet<string> LoadResxKeys(string path)
    {
        var doc = XDocument.Load(path);
        return doc.Root!
            .Elements("data")
            .Select(e => e.Attribute("name")?.Value)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToHashSet()!;
    }

    private IRenderedComponent<DataFormSut> RenderForm(CultureInfo? culture)
    {
        CultureInfo? originalCulture = null;
        CultureInfo? originalUiCulture = null;

        if (culture is not null)
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUiCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        try
        {
            return Ctx.Render<DataFormSut>(parameters => parameters
                .Add(p => p.Model, new CustomerFormModel())
                .Add(p => p.ShowDefaultActions, true));
        }
        finally
        {
            if (culture is not null)
            {
                CultureInfo.CurrentCulture = originalCulture!;
                CultureInfo.CurrentUICulture = originalUiCulture!;
            }
        }
    }

    private IRenderedComponent<LowlandTech.Components.FormField> RenderFormField(
        CultureInfo? culture,
        FormFieldType fieldType,
        Type? enumType = null)
    {
        CultureInfo? originalCulture = null;
        CultureInfo? originalUiCulture = null;

        if (culture is not null)
        {
            originalCulture = CultureInfo.CurrentCulture;
            originalUiCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        try
        {
            return Ctx.Render<LowlandTech.Components.FormField>(p =>
            {
                p.Add(x => x.FieldName, "field");
                p.Add(x => x.FieldType, fieldType);
                p.Add(x => x.Label, "Field");

                if (enumType is not null)
                {
                    p.Add(x => x.EnumType, enumType);
                }
            });
        }
        finally
        {
            if (culture is not null)
            {
                CultureInfo.CurrentCulture = originalCulture!;
                CultureInfo.CurrentUICulture = originalUiCulture!;
            }
        }
    }
}