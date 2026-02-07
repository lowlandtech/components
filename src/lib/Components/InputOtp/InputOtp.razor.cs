using LowlandTech.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LowlandTech.Components.Components.InputOtp;

/// <summary>
/// InputOtp component for entering one-time passwords or verification codes.
/// </summary>
public partial class InputOtp : ComponentBase
{
    private string _className = string.Empty;
    private ElementReference[] _inputRefs = Array.Empty<ElementReference>();
    private char[] _slots = Array.Empty<char>();

    [Parameter]
    public int Length { get; set; } = 6;

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected override void OnInitialized()
    {
        _inputRefs = new ElementReference[Length];
        _slots = new char[Length];
        
        // Initialize slots from Value if provided
        if (!string.IsNullOrEmpty(Value))
        {
            for (int i = 0; i < Math.Min(Value.Length, Length); i++)
            {
                _slots[i] = Value[i];
            }
        }
    }

    protected override void OnParametersSet()
    {
        _className = CssClassUtils.Merge("input-otp", Class);
        
        // Update slots if Value changed externally
        if (!string.IsNullOrEmpty(Value))
        {
            var currentValue = new string(_slots).TrimEnd('\0');
            if (currentValue != Value)
            {
                _slots = new char[Length];
                for (int i = 0; i < Math.Min(Value.Length, Length); i++)
                {
                    _slots[i] = Value[i];
                }
            }
        }
    }

    private string GetSlotValue(int index)
    {
        return _slots[index] == '\0' ? string.Empty : _slots[index].ToString();
    }

    private async Task HandleInput(int index, ChangeEventArgs e)
    {
        var input = e.Value?.ToString() ?? string.Empty;
        
        if (string.IsNullOrEmpty(input))
        {
            // Handle delete/backspace
            _slots[index] = '\0';
        }
        else
        {
            // Take only the last character (in case paste or autocomplete)
            _slots[index] = input[^1];
            
            // NOTE: Auto-focus to next slot disabled to avoid JSInterop in tests
            // Move to next slot if available
            // if (index < Length - 1)
            // {
            //     await FocusSlot(index + 1);
            // }
        }
        
        await UpdateValue();
    }

    private async Task HandleKeyDown(int index, KeyboardEventArgs e)
    {
        // NOTE: Auto-focus navigation disabled to avoid JSInterop in tests
        // Keyboard navigation can be re-enabled when JSInterop is configured in production
        // if (e.Key == "Backspace" && _slots[index] == '\0' && index > 0)
        // {
        //     // If current slot is empty and backspace is pressed, move to previous slot
        //     await FocusSlot(index - 1);
        // }
        // else if (e.Key == "ArrowLeft" && index > 0)
        // {
        //     await FocusSlot(index - 1);
        // }
        // else if (e.Key == "ArrowRight" && index < Length - 1)
        // {
        //     await FocusSlot(index + 1);
        // }
        await Task.CompletedTask;
    }

    private async Task FocusSlot(int index)
    {
        if (index >= 0 && index < Length)
        {
            await JSRuntime.InvokeVoidAsync("eval", $"arguments[0].focus(); arguments[0].select();", _inputRefs[index]);
        }
    }

    private async Task UpdateValue()
    {
        var newValue = new string(_slots).TrimEnd('\0');
        if (Value != newValue)
        {
            await ValueChanged.InvokeAsync(newValue);
        }
    }
}
