namespace LowlandTech.Components.Tests.CBLK0001.Integration.US01.ActiveOwnedBlocks;

/// <summary>
/// The component a plugin owner contributes to the session surface.
///
/// <para>
/// The type is public and lives in the test assembly on purpose: the
/// assembly-scanning fallback of the session block registry publishes every
/// loaded component under its own type name, so this type is exactly the
/// reappearance path the withdrawal has to close. The bridge must never let
/// this type become reachable under its raw type name, neither while the owner
/// is active nor after it withdrew.
/// </para>
/// </summary>
public sealed class FakePluginSessionBlock : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder) =>
        builder.AddContent(0, "Plugin session block");
}
