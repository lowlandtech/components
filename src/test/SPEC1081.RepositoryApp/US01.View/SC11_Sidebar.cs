using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "11")]
[UserStory("01", "Right sidebar appears only when selected content needs details")]
public class SC11_Sidebar : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Sidebar is hidden by default when no file is selected")]
    [Trait(Spec.UAC, "01")]
    [Given("the RepositoryApp is rendered with no selected files")]
    [When("the Code files view is shown")]
    [Then("the sidebar should not reserve a details column")]
    public void UAC01_SidebarHiddenByDefaultWhenNoFileIsSelected()
    {
        var cut = RenderRepositoryApp();

        cut.FindAll("[data-slot='repository-app-sidebar']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC12: Sidebar renders after a file is selected")]
    [Trait(Spec.UAC, "12")]
    [Given("the RepositoryApp is rendered in the Code files view")]
    [When("a file row is selected")]
    [Then("the sidebar should render the details column")]
    public void UAC12_SidebarRendersAfterFileIsSelected()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        cut.Find("[data-slot='repository-app-sidebar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Sidebar is hidden when a path is selected")]
    [Trait(Spec.UAC, "02")]
    [Given("the Code files view is shown with a non-empty path")]
    [When("the content area is inspected")]
    [Then("the sidebar should not be rendered")]
    public void UAC02_SidebarHiddenWhenPathIsSet()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPath("src");
        cut.Render();

        cut.FindAll("[data-slot='repository-app-sidebar']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Sidebar is hidden on non-Code views")]
    [Trait(Spec.UAC, "03")]
    [Given("a RepositoryApp is rendered on a non-Code view (e.g. Issues)")]
    [When("the content area is inspected")]
    [Then("the sidebar should not be rendered")]
    public void UAC03_SidebarHiddenOnNonCodeView()
    {
        var cut = RenderRepositoryApp();
        StateService.SetView(RepositoryView.Issues);
        cut.Render();

        cut.FindAll("[data-slot='repository-app-sidebar']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC04: Sidebar About section renders the description")]
    [Trait(Spec.UAC, "04")]
    [Given("a file is selected and the sidebar is visible")]
    [When("the About section is inspected")]
    [Then("it should display the current repository's description")]
    public void UAC04_SidebarAboutRendersDescription()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var about = cut.Find("[data-slot='repo-sidebar-about']");
        about.TextContent.ShouldContain(StateService.State.Current!.Description);
    }

    [Fact(DisplayName = "UAC05: Sidebar Topics section renders one chip per topic")]
    [Trait(Spec.UAC, "05")]
    [Given("a file is selected and the sidebar is visible")]
    [When("the Topics section is inspected")]
    [Then("one chip per topic from the current repository should be rendered")]
    public void UAC05_SidebarTopicsRenderChips()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var topics = cut.Find("[data-slot='repo-sidebar-topics']");
        topics.TextContent.ShouldContain("blazor");
        topics.TextContent.ShouldContain("dotnet");
        topics.TextContent.ShouldContain("foundry");
        topics.TextContent.ShouldContain("plugin");
        topics.TextContent.ShouldContain("graph");
    }

    [Fact(DisplayName = "UAC06: Sidebar Releases section shows up to 3 cards")]
    [Trait(Spec.UAC, "06")]
    [Given("a file is selected and the sidebar is visible with multiple releases in the state")]
    [When("the Releases section is inspected")]
    [Then("up to 3 release cards should be rendered")]
    public void UAC06_SidebarReleasesShowsTopThree()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var releases = cut.Find("[data-slot='repo-sidebar-releases']");
        var expected = StateService.State.Releases.Take(3).ToList();
        foreach (var release in expected)
        {
            releases.TextContent.ShouldContain(release.Title);
        }
    }

    [Fact(DisplayName = "UAC07: Sidebar Languages section has a stacked bar")]
    [Trait(Spec.UAC, "07")]
    [Given("a file is selected and the sidebar is visible with languages in the state")]
    [When("the Languages section is inspected")]
    [Then("a stacked bar with one segment per language should be rendered")]
    public void UAC07_SidebarLanguagesHasStackedBar()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var languages = cut.Find("[data-slot='repo-sidebar-languages']");
        var segments = languages.QuerySelectorAll("span.block");
        segments.Length.ShouldBe(StateService.State.Current!.Languages.Count);
    }

    [Fact(DisplayName = "UAC08: Sidebar Languages bar widths sum to ~100%")]
    [Trait(Spec.UAC, "08")]
    [Given("the languages bar is rendered from the seed")]
    [When("the inline width styles are summed")]
    [Then("the total should be 100 within rounding tolerance")]
    public void UAC08_LanguagesBarWidthsSumTo100()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var segments = cut.Find("[data-slot='repo-sidebar-languages']")
            .QuerySelectorAll("span.block");

        decimal total = 0;
        foreach (var seg in segments)
        {
            var style = seg.GetAttribute("style") ?? string.Empty;
            var idx = style.IndexOf("width:", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) continue;
            var rest = style[(idx + 6)..];
            var pctIdx = rest.IndexOf('%');
            if (pctIdx < 0) continue;
            var numberText = rest[..pctIdx].Trim();
            if (decimal.TryParse(numberText, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var v))
            {
                total += v;
            }
        }

        total.ShouldBeInRange(99m, 101m);
    }

    [Fact(DisplayName = "UAC09: Sidebar root has the hidden lg:flex responsive classes")]
    [Trait(Spec.UAC, "09")]
    [Given("the sidebar is rendered")]
    [When("the sidebar root is inspected")]
    [Then("it should carry the hidden lg:flex responsive shell classes")]
    public void UAC09_SidebarRootHasResponsiveShellClasses()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        var sidebar = cut.Find("[data-slot='repository-app-sidebar']");
        sidebar.ClassList.ShouldContain("hidden");
        sidebar.ClassList.ShouldContain("lg:flex");
    }

    [Fact(DisplayName = "UAC10: Repository without topics hides the topics section")]
    [Trait(Spec.UAC, "10")]
    [Given("a repository with an empty topics list is rendered with the sidebar visible")]
    [When("the sidebar is inspected")]
    [Then("the Topics section should not be present")]
    public void UAC10_RepositoryWithoutTopicsHidesSection()
    {
        var noTopicsState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with
            {
                Topics = []
            }
        };
        var cut = RenderRepositoryAppWithState(noTopicsState);
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        cut.FindAll("[data-slot='repo-sidebar-topics']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC11: Repository with no releases hides the releases section")]
    [Trait(Spec.UAC, "11")]
    [Given("a state with no releases is rendered with the sidebar visible")]
    [When("the sidebar is inspected")]
    [Then("the Releases section should not be present")]
    public void UAC11_RepositoryWithoutReleasesHidesSection()
    {
        var noReleasesState = RepositoryUseCaseData.Primary() with
        {
            Releases = []
        };
        var cut = RenderRepositoryAppWithState(noReleasesState);
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        cut.FindAll("[data-slot='repo-sidebar-releases']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC13: Selecting a file renders the file section in the sidebar")]
    [Trait(Spec.UAC, "13")]
    [Given("a file has been selected from the file list")]
    [When("the sidebar is rendered")]
    [Then("a file header should appear showing the file name and path")]
    public void UAC13_SelectedFileRendersFileSection()
    {
        var cut = RenderRepositoryApp();
        var file = StateService.State.Files.First(f => f.LastCommit is not null);
        StateService.ToggleFileSelection(file.Path);
        cut.Render();

        var section = cut.Find("[data-slot='repo-sidebar-file']");
        section.ShouldNotBeNull();
        section.TextContent.ShouldContain(file.Name);
        section.TextContent.ShouldContain(file.Path);
    }

    [Fact(DisplayName = "UAC14: Selected file section shows the last commit message")]
    [Trait(Spec.UAC, "14")]
    [Given("a file with a last commit is selected")]
    [When("the sidebar is rendered")]
    [Then("the sidebar should show the commit message, author, short SHA, and branch")]
    public void UAC14_SelectedFileShowsLastCommit()
    {
        var cut = RenderRepositoryApp();
        var file = StateService.State.Files.First(f => f.LastCommit is not null);
        StateService.ToggleFileSelection(file.Path);
        cut.Render();

        var commit = cut.Find("[data-slot='repo-sidebar-file-commit']");
        commit.ShouldNotBeNull();
        commit.TextContent.ShouldContain(file.LastCommit!.Message);
        commit.TextContent.ShouldContain(file.LastCommit.Author.Name);
        commit.TextContent.ShouldContain(file.LastCommit.ShortSha);
        commit.TextContent.ShouldContain(StateService.State.Ui.SelectedBranch);
    }

    [Fact(DisplayName = "UAC15: Sidebar without a selected file does not render the file section")]
    [Trait(Spec.UAC, "15")]
    [Given("the sidebar is rendered with no file selected")]
    [When("the sidebar is inspected")]
    [Then("the file section should not be present")]
    public void UAC15_NoSelectionHidesFileSection()
    {
        var cut = RenderRepositoryApp();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();
        StateService.ToggleFileSelection(StateService.State.Files.First().Path);
        cut.Render();

        cut.FindAll("[data-slot='repo-sidebar-file']").Count.ShouldBe(0);
        cut.FindAll("[data-slot='repo-sidebar-file-commit']").Count.ShouldBe(0);
    }
}
