using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Sheet;

public partial class Sheet : ComponentBase
{
    private bool _open;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Open
    {
        get => _open;
        set
        {
            if (_open != value)
            {
                _open = value;
                OpenChanged.InvokeAsync(_open);
            }
        }
    }

    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    internal void OpenSheet()
    {
        Open = true;
        StateHasChanged();
    }

    internal void CloseSheet()
    {
        Open = false;
        StateHasChanged();
    }
}
