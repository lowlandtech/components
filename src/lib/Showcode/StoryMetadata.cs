namespace LowlandTech.Components;

public class StoryMetadata
{
    public string Slug { get; set; } = "";
    public string RouteSlug { get; set; } = "";
    public string Name { get; set; } = "";

    // The showcase pages hand this type straight to DynamicComponent, whose Type member
    // declares that the component keeps its members. The metadata carries the same
    // requirement, so the trimmer can prove the render instead of warning at it.
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    public Type Type { get; set; } = typeof(StoryComponent);
}
