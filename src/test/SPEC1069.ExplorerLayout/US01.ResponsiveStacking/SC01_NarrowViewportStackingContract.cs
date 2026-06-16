using System.Reflection;

namespace LowlandTech.Components.Tests.SPEC1069.ExplorerLayout.US01.ResponsiveStacking;

/// <summary>
/// SC01 — Drift guard for the ExplorerLayout responsive stacking contract.
///
/// Background: ExplorerLayout is a side-by-side List/Detail panel split with
/// a draggable splitter. At narrow viewports the fixed pixel widths
/// (DefaultWidth, MinWidth) collapse the layout to an unusable stripe. The
/// fix is a single canonical CSS file that:
///
///   1. Lives at <c>src/components/lib/wwwroot/css/explorer-layout.css</c>
///      so all host apps load it via
///      <c>_content/LowlandTech.Components/css/explorer-layout.css</c>.
///   2. Declares an <c>@media (max-width: 1024px)</c> block that switches
///      the layout to <c>flex-direction: column</c>, drops the sidebar's
///      pixel width constraint, hides the <c>PanelSplitter</c>, and lets
///      the page scroll vertically instead of confining each pane to the
///      viewport.
///   3. Targets the right pane via the <c>explorer-detail</c> class on the
///      detail wrapper in <c>ExplorerLayout.razor</c> — without that
///      stable class hook, the rule has nothing to attach to.
///   4. Is referenced from each host app's <c>App.razor</c> head — without
///      the &lt;link&gt;, the rules never reach the browser.
///
/// History (preserved in commit log): the first iteration put the rules
/// in <c>src/shell/wwwroot/css/input.css</c>. The web app (port 7088) has
/// its own input.css and never picked them up — viewport stacking failed
/// silently. Moving the rules into the components library + linking from
/// each App.razor head is the canonical contract this scenario pins.
///
/// Each UAC reads the relevant source file out of the test assembly's
/// embedded resources (see LowlandTech.Components.Tests.csproj for the
/// EmbeddedResource entries) and asserts the contract piece survives.
/// </summary>
[Trait(Spec.SPEC, "1069")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "ExplorerLayout stacks panes vertically on narrow viewports")]
public class SC01_NarrowViewportStackingContract
{
    private const string MediaQuery = "@media (max-width: 1024px)";
    private const string CssLink = "_content/LowlandTech.Components/css/explorer-layout.css";

    private static string ReadEmbedded(string logicalName)
    {
        var asm = typeof(SC01_NarrowViewportStackingContract).Assembly;
        using var stream = asm.GetManifestResourceStream(logicalName);
        stream.ShouldNotBeNull(
            $"Expected '{logicalName}' embedded as a manifest resource in the test assembly. " +
            "Wire it up in LowlandTech.Components.Tests.csproj via <EmbeddedResource>.");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }

    private static string LoadCss() => ReadEmbedded("explorer-layout.css");
    private static string LoadExplorerLayoutRazor() => ReadEmbedded("ExplorerLayout.razor");

    /// <summary>
    /// Brace-balanced extraction of the body of a <c>@media (...)</c> block.
    /// Returns the substring from the opening brace through the matching
    /// closing brace, inclusive. Handles the nested rule braces inside the
    /// media block without a regex meltdown.
    /// </summary>
    private static string ExtractMediaBlock(string css, string query)
    {
        var idx = css.IndexOf(query, StringComparison.Ordinal);
        idx.ShouldBeGreaterThanOrEqualTo(0,
            $"@media query '{query}' not found in CSS. The 1024px breakpoint " +
            "is the contract — reverting it returns the layout to fixed " +
            "side-by-side and narrow viewports collapse the list pane.");
        var openBrace = css.IndexOf('{', idx);
        openBrace.ShouldBeGreaterThan(0, "media block has no opening brace");
        var depth = 0;
        for (var i = openBrace; i < css.Length; i++)
        {
            if (css[i] == '{') depth++;
            else if (css[i] == '}')
            {
                depth--;
                if (depth == 0) return css.Substring(openBrace, i - openBrace + 1);
            }
        }
        throw new InvalidOperationException(
            $"unbalanced braces extracting media block for '{query}'");
    }

