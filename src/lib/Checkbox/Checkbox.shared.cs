namespace LowlandTech.Components.Checkbox;

/// <summary>
/// Extends the auto-generated Checkbox partial class.
/// </summary>
public partial class Checkbox
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public bool Disabled { get; set; }
}
