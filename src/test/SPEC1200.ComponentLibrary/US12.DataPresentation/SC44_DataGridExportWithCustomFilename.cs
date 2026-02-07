using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "44")]
public class SC44_DataGridExportWithCustomFilename : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the downloaded file should use that name")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DownloadedFileUsesCustomName()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when export methods support custom filename parameter
        // Given I have a DataGrid
        // When I call export methods with a custom filename
        // Then the downloaded file should use that name
    }
}
