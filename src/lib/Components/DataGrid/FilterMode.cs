namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// Defines the filter mode for the DataGrid.
/// </summary>
public enum FilterMode
{
    /// <summary>
    /// No filtering.
    /// </summary>
    None,

    /// <summary>
    /// Simple filter row with text inputs.
    /// </summary>
    Simple,

    /// <summary>
    /// Advanced filter menu with operators and multiple conditions.
    /// </summary>
    Advanced
}
