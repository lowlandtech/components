namespace LowlandTech.Components;

/// <summary>
/// Extends the auto-generated Textarea partial class.
/// </summary>
public partial class Textarea
{
    /// <summary>
    /// The number of rows for the textarea.
    /// </summary>
    [Parameter] public int Rows { get; set; } = 3;

    /// <summary>
    /// The placeholder text.
    /// </summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// Whether the textarea is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Whether the textarea is read-only.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Whether the textarea has an invalid/error state.
    /// </summary>
    [Parameter] public bool Invalid { get; set; }

    /// <summary>
    /// The value of the textarea.
    /// </summary>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// Callback when the value changes.
    /// </summary>
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Callback when the textarea loses focus.
    /// </summary>
    [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

    /// <summary>
    /// Callback when the textarea receives focus.
    /// </summary>
    [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>
    /// Callback when the change event fires.
    /// </summary>
    [Parameter] public EventCallback<ChangeEventArgs> OnChange { get; set; }
}
