namespace LowlandTech.Components.E2E.Infrastructure;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class GivenAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class WhenAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ThenAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}
