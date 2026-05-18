namespace LowlandTech.Components.Stories;

public sealed class StoriesAppPlugin : IAppPlugin
{
    public PluginDescriptor Descriptor { get; } = new()
    {
        Id = "components-stories",
        Name = "Component Stories",
        Version = "1.0.0",
    };

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ShowcodeState>();
        StoryRegistry.RegisterAllStories(typeof(StoriesAppPlugin).Assembly);
    }

    public void Use(object host)
    {
        if (host is IServiceProvider sp)
            sp.GetRequiredService<IRouteAssemblyRegistry>().Add(typeof(StoriesAppPlugin).Assembly);
    }

    public IReadOnlyList<string> StylesheetLinks =>
    [
        "_content/LowlandTech.Components.Stories/css/stories.css",
        "_content/LowlandTech.Components.Stories/css/sonner.css",
        "_content/LowlandTech.Components.Stories/css/rtl.css",
    ];
}
