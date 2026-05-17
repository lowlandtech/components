namespace LowlandTech.Components.App.Showcode;

public static class StoryRegistry
{
    private static readonly Dictionary<string, StoryMetadata> _stories = new();

    public static StoryGroup Root { get; private set; } = new() { Name = "Root" };

    public static IEnumerable<StoryMetadata> GetAll() => _stories.Values;

    public static StoryMetadata? GetBySlug(string slug) =>
        _stories.GetValueOrDefault(slug);

    public static StoryMetadata? GetFirst() => _stories.Values.FirstOrDefault();

    public static void Register(Type type)
    {
        var slug = type.FullName!.Replace(".", "_").ToLowerInvariant();
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? slug;
        var path = displayName.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var metadata = new StoryMetadata { Slug = slug, Name = path.Last(), Type = type };
        _stories[slug] = metadata;
        FileInto(Root, path, metadata);
    }

    public static void RegisterAllStories(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract) continue;
            if (type.IsSubclassOf(typeof(StoryComponent)))
                Register(type);
        }
    }

    private static void FileInto(StoryGroup group, string[] path, StoryMetadata metadata)
    {
        if (path.Length == 1)
        {
            group.Stories.Add(metadata);
            return;
        }

        var child = group.Children.FirstOrDefault(g => g.Name == path[0]);
        if (child is null)
        {
            child = new StoryGroup { Name = path[0] };
            group.Children.Add(child);
        }
        FileInto(child, path[1..], metadata);
    }
}
