using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Sidebar;

public partial class SidebarTrigger : ComponentBase
{
    [CascadingParameter] private SidebarProvider? Provider { get; set; }

    [Parameter] public string? Class { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string Classes => CssClassUtils.Merge(Class);

    private Dictionary<string, object> MergedAttributes
    {
        get
        {
            var attrs = new Dictionary<string, object>
            {
                ["data-sidebar"] = "trigger",
                ["data-slot"] = "sidebar-trigger"
            };

            if (AdditionalAttributes != null)
            {
                foreach (var attr in AdditionalAttributes)
                {
                    attrs[attr.Key] = attr.Value;
                }
            }

            return attrs;
        }
    }

    private async Task HandleClickAsync(MouseEventArgs args)
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }

        if (Provider != null)
        {
            await Provider.ToggleSidebarAsync();
        }
    }
}
