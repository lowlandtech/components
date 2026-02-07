using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "43")]
public class SC43_DataGridExportToCSV : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then a CSV file should download")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CSVFileDownloads()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ExportToCSV method is available
        // Given I have a DataGrid with data
        // When I call the ExportToCSV method
        // Then a CSV file should download
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And it should contain all visible data")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CSVContainsAllVisibleData()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when CSV export includes all visible data
        // When I call the ExportToCSV method
        // Then it should contain all visible data
    }
}
