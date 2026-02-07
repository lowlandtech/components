using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC36: DataGrid supports column visibility
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC36 - DataGrid column visibility
/// 
/// Given a DataGrid with columns using Visible property
/// When a column's Visible is set to false
/// Then that column should not render
/// And I can toggle visibility programmatically
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "36")]
public class SC36_DataGridColumnVisibility
{
    private record TestItem(int Id, string Name, decimal Price);

    [Fact(DisplayName = "Column with Visible=false does not render")]
    [Trait(Spec.UAC, "01")]
    public void Column_WithVisibleFalse_DoesNotRender()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When - Create grid with 3 columns, but one is invisible
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                // Visible column
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.AddAttribute(3, "Visible", true);
                builder.CloseComponent();

                // Hidden column
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Name");
                builder.AddAttribute(6, "Property", "Name");
                builder.AddAttribute(7, "Visible", false);
                builder.CloseComponent();

                // Visible column
                builder.OpenComponent<DataGridColumn<TestItem>>(8);
                builder.AddAttribute(9, "Title", "Price");
                builder.AddAttribute(10, "Property", "Price");
                builder.AddAttribute(11, "Visible", true);
                builder.CloseComponent();
            }));

        // Then - Only 2 columns should render
        var headerCells = cut.FindAll("thead tr th");
        Assert.Equal(2, headerCells.Count);
        Assert.Contains("ID", headerCells[0].TextContent);
        Assert.Contains("Price", headerCells[1].TextContent);

        var dataCells = cut.FindAll("tbody tr td");
        Assert.Equal(2, dataCells.Count);
        Assert.Contains("1", dataCells[0].TextContent);
        // Price could be formatted with comma or period depending on culture
        var priceText = dataCells[1].TextContent;
        Assert.True(priceText.Contains("99") && priceText.Contains("99"), 
            $"Expected price to contain '99', but got '{priceText}'");
    }

    [Fact(DisplayName = "Columns are visible by default when Visible is not set")]
    [Trait(Spec.UAC, "02")]
    public void Column_WithoutVisibleProperty_IsVisibleByDefault()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When - Create grid without specifying Visible property
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                // No Visible property set - should default to true
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(2);
                builder.AddAttribute(3, "Title", "Name");
                builder.AddAttribute(4, "Property", "Name");
                // No Visible property set - should default to true
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(5);
                builder.AddAttribute(6, "Title", "Price");
                builder.AddAttribute(7, "Property", "Price");
                // No Visible property set - should default to true
                builder.CloseComponent();
            }));

        // Then - All 3 columns should render
        var headerCells = cut.FindAll("thead tr th");
        Assert.Equal(3, headerCells.Count);

        var dataCells = cut.FindAll("tbody tr td");
        Assert.Equal(3, dataCells.Count);
    }
}
