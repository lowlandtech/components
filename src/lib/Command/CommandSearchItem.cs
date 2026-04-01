using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Command;

public class CommandSearchItem
{
    public string Value { get; set; }
    public string Label { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public RenderFragment? Icon { get; set; }
    public string? Shortcut { get; set; }
    public Action? OnExecute { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public CommandSearchItem()
    {
        Value = string.Empty;
        Label = string.Empty;
    }

    public CommandSearchItem(string value, string label, string? url = null, string? description = null, RenderFragment? icon = null, string? shortcut = null)
    {
        Value = value;
        Label = label;
        Url = url;
        Description = description;
        Icon = icon;
        Shortcut = shortcut;
    }
}
