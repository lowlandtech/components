using LowlandTech.Core.Plugins;

namespace LowlandTech.Components.Tests.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// A plugin page component the host resolves through its trusted startup map.
/// </summary>
/// <remarks>
/// The component carries the real Components-owned page attribute, so the
/// acceptance inventory proves that the adapter reads the metadata a plugin
/// really declares instead of a metadata value the test invented.
/// </remarks>
[PluginPage(Label = "Reports", Icon = "table", Order = 20, Group = "Operations", SubGroup = "Daily")]
public sealed class FakePluginPage : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder) =>
        builder.AddMarkupContent(0, "<section data-testid=\"fake-plugin-page\">plugin page</section>");
}
