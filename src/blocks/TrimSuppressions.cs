using System.Diagnostics.CodeAnalysis;

// The Razor source generator renders the session block components with
// DynamicComponent. Its Type setter carries a DynamicallyAccessedMembers
// requirement, and the generated render tree sets the parameter through the
// reflection-based parameter assignment that Microsoft.AspNetCore.Components
// uses for a component parameter, which the trimmer cannot verify. The value is
// the component type that the descriptor registry discovered for the block, and
// the host that publishes trimmed preserves the discovered component types and
// their members, which the trimmed-host preservation probe checks. The
// suppression names the two generated render methods, so no other IL2111 is
// silenced.
[assembly: UnconditionalSuppressMessage(
    "Trimming",
    "IL2111",
    Scope = "member",
    Target = "~M:LowlandTech.Components.SessionContextBlock.BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder)",
    Justification = "The generated render tree assigns a discovered component type to DynamicComponent.Type; the trimmed host preserves the discovered component types.")]
[assembly: UnconditionalSuppressMessage(
    "Trimming",
    "IL2111",
    Scope = "member",
    Target = "~M:LowlandTech.Components.SessionContextBlockOverlayContent.BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder)",
    Justification = "The generated render tree assigns a discovered component type to DynamicComponent.Type; the trimmed host preserves the discovered component types.")]
