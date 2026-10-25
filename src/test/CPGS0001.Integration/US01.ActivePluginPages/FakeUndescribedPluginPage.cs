namespace LowlandTech.Components.Tests.CPGS0001.Integration.US01.ActivePluginPages;

/// <summary>
/// A plugin page component that carries no page metadata at all.
/// </summary>
/// <remarks>
/// A component without the page attribute is not a page. The adapter has to
/// refuse it by name instead of publishing an empty navigation entry for it.
/// </remarks>
public sealed class FakeUndescribedPluginPage : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder) =>
        builder.AddMarkupContent(0, "<section data-testid=\"fake-undescribed-plugin-page\">undescribed plugin page</section>");
}
