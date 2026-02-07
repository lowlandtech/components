using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Command;

public partial class CommandItem : ComponentBase, IDisposable
{
    [CascadingParameter]
    public Command? Parent { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public EventCallback OnSelect { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private int ItemIndex { get; set; }

    private bool IsSelected => Parent != null && Parent.SelectedIndex == ItemIndex;

    private bool IsVisible
    {
        get
        {
            if (Parent == null) return true;
            
            var visibleItems = Parent.GetVisibleItems();
            return visibleItems.Contains(this);
        }
    }

    private string ItemClass
    {
        get
        {
            var classes = new List<string> { "command-item" };
            
            if (IsSelected)
            {
                classes.Add("command-item-selected");
            }
            
            if (!string.IsNullOrEmpty(Class))
            {
                classes.Add(Class);
            }
            
            return string.Join(" ", classes);
        }
    }

    protected override void OnInitialized()
    {
        Parent?.RegisterItem(this);
        UpdateItemIndex();
    }

    private void UpdateItemIndex()
    {
        if (Parent != null)
        {
            ItemIndex = Parent.Items.IndexOf(this);
        }
    }

    private async Task HandleClickAsync()
    {
        await InvokeSelectAsync();
    }

    internal async Task InvokeSelectAsync()
    {
        if (OnSelect.HasDelegate)
        {
            await OnSelect.InvokeAsync();
        }
    }

    public void Dispose()
    {
        Parent?.UnregisterItem(this);
    }
}
