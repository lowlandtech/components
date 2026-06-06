using LowlandTech.Components.Core;
using LowlandTech.Components.Core.Validation;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US04.FieldEventDiscovery;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "FieldEventDiscoveryService")]
public class SC01_FieldEventDiscovery : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resolves default intent key when no registry registered")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DefaultIntentKey()
    {
        var service = new FieldEventDiscoveryService(Array.Empty<IFormFieldRegistry>());

        var intentKey = service.GetIntentKey("email", "Changing");

        intentKey.ShouldBe("email_Changing_intent");
    }

    [Fact(DisplayName = "UAC02: Registry with matching field returns custom intent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RegistryReturnsCustomIntent()
    {
        var registry = new TestFormFieldRegistry(new Dictionary<string, string>
        {
            ["Changing"] = "custom_email_changing"
        });

        var service = new FieldEventDiscoveryService(new[] { registry });

        var intentKey = service.GetIntentKey("email", "Changing");

        intentKey.ShouldBe("custom_email_changing");
    }

    [Fact(DisplayName = "UAC03: BuildIntent creates FormFieldIntent with payload")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_BuildIntentWithPayload()
    {
        var service = new FieldEventDiscoveryService(Array.Empty<IFormFieldRegistry>());

        var intent = service.BuildIntent("name", "Changed", "John");

        intent.FieldName.ShouldBe("name");
        intent.EventName.ShouldBe("Changed");
        intent.IntentKey.ShouldBe("name_Changed_intent");
        intent.Payload.ShouldBe("John");
    }

    [Fact(DisplayName = "UAC04: Multiple registries queried by priority")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_PriorityOrdering()
    {
        var lowPriority = new TestFormFieldRegistry(
            new Dictionary<string, string> { ["Blur"] = "low_priority" },
            priority: 1);

        var highPriority = new TestFormFieldRegistry(
            new Dictionary<string, string> { ["Blur"] = "high_priority" },
            priority: 100);

        var service = new FieldEventDiscoveryService(new[] { lowPriority, highPriority });

        var intentKey = service.GetIntentKey("any", "Blur");

        intentKey.ShouldBe("high_priority");
    }

    [Fact(DisplayName = "UAC05: Falls back to default when registry has no matching event")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_FallbackToDefault()
    {
        var registry = new TestFormFieldRegistry(new Dictionary<string, string>
        {
            ["Changing"] = "custom"
        });

        var service = new FieldEventDiscoveryService(new[] { registry });

        var intentKey = service.GetIntentKey("email", "Focus");

        intentKey.ShouldBe("email_Focus_intent");
    }

    private class TestFormFieldRegistry : IFormFieldRegistry
    {
        private readonly IReadOnlyDictionary<string, string> _eventMap;

        public TestFormFieldRegistry(IReadOnlyDictionary<string, string> eventMap, int priority = 0)
        {
            _eventMap = eventMap;
            Priority = priority;
        }

        public int Priority { get; }

        public IFormFieldBehavior? GetBehavior(FormFieldType fieldType) => null;

        public IFormFieldBehavior? GetBehavior(string fieldName) =>
            new TestFormFieldBehavior(_eventMap);
    }

    private class TestFormFieldBehavior : IFormFieldBehavior
    {
        public TestFormFieldBehavior(IReadOnlyDictionary<string, string> eventMap)
        {
            EventMap = eventMap;
        }

        public FormFieldType FieldType => FormFieldType.Text;
        public Type ComponentType => typeof(LowlandTech.Components.FormField);
        public IValidatorDescription? Validator => null;
        public string? Mask => null;
        public IReadOnlyDictionary<string, string> EventMap { get; }
    }
}
