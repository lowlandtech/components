using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Command;

public partial class CommandList : ComponentBase
{
    [CascadingParameter]
    public Command? Parent { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string EmptyMessage { get; set; } = "No results found.";

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string RootClass => CssClassUtils.Merge("command-list", Class);
}
