namespace LowlandTech.Components;

public class StoryMetadata
{
    public string Slug { get; set; } = "";
    public string RouteSlug { get; set; } = "";
    public string Name { get; set; } = "";
    public Type Type { get; set; } = typeof(StoryComponent);
}
