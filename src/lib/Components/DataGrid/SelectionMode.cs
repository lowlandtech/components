namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// Specifies the row selection mode for a DataGrid.
/// </summary>
public enum SelectionMode
{
    /// <summary>
    /// No row selection is allowed.
    /// </summary>
    None,

    /// <summary>
    /// Only one row can be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple rows can be selected.
    /// </summary>
    Multiple
}
