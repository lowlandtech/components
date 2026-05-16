namespace LowlandTech.Components.Button;

public partial class Button : TinyComponentBase
{
    /// <summary>
    /// Callback invoked when the button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    private async Task HandleOnClick(MouseEventArgs e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(e);
    }
}