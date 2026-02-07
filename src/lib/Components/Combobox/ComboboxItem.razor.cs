using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Combobox;

/// <summary>
/// ComboboxItem component that represents an item in the combobox list.
/// </summary>
public partial class ComboboxItem : ComponentBase, IDisposable
{
    /// <summary>
    /// The content to render for this item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The value of this item.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// The label/text to display and search against.
    /// If not provided, uses the Value.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// The parent Combobox component.
    /// </summary>
    [CascadingParameter]
    public Combobox? Parent { get; set; }

    protected override void OnInitialized()
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("ComboboxItem must be used inside a Combobox component");
        }

        // Use Value as Label if Label is not provided
        if (string.IsNullOrEmpty(Label))
        {
            Label = Value;
        }

        Parent.RegisterItem(this);
    }

    public void Dispose()
    {
        Parent?.UnregisterItem(this);
    }
}
