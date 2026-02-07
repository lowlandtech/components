using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC35: DataGrid supports column picking
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC35 - DataGrid column picking
/// 
/// Given a DataGrid with AllowColumnPicking enabled
/// When I render the DataGrid
/// Then a column picker dropdown should appear
/// And I can show/hide columns by selecting them
/// And hidden columns should not appear in the grid
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "35")]
public class SC35_DataGridSupportsColumnPicking
{
    private record TestItem(int Id, string Name, decimal Price);

    [Fact(DisplayName = "Column picker appears when AllowColumnPicking is enabled")]
    [Trait(Spec.UAC, "01")]
    public void ColumnPicker_WhenEnabled_Appears()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When - Create grid with AllowColumnPicking enabled
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowColumnPicking, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Title", "Name");
                builder.AddAttribute(5, "Property", "Name");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(6);
                builder.AddAttribute(7, "Title", "Price");
                builder.AddAttribute(8, "Property", "Price");
                builder.CloseComponent();
            }));

        // Then - Column picker should be visible
        var columnPicker = cut.Find(".datagrid-column-picker");
        Assert.NotNull(columnPicker);
        
        // And should have checkboxes for all columns
        var checkboxes = cut.FindAll(".column-picker-item input[type='checkbox']");
        Assert.Equal(3, checkboxes.Count);
    }

    [Fact(DisplayName = "Can toggle column visibility via column picker")]
    [Trait(Spec.UAC, "02")]
    public void ColumnPicker_CanToggleColumnVisibility()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When - Create grid with column picker
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowColumnPicking, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Title", "Name");
                builder.AddAttribute(5, "Property", "Name");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(6);
                builder.AddAttribute(7, "Title", "Price");
                builder.AddAttribute(8, "Property", "Price");
                builder.CloseComponent();
            }));

        // Then - Initially all columns should be visible
        var headerCells = cut.FindAll("thead tr th");
        Assert.Equal(3, headerCells.Count);

        // When - Uncheck the second column (Name)
        var nameCheckbox = cut.FindAll(".column-picker-item input[type='checkbox']")[1];
        nameCheckbox.Change(false);

        // Then - Only 2 columns should be visible
        headerCells = cut.FindAll("thead tr th");
        Assert.Equal(2, headerCells.Count);
        Assert.Contains("ID", headerCells[0].TextContent);
        Assert.Contains("Price", headerCells[1].TextContent);
    }

    [Fact(DisplayName = "Hidden columns via picker do not appear in grid")]
    [Trait(Spec.UAC, "03")]
    public void ColumnPicker_HiddenColumns_DoNotAppearInGrid()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m),
            new(2, "Product B", 149.99m)
        };

        // When - Create grid with column picker
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowColumnPicking, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Title", "Name");
                builder.AddAttribute(5, "Property", "Name");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(6);
                builder.AddAttribute(7, "Title", "Price");
                builder.AddAttribute(8, "Property", "Price");
                builder.CloseComponent();
            }));

        // When - Hide first column (ID)
        var idCheckbox = cut.FindAll(".column-picker-item input[type='checkbox']")[0];
        idCheckbox.Change(false);

        // Then - ID column should not appear in headers
        var headerCells = cut.FindAll("thead tr th");
        Assert.Equal(2, headerCells.Count);
        Assert.DoesNotContain(headerCells, cell => cell.TextContent.Contains("ID"));

        // And - ID data should not appear in rows
        var dataCells = cut.FindAll("tbody tr td");
        Assert.Equal(4, dataCells.Count); // 2 rows × 2 visible columns
        
        // Verify the visible data
        Assert.Contains("Product A", dataCells[0].TextContent);
        Assert.Contains("Product B", dataCells[2].TextContent);
    }
}
