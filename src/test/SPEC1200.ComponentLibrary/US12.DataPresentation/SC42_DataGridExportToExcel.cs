using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "42")]
public class SC42_DataGridExportToExcel : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then an Excel file should download")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ExcelFileDownloads()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ExportToExcel method is available
        // Given I have a DataGrid with data
        // When I call the ExportToExcel method
        // Then an Excel file should download
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And it should contain all visible columns and rows")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ExcelContainsAllVisibleColumnsAndRows()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Excel export includes all visible data
        // When I call the ExportToExcel method
        // Then it should contain all visible columns and rows
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And formatting should be preserved where possible")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_FormattingIsPreserved()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Excel export preserves formatting
        // When I call the ExportToExcel method
        // Then formatting should be preserved where possible
    }
}
