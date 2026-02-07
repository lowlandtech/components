using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Command;

public partial class CommandInput : ComponentBase
{
    [CascadingParameter]
    public Command? Parent { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Value => Parent?.FilterValue ?? string.Empty;

    private string RootClass => CssClassUtils.Merge("command-input", Class);

    private async Task HandleInputAsync(ChangeEventArgs args)
    {
        var value = args.Value?.ToString() ?? string.Empty;
        Parent?.UpdateFilter(value);
        await Task.CompletedTask;
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (Parent != null)
        {
            await Parent.HandleKeyDownAsync(e);
        }
    }
}
