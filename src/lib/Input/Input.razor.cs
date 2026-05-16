using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Input;

public partial class Input : TinyComponentBase
{
    [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

    private ElementReference _inputRef;

    public ValueTask FocusAsync()
        => _inputRef.FocusAsync();

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (OnKeyUp.HasDelegate)
            await OnKeyUp.InvokeAsync(e);
    }
}
