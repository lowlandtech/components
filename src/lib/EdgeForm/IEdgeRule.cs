namespace LowlandTech.Components.EdgeForm;

public interface IEdgeRule
{
    string Name { get; }
    string FailMessage { get; }
    bool Evaluate(IReadOnlyDictionary<string, object?> values);
}
