namespace LowlandTech.Components;

public class StoryGroup
{
    public string Name { get; set; } = "";
    public List<StoryMetadata> Stories { get; set; } = new();
    public List<StoryGroup> Children { get; set; } = new();
}
