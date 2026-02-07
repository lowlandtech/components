using Xunit;
using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "40")]
public class SC40_DataGridLoadingIndicator : Scenario
{
    private record Person(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then a loading overlay should appear")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_LoadingOverlayAppears()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        var data = new List<Person> { new(1, "Alice"), new(2, "Bob") };

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.IsLoading, true)
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        var loadingOverlay = cut.Find(".datagrid-loading-overlay");
        loadingOverlay.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And the grid should indicate data is being fetched")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_GridIndicatesDataIsBeingFetched()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        var data = new List<Person> { new(1, "Alice"), new(2, "Bob") };

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.IsLoading, true)
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        var loadingIndicator = cut.Find(".datagrid-loading-indicator");
        loadingIndicator.ShouldNotBeNull();
        loadingIndicator.TextContent.ShouldContain("Loading");
    }
}