    [Fact(DisplayName = "UAC01: explorer-layout.css declares the 1024px media query")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MediaQueryExists()
    {
        var css = LoadCss();
        css.ShouldContain(MediaQuery,
            customMessage:
                "explorer-layout.css must declare an `@media (max-width: 1024px)` " +
                "block. Without it, the side-by-side split holds at every " +
                "viewport and the list pane (e.g. WorkitemPage DefaultWidth=860) " +
                "collapses unusably below ~900px.");
    }

    [Fact(DisplayName = "UAC02: media block switches the root to flex-direction: column")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RootBecomesColumn()
    {
        var block = ExtractMediaBlock(LoadCss(), MediaQuery);
        block.ShouldContain(".explorer-layout",
            customMessage: "The media block must target `.explorer-layout` to flip the root.");
        block.ShouldContain("flex-direction: column",
            customMessage:
                "The media block must set `flex-direction: column` on .explorer-layout " +
                "so the List pane stacks above the Detail pane on narrow viewports.");
    }

    [Fact(DisplayName = "UAC03: media block lets the layout grow beyond viewport height")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RootScrollsVertically()
    {
        var block = ExtractMediaBlock(LoadCss(), MediaQuery);
        block.ShouldContain("height: auto",
            customMessage:
                "Stacked layout must release the desktop `h-[calc(100vh-3.5rem)]` " +
                "constraint to `height: auto` so both panes can be visible.");
        block.ShouldContain("overflow-y: auto",
            customMessage:
                "Stacked layout must let the layout root scroll vertically — " +
                "the desktop `overflow-hidden` would otherwise clip the lower pane.");
    }

    [Fact(DisplayName = "UAC04: media block makes the sidebar full width")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SidebarFullWidth()
    {
        var block = ExtractMediaBlock(LoadCss(), MediaQuery);
        block.ShouldContain(".explorer-sidebar",
            customMessage: "The media block must override the sidebar width.");
        block.ShouldContain("width: 100%",
            customMessage:
                "The sidebar's inline `width: var(--panel-width)` (set by " +
                "PanelSplitter persistence) must be overridden to 100% in stacked mode.");
        block.ShouldContain("max-width: 100%",
            customMessage:
                "The sidebar's inline `max-width: calc(100% - 336px)` must be " +
                "released so the sidebar can fill the row.");
    }

    [Fact(DisplayName = "UAC05: media block hides the panel splitter")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SplitterHidden()
    {
        var block = ExtractMediaBlock(LoadCss(), MediaQuery);
        block.ShouldContain("[data-slot=\"panel-splitter\"]",
            customMessage:
                "The media block must hide the PanelSplitter — its drag handle " +
                "is meaningless when the panes are stacked vertically.");
        block.ShouldContain("display: none",
            customMessage: "PanelSplitter must be hidden via display: none.");
    }

    [Fact(DisplayName = "UAC06: media block targets the .explorer-detail wrapper")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DetailPaneTargeted()
    {
        var block = ExtractMediaBlock(LoadCss(), MediaQuery);
        block.ShouldContain(".explorer-detail",
            customMessage:
                "The media block must target `.explorer-detail` so the right " +
                "pane gets the stacked-mode flex/overflow rules. ExplorerLayout.razor " +
                "must keep the matching class on the right-pane wrapper.");
    }

    [Fact(DisplayName = "UAC07: ExplorerLayout right pane carries the .explorer-detail class")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RazorMarkupCarriesDetailClass()
    {
        var razor = LoadExplorerLayoutRazor();
        razor.ShouldContain("class=\"explorer-detail",
            customMessage:
                "ExplorerLayout.razor must render the right pane with the " +
                "`explorer-detail` class — that's the hook the stacked-mode CSS " +
                "rule attaches to. Removing the class breaks the responsive stack.");
    }

    [Fact(DisplayName = "UAC08: src/web/Components/App.razor links explorer-layout.css")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_WebAppLinksCss()
    {
        var html = ReadEmbedded("web.App.razor");
        html.ShouldContain(CssLink,
            customMessage:
                "src/web/Components/App.razor must link " +
                "`_content/LowlandTech.Components/css/explorer-layout.css` in <head>. " +
                "Without it, the rules never reach the browser running on port 7088.");
    }

    [Fact(DisplayName = "UAC09: src/shell/Features/Shell/App.razor links explorer-layout.css")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ShellAppLinksCss()
    {
        var html = ReadEmbedded("shell.App.razor");
        html.ShouldContain(CssLink,
            customMessage:
                "src/shell/Features/Shell/App.razor must link " +
                "`_content/LowlandTech.Components/css/explorer-layout.css` in <head>.");
    }

    [Fact(DisplayName = "UAC10: src/app/Components/App.razor links explorer-layout.css")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_AdminAppLinksCss()
    {
        var html = ReadEmbedded("app.App.razor");
        html.ShouldContain(CssLink,
            customMessage:
                "src/app/Components/App.razor must link " +
                "`_content/LowlandTech.Components/css/explorer-layout.css` in <head>.");
    }

    [Fact(DisplayName = "UAC11: ExplorerLayout renders the collapsed class from component state")]
    [Trait(Spec.UAC, "11")]
    [Given("ExplorerLayout owns collapsed sidebar state")]
    [When("the layout root class is rendered")]
    [Then("the explorer-collapsed class is included through the Blazor class binding")]
    public void UAC11_CollapsedClassIsRenderedFromState()
    {
        var razor = LoadExplorerLayoutRazor();

        razor.ShouldContain("private string LayoutClass",
            customMessage:
                "ExplorerLayout must compute its root class from component state, not only by mutating the DOM after render.");
        razor.ShouldContain("_collapsed ? \"explorer-collapsed\"",
            customMessage:
                "The collapsed state must render `explorer-collapsed` so the show-sidebar button can restore the hidden pane reliably.");
        razor.ShouldContain("class=\"@LayoutClass\"",
            customMessage:
                "The ExplorerLayout root element must use LayoutClass for the collapse CSS contract.");
    }

    [Fact(DisplayName = "UAC12: sidebar toggles use the shared pane toggle component")]
    [Trait(Spec.UAC, "12")]
    [Given("ExplorerLayout renders open and collapse controls for its left pane")]
    [When("the ExplorerLayout markup is declared")]
    [Then("both controls use the shared PaneToggleButton component instead of local button chrome")]
    public void UAC12_SidebarTogglesUseSharedPaneToggleComponent()
    {
        var razor = LoadExplorerLayoutRazor();

        razor.ShouldContain("<PaneToggleButton Side=\"left\"",
            customMessage:
                "ExplorerLayout should share the same pane toggle component as SessionContent instead of declaring a local button.");
        razor.ShouldContain("@if (ShowLeftPane && _collapsed)",
            customMessage:
                "The show-sidebar button must only render while the left sidebar is collapsed; NodeTypes and EdgeTypes should not show it when the sidebar is open.");
        razor.ShouldContain("Action=\"open\"",
            customMessage:
                "The collapsed ExplorerLayout toggle must use the shared open variant.");
        razor.ShouldContain("Action=\"close\"",
            customMessage:
                "The visible sidebar header must use the shared close variant integrated into the title row.");
        razor.ShouldContain("BorderSide=\"right\"",
            customMessage:
                "The sidebar collapse button sits at the leading edge of the title row and must divide itself from the title with a right border.");
        razor.ShouldContain("Class=\"explorer-sidebar-toggle absolute top-0 left-0 z-50\"",
            customMessage:
                "The collapsed reveal button may position the shared component, but should not own its visual chrome.");
        razor.ShouldContain("Class=\"explorer-sidebar-collapse-btn shrink-0\"",
            customMessage:
                "The collapse button should stay in the existing sidebar header/title row.");
        razor.IndexOf("TestId=\"explorer-sidebar-collapse\"", StringComparison.Ordinal)
            .ShouldBeLessThan(
                razor.IndexOf("explorer-sidebar-header-slot", StringComparison.Ordinal),
                "The collapse button must be rendered before the title slot, not after the NodeTypes/EdgeTypes count.");
        razor.ShouldNotContain("private const string SidebarOpenButtonClass",
            customMessage:
                "ExplorerLayout must not re-declare the shared session header button class.");
        razor.ShouldNotContain("h-7 w-7",
            customMessage:
                "The old compact Explorer-only sizing should not return.");
        razor.ShouldNotContain("top-0.5 left-0.5",
            customMessage:
                "The reveal button should align flush with the header grid, not float inset from it.");
    }
}
