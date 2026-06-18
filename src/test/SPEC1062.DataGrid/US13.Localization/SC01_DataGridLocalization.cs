using System.Globalization;
using System.Xml.Linq;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US13.Localization;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.US, "13")]
[Trait(Spec.SC, "01")]
[UserStory("13", "DataGrid localization")]
public class SC01_DataGridLocalization : ViewScenario
{
    [Fact(DisplayName = "UAC01: Default culture renders English pager strings")]
    [Trait(Spec.UAC, "01")]
    [Given("a paging DataGrid with AlwaysShowPager")]
    [When("the current UI culture is the default")]
    [Then("the pager displays English labels")]
    public void UAC01_DefaultCultureRendersEnglish()
    {
        var cut = RenderGrid(culture: null, allowPaging: true, alwaysShowPager: true);

        var pager = cut.Find("[data-slot='data-grid-pager']");
        pager.TextContent.ShouldContain("Rows");
        pager.TextContent.ShouldContain("Previous");
        pager.TextContent.ShouldContain("Next");
        pager.TextContent.ShouldContain("Page 1 of 1");
    }

    [Fact(DisplayName = "UAC02: Danish culture renders translated pager strings")]
    [Trait(Spec.UAC, "02")]
    [Given("a paging DataGrid with AlwaysShowPager")]
    [When("the current UI culture is Danish")]
    [Then("the pager displays Danish labels")]
    public void UAC02_DanishCultureRendersDanish()
    {
        var cut = RenderGrid(culture: new CultureInfo("da-DK"), allowPaging: true, alwaysShowPager: true);

        var pager = cut.Find("[data-slot='data-grid-pager']");
        pager.TextContent.ShouldContain("Rækker");
        pager.TextContent.ShouldContain("Forrige");
        pager.TextContent.ShouldContain("Næste");
        pager.TextContent.ShouldContain("Side 1 af 1");
    }

    [Fact(DisplayName = "UAC03: Empty state renders translated string for the current culture")]
    [Trait(Spec.UAC, "03")]
    [Given("a DataGrid with no items and the Danish culture")]
    [When("the empty state is rendered")]
    [Then("the empty title displays the Danish translation")]
    public void UAC03_EmptyStateRendersTranslated()
    {
        var cut = RenderGrid(culture: new CultureInfo("da-DK"), items: []);

        cut.Find("[data-slot='data-grid-empty']").TextContent.ShouldContain("Ingen resultater fundet.");
    }

    [Fact(DisplayName = "UAC04: Selection and expansion labels are localized")]
    [Trait(Spec.UAC, "04")]
    [Given("a DataGrid with multiple selection and detail rows in Danish")]
    [When("the header and row controls are rendered")]
    [Then("the select-all and select-row labels are localized")]
    public void UAC04_SelectionLabelsLocalized()
    {
        var cut = RenderGrid(
            culture: new CultureInfo("da-DK"),
            selectionMode: DataGridSelectionMode.Multiple,
            detailTemplate: item => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, item.Name);
                builder.CloseElement();
            });

        cut.Find("[data-slot='data-grid-select-all']").GetAttribute("aria-label").ShouldBe("Vælg alle synlige rækker");
        cut.Find("[data-slot='data-grid-selection-input']").GetAttribute("aria-label").ShouldBe("Vælg række");
        cut.Find("[data-slot='data-grid-expand-button']").GetAttribute("aria-label").ShouldBeNull();
        cut.Find("[data-slot='data-grid-header-cell'].cn-data-grid-expand-cell").TextContent.ShouldContain("Udvid");
    }

    [Fact(DisplayName = "UAC05: Filter input aria-label includes localized prefix")]
    [Trait(Spec.UAC, "05")]
    [Given("a filtering DataGrid in Danish")]
    [When("the filter row is rendered")]
    [Then("the filter input aria-label starts with the Danish filter prefix")]
    public void UAC05_FilterAriaLabelLocalized()
    {
        var cut = RenderGrid(culture: new CultureInfo("da-DK"), allowFiltering: true);

        var ariaLabel = cut.Find("[data-slot='data-grid-filter-input']").GetAttribute("aria-label");
        ariaLabel.ShouldNotBeNull();
        ariaLabel.ShouldStartWith("Filtrér");
    }

    [Fact(DisplayName = "UAC06: All locale DataGridStrings.resx files have the same keys as English")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_AllLocaleFilesHaveSameKeys()
    {
        AssertResxCatalogParity("DataGridStrings");
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
        var startDir = new DirectoryInfo(AppContext.BaseDirectory);
        var current = startDir;
        DirectoryInfo? projectRoot = null;

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Directory.Build.props")))
            {
                projectRoot = current;
                break;
            }
            current = current.Parent;
        }

        var root = projectRoot ?? (current != null ? current : new DirectoryInfo(Path.GetPathRoot(startDir.FullName)));

        var candidate = Path.Combine(root.FullName, "src", "components", "lib", "Resources");
        if (File.Exists(Path.Combine(candidate, "DataGridStrings.en.resx")))
        {
            return candidate;
        }

        var candidate2 = Path.Combine(root.FullName, "components", "src", "lib", "Resources");
        if (File.Exists(Path.Combine(candidate2, "DataGridStrings.en.resx")))
        {
            return candidate2;
        }

        throw new DirectoryNotFoundException($"Could not locate component Resources directory. Checked: {candidate}, {candidate2}");
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

    private IRenderedComponent<DataGrid<DataGridPerson>> RenderGrid(
        CultureInfo? culture = null,
        bool allowPaging = false,
        bool alwaysShowPager = false,
        bool allowFiltering = false,
        DataGridSelectionMode selectionMode = DataGridSelectionMode.None,
        RenderFragment<DataGridPerson>? detailTemplate = null,
        IReadOnlyList<DataGridPerson>? items = null)
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
            return Ctx.Render<DataGrid<DataGridPerson>>(parameters => parameters
                .Add(component => component.Items, items ?? DataGridPerson.Sample)
                .Add(component => component.AllowPaging, allowPaging)
                .Add(component => component.AlwaysShowPager, alwaysShowPager)
                .Add(component => component.AllowFiltering, allowFiltering)
                .Add(component => component.SelectionMode, selectionMode)
                .Add(component => component.DetailTemplate, detailTemplate)
                .Add(component => component.ChildContent, BuildColumns()));
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

    private static RenderFragment BuildColumns()
    {
        return builder =>
        {
            builder.OpenComponent<DataGridColumn<DataGridPerson>>(0);
            builder.AddAttribute(1, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Name));
            builder.AddAttribute(2, nameof(DataGridColumn<DataGridPerson>.Title), "Name");
            builder.CloseComponent();

            builder.OpenComponent<DataGridColumn<DataGridPerson>>(3);
            builder.AddAttribute(4, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Role));
            builder.AddAttribute(5, nameof(DataGridColumn<DataGridPerson>.Title), "Role");
            builder.CloseComponent();
        };
    }
}