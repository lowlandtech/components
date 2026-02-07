using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC10: DataGrid supports filter templates
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "10")]
public class SC10_DataGridSupportsFilterTemplates : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Filter templates not yet implemented", DisplayName = "Then the column should use the custom filter template")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_UsesCustomFilterTemplate()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when filter templates are available
    }

    [Fact(Skip = "Filter templates not yet implemented", DisplayName = "And I can implement any custom filtering UI")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_SupportsCustomFilteringUI()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when filter templates are available
    }
}
