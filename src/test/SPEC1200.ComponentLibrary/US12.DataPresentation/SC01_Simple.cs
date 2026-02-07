using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "01")]
public class SC01_DataGridRendersBasicTableStructure_Simple : Scenario
{
    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact(DisplayName = "DataGrid with ChildContent displays headers")]
    public void DataGridWithChildContentDisplaysHeaders()
    {
        using var ctx = new BunitContext();
        
        var data = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Test" }
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Property", "Name");
                builder.AddAttribute(2, "Title", "Product Name");
                builder.CloseComponent();
            }));

        var markup = cut.Markup;
        Console.WriteLine(markup);
        
        var header = cut.Find("thead");
        Assert.Contains("Product Name", header.TextContent);
    }
}
