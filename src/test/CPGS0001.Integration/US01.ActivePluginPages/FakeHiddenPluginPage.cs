using LowlandTech.Core.Plugins;

namespace LowlandTech.Components.Tests.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// A plugin page component that declares itself for direct routing but keeps
/// itself out of the navigation.
/// </summary>
[PluginPage(Label = "Hidden", Icon = "eye-off", Order = 30, Group = "Operations", ShowInNav = false)]
public sealed class FakeHiddenPluginPage : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder) =>
        builder.AddMarkupContent(0, "<section data-testid=\"fake-hidden-plugin-page\">hidden plugin page</section>");
}
