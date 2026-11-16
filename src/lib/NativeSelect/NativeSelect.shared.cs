namespace LowlandTech.Components;

public partial class NativeSelect
{
    /// <summary>
    /// The selected value. Two-way bindable like the rest of the form components:
    /// a caller writes <c>@bind-Value</c> and the control raises the change.
    /// </summary>
    [Parameter, PropertyGrid(Group = "Behavior", Description = "The selected option value")]
    public string? Value { get; set; }

    /// <summary>Raised with the newly selected value.</summary>
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    private string SizeValue => string.IsNullOrWhiteSpace(size) ? "default" : size!;

    private async Task HandleChangeAsync(ChangeEventArgs args)
    {
        var next = args.Value?.ToString();
        Value = next;
        await ValueChanged.InvokeAsync(next);
    }

    private string WrapperClass
    {
        get
        {
            // w-full so the wrapper fills the column it lives in (form
            // rows, grid cells, flex children). Was w-fit which made
            // the wrapper content-sized — fine for "shrink to label"
            // contexts but broken in "fill the row's value cell" layouts
            // where the chevron icon would clip awkwardly.
            const string baseClass = "cn-native-select-wrapper group/native-select relative inline-flex w-full has-[select:disabled]:opacity-50";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
