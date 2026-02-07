using Xunit;
using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Microsoft.AspNetCore.Components;
using SortDir = LowlandTech.Components.Components.DataGrid.SortDirection;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "39")]
public class SC39_DataGridSupportsServerSideDataLoading : Scenario
{
    private record Person(int Id, string Name, int Age);
    private List<Person> _serverData = new();
    private LoadDataArgs? _lastLoadDataArgs;

    protected override Task GivenAsync()
    {
        // Initialize server data
        _serverData = new List<Person>
        {
            new(1, "Alice", 30),
            new(2, "Bob", 25),
            new(3, "Charlie", 35),
            new(4, "David", 40),
            new(5, "Eve", 28)
        };
        return Task.CompletedTask;
    }

    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then the LoadData event should fire")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_LoadDataEventFires()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        var loadDataCalled = false;

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.LoadData, EventCallback.Factory.Create<LoadDataArgs>(this, args =>
            {
                loadDataCalled = true;
                _lastLoadDataArgs = args;
            }))
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        // LoadData should fire on initialization
        loadDataCalled.ShouldBeTrue();
        _lastLoadDataArgs.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And LoadDataArgs should include Skip, Top, OrderBy, and Filter")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_LoadDataArgsIncludeParameters()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.LoadData, EventCallback.Factory.Create<LoadDataArgs>(this, args =>
            {
                _lastLoadDataArgs = args;
            }))
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        _lastLoadDataArgs.ShouldNotBeNull();
        _lastLoadDataArgs.Skip.ShouldBe(0); // First page
        _lastLoadDataArgs.Top.ShouldBe(2); // PageSize
        // OrderBy and Filter can be null initially
    }

    [Fact(DisplayName = "Then LoadData should fire with updated parameters")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_LoadDataFiresWithUpdatedParameters()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        var loadDataCallCount = 0;
        var clientData = new List<Person>();

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.Data, clientData) // Start with empty data
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.AllowFiltering, true)
            .Add(p => p.FilterMode, FilterMode.Simple)
            .Add(p => p.Count, 5) // Server total
            .Add(p => p.LoadData, EventCallback.Factory.Create<LoadDataArgs>(this, args =>
            {
                loadDataCallCount++;
                _lastLoadDataArgs = args;
                // Simulate server returning data
                clientData.Clear();
                clientData.AddRange(_serverData.Skip(args.Skip).Take(args.Top));
            }))
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name")
                .Add(p => p.Sortable, true)
                .Add(p => p.Filterable, true))
        );

        var initialCallCount = loadDataCallCount;

        // Trigger sort
        await cut.InvokeAsync(() => cut.Instance.SetSort(nameof(Person.Name), SortDir.Ascending));
        
        loadDataCallCount.ShouldBeGreaterThan(initialCallCount);
        _lastLoadDataArgs.ShouldNotBeNull();
        _lastLoadDataArgs.OrderBy.ShouldNotBeNull();
        _lastLoadDataArgs.OrderBy.ShouldContain("Name");
    }

    [Fact(DisplayName = "And I can fetch data from an API")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_CanFetchDataFromAPI()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        var clientData = new List<Person>();

        // Simulate API data fetch
        Task<(List<Person> Data, int TotalCount)> FetchDataFromAPI(LoadDataArgs args)
        {
            var query = _serverData.AsQueryable();
            
            // Apply filtering
            // (In real scenario, this would be OData or similar)
            
            var totalCount = query.Count();
            var data = query.Skip(args.Skip).Take(args.Top).ToList();
            
            return Task.FromResult((data, totalCount));
        }

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.Data, clientData)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.Count, _serverData.Count)
            .Add(p => p.LoadData, EventCallback.Factory.Create<LoadDataArgs>(this, async args =>
            {
                var (data, totalCount) = await FetchDataFromAPI(args);
                clientData.Clear();
                clientData.AddRange(data);
            }))
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        // Verify data was loaded
        clientData.Count.ShouldBe(2); // First page with PageSize=2
        clientData[0].Name.ShouldBe("Alice");
        clientData[1].Name.ShouldBe("Bob");
    }
}
