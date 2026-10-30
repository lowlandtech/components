using System.Text.Json.Nodes;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

// The probe exercises the component surface that the trim suppressions of the
// packages cover. A trimmed publish removes the members that nothing proves are
// needed, so every observation below is a preservation check: the same code that
// works untrimmed has to work here, in a trimmed process.
var observations = new Dictionary<string, object?>(StringComparer.Ordinal);

// 1. Descriptor discovery over the loaded assemblies (Assembly.GetTypes,
//    Type.GetProperties, Activator.CreateInstance, provider options).
var registry = new ComponentDescriptorRegistry();
var card = registry.All.FirstOrDefault(descriptor => descriptor.Element == "probe-card");
observations["discovered"] = card is not null;
observations["properties"] = card?.Properties.Select(property => property.Name).ToArray() ?? [];
observations["choiceOptions"] = card?.Properties.FirstOrDefault(property => property.Name == "Choice")?.Options ?? [];
observations["titleLabel"] = card?.Properties.FirstOrDefault(property => property.Name == "Title")?.Label;
observations["titleDefault"] = card?.Properties.FirstOrDefault(property => property.Name == "Title")?.DefaultValue;

// 2. The dynamic block descriptor of a runtime entity type (Type.GetProperties
//    over a model that the descriptor receives at run time).
var dynamicDescriptor = ReflectionDynamicBlockDescriptor.Create(typeof(ProbeModel));
observations["dynamicFields"] = dynamicDescriptor.Fields.Select(field => field.Key).ToArray();

// 3. The session prompt payload (the generated JSON contract of src/lib).
var context = new SessionContext();
using (var entity = System.Text.Json.JsonDocument.Parse("{\"value\":7}"))
{
    context.Publish(Guid.NewGuid(), "probe", entity.RootElement.Clone());
}
observations["promptJson"] = context.ToPromptJson();

// 4. The container grid layout payload and its restore path (the generated JSON
//    contract and the section default-value capture).
var grid = new ContainerGrid();
grid.SetLayout(new ContainerGridLayout(2, 2, [new ContainerSectionLayout("s1", 1, 1, 1, 1)]));
var layoutJson = await grid.SaveLayoutAsync();
observations["gridJson"] = layoutJson;
await grid.RestoreLayoutAsync(layoutJson);
observations["gridColumnsAfterRestore"] = grid.Columns;

// 5. A real render of the DataForm component: field discovery, value snapshot,
//    display value and the field rendering all run inside the trimmed process.
var services = new ServiceCollection().AddLogging().AddLocalization().BuildServiceProvider();
await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
var model = new ProbeModel { Name = "Ada", Age = 36 };
var html = await renderer.Dispatcher.InvokeAsync(async () =>
{
    var output = await renderer.RenderComponentAsync<DataForm>(
        ParameterView.FromDictionary(new Dictionary<string, object?> { ["Model"] = model }));
    return output.ToHtmlString();
});
observations["dataFormRendered"] = html.Length > 0;
observations["dataFormHasNameField"] = html.Contains("Name", StringComparison.Ordinal);
observations["dataFormHasAgeField"] = html.Contains("Age", StringComparison.Ordinal);

// 6. Designable parameter detection over a component instance (TinyComponentBase
//    reads the public properties of the instance type).
var designable = new ProbeCard();
observations["designable"] = designable.HasDesignableParametersProbe;

// 7. The story registry scanning a loaded assembly (Assembly.GetTypes over the
//    assembly the host hands over).
StoryRegistry.RegisterAllStories(typeof(ProbeStory).Assembly);
observations["storyRegistered"] = StoryRegistry.GetAll().Any(story => story.Type == typeof(ProbeStory));

// The result is built as a JsonNode so the probe itself needs no reflection-based
// serialization: the probe reports, it does not prove the serializer.
var result = new JsonObject();
foreach (var (key, value) in observations)
{
    result[key] = value switch
    {
        null => null,
        string text => JsonValue.Create(text),
        bool flag => JsonValue.Create(flag),
        int number => JsonValue.Create(number),
        IEnumerable<string> items => new JsonArray(items.Select(item => (JsonNode?)JsonValue.Create(item)).ToArray()),
        _ => JsonValue.Create(value.ToString())
    };
}

Console.WriteLine("PROBE:" + result.ToJsonString());

/// <summary>The entity model the probe hands to the dynamic block descriptor.</summary>
public sealed class ProbeModel
{
    public string Name { get; set; } = "Ada";

    public int Age { get; set; } = 36;
}

/// <summary>A story the registry discovers by scanning the host assembly.</summary>
[System.ComponentModel.DisplayName("Probe/Story")]
public sealed class ProbeStory : StoryComponent
{
}

/// <summary>A designable component the descriptor registry discovers.</summary>
[Properties(Element = "probe-card", Group = "Probe", Description = "CTRM0001 preservation probe")]
public sealed class ProbeCard : TinyComponentBase
{
    [Parameter, PropertyGrid(Label = "Probe title")]
    public string Title { get; set; } = "hello";

    [Parameter, PropertyGrid(Editor = PropertyEditor.Select, Options = ["one", "two"])]
    public string Choice { get; set; } = "one";

    /// <summary>Exposes the protected designable detection to the probe.</summary>
    public bool HasDesignableParametersProbe => HasPropertyGridParameters;
}
