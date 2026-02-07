using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "56")]
public class SC56_DataGridColumnFooters : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then a footer row should appear at the bottom")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_FooterRowAppearsAtBottom()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when FooterTemplate feature is available
        // Given I have columns with FooterTemplate
        // When I render the DataGrid
        // Then a footer row should appear at the bottom
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And each column's FooterTemplate should render")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_EachColumnFooterTemplateRenders()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when FooterTemplate rendering is available
        // When I render the DataGrid
        // Then each column's FooterTemplate should render
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can display aggregate values or summaries")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_CanDisplayAggregateValuesOrSummaries()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when footer aggregates are available
        // When I render the DataGrid
        // Then I can display aggregate values or summaries
    }
}
