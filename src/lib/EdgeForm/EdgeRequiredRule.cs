namespace LowlandTech.Components.EdgeForm;

public class EdgeRequiredRule(string fieldName) : IEdgeRule
{
    public string Name => $"required:{fieldName}";
    public string FailMessage => $"{fieldName} is required.";

    public bool Evaluate(IReadOnlyDictionary<string, object?> values)
    {
        if (!values.TryGetValue(fieldName, out var v)) return false;
        return v switch
        {
            null => false,
            string s => !string.IsNullOrWhiteSpace(s),
            bool b => b,
            _ => true
        };
    }
}
